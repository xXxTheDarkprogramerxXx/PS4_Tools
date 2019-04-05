using PS4_Tools.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PS4_Tools
{
    public class RiffChunk
    {
        public string ChunkId { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }

        public static RiffChunk Parse(BinaryReader reader)
        {
            var chunk = new RiffChunk
            {
                ChunkId = Utils.ReadUTF8String(reader, 4),
                Size = reader.ReadInt32(),
                Type = Utils.ReadUTF8String(reader, 4)
            };

            if (chunk.ChunkId != "RIFF")
            {
                throw new InvalidDataException("Not a valid RIFF file");
            }

            return chunk;
        }
    }

    public class RiffSubChunk
    {
        public string SubChunkId { get; set; }
        public int SubChunkSize { get; set; }
        public byte[] Extra { get; set; }

        public RiffSubChunk(BinaryReader reader)
        {
            SubChunkId = Utils.ReadUTF8String(reader, 4);
            SubChunkSize = reader.ReadInt32();
        }
    }

    public class WaveFormatExtensible
    {
        public int Size { get; set; }
        public int ValidBitsPerSample { get; set; }
        public int SamplesPerBlock
        {
            get { return ValidBitsPerSample; }
            set { ValidBitsPerSample = value; }
        }
        public uint ChannelMask { get; set; }
        public Guid SubFormat { get; set; }
        public byte[] Extra { get; set; }

        protected WaveFormatExtensible(BinaryReader reader)
        {
            Size = reader.ReadInt16();

            ValidBitsPerSample = reader.ReadInt16();
            ChannelMask = reader.ReadUInt32();
            SubFormat = new Guid(reader.ReadBytes(16));
        }

        public static WaveFormatExtensible Parse(RiffParser parser, BinaryReader reader) =>
            new WaveFormatExtensible(reader);
    }

    public static class WaveFormatTags
    {
        public static int WaveFormatPcm { get; } = 0x0001;
        public static int WaveFormatExtensible { get; } = 0xFFFE;
    }

    public class WaveFmtChunk : RiffSubChunk
    {
        public int FormatTag { get; set; }
        public int ChannelCount { get; set; }
        public int SampleRate { get; set; }
        public int AvgBytesPerSec { get; set; }
        public int BlockAlign { get; set; }
        public int BitsPerSample { get; set; }
        public WaveFormatExtensible Ext { get; set; }

        protected WaveFmtChunk(RiffParser parser, BinaryReader reader) : base(reader)
        {
            FormatTag = reader.ReadUInt16();
            ChannelCount = reader.ReadInt16();
            SampleRate = reader.ReadInt32();
            AvgBytesPerSec = reader.ReadInt32();
            BlockAlign = reader.ReadInt16();
            BitsPerSample = reader.ReadInt16();

            if (FormatTag == WaveFormatTags.WaveFormatExtensible && parser.FormatExtensibleParser != null)
            {
                long startOffset = reader.BaseStream.Position + 2;
                Ext = parser.FormatExtensibleParser(parser, reader);

                long endOffset = startOffset + Ext.Size;
                int remainingBytes = (int)Math.Max(endOffset - reader.BaseStream.Position, 0);
                Ext.Extra = reader.ReadBytes(remainingBytes);
            }
        }

        public static WaveFmtChunk Parse(RiffParser parser, BinaryReader reader) => new WaveFmtChunk(parser, reader);
    }

    public class WaveSmplChunk : RiffSubChunk
    {
        public int Manufacturer { get; set; }
        public int Product { get; set; }
        public int SamplePeriod { get; set; }
        public int MidiUnityNote { get; set; }
        public int MidiPitchFraction { get; set; }
        public int SmpteFormat { get; set; }
        public int SmpteOffset { get; set; }
        public int SampleLoops { get; set; }
        public int SamplerData { get; set; }
        public SampleLoop[] Loops { get; set; }

        protected WaveSmplChunk(BinaryReader reader) : base(reader)
        {
            Manufacturer = reader.ReadInt32();
            Product = reader.ReadInt32();
            SamplePeriod = reader.ReadInt32();
            MidiUnityNote = reader.ReadInt32();
            MidiPitchFraction = reader.ReadInt32();
            SmpteFormat = reader.ReadInt32();
            SmpteOffset = reader.ReadInt32();
            SampleLoops = reader.ReadInt32();
            SamplerData = reader.ReadInt32();
            Loops = new SampleLoop[SampleLoops];

            for (int i = 0; i < SampleLoops; i++)
            {
                Loops[i] = new SampleLoop
                {
                    CuePointId = reader.ReadInt32(),
                    Type = reader.ReadInt32(),
                    Start = reader.ReadInt32(),
                    End = reader.ReadInt32(),
                    Fraction = reader.ReadInt32(),
                    PlayCount = reader.ReadInt32()
                };
            }
        }

        public static WaveSmplChunk Parse(RiffParser parser, BinaryReader reader) => new WaveSmplChunk(reader);
    }

    public class SampleLoop
    {
        public int CuePointId { get; set; }
        public int Type { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Fraction { get; set; }
        public int PlayCount { get; set; }
    }

    public class WaveFactChunk : RiffSubChunk
    {
        public int SampleCount { get; set; }

        protected WaveFactChunk(BinaryReader reader) : base(reader)
        {
            SampleCount = reader.ReadInt32();
        }

        public static WaveFactChunk Parse(RiffParser parser, BinaryReader reader) => new WaveFactChunk(reader);
    }

    public class WaveDataChunk : RiffSubChunk
    {
        public byte[] Data { get; set; }

        protected WaveDataChunk(RiffParser parser, BinaryReader reader) : base(reader)
        {
            if (parser.ReadDataChunk)
            {
                Data = reader.ReadBytes(SubChunkSize);
            }
        }

        public static WaveDataChunk Parse(RiffParser parser, BinaryReader reader) => new WaveDataChunk(parser, reader);
    }

    public class RiffParser
    {
        public RiffChunk RiffChunk { get; set; }
        public bool ReadDataChunk { get; set; } = true;
        private Dictionary<string, RiffSubChunk> SubChunks { get; } = new Dictionary<string, RiffSubChunk>();

        private Dictionary<string, Func<RiffParser, BinaryReader, RiffSubChunk>> RegisteredSubChunks { get; } =
            new Dictionary<string, Func<RiffParser, BinaryReader, RiffSubChunk>>
            {
                ["fmt "] = WaveFmtChunk.Parse,
                ["smpl"] = WaveSmplChunk.Parse,
                ["fact"] = WaveFactChunk.Parse,
                ["data"] = WaveDataChunk.Parse
            };

        public Func<RiffParser, BinaryReader, WaveFormatExtensible> FormatExtensibleParser { get; set; } = WaveFormatExtensible.Parse;

        public void RegisterSubChunk(string id, Func<RiffParser, BinaryReader, RiffSubChunk> subChunkReader)
        {
            if (id.Length != 4)
            {
                throw new NotSupportedException("Subchunk ID must be 4 characters long");
            }

            RegisteredSubChunks[id] = subChunkReader;
        }

        public void ParseRiff(Stream file)
        {
            using (BinaryReader reader = new BinaryReader(file))
            {
                RiffChunk = RiffChunk.Parse(reader);
                SubChunks.Clear();

                // Size is counted from after the ChunkSize field, not the RiffType field
                long startOffset = reader.BaseStream.Position - 4;
                long endOffset = startOffset + RiffChunk.Size;

                // Make sure 8 bytes are available for the subchunk header
                while (reader.BaseStream.Position + 8 < endOffset)
                {
                    RiffSubChunk subChunk = ParseSubChunk(reader);
                    SubChunks[subChunk.SubChunkId] = subChunk;
                }
            }
        }

        public List<RiffSubChunk> GetAllSubChunks() => SubChunks.Values.ToList();

        public T GetSubChunk<T>(string id) where T : RiffSubChunk
        {
            RiffSubChunk chunk;
            SubChunks.TryGetValue(id, out chunk);
            return chunk as T;
        }

        private RiffSubChunk ParseSubChunk(BinaryReader reader)
        {
            string id = Utils.ReadUTF8String(reader, 4);
            reader.BaseStream.Position -= 4;
            long startOffset = reader.BaseStream.Position + 8;
            Func<RiffParser, BinaryReader, RiffSubChunk> parser;
            RiffSubChunk subChunk = RegisteredSubChunks.TryGetValue(id, out parser) ? parser(this, reader) : new RiffSubChunk(reader);

            long endOffset = startOffset + subChunk.SubChunkSize;
            int remainingBytes = (int)Math.Max(endOffset - reader.BaseStream.Position, 0);
            subChunk.Extra = reader.ReadBytes(remainingBytes);

            reader.BaseStream.Position = endOffset + (endOffset & 1); // Subchunks are 2-byte aligned
            return subChunk;
        }
    }

    internal class At9FactChunk : WaveFactChunk
    {
        public int InputOverlapDelaySamples { get; set; }
        public int EncoderDelaySamples { get; set; }

        protected At9FactChunk(BinaryReader reader) : base(reader)
        {
            InputOverlapDelaySamples = reader.ReadInt32();
            EncoderDelaySamples = reader.ReadInt32();
        }

        public static At9FactChunk ParseAt9(RiffParser parser, BinaryReader reader) => new At9FactChunk(reader);
    }

    internal class At9WaveExtensible : WaveFormatExtensible
    {
        public int VersionInfo { get; set; }
        public byte[] ConfigData { get; set; }
        public int Reserved { get; set; }

        protected At9WaveExtensible(BinaryReader reader) : base(reader)
        {
            VersionInfo = reader.ReadInt32();
            ConfigData = reader.ReadBytes(4);
            Reserved = reader.ReadInt32();
        }

        public static At9WaveExtensible ParseAt9(RiffParser parser, BinaryReader reader) =>
            new At9WaveExtensible(reader);
    }

    public static class MediaSubtypes
    {
        public static Guid MediaSubtypePcm { get; } = new Guid("00000001-0000-0010-8000-00AA00389B71");
        public static Guid MediaSubtypeAtrac9 { get; } = new Guid("47E142D2-36BA-4d8d-88FC-61654F8C836C");
    }

    internal class At9DataChunk : RiffSubChunk
    {
        public int FrameCount { get; set; }
        public byte[][] AudioData { get; set; }

        public At9DataChunk(RiffParser parser, BinaryReader reader) : base(reader)
        {
            // Do not trust the BlockAlign field in the fmt chunk to equal the superframe size.
            // Some AT9 files have an invalid number in there.
            // Calculate the size using the ATRAC9 DataConfig instead.

            At9WaveExtensible ext = parser.GetSubChunk<WaveFmtChunk>("fmt ")?.Ext as At9WaveExtensible;
            if (ext == null)
                throw new InvalidDataException("fmt chunk must come before data chunk");

            At9FactChunk fact = parser.GetSubChunk<At9FactChunk>("fact");
            if (fact == null)
                throw new InvalidDataException("fact chunk must come before data chunk");

            var config = new LibAtrac9.Atrac9Config(ext.ConfigData);
            FrameCount = (fact.SampleCount + fact.EncoderDelaySamples).DivideByRoundUp(config.SuperframeSamples);
            int dataSize = FrameCount * config.SuperframeBytes;

            if (dataSize > reader.BaseStream.Length - reader.BaseStream.Position)
            {
                throw new InvalidDataException("Required AT9 length is greater than the number of bytes remaining in the file.");
            }

            AudioData = reader.BaseStream.DeInterleave(dataSize, config.SuperframeBytes, FrameCount);
        }

        public static At9DataChunk ParseAt9(RiffParser parser, BinaryReader reader) => new At9DataChunk(parser, reader);
    }
}
