using LibAtrac9;
using PS4_Tools.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static PS4_Tools.Media.Atrac9;

namespace PS4_Tools
{
    public class At9Configuration : Configuration
    {
    }
    public abstract class AudioReader<TReader, TStructure, TConfig> : IAudioReader
       where TReader : AudioReader<TReader, TStructure, TConfig>
       where TConfig : Configuration, new()
    {
        public IAudioFormat ReadFormat(Stream stream) => ReadStream(stream).AudioFormat;
        public IAudioFormat ReadFormat(byte[] file) => ReadByteArray(file).AudioFormat;

        public AudioData Read(Stream stream) => ReadStream(stream).Audio;
        public AudioData Read(byte[] file) => ReadByteArray(file).Audio;

        public AudioWithConfig ReadWithConfig(Stream stream) => ReadStream(stream);
        public AudioWithConfig ReadWithConfig(byte[] file) => ReadByteArray(file);

        public TStructure ReadMetadata(Stream stream) => ReadStructure(stream, false);

        protected virtual TConfig GetConfiguration(TStructure structure) => new TConfig();
        public abstract TStructure ReadFile(Stream stream, bool readAudioData = true);
        protected abstract IAudioFormat ToAudioStream(TStructure structure);

        private AudioWithConfig ReadByteArray(byte[] file)
        {
            using (var stream = new MemoryStream(file))
            {
                return ReadStream(stream);
            }
        }

        private AudioWithConfig ReadStream(Stream stream)
        {
            TStructure structure = ReadStructure(stream);
            return new AudioWithConfig(ToAudioStream(structure), GetConfiguration(structure));
        }

        private TStructure ReadStructure(Stream stream, bool readAudioData = true)
        {
            if (!stream.CanSeek)
            {
                throw new NotSupportedException("A seekable stream is required");
            }

            return ReadFile(stream, readAudioData);
        }
    }
    public class At9Reader : AudioReader<At9Reader, At9Structure, At9Configuration>
    {
       
        private static void ValidateAt9File(RiffParser parser)
        {
            if (parser.RiffChunk.Type != "WAVE")
            {
                throw new InvalidDataException("Not a valid WAVE file");
            }

            WaveFmtChunk fmt = parser.GetSubChunk<WaveFmtChunk>("fmt "); if (fmt == null) throw new InvalidDataException("File must have a valid fmt chunk");
            At9WaveExtensible ext = fmt.Ext as At9WaveExtensible;if(ext == null) throw new InvalidDataException("File must have a format chunk extension");
            if (parser.GetSubChunk<At9FactChunk>("fact") == null) throw new InvalidDataException("File must have a valid fact chunk");
            if (parser.GetSubChunk<At9DataChunk>("data") == null) throw new InvalidDataException("File must have a valid data chunk");

            if (fmt.ChannelCount == 0) throw new InvalidDataException("Channel count must not be zero");

            if (ext.SubFormat != MediaSubtypes.MediaSubtypeAtrac9)
                throw new InvalidDataException($"Must contain ATRAC9 data. Has unsupported SubFormat {ext.SubFormat}");
        }

        public override At9Structure ReadFile(Stream stream, bool readAudioData = true)
        {
             var structure = new At9Structure();
            var parser = new RiffParser { ReadDataChunk = readAudioData };
            parser.RegisterSubChunk("fact", At9FactChunk.ParseAt9);
            parser.RegisterSubChunk("data", At9DataChunk.ParseAt9);
            parser.FormatExtensibleParser = At9WaveExtensible.ParseAt9;
            parser.ParseRiff(stream);

            ValidateAt9File(parser);

            var fmt = parser.GetSubChunk<WaveFmtChunk>("fmt ");
            var ext = (At9WaveExtensible)fmt.Ext;
            var fact = parser.GetSubChunk<At9FactChunk>("fact");
            var data = parser.GetSubChunk<At9DataChunk>("data");
            var smpl = parser.GetSubChunk<WaveSmplChunk>("smpl");

            structure.Config = new LibAtrac9.Atrac9Config(ext.ConfigData);
            structure.SampleCount = fact.SampleCount;
            structure.EncoderDelay = fact.EncoderDelaySamples;
            structure.Version = ext.VersionInfo;
            structure.AudioData = data.AudioData;
            structure.SuperframeCount = data.FrameCount;

            if (smpl?.Loops?.FirstOrDefault() != null)
            {
                structure.LoopStart = smpl.Loops[0].Start - structure.EncoderDelay;
                structure.LoopEnd = smpl.Loops[0].End - structure.EncoderDelay;
                structure.Looping = structure.LoopEnd > structure.LoopStart;
            }

            return structure;
        }

        protected override IAudioFormat ToAudioStream(At9Structure structure)
        {
            return new Atrac9FormatBuilder(structure.AudioData, structure.Config, structure.SampleCount, structure.EncoderDelay)
            .WithLoop(structure.Looping, structure.LoopStart, structure.LoopEnd)
            .Build();
        }
    }

    public class WaveStructure
    {
        public List<RiffSubChunk> RiffSubChunks { get; set; }

        /// <summary>The number of channels in the WAVE file.</summary>
        public int ChannelCount { get; set; }

        /// <summary>The audio sample rate.</summary>
        public int SampleRate { get; set; }

        /// <summary>The number of bits per audio sample.</summary>
        public int BitsPerSample { get; set; }

        /// <summary>The number of samples in the audio file.</summary>
        public int SampleCount { get; set; }

        /// <summary>This flag is set if the file loops.</summary>
        public bool Looping { get; set; }

        /// <summary>The loop start position in samples.</summary>
        public int LoopStart { get; set; }

        /// <summary>The loop end position in samples.</summary>
        public int LoopEnd { get; set; }

        public short[][] AudioData16 { get; set; }
        public byte[][] AudioData8 { get; set; }
    }

    /// <summary>
    /// The different audio codecs used in Wave files.
    /// </summary>
    public enum WaveCodec
    {
        /// <summary>
        /// 16-bit PCM.
        /// </summary>
        Pcm16Bit,
        /// <summary>
        /// 8-bit PCM.
        /// </summary>
        Pcm8Bit
    }

    public class WaveConfiguration : Configuration
    {
        public WaveCodec Codec { get; set; }
    }

    public class WaveReader : AudioReader<WaveReader, WaveStructure, WaveConfiguration>
    {
        public override WaveStructure ReadFile(Stream stream, bool readAudioData = true)
        {
            var structure = new WaveStructure();
            var parser = new RiffParser { ReadDataChunk = readAudioData };
            parser.ParseRiff(stream);

            ValidateWaveFile(parser);

            var fmt = parser.GetSubChunk<WaveFmtChunk>("fmt ");
            var data = parser.GetSubChunk<WaveDataChunk>("data");
            var smpl = parser.GetSubChunk<WaveSmplChunk>("smpl");

            int bytesPerSample = fmt.BitsPerSample.DivideByRoundUp(8);
            structure.RiffSubChunks = parser.GetAllSubChunks();
            structure.SampleCount = data.SubChunkSize / bytesPerSample / fmt.ChannelCount;
            structure.SampleRate = fmt.SampleRate;
            structure.BitsPerSample = fmt.BitsPerSample;
            structure.ChannelCount = fmt.ChannelCount;

            if (smpl?.Loops?.FirstOrDefault() != null)
            {
                structure.LoopStart = smpl.Loops[0].Start;
                structure.LoopEnd = smpl.Loops[0].End;
                structure.Looping = structure.LoopEnd > structure.LoopStart;
            }

            if (!readAudioData) return structure;

            switch (fmt.BitsPerSample)
            {
                case 16:
                    structure.AudioData16 = data.Data.InterleavedByteToShort(fmt.ChannelCount);
                    break;
                case 8:
                    structure.AudioData8 = data.Data.DeInterleave(bytesPerSample, fmt.ChannelCount);
                    break;
            }
            return structure;
        }

        protected override IAudioFormat ToAudioStream(WaveStructure structure)
        {
            switch (structure.BitsPerSample)
            {
                case 16:
                    return new Pcm16FormatBuilder(structure.AudioData16, structure.SampleRate)
                        .WithLoop(structure.Looping, structure.LoopStart, structure.LoopEnd)
                        .Build();
                case 8:
                    return new Pcm8FormatBuilder(structure.AudioData8, structure.SampleRate)
                        .WithLoop(structure.Looping, structure.LoopStart, structure.LoopEnd)
                        .Build();
                default:
                    return null;
            }
        }

        private static void ValidateWaveFile(RiffParser parser)
        {
            if (parser.RiffChunk.Type != "WAVE")
            {
                throw new InvalidDataException("Not a valid WAVE file");
            }

            WaveFmtChunk fmt = parser.GetSubChunk<WaveFmtChunk>("fmt ");if(fmt == null) throw new InvalidDataException("File must have a valid fmt chunk");
            if (parser.GetSubChunk<WaveDataChunk>("data") == null) throw new InvalidDataException("File must have a valid data chunk");

            int bytesPerSample = fmt.BitsPerSample.DivideByRoundUp(8);

            if (fmt.FormatTag != WaveFormatTags.WaveFormatPcm && fmt.FormatTag != WaveFormatTags.WaveFormatExtensible)
                throw new InvalidDataException($"Must contain PCM data. Has unsupported format {fmt.FormatTag}");

            if (fmt.BitsPerSample != 16 && fmt.BitsPerSample != 8)
                throw new InvalidDataException($"Must have 8 or 16 bits per sample, not {fmt.BitsPerSample} bits per sample");

            if (fmt.ChannelCount == 0) throw new InvalidDataException("Channel count must not be zero");

            if (fmt.BlockAlign != bytesPerSample * fmt.ChannelCount)
                throw new InvalidDataException("File has invalid block alignment");

            if (fmt.Ext != null && fmt.Ext.SubFormat != MediaSubtypes.MediaSubtypePcm)
                throw new InvalidDataException($"Must contain PCM data. Has unsupported SubFormat {fmt.Ext.SubFormat}");
        }
    }
     public class WaveWriter : AudioWriter<WaveWriter, WaveConfiguration>
    {
        private Pcm16Format Pcm16 { get; set; }
        private Pcm8Format Pcm8 { get; set; }
        private IAudioFormat AudioFormat { get; set; }

        private WaveCodec Codec => Configuration.Codec;
        private int ChannelCount => AudioFormat.ChannelCount;
        private int SampleCount => AudioFormat.SampleCount;
        private int SampleRate => AudioFormat.SampleRate;
        private bool Looping => AudioFormat.Looping;
        private int LoopStart => AudioFormat.LoopStart;
        private int LoopEnd => AudioFormat.LoopEnd;
        protected override int FileSize => 8 + RiffChunkSize;
        private int RiffChunkSize => 4 + 8 + FmtChunkSize + 8 + DataChunkSize
            + (Looping ? 8 + SmplChunkSize : 0);
        private int FmtChunkSize => ChannelCount > 2 ? 40 : 16;
        private int DataChunkSize => ChannelCount * SampleCount * BytesPerSample;
        private int SmplChunkSize => 0x3c;

        private int BitDepth => Configuration.Codec == WaveCodec.Pcm16Bit ? 16 : 8;
        private int BytesPerSample => BitDepth.DivideByRoundUp(8);
        private int BytesPerSecond => SampleRate * BytesPerSample * ChannelCount;
        private int BlockAlign => BytesPerSample * ChannelCount;

        protected override void SetupWriter(AudioData audio)
        {
            var parameters = new CodecParameters { Progress = Configuration.Progress };

            switch (Codec)
            {
                case WaveCodec.Pcm16Bit:
                    Pcm16 = audio.GetFormat<Pcm16Format>(parameters);
                    AudioFormat = Pcm16;
                    break;
                case WaveCodec.Pcm8Bit:
                    Pcm8 = audio.GetFormat<Pcm8Format>(parameters);
                    AudioFormat = Pcm8;
                    break;
            }
        }

        protected override void WriteStream(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                stream.Position = 0;
                WriteRiffHeader(writer);
                WriteFmtChunk(writer);
                WriteDataChunk(writer);
                if (Looping)
                    WriteSmplChunk(writer);
            }
        }

        private void WriteRiffHeader(BinaryWriter writer)
        {
            writer.Write("RIFF");
            writer.Write(RiffChunkSize);
            writer.Write("WAVE");
        }

        private void WriteFmtChunk(BinaryWriter writer)
        {
            // Every chunk should be 2-byte aligned
            writer.BaseStream.Position += writer.BaseStream.Position & 1;
            writer.Write("fmt ");
            writer.Write(FmtChunkSize);
            writer.Write((short)(ChannelCount > 2 ? WaveFormatTags.WaveFormatExtensible : WaveFormatTags.WaveFormatPcm));
            writer.Write((short)ChannelCount);
            writer.Write(SampleRate);
            writer.Write(BytesPerSecond);
            writer.Write((short)BlockAlign);
            writer.Write((short)BitDepth);

            if (ChannelCount > 2)
            {
                writer.Write((short)22);
                writer.Write((short)BitDepth);
                writer.Write(GetChannelMask(ChannelCount));
                writer.Write(MediaSubtypes.MediaSubtypePcm.ToByteArray());
            }
        }

        private void WriteDataChunk(BinaryWriter writer)
        {
            writer.BaseStream.Position += writer.BaseStream.Position & 1;
            
            writer.Write("data");
            writer.Write(DataChunkSize);

            switch (Codec)
            {
                case WaveCodec.Pcm16Bit:
                    byte[] audioData = Pcm16.Channels.ShortToInterleavedByte();
                    writer.BaseStream.Write(audioData, 0, audioData.Length);
                    break;
                case WaveCodec.Pcm8Bit:
                    Pcm8.Channels.Interleave(writer.BaseStream, BytesPerSample);
                    break;
            }
        }

        private void WriteSmplChunk(BinaryWriter writer)
        {
            writer.BaseStream.Position += writer.BaseStream.Position & 1;
            writer.Write("smpl");
            writer.Write(SmplChunkSize);
            for (int i = 0; i < 7; i++)
                writer.Write(0);
            writer.Write(1);
            for (int i = 0; i < 3; i++)
                writer.Write(0);
            writer.Write(LoopStart);
            writer.Write(LoopEnd);
            writer.Write(0);
            writer.Write(0);
        }

        private static int GetChannelMask(int channelCount)
        {
            //Nothing special about these masks. I just choose
            //whatever channel combinations seemed okay.
            switch (channelCount)
            {
                case 4:
                    return 0x0033;
                case 5:
                    return 0x0133;
                case 6:
                    return 0x0633;
                case 7:
                    return 0x01f3;
                case 8:
                    return 0x06f3;
                default:
                    return (1 << channelCount) - 1;
            }
        }
    }

    internal static class AudioInfo
    {
        public static readonly Dictionary<FileType, ContainerType> Containers = new Dictionary<FileType, ContainerType>
        {
            [FileType.Wave] = new ContainerType("WAVE", new[] { "wav" }, "WAVE Audio File", () => new WaveReader(), () => new WaveWriter()),

            [FileType.Atrac9] = new ContainerType("ATRAC9", new[] { "at9" }, "ATRAC9 Audio File", () => new At9Reader(), null)
        };

        public static readonly Dictionary<string, FileType> Extensions =
            Containers.SelectMany(x => x.Value.Extensions.Select(y => new { y, x.Key }))
            .ToDictionary(x => x.y, x => x.Key);

        public static FileType GetFileTypeFromName(string fileName)
        {
            string extension = Path.GetExtension(fileName)?.TrimStart('.').ToLower() ?? "";
            FileType fileType;
            Extensions.TryGetValue(extension, out fileType);
            return fileType;
        }
    }
    public static class Pcm8Codec
    {
        public static byte[] Encode(short[] array)
        {
            var output = new byte[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = (byte)((array[i] + short.MaxValue + 1) >> 8);
            }

            return output;
        }

        public static short[] Decode(byte[] array)
        {
            var output = new short[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = (short)((array[i] - 0x80) << 8);
            }

            return output;
        }

        public static byte[] EncodeSigned(short[] array)
        {
            var output = new byte[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = (byte)(array[i] >> 8);
            }

            return output;
        }

        public static short[] DecodeSigned(byte[] array)
        {
            var output = new short[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = (short)(array[i] << 8);
            }

            return output;
        }
    }
    public class Pcm8Format : AudioFormatBase<Pcm8Format, Pcm8FormatBuilder, CodecParameters>
    {
        public byte[][] Channels { get; }
        public virtual bool Signed { get; } = false;

        public Pcm8Format() { Channels = new byte[0][]; }
        public Pcm8Format(byte[][] channels, int sampleRate) : this(new Pcm8FormatBuilder(channels, sampleRate)) { }
        internal Pcm8Format(Pcm8FormatBuilder b) : base(b) { Channels = b.Channels; }

        public override Pcm16Format ToPcm16()
        {
            var channels = new short[ChannelCount][];

            for (int i = 0; i < ChannelCount; i++)
            {
                channels[i] = Signed ? Pcm8Codec.DecodeSigned(Channels[i]) : Pcm8Codec.Decode(Channels[i]);
            }

            return new Pcm16FormatBuilder(channels, SampleRate)
                .WithLoop(Looping, LoopStart, LoopEnd)
                .WithTracks(Tracks)
                .Build();
        }

        public override Pcm8Format EncodeFromPcm16(Pcm16Format pcm16)
        {
            var channels = new byte[pcm16.ChannelCount][];

            for (int i = 0; i < pcm16.ChannelCount; i++)
            {
                channels[i] = Signed ? Pcm8Codec.EncodeSigned(pcm16.Channels[i]) : Pcm8Codec.Encode(pcm16.Channels[i]);
            }

            return new Pcm8FormatBuilder(channels, pcm16.SampleRate, Signed)
                .WithLoop(pcm16.Looping, pcm16.LoopStart, pcm16.LoopEnd)
                .WithTracks(pcm16.Tracks)
                .Build();
        }

        protected override Pcm8Format AddInternal(Pcm8Format pcm8)
        {
            Pcm8FormatBuilder copy = GetCloneBuilder();
            copy.Channels = Channels.Concat(pcm8.Channels).ToArray();
            return copy.Build();
        }

        protected override Pcm8Format GetChannelsInternal(int[] channelRange)
        {
            var channels = new List<byte[]>();

            foreach (int i in channelRange)
            {
                if (i < 0 || i >= Channels.Length)
                    throw new ArgumentException($"Channel {i} does not exist.", nameof(channelRange));
                channels.Add(Channels[i]);
            }

            Pcm8FormatBuilder copy = GetCloneBuilder();
            copy.Channels = channels.ToArray();
            return copy.Build();
        }

        public static Pcm8FormatBuilder GetBuilder(byte[][] channels, int sampleRate) => new Pcm8FormatBuilder(channels, sampleRate);
        public override Pcm8FormatBuilder GetCloneBuilder() => GetCloneBuilderBase(new Pcm8FormatBuilder(Channels, SampleRate));
    }

    public class Pcm8SignedFormat : Pcm8Format
    {
        public override bool Signed { get; } = true;
        public Pcm8SignedFormat() { }
        public Pcm8SignedFormat(byte[][] channels, int sampleRate) : base(new Pcm8FormatBuilder(channels, sampleRate)) { }
        internal Pcm8SignedFormat(Pcm8FormatBuilder b) : base(b) { }
    }

    public class Pcm8FormatBuilder : AudioFormatBaseBuilder<Pcm8Format, Pcm8FormatBuilder, CodecParameters>
    {
        public byte[][] Channels { get; set; }
        public bool Signed { get; set; }
        public override int ChannelCount => Channels.Length;

        public Pcm8FormatBuilder(byte[][] channels, int sampleRate, bool signed = false)
        {
            if (channels == null || channels.Length < 1)
                throw new InvalidDataException("Channels parameter cannot be empty or null");

            Channels = channels.ToArray();
            SampleCount = Channels[0]?.Length ?? 0;
            SampleRate = sampleRate;
            Signed = signed;

            foreach (byte[] channel in Channels)
            {
                if (channel == null)
                    throw new InvalidDataException("All provided channels must be non-null");

                if (channel.Length != SampleCount)
                    throw new InvalidDataException("All channels must have the same sample count");
            }
        }

        public override Pcm8Format Build() => Signed ? new Pcm8SignedFormat(this) : new Pcm8Format(this);
    }

    public enum FileType
    {
        Unknown = 0,
        Wave,
        Dsp,
        Brstm,
        Bcstm,
        Bfstm,
        Idsp,
        Hps,
        Adx,
        Hca,
        Genh,
        Atrac9
    }

    public class ContainerType
    {
        public string DisplayName { get; }
        public IEnumerable<string> Extensions { get; }
        public string Description { get; }
        public Func<IAudioReader> GetReader { get; }
        public Func<IAudioWriter> GetWriter { get; }

        public ContainerType(string displayName, IEnumerable<string> extensions, string description, Func<IAudioReader> getReader, Func<IAudioWriter> getWriter)
        {
            DisplayName = displayName;
            Extensions = extensions.ToList();
            Description = description;
            GetReader = getReader;
            GetWriter = getWriter;
        }
    }

    public interface IAudioReader
    {
        IAudioFormat ReadFormat(Stream stream);
        IAudioFormat ReadFormat(byte[] file);
        AudioData Read(Stream stream);
        AudioData Read(byte[] file);
        AudioWithConfig ReadWithConfig(Stream stream);
        AudioWithConfig ReadWithConfig(byte[] file);
    }
    public interface IAudioWriter
    {
        void WriteToStream(IAudioFormat audio, Stream stream, Configuration configuration = null);
        byte[] GetFile(IAudioFormat audio, Configuration configuration = null);

        void WriteToStream(AudioData audio, Stream stream, Configuration configuration = null);
    }

        public abstract class AudioWriter<TWriter, TConfig> : IAudioWriter
          where TWriter : AudioWriter<TWriter, TConfig>
          where TConfig : Configuration, new()
    {
        public byte[] GetFile(IAudioFormat audio, Configuration configuration = null) => GetByteArray(new AudioData(audio), configuration as TConfig);
        public void WriteToStream(IAudioFormat audio, Stream stream, Configuration configuration = null) => WriteStream(new AudioData(audio), stream, configuration as TConfig);

        public byte[] GetFile(AudioData audio, Configuration configuration = null) => GetByteArray(audio, configuration as TConfig);
        public void WriteToStream(AudioData audio, Stream stream, Configuration configuration = null) => WriteStream(audio, stream, configuration as TConfig);

        protected AudioData AudioStream { get; set; }
        public TConfig Configuration { get; set; } = new TConfig();
        protected abstract int FileSize { get; }

        protected abstract void SetupWriter(AudioData audio);
        protected abstract void WriteStream(Stream stream);

        private byte[] GetByteArray(AudioData audio, TConfig configuration = null)
        {
            Configuration = configuration ?? Configuration;
            SetupWriter(audio);

            MemoryStream stream;
            byte[] file = null;

            if (FileSize == -1)
            {
                stream = new MemoryStream(0);
            }
            else
            {
                file = new byte[FileSize];
                stream = new MemoryStream();
            }

            WriteStream(stream);

            return FileSize == -1 ? stream.ToArray() : file;
        }

        private void WriteStream(AudioData audio, Stream stream, TConfig configuration = null)
        {
            Configuration = configuration ?? Configuration;
            SetupWriter(audio);
            if (stream.Length != FileSize && FileSize != -1)
            {
                try
                {
                    stream.SetLength(FileSize);
                }
                catch (NotSupportedException ex)
                {
                    throw new ArgumentException("Stream is too small.", nameof(stream), ex);
                }
            }

            WriteStream(stream);
        }
    }

    public class AudioData
    {
        private Dictionary<Type, IAudioFormat> Formats { get; } = new Dictionary<Type, IAudioFormat>();

        private void AddFormat(IAudioFormat format) => Formats[format.GetType()] = format;

        public AudioData(IAudioFormat audioFormat)
        {
            AddFormat(audioFormat);
        }

        public T GetFormat<T>(CodecParameters configuration = null) where T : class, IAudioFormat, new()
        {
            var format = GetAudioFormat<T>();

            if (format != null)
            {
                return format;
            }

            CreatePcm16(configuration);
            CreateFormat<T>(configuration);

            return GetAudioFormat<T>();
        }

        public IEnumerable<IAudioFormat> GetAllFormats() => Formats.Values;

        public IEnumerable<Type> ListAvailableFormats() => Formats.Keys;

        public void SetLoop(bool loop, int loopStart, int loopEnd)
        {
            foreach (Type format in Formats.Keys.ToList())
            {
                Formats[format] = Formats[format].WithLoop(loop, loopStart, loopEnd);
            }
        }
        public void SetLoop(bool loop)
        {
            foreach (Type format in Formats.Keys.ToList())
            {
                Formats[format] = Formats[format].WithLoop(loop);
            }
        }

        public static AudioData Combine(params AudioData[] audio)
        {
            if (audio == null || audio.Length <= 0 || audio.Any(x => x == null))
                throw new ArgumentException("Audio cannot be null, empty, or have any null elements");

            List<Type> commonTypes = audio
                .Select(x => x.ListAvailableFormats())
                .Aggregate((x, y) => x.Intersect(y))
                .ToList();

            Type formatToUse;

            if (commonTypes.Count == 0 || commonTypes.Count == 1 && commonTypes.Contains(typeof(Pcm16Format)))
            {
                formatToUse = typeof(Pcm16Format);
                foreach (AudioData a in audio)
                {
                    a.CreatePcm16();
                }
            }
            else
            {
                formatToUse = commonTypes.First(x => x != typeof(Pcm16Format));
            }

            IAudioFormat combined = audio[0].Formats[formatToUse];

            foreach (IAudioFormat format in audio.Select(x => x.Formats[formatToUse]).Skip(1))
            {
                if (combined.TryAdd(format, out combined) == false)
                {
                    throw new ArgumentException("Audio streams cannot be added together");
                }
            }

            return new AudioData(combined);
        }

        private T GetAudioFormat<T>() where T : class, IAudioFormat
        {
            IAudioFormat format;
            Formats.TryGetValue(typeof(T), out format);

            return (T)format;
        }

        private void CreateFormat<T>(CodecParameters configuration = null) where T : class, IAudioFormat, new()
        {
            var pcm = GetAudioFormat<Pcm16Format>();
            AddFormat(new T().EncodeFromPcm16(pcm, configuration));
        }

        private void CreatePcm16(CodecParameters configuration = null)
        {
            if (GetAudioFormat<Pcm16Format>() == null)
            {
                AddFormat(Formats.First().Value.ToPcm16(configuration));
            }
        }
    }

    public class Configuration
    {
        public IProgressReport Progress { get; set; }
        /// <summary>
        /// If <c>true</c>, trims the output file length to the set LoopEnd.
        /// If <c>false</c> or if the <see cref="IAudioFormat"/> does not loop,
        /// the output file is not trimmed.
        /// Default is <c>true</c>.
        /// </summary>
        public bool TrimFile { get; set; } = true;
    }

    public class AudioWithConfig
    {
        public AudioWithConfig(IAudioFormat audioFormat, Configuration configuration)
        {
            AudioFormat = audioFormat;
            Configuration = configuration;
        }

        public IAudioFormat AudioFormat { get; }
        public AudioData Audio => new AudioData(AudioFormat);
        public Configuration Configuration { get; }
    }

    public interface IProgressReport
    {
        /// <summary>
        /// Sets the current value of the <see cref="IProgressReport"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void Report(int value);

        /// <summary>
        /// Adds <paramref name="value"/> to the current value of the <see cref="IProgressReport"/>.
        /// </summary>
        /// <param name="value">The amount to add.</param>
        void ReportAdd(int value);

        /// <summary>
        /// Sets the maximum value of the <see cref="IProgressReport"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The maximum value to set.</param>
        void SetTotal(int value);

        /// <summary>
        /// Logs a message to the <see cref="IProgressReport"/> object.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void LogMessage(string message);
    }

    public class CodecParameters
    {
        public IProgressReport Progress { get; set; }
        public int SampleCount { get; set; } = -1;

        public CodecParameters() { }

        protected CodecParameters(CodecParameters source)
        {
            if (source == null) return;
            Progress = source.Progress;
            SampleCount = source.SampleCount;
        }
    }

    public class Pcm16FormatBuilder : AudioFormatBaseBuilder<Pcm16Format, Pcm16FormatBuilder, CodecParameters>
    {
        public short[][] Channels { get; set; }
        public override int ChannelCount => Channels.Length;

        public Pcm16FormatBuilder(short[][] channels, int sampleRate)
        {
            if (channels == null || channels.Length < 1)
                throw new InvalidDataException("Channels parameter cannot be empty or null");

            Channels = channels.ToArray();
            SampleCount = Channels[0]?.Length ?? 0;
            SampleRate = sampleRate;

            foreach (short[] channel in Channels)
            {
                if (channel == null)
                    throw new InvalidDataException("All provided channels must be non-null");

                if (channel.Length != SampleCount)
                    throw new InvalidDataException("All channels must have the same sample count");
            }
        }

        public override Pcm16Format Build() => new Pcm16Format(this);
    }



    /// <summary>
    /// A 16-bit PCM audio stream.
    /// The stream can contain any number of individual channels.
    /// </summary>
    public class Pcm16Format : AudioFormatBase<Pcm16Format, Pcm16FormatBuilder, CodecParameters>
    {
        public short[][] Channels { get; }

        public Pcm16Format() { Channels = new short[0][]; }
        public Pcm16Format(short[][] channels, int sampleRate) : this(new Pcm16FormatBuilder(channels, sampleRate)) { }
        internal Pcm16Format(Pcm16FormatBuilder b) : base(b) { Channels = b.Channels; }

        public override Pcm16Format ToPcm16() => GetCloneBuilder().Build();
        public override Pcm16Format EncodeFromPcm16(Pcm16Format pcm16) => pcm16.GetCloneBuilder().Build();

        protected override Pcm16Format AddInternal(Pcm16Format pcm16)
        {
            Pcm16FormatBuilder copy = GetCloneBuilder();
            copy.Channels = Channels.Concat(pcm16.Channels).ToArray();
            return copy.Build();
        }

        protected override Pcm16Format GetChannelsInternal(int[] channelRange)
        {
            var channels = new List<short[]>();

            foreach (int i in channelRange)
            {
                if (i < 0 || i >= Channels.Length)
                    throw new ArgumentException($"Channel {i} does not exist.", nameof(channelRange));
                channels.Add(Channels[i]);
            }

            Pcm16FormatBuilder copy = GetCloneBuilder();
            copy.Channels = channels.ToArray();
            return copy.Build();
        }

        public static Pcm16FormatBuilder GetBuilder(short[][] channels, int sampleRate) => new Pcm16FormatBuilder(channels, sampleRate);
        public override Pcm16FormatBuilder GetCloneBuilder() => GetCloneBuilderBase(new Pcm16FormatBuilder(Channels, SampleRate));


    }

    public interface IAudioFormat
    {
        int SampleCount { get; }
        int SampleRate { get; }
        int ChannelCount { get; }
        int LoopStart { get; }
        int LoopEnd { get; }
        bool Looping { get; }

        IAudioFormat WithLoop(bool loop, int loopStart, int loopEnd);
        IAudioFormat WithLoop(bool loop);
        Pcm16Format ToPcm16();
        Pcm16Format ToPcm16(CodecParameters config);
        IAudioFormat EncodeFromPcm16(Pcm16Format pcm16);
        IAudioFormat EncodeFromPcm16(Pcm16Format pcm16, CodecParameters config);
        IAudioFormat GetChannels(params int[] channelRange);
        bool TryAdd(IAudioFormat format, out IAudioFormat result);
    }
    /// <summary>
    /// Defines an audio track in an audio stream.
    /// Each track is composed of one or two channels.
    /// </summary>
    public class AudioTrack
    {
        public AudioTrack(int channelCount, int channelLeft, int channelRight, int panning, int volume)
        {
            ChannelCount = channelCount;
            ChannelLeft = channelLeft;
            ChannelRight = channelRight;
            Panning = panning;
            Volume = volume;
        }

        public AudioTrack(int channelCount, int channelLeft, int channelRight)
        {
            ChannelCount = channelCount;
            ChannelLeft = channelLeft;
            ChannelRight = channelRight;
        }

        public AudioTrack() { }

        /// <summary>
        /// The volume of the track. Ranges from
        /// 0 to 127 (0x7f).
        /// </summary>
        public int Volume { get; set; } = 0x7f;

        /// <summary>
        /// The panning of the track. Ranges from
        /// 0 (Completely to the left) to 127 (0x7f)
        /// (Completely to the right) with the center
        /// at 64 (0x40).
        /// </summary>
        public int Panning { get; set; } = 0x40;

        /// <summary>
        /// The number of channels in the track.
        /// If <c>1</c>, only <see cref="ChannelLeft"/>
        /// will be used for the mono track.
        /// If <c>2</c>, both <see cref="ChannelLeft"/>
        /// and <see cref="ChannelRight"/> will be used.
        /// </summary>
        public int ChannelCount { get; set; }

        /// <summary>
        /// The zero-based ID of the left channel in a stereo
        /// track, or the only channel in a mono track.
        /// </summary>
        public int ChannelLeft { get; set; }

        /// <summary>
        /// The zero-based ID of the right channel in
        /// a stereo track.
        /// </summary>
        public int ChannelRight { get; set; }

        public int SurroundPanning { get; set; }
        public int Flags { get; set; }

        public static IEnumerable<AudioTrack> GetDefaultTrackList(int channelCount)
        {
            int trackCount = channelCount.DivideByRoundUp(2);
            for (int i = 0; i < trackCount; i++)
            {
                int trackChannelCount = Math.Min(channelCount - i * 2, 2);
                yield return new AudioTrack
                {
                    ChannelCount = trackChannelCount,
                    ChannelLeft = i * 2,
                    ChannelRight = trackChannelCount >= 2 ? i * 2 + 1 : 0
                };
            }
        }
    }

    public abstract class AudioFormatBase<TFormat, TBuilder, TConfig> : IAudioFormat
        where TFormat : AudioFormatBase<TFormat, TBuilder, TConfig>
        where TBuilder : AudioFormatBaseBuilder<TFormat, TBuilder, TConfig>
        where TConfig : CodecParameters, new()
    {
        private readonly List<AudioTrack> _tracks;
        public int SampleRate { get; }
        public int ChannelCount { get; }
        public int UnalignedSampleCount { get; }
        public int UnalignedLoopStart { get; }
        public int UnalignedLoopEnd { get; }
        public virtual int SampleCount => UnalignedSampleCount;
        public virtual int LoopStart => UnalignedLoopStart;
        public virtual int LoopEnd => UnalignedLoopEnd;
        public bool Looping { get; }
        public List<AudioTrack> Tracks { get; }

        IAudioFormat IAudioFormat.EncodeFromPcm16(Pcm16Format pcm16) => EncodeFromPcm16(pcm16);
        IAudioFormat IAudioFormat.EncodeFromPcm16(Pcm16Format pcm16, CodecParameters config) => EncodeFromPcm16(pcm16, GetDerivedParameters(config));
        IAudioFormat IAudioFormat.GetChannels(params int[] channelRange) => GetChannels(channelRange);
        IAudioFormat IAudioFormat.WithLoop(bool loop, int loopStart, int loopEnd) => WithLoop(loop, loopStart, loopEnd);
        IAudioFormat IAudioFormat.WithLoop(bool loop) => WithLoop(loop);

        public abstract Pcm16Format ToPcm16();
        public virtual Pcm16Format ToPcm16(CodecParameters config) => ToPcm16();
        public virtual Pcm16Format ToPcm16(TConfig config) => ToPcm16((CodecParameters)config);
        public abstract TFormat EncodeFromPcm16(Pcm16Format pcm16);
        public virtual TFormat EncodeFromPcm16(Pcm16Format pcm16, TConfig config) => EncodeFromPcm16(pcm16);

        protected AudioFormatBase() { }
        protected AudioFormatBase(TBuilder builder)
        {
            UnalignedSampleCount = builder.SampleCount;
            SampleRate = builder.SampleRate;
            ChannelCount = builder.ChannelCount;
            Looping = builder.Looping;
            UnalignedLoopStart = builder.LoopStart;
            UnalignedLoopEnd = builder.LoopEnd;
            _tracks = builder.Tracks;
            Tracks = _tracks != null && _tracks.Count > 0 ? _tracks : AudioTrack.GetDefaultTrackList(ChannelCount).ToList();
        }

        public TFormat GetChannels(params int[] channelRange)
        {
            if (channelRange == null)
                throw new ArgumentNullException(nameof(channelRange));

            return GetChannelsInternal(channelRange);
        }

        protected abstract TFormat GetChannelsInternal(int[] channelRange);

        public virtual TFormat WithLoop(bool loop) => GetCloneBuilder().WithLoop(loop).Build();
        public virtual TFormat WithLoop(bool loop, int loopStart, int loopEnd) =>
            GetCloneBuilder().WithLoop(loop, loopStart, loopEnd).Build();

        public bool TryAdd(IAudioFormat format, out IAudioFormat result)
        {
            result = null;
            var castFormat = format as TFormat;
            if (castFormat == null) return false;
            try
            {
                result = Add(castFormat);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public virtual TFormat Add(TFormat format)
        {
            if (format.UnalignedSampleCount != UnalignedSampleCount)
            {
                throw new ArgumentException("Only audio streams of the same length can be added to each other.");
            }

            return AddInternal(format);
        }

        protected abstract TFormat AddInternal(TFormat format);
        public abstract TBuilder GetCloneBuilder();

        protected TBuilder GetCloneBuilderBase(TBuilder builder)
        {
            builder.SampleCount = UnalignedSampleCount;
            builder.SampleRate = SampleRate;
            builder.Looping = Looping;
            builder.LoopStart = UnalignedLoopStart;
            builder.LoopEnd = UnalignedLoopEnd;
            builder.Tracks = _tracks;
            return builder;
        }

        private TConfig GetDerivedParameters(CodecParameters param)
        {
            if (param == null) return null;
            var config = param as TConfig;
            if (config != null) return config;

            return new TConfig
            {
                SampleCount = param.SampleCount,
                Progress = param.Progress
            };
        }
    }

    public abstract class AudioFormatBaseBuilder<TFormat, TBuilder, TConfig>
        where TFormat : AudioFormatBase<TFormat, TBuilder, TConfig>
        where TBuilder : AudioFormatBaseBuilder<TFormat, TBuilder, TConfig>
        where TConfig : CodecParameters, new()
    {
        public abstract int ChannelCount { get; }
        protected internal bool Looping { get; set; }
        protected internal int LoopStart { get; set; }
        protected internal int LoopEnd { get; set; }
        protected internal int SampleCount { get; set; }
        protected internal int SampleRate { get; set; }
        protected internal List<AudioTrack> Tracks { get; set; }

        public abstract TFormat Build();

        public virtual TBuilder WithLoop(bool loop, int loopStart, int loopEnd)
        {
            if (!loop)
            {
                return WithLoop(false);
            }

            if (loopStart < 0 || loopStart > SampleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(loopStart), loopStart, "Loop points must be less than the number of samples and non-negative.");
            }

            if (loopEnd < 0 || loopEnd > SampleCount)
            {
                throw new ArgumentOutOfRangeException(nameof(loopEnd), loopEnd, "Loop points must be less than the number of samples and non-negative.");
            }

            if (loopEnd < loopStart)
            {
                throw new ArgumentOutOfRangeException(nameof(loopEnd), loopEnd, "The loop end must be greater than the loop start");
            }

            Looping = true;
            LoopStart = loopStart;
            LoopEnd = loopEnd;

            return this as TBuilder;
        }

        public virtual TBuilder WithLoop(bool loop)
        {
            Looping = loop;
            LoopStart = 0;
            LoopEnd = loop ? SampleCount : 0;
            return this as TBuilder;
        }

        public TBuilder WithTracks(IEnumerable<AudioTrack> tracks)
        {
            Tracks = tracks?.ToList();
            return this as TBuilder;
        }
    }

    public class Atrac9Parameters : CodecParameters
    {
        public Atrac9Parameters() { }
        public Atrac9Parameters(CodecParameters source) : base(source) { }
    }

    public class Atrac9Format : AudioFormatBase<Atrac9Format, Atrac9FormatBuilder, Atrac9Parameters>
    {
        public byte[][] AudioData { get; }
        public Atrac9Config Config { get; }
        public int EncoderDelay { get; }

        internal Atrac9Format(Atrac9FormatBuilder b) : base(b)
        {
            AudioData = b.AudioData;
            Config = b.Config;
            EncoderDelay = b.EncoderDelay;
        }

        public override Pcm16Format ToPcm16() => ToPcm16(null);
        public override Pcm16Format ToPcm16(CodecParameters config)
        {
            short[][] audio = Decode(config);

            return new Pcm16FormatBuilder(audio, SampleRate)
                .WithLoop(Looping, UnalignedLoopStart, UnalignedLoopEnd)
                .Build();
        }

        private short[][] Decode(CodecParameters parameters)
        {
            IProgressReport progress = parameters?.Progress;
            progress?.SetTotal(AudioData.Length);

            var decoder = new Atrac9Decoder();
            decoder.Initialize(Config.ConfigData);
            Atrac9Config config = decoder.Config;
            var pcmOut = Utils.CreateJaggedArray<short[][]>(config.ChannelCount, SampleCount);
            var pcmBuffer = Utils.CreateJaggedArray<short[][]>(config.ChannelCount, config.SuperframeSamples);

            for (int i = 0; i < AudioData.Length; i++)
            {
                decoder.Decode(AudioData[i], pcmBuffer);
                CopyBuffer(pcmBuffer, pcmOut, EncoderDelay, i);
                progress?.ReportAdd(1);
            }
            return pcmOut;
        }

        private static void CopyBuffer(short[][] bufferIn, short[][] bufferOut, int startIndex, int bufferIndex)
        {
            if (bufferIn == null || bufferOut == null || bufferIn.Length == 0 || bufferOut.Length == 0)
            {
                throw new ArgumentException(
                    $"{nameof(bufferIn)} and {nameof(bufferOut)} must be non-null with a length greater than 0");
            }

            int bufferLength = bufferIn[0].Length;
            int outLength = bufferOut[0].Length;

            int currentIndex = bufferIndex * bufferLength - startIndex;
            int remainingElements = Math.Min(outLength - currentIndex, outLength);
            int srcStart = Utils.Clamp(0 - currentIndex, 0, bufferLength);
            int destStart = Math.Max(currentIndex, 0);

            int length = Math.Min(bufferLength - srcStart, remainingElements);
            if (length <= 0) return;

            for (int c = 0; c < bufferOut.Length; c++)
            {
                Array.Copy(bufferIn[c], srcStart, bufferOut[c], destStart, length);
            }
        }

        public override Atrac9Format EncodeFromPcm16(Pcm16Format pcm16)
        {
            throw new NotImplementedException();
        }

        public override Atrac9FormatBuilder GetCloneBuilder()
        {
            throw new NotImplementedException();
        }

        protected override Atrac9Format AddInternal(Atrac9Format format)
        {
            throw new NotImplementedException();
        }

        protected override Atrac9Format GetChannelsInternal(int[] channelRange)
        {
            throw new NotImplementedException();
        }
    }

    public class Atrac9FormatBuilder : AudioFormatBaseBuilder<Atrac9Format, Atrac9FormatBuilder, Atrac9Parameters>
    {
        public Atrac9Config Config { get; }
        public byte[][] AudioData { get; }
        public override int ChannelCount => Config.ChannelCount;
        public int EncoderDelay { get; }

        public Atrac9FormatBuilder(byte[][] audioData, Atrac9Config config, int sampleCount, int encoderDelay)
        {
            AudioData = audioData;
            if (audioData == null)
            {
                throw new ArgumentNullException(nameof(audioData));
            }
            Config = config;
            if (Config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            SampleRate = config.SampleRate;
            SampleCount = sampleCount;
            EncoderDelay = encoderDelay;
        }

        public override Atrac9Format Build() => new Atrac9Format(this);
    }
}
