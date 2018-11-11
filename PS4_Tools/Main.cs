/* Copyright (c)  2018 TheDarkporgramer
*
*
* 
*/

using GameArchives;
using LibArchiveExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.IO.Compression;
using DiscUtils.Iso9660;

#region << VGAudio >>
using VGAudio.Codecs.Atrac9;
using VGAudio.Formats;
using VGAudio.Formats.Atrac9;
using VGAudio.Utilities.Riff;
using VGAudio.Containers.At9;
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
#endregion << VGAudio >>

using Newtonsoft.Json;

namespace PS4_Tools
{
    public class PS4_Tools
    {
        public static string AppCommonPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("PS4_Tools.dll", "");
        }

        /// <summary>
        /// Deletes all files within a Directory and also deletes a Directory
        /// </summary>
        /// <param name="target_dir"></param>
        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, false);
            }
            catch (Exception ex)
            {
                //we dont log anything here it should be okay
            }
        }
    }

    public class SELF
    {

    }

    public class Media
    {
        public class Atrac9
        {

            public static void LoadAt9(string at9file)
            {
                bool readAudioData = true;
                byte[] configData = new byte[4] { 0xFE, 0x18, 0x28, 0x00 };

                LibAtrac9.Atrac9Config config = new LibAtrac9.Atrac9Config(configData);

                // Initialize the decoder
                var decoder = new LibAtrac9.Atrac9Decoder();
                decoder.Initialize(configData);

                // Create a buffer for the output PCM
                var pcmBuffer = new short[decoder.Config.ChannelCount][];
                for (int i = 0; i < pcmBuffer.Length; i++)
                {
                    pcmBuffer[i] = new short[decoder.Config.SuperframeSamples];
                }

                // Decode each superframe
                //for (int i = 0; i < atrac9Data.Length; i++)
                //{
                //    decoder.Decode(atrac9Data[i], pcmBuffer);

                //    // Use the decoded audio in pcmBuffer however you want
                //}

                VGAudio.Containers.At9.At9Reader reader = new VGAudio.Containers.At9.At9Reader();
                reader.Read(new System.IO.FileStream(at9file, FileMode.Open, FileAccess.Read));

                reader.ReadWithConfig(new System.IO.FileStream(at9file, FileMode.Open, FileAccess.Read));

            }
        }
    }

    public class Image
    {
        /// <summary>
        /// PS4 PNG Image Handling
        /// </summary>
        public class PNG
        {
            /// <summary>
            /// Resize the image to the specified width and height.
            /// </summary>
            /// <param name="image">The image to resize.</param>
            /// <param name="width">The width to resize to.</param>
            /// <param name="height">The height to resize to.</param>
            /// <returns>The resized image.</returns>
            private static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
            {
                var destRect = new System.Drawing.Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }

            /// <summary>
            /// this convers images to 24bbp
            /// </summary>
            /// <param name="img"></param>
            /// <returns></returns>
            private static Bitmap ConvertTo24bpp(Bitmap img)
            {
                var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(bmp))
                    gr.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height));
                return bmp;
            }

            /// <summary>
            /// Converts a file location of a bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Original Bitmap Location</param>
            /// <param name="outputfile">Original Bitmap Location</param>
            public void Create_PS4_Compatible_PNG(string inputfile, string outputfile)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                returnbmp.Save(outputfile);//saves the file location
            }

            /// <summary>
            /// Converts a file location of a bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Original Bitmap Location</param>
            /// <param name="outputfile">Original Bitmap Location</param>
            /// <returns>PS4 Comptatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(string inputfile)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //reutrn new butmap
                return returnbmp;
            }

            /// <summary>
            /// Converts a Bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Normal Bitmap</param>
            /// <returns>PS4 Comptatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(Bitmap inputfile)
            {
                //create new bitmap
                Bitmap returnbmp = inputfile;

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //return ps4 compatible one
                return returnbmp;

            }

            /// <summary>
            /// Converts a stream to a ps4 compatible bitmap
            /// </summary>
            /// <param name="inputfile">Stream</param>
            /// <returns>PS4 COmpatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(Stream inputfile)
            {
                //create new bitmap
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //return ps4 compatible one
                return returnbmp;
            }
        }

        /// <summary>
        /// PS4 DDS Image Handling
        /// </summary>
        public class DDS
        {
            public static void SavePNGFromDDS(string DDSFilePath, string savepath)
            {
                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(savepath);

            }

            public static Stream GetStreamFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();

                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(rtnStream);

                return rtnStream;

            }

            public static Bitmap GetBitmapFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();

                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(rtnStream);

                System.Drawing.Image rtnimage = System.Drawing.Image.FromStream(rtnStream);
                return (Bitmap)rtnimage;
            }
        }

        /// <summary>
        /// Gim File Handling (Haven't seen one inside PS4 but whatever)
        /// </summary>
        public class GIMImages
        {
            #region << Gim >>

            public interface IPixelOrderIterator
            {
                int X { get; }
                int Y { get; }
                void Next();
            }

            public class TiledPixelOrderIterator : IPixelOrderIterator
            {
                int Width;
                int Height;

                int CurrentTileX;
                int CurrentTileY;
                int CounterInTile;

                int TileWidth;
                int TileHeight;

                public TiledPixelOrderIterator(int width, int height, int tileWidth, int tileHeight)
                {
                    Width = width;
                    Height = height;
                    CurrentTileX = 0;
                    CurrentTileY = 0;
                    CounterInTile = 0;
                    TileWidth = tileWidth;
                    TileHeight = tileHeight;
                }

                public int X { get { return CurrentTileX + (CounterInTile % TileWidth); } }
                public int Y { get { return CurrentTileY + (CounterInTile / TileWidth); } }

                public void Next()
                {
                    ++CounterInTile;

                    if (CounterInTile == TileWidth * TileHeight)
                    {
                        CounterInTile = 0;
                        CurrentTileX += TileWidth;
                        if (CurrentTileX >= Width)
                        {
                            CurrentTileX = 0;
                            CurrentTileY += TileHeight;
                        }
                    }
                }
            }

            public class LinearPixelOrderIterator : IPixelOrderIterator
            {
                int Width;
                int Height;
                int Counter;

                public LinearPixelOrderIterator(int width, int height)
                {
                    Width = width;
                    Height = height;
                    Counter = 0;
                }

                public int X { get { return Counter % Width; } }
                public int Y { get { return Counter / Width; } }

                public void Next()
                {
                    ++Counter;
                }
            }

            class Util
            {
                public static void CopyByteArrayPart(IList<byte> from, int locationFrom, IList<byte> to, int locationTo, int count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        to[locationTo + i] = from[locationFrom + i];
                    }
                }
            }


            class FileInfoSection : ISection
            {
                public ushort Type;
                public ushort Unknown;
                public uint PartSizeDuplicate;
                public uint PartSize;
                public uint Unknown2;

                public byte[] FileInfo;

                public FileInfoSection(byte[] File, int Offset)
                {
                    Type = BitConverter.ToUInt16(File, Offset);
                    Unknown = BitConverter.ToUInt16(File, Offset + 0x02);
                    PartSizeDuplicate = BitConverter.ToUInt32(File, Offset + 0x04);
                    PartSize = BitConverter.ToUInt32(File, Offset + 0x08);
                    Unknown2 = BitConverter.ToUInt32(File, Offset + 0x0C);

                    uint size = PartSize - 0x10;
                    FileInfo = new byte[size];
                    Util.CopyByteArrayPart(File, Offset + 0x10, FileInfo, 0, (int)size);
                }

                public uint GetPartSize()
                {
                    return PartSize;
                }


                public void Recalculate(int NewFilesize)
                {
                    PartSize = (uint)FileInfo.Length + 0x10;
                    PartSizeDuplicate = PartSize;
                }


                public byte[] Serialize()
                {
                    List<byte> serialized = new List<byte>((int)PartSize);
                    serialized.AddRange(BitConverter.GetBytes(Type));
                    serialized.AddRange(BitConverter.GetBytes(Unknown));
                    serialized.AddRange(BitConverter.GetBytes(PartSizeDuplicate));
                    serialized.AddRange(BitConverter.GetBytes(PartSize));
                    serialized.AddRange(BitConverter.GetBytes(Unknown2));
                    serialized.AddRange(FileInfo);
                    return serialized.ToArray();
                }
            }

            class Splitter
            {
                public static int Split(List<string> args)
                {
                    string Filename = args[0];
                    GIM[] gims = new GIM[3];
                    gims[0] = new GIM(Filename); ;
                    gims[1] = new GIM(Filename); ;
                    gims[2] = new GIM(Filename); ;
                    System.IO.File.WriteAllBytes(
                        System.IO.Path.GetDirectoryName(Filename) + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(Filename) + "_resave" + System.IO.Path.GetExtension(Filename),
                        gims[0].Serialize());

                    for (int i = 0; i < gims.Length; ++i)
                    {
                        GIM gim = gims[i];
                        gim.ReduceToOneImage(i);
                        System.IO.File.WriteAllBytes(
                            System.IO.Path.GetDirectoryName(Filename) + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(Filename) + i.ToString() + System.IO.Path.GetExtension(Filename),
                            gim.Serialize());
                    }

                    return 0;
                }
            }

            class Program
            {
                public static int Homogenize(List<string> args)
                {
                    if (args.Count == 0)
                    {
                        Console.WriteLine("HomogenizePalette in.gim [out.gim]");
                        Console.WriteLine("Overwrites in.gim when no out.gim is provided.");
                    }

                    string infilename = args[0];
                    string outfilename = args.Count > 1 ? args[1] : args[0];

                    GIM gim = new GIM(infilename);
                    gim.HomogenizePalette();
                    System.IO.File.WriteAllBytes(outfilename, gim.Serialize());


                    return 0;
                }
            }

            public class GimToPng
            {
                public static int Execute(List<string> args)
                {
                    if (args.Count == 0)
                    {
                        Console.WriteLine("Usage: GimToPng file.gim");
                        return -1;
                    }

                    string filename = args[0];
                    List<string> convertedFilenames = ConvertGimFileToPngFiles(filename);
                    return (convertedFilenames != null && convertedFilenames.Count > 0) ? 0 : -1;
                }

                public static List<string> ConvertGimFileToPngFiles(string filename)
                {
                    GIM gim = new GIM(filename);
                    int filenum = 0;
                    List<string> names = new List<string>();
                    foreach (Bitmap bmp in gim.ConvertToBitmaps())
                    {
                        string newname = filename + "." + filenum + ".png";
                        bmp.Save(newname);
                        names.Add(newname);
                    }
                    return names;
                }
            }

            class EndOfImageSection : ISection
            {
                public ushort Type;
                public ushort Unknown;
                public uint EndOfImageAddress;
                public uint PartSize;
                public uint Unknown2;

                public EndOfImageSection(byte[] File, int Offset)
                {
                    Type = BitConverter.ToUInt16(File, Offset);
                    Unknown = BitConverter.ToUInt16(File, Offset + 0x02);
                    EndOfImageAddress = BitConverter.ToUInt32(File, Offset + 0x04);
                    PartSize = BitConverter.ToUInt32(File, Offset + 0x08);
                    Unknown2 = BitConverter.ToUInt32(File, Offset + 0x0C);
                }


                public uint GetPartSize()
                {
                    return PartSize;
                }


                public void Recalculate(int NewFilesize)
                {
                    EndOfImageAddress = (uint)NewFilesize;
                }


                public byte[] Serialize()
                {
                    List<byte> serialized = new List<byte>((int)PartSize);
                    serialized.AddRange(BitConverter.GetBytes(Type));
                    serialized.AddRange(BitConverter.GetBytes(Unknown));
                    serialized.AddRange(BitConverter.GetBytes(EndOfImageAddress));
                    serialized.AddRange(BitConverter.GetBytes(PartSize));
                    serialized.AddRange(BitConverter.GetBytes(Unknown2));
                    return serialized.ToArray();
                }
            }


            class EndOfFileSection : ISection
            {

                public ushort Type;
                public ushort Unknown;
                public uint EndOfFileAddress;
                public uint PartSize;
                public uint Unknown2;

                public EndOfFileSection(byte[] File, int Offset)
                {
                    Type = BitConverter.ToUInt16(File, Offset);
                    Unknown = BitConverter.ToUInt16(File, Offset + 0x02);
                    EndOfFileAddress = BitConverter.ToUInt32(File, Offset + 0x04);
                    PartSize = BitConverter.ToUInt32(File, Offset + 0x08);
                    Unknown2 = BitConverter.ToUInt32(File, Offset + 0x0C);
                }


                public uint GetPartSize()
                {
                    return PartSize;
                }


                public void Recalculate(int NewFilesize)
                {
                    EndOfFileAddress = (uint)NewFilesize;
                }


                public byte[] Serialize()
                {
                    List<byte> serialized = new List<byte>((int)PartSize);
                    serialized.AddRange(BitConverter.GetBytes(Type));
                    serialized.AddRange(BitConverter.GetBytes(Unknown));
                    serialized.AddRange(BitConverter.GetBytes(EndOfFileAddress));
                    serialized.AddRange(BitConverter.GetBytes(PartSize));
                    serialized.AddRange(BitConverter.GetBytes(Unknown2));
                    return serialized.ToArray();
                }
            }

            class HeaderSection : ISection
            {
                public byte[] Header;
                public HeaderSection(byte[] File, int Offset)
                {
                    Header = new byte[0x10];

                    Util.CopyByteArrayPart(File, Offset, Header, 0, 0x10);
                }

                public uint GetPartSize()
                {
                    return 0x10;
                }

                public void Recalculate(int NewFilesize)
                {
                    return;
                }


                public byte[] Serialize()
                {
                    return Header;
                }
            }

            class PaletteSection : ISection
            {
                public int Offset;

                public ushort Type;
                public ushort Unknown;
                public uint PartSizeDuplicate;
                public uint PartSize;
                public uint Unknown2;

                public ushort DataOffset;
                public ushort Unknown3;
                public ImageFormat Format;
                public ushort Unknown4;
                public ushort ColorDepth;
                public ushort Unknown5;
                public ushort Unknown6;
                public ushort Unknown7;

                public ushort Unknown8;
                public ushort Unknown9;
                public ushort Unknown10;
                public ushort Unknown11;
                public uint Unknown12;
                public uint Unknown13;

                public uint PartSizeMinus0x10;
                public uint Unknown14;
                public ushort Unknown15;
                public ushort LayerCount;
                public ushort Unknown17;
                public ushort FrameCount;

                public uint[] PaletteOffsets;
                public byte[][] PalettesRawBytes;
                public List<List<uint>> Palettes;


                public uint PaletteCount;

                public PaletteSection(byte[] File, int Offset)
                {
                    this.Offset = Offset;


                    Type = BitConverter.ToUInt16(File, Offset);
                    Unknown = BitConverter.ToUInt16(File, Offset + 0x02);
                    PartSizeDuplicate = BitConverter.ToUInt32(File, Offset + 0x04);
                    PartSize = BitConverter.ToUInt32(File, Offset + 0x08);
                    Unknown2 = BitConverter.ToUInt32(File, Offset + 0x0C);

                    DataOffset = BitConverter.ToUInt16(File, Offset + 0x10);
                    Unknown3 = BitConverter.ToUInt16(File, Offset + 0x12);
                    Format = (ImageFormat)BitConverter.ToUInt16(File, Offset + 0x14);
                    Unknown4 = BitConverter.ToUInt16(File, Offset + 0x16);
                    ColorDepth = BitConverter.ToUInt16(File, Offset + 0x18);
                    Unknown5 = BitConverter.ToUInt16(File, Offset + 0x1A);
                    Unknown6 = BitConverter.ToUInt16(File, Offset + 0x1C);
                    Unknown7 = BitConverter.ToUInt16(File, Offset + 0x1E);

                    Unknown8 = BitConverter.ToUInt16(File, Offset + 0x20);
                    Unknown9 = BitConverter.ToUInt16(File, Offset + 0x22);
                    Unknown10 = BitConverter.ToUInt16(File, Offset + 0x24);
                    Unknown11 = BitConverter.ToUInt16(File, Offset + 0x26);
                    Unknown12 = BitConverter.ToUInt32(File, Offset + 0x28);
                    Unknown13 = BitConverter.ToUInt32(File, Offset + 0x2C);

                    PartSizeMinus0x10 = BitConverter.ToUInt32(File, Offset + 0x30);
                    Unknown14 = BitConverter.ToUInt32(File, Offset + 0x34);
                    Unknown15 = BitConverter.ToUInt16(File, Offset + 0x38);
                    LayerCount = BitConverter.ToUInt16(File, Offset + 0x3A);
                    Unknown17 = BitConverter.ToUInt16(File, Offset + 0x3C);
                    FrameCount = BitConverter.ToUInt16(File, Offset + 0x3E);

                    PaletteCount = Math.Max(LayerCount, FrameCount);
                    PaletteOffsets = new uint[PaletteCount];
                    for (int i = 0; i < PaletteCount; ++i)
                    {
                        PaletteOffsets[i] = BitConverter.ToUInt32(File, Offset + 0x40 + i * 0x04);
                    }


                    PalettesRawBytes = new byte[PaletteCount][];
                    for (int i = 0; i < PaletteOffsets.Length; ++i)
                    {
                        uint poffs = PaletteOffsets[i];
                        int size = ColorDepth * GetBytePerColor();
                        PalettesRawBytes[i] = new byte[size];

                        Util.CopyByteArrayPart(File, Offset + (int)poffs + 0x10, PalettesRawBytes[i], 0, size);
                    }


                    Palettes = new List<List<uint>>();
                    foreach (byte[] pal in PalettesRawBytes)
                    {
                        int BytePerColor = GetBytePerColor();
                        List<uint> IndividualPalette = new List<uint>();
                        for (int i = 0; i < pal.Length; i += BytePerColor)
                        {
                            uint color = 0;
                            if (BytePerColor == 4)
                            {
                                color = BitConverter.ToUInt32(pal, i);
                            }
                            else if (BytePerColor == 2)
                            {
                                color = BitConverter.ToUInt16(pal, i);
                            }
                            IndividualPalette.Add(color);
                        }
                        Palettes.Add(IndividualPalette);
                    }


                    return;
                }

                public int GetBytePerColor()
                {
                    if (Format == ImageFormat.RGBA4444)
                    {
                        return 2;
                    }
                    return 4;
                }


                public uint GetPartSize()
                {
                    return PartSize;
                }
                public void Recalculate(int NewFilesize)
                {
                    if (PaletteOffsets.Length != PalettesRawBytes.Length)
                    {
                        PaletteOffsets = new uint[PalettesRawBytes.Length];
                    }
                    uint totalLength = 0;
                    for (int i = 0; i < PalettesRawBytes.Length; ++i)
                    {
                        PaletteOffsets[i] = totalLength + 0x40;
                        totalLength += (uint)PalettesRawBytes[i].Length;
                    }

                    PartSize = totalLength + 0x50;
                    PartSizeDuplicate = totalLength + 0x50;
                    PartSizeMinus0x10 = totalLength + 0x40;
                    LayerCount = 1;
                    FrameCount = 1;
                }


                public byte[] Serialize()
                {
                    List<byte> serialized = new List<byte>((int)PartSize);
                    serialized.AddRange(BitConverter.GetBytes(Type));
                    serialized.AddRange(BitConverter.GetBytes(Unknown));
                    serialized.AddRange(BitConverter.GetBytes(PartSizeDuplicate));
                    serialized.AddRange(BitConverter.GetBytes(PartSize));
                    serialized.AddRange(BitConverter.GetBytes(Unknown2));

                    serialized.AddRange(BitConverter.GetBytes(DataOffset));
                    serialized.AddRange(BitConverter.GetBytes(Unknown3));
                    serialized.AddRange(BitConverter.GetBytes((ushort)Format));
                    serialized.AddRange(BitConverter.GetBytes(Unknown4));
                    serialized.AddRange(BitConverter.GetBytes(ColorDepth));
                    serialized.AddRange(BitConverter.GetBytes(Unknown5));
                    serialized.AddRange(BitConverter.GetBytes(Unknown6));
                    serialized.AddRange(BitConverter.GetBytes(Unknown7));

                    serialized.AddRange(BitConverter.GetBytes(Unknown8));
                    serialized.AddRange(BitConverter.GetBytes(Unknown9));
                    serialized.AddRange(BitConverter.GetBytes(Unknown10));
                    serialized.AddRange(BitConverter.GetBytes(Unknown11));
                    serialized.AddRange(BitConverter.GetBytes(Unknown12));
                    serialized.AddRange(BitConverter.GetBytes(Unknown13));

                    serialized.AddRange(BitConverter.GetBytes(PartSizeMinus0x10));
                    serialized.AddRange(BitConverter.GetBytes(Unknown14));
                    serialized.AddRange(BitConverter.GetBytes(Unknown15));
                    serialized.AddRange(BitConverter.GetBytes(LayerCount));
                    serialized.AddRange(BitConverter.GetBytes(Unknown17));
                    serialized.AddRange(BitConverter.GetBytes(FrameCount));

                    for (int i = 0; i < PaletteOffsets.Length; ++i)
                    {
                        serialized.AddRange(BitConverter.GetBytes(PaletteOffsets[i]));
                    }
                    while (serialized.Count % 16 != 0)
                    {
                        serialized.Add(0x00);
                    }
                    int BytePerColor = GetBytePerColor();
                    foreach (List<uint> pal in Palettes)
                    {
                        foreach (uint col in pal)
                        {
                            if (BytePerColor == 4)
                            {
                                serialized.AddRange(BitConverter.GetBytes(col));
                            }
                            else if (BytePerColor == 2)
                            {
                                serialized.AddRange(BitConverter.GetBytes((ushort)col));
                            }
                        }
                    }
                    return serialized.ToArray();
                }
            }

            enum ImageFormat : short
            {
                RGBA5650 = 0,
                RGBA5551 = 1,
                RGBA4444 = 2,
                RGBA8888 = 3,
                Index4 = 4,
                Index8 = 5,
                Index16 = 6,
                Index32 = 7,
            }

            enum PixelOrder : short
            {
                Normal = 0,
                Faster = 1
            }

            class ImageSection : ISection
            {
                public int Offset;

                public ushort Type;
                public ushort Unknown;
                public uint PartSizeDuplicate;
                public uint PartSize;
                public uint Unknown2;

                public ushort DataOffset;
                public ushort Unknown3;
                public ImageFormat Format;
                public PixelOrder PxOrder;
                public ushort Width;
                public ushort Height;
                public ushort ColorDepth;
                public ushort Unknown7;

                public ushort Unknown8;
                public ushort Unknown9;
                public ushort Unknown10;
                public ushort Unknown11;
                public uint Unknown12;
                public uint Unknown13;

                public uint PartSizeMinus0x10;
                public uint Unknown14;
                public ushort Unknown15;
                public ushort LayerCount;
                public ushort Unknown17;
                public ushort FrameCount;

                public uint[] ImageOffsets;
                public byte[][] ImagesRawBytes;
                public List<List<uint>> Images;


                public uint ImageCount;

                public ImageSection(byte[] File, int Offset)
                {
                    this.Offset = Offset;


                    Type = BitConverter.ToUInt16(File, Offset);
                    Unknown = BitConverter.ToUInt16(File, Offset + 0x02);
                    PartSizeDuplicate = BitConverter.ToUInt32(File, Offset + 0x04);
                    PartSize = BitConverter.ToUInt32(File, Offset + 0x08);
                    Unknown2 = BitConverter.ToUInt32(File, Offset + 0x0C);

                    DataOffset = BitConverter.ToUInt16(File, Offset + 0x10);
                    Unknown3 = BitConverter.ToUInt16(File, Offset + 0x12);
                    Format = (ImageFormat)BitConverter.ToUInt16(File, Offset + 0x14);
                    PxOrder = (PixelOrder)BitConverter.ToUInt16(File, Offset + 0x16);
                    Width = BitConverter.ToUInt16(File, Offset + 0x18);
                    Height = BitConverter.ToUInt16(File, Offset + 0x1A);
                    ColorDepth = BitConverter.ToUInt16(File, Offset + 0x1C);
                    Unknown7 = BitConverter.ToUInt16(File, Offset + 0x1E);

                    Unknown8 = BitConverter.ToUInt16(File, Offset + 0x20);
                    Unknown9 = BitConverter.ToUInt16(File, Offset + 0x22);
                    Unknown10 = BitConverter.ToUInt16(File, Offset + 0x24);
                    Unknown11 = BitConverter.ToUInt16(File, Offset + 0x26);
                    Unknown12 = BitConverter.ToUInt32(File, Offset + 0x28);
                    Unknown13 = BitConverter.ToUInt32(File, Offset + 0x2C);

                    PartSizeMinus0x10 = BitConverter.ToUInt32(File, Offset + 0x30);
                    Unknown14 = BitConverter.ToUInt32(File, Offset + 0x34);
                    Unknown15 = BitConverter.ToUInt16(File, Offset + 0x38);
                    LayerCount = BitConverter.ToUInt16(File, Offset + 0x3A);
                    Unknown17 = BitConverter.ToUInt16(File, Offset + 0x3C);
                    FrameCount = BitConverter.ToUInt16(File, Offset + 0x3E);

                    ImageCount = Math.Max(LayerCount, FrameCount);
                    ImageOffsets = new uint[ImageCount];
                    for (int i = 0; i < ImageCount; ++i)
                    {
                        ImageOffsets[i] = BitConverter.ToUInt32(File, Offset + 0x40 + i * 0x04);
                    }


                    ImagesRawBytes = new byte[ImageCount][];
                    for (int i = 0; i < ImageOffsets.Length; ++i)
                    {
                        uint poffs = ImageOffsets[i];
                        uint nextpoffs;
                        if (i == ImageOffsets.Length - 1)
                        {
                            nextpoffs = PartSizeMinus0x10;
                        }
                        else
                        {
                            nextpoffs = ImageOffsets[i + 1];
                        }
                        uint size = nextpoffs - poffs;
                        ImagesRawBytes[i] = new byte[size];

                        Util.CopyByteArrayPart(File, Offset + (int)poffs + 0x10, ImagesRawBytes[i], 0, (int)size);
                    }



                    Images = new List<List<uint>>();
                    foreach (byte[] img in ImagesRawBytes)
                    {
                        int BitPerPixel = GetBitPerPixel();
                        List<uint> IndividualImage = new List<uint>();
                        for (int cnt = 0; cnt < img.Length * 8; cnt += BitPerPixel)
                        {
                            uint color = 0;
                            int i = cnt / 8;
                            switch (BitPerPixel)
                            {
                                case 4:
                                    if (cnt % 8 != 0)
                                    {
                                        color = (img[i] & 0xF0u) >> 4;
                                    }
                                    else
                                    {
                                        color = (img[i] & 0x0Fu);
                                    }
                                    break;
                                case 8:
                                    color = img[i];
                                    break;
                                case 16:
                                    color = BitConverter.ToUInt16(img, i);
                                    break;
                                case 32:
                                    color = BitConverter.ToUInt32(img, i);
                                    break;
                            }
                            IndividualImage.Add(color);
                        }
                        Images.Add(IndividualImage);
                    }


                    return;
                }

                public int GetBitPerPixel()
                {
                    switch (Format)
                    {
                        case ImageFormat.Index4:
                            return 4;
                        case ImageFormat.Index8:
                            return 8;
                        case ImageFormat.Index16:
                        case ImageFormat.RGBA4444:
                        case ImageFormat.RGBA5551:
                        case ImageFormat.RGBA5650:
                            return 16;
                        case ImageFormat.Index32:
                        case ImageFormat.RGBA8888:
                            return 32;
                    }
                    return 0;
                }

                public uint GetPartSize()
                {
                    return PartSize;
                }


                public void Recalculate(int NewFilesize)
                {
                    if (ImageOffsets.Length != ImagesRawBytes.Length)
                    {
                        ImageOffsets = new uint[ImagesRawBytes.Length];
                    }

                    uint totalLength = 0;
                    for (int i = 0; i < ImagesRawBytes.Length; ++i)
                    {
                        ImageOffsets[i] = totalLength + 0x40;
                        totalLength += (uint)ImagesRawBytes[i].Length;
                    }

                    PartSize = totalLength + 0x50;
                    PartSizeDuplicate = totalLength + 0x50;
                    PartSizeMinus0x10 = totalLength + 0x40;
                    LayerCount = 1;
                    FrameCount = 1;

                }

                private static Color ColorFromRGBA5650(uint color)
                {
                    int r = (int)(((color & 0x0000001F)) << 3);
                    int g = (int)(((color & 0x000007E0) >> 5) << 2);
                    int b = (int)(((color & 0x0000F800) >> 11) << 3);
                    return Color.FromArgb(0, r, g, b);
                }
                private static Color ColorFromRGBA5551(uint color)
                {
                    int r = (int)(((color & 0x0000001F)) << 3);
                    int g = (int)(((color & 0x000003E0) >> 5) << 3);
                    int b = (int)(((color & 0x00007C00) >> 10) << 3);
                    int a = (int)(((color & 0x00008000) >> 15) << 7);
                    return Color.FromArgb(a, r, g, b);
                }
                private static Color ColorFromRGBA4444(uint color)
                {
                    int r = (int)(((color & 0x0000000F)) << 4);
                    int g = (int)(((color & 0x000000F0) >> 4) << 4);
                    int b = (int)(((color & 0x00000F00) >> 8) << 4);
                    int a = (int)(((color & 0x0000F000) >> 12) << 4);
                    return Color.FromArgb(a, r, g, b);
                }
                private static Color ColorFromRGBA8888(uint color)
                {
                    int r = (int)((color & 0x000000FF));
                    int g = (int)((color & 0x0000FF00) >> 8);
                    int b = (int)((color & 0x00FF0000) >> 16);
                    int a = (int)((color & 0xFF000000) >> 24);
                    return Color.FromArgb(a, r, g, b);
                }

                public List<Bitmap> ConvertToBitmaps(PaletteSection psec)
                {
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    for (int i = 0; i < Images.Count; ++i)
                    {
                        int w = (ushort)(Width >> i);
                        int h = (ushort)(Height >> i);

                        Bitmap bmp = new Bitmap(w, h);

                        IPixelOrderIterator pixelPosition;
                        switch (PxOrder)
                        {
                            case PixelOrder.Normal:
                                pixelPosition = new LinearPixelOrderIterator(w, h);
                                break;
                            case PixelOrder.Faster:
                                pixelPosition = new GimPixelOrderFasterIterator(w, h, GetBitPerPixel());
                                break;
                            default:
                                throw new Exception("Unexpected pixel order: " + PxOrder);
                        }

                        for (int idx = 0; idx < Images[i].Count; ++idx)
                        {
                            uint rawcolor = Images[i][idx];
                            Color color;

                            switch (Format)
                            {
                                case ImageFormat.RGBA5650:
                                    color = ColorFromRGBA5650(rawcolor);
                                    break;
                                case ImageFormat.RGBA5551:
                                    color = ColorFromRGBA5551(rawcolor);
                                    break;
                                case ImageFormat.RGBA4444:
                                    color = ColorFromRGBA4444(rawcolor);
                                    break;
                                case ImageFormat.RGBA8888:
                                    color = ColorFromRGBA8888(rawcolor);
                                    break;
                                case ImageFormat.Index4:
                                case ImageFormat.Index8:
                                case ImageFormat.Index16:
                                case ImageFormat.Index32:
                                    switch (psec.Format)
                                    {
                                        case ImageFormat.RGBA5650:
                                            color = ColorFromRGBA5650(psec.Palettes[i][(int)rawcolor]);
                                            break;
                                        case ImageFormat.RGBA5551:
                                            color = ColorFromRGBA5551(psec.Palettes[i][(int)rawcolor]);
                                            break;
                                        case ImageFormat.RGBA4444:
                                            color = ColorFromRGBA4444(psec.Palettes[i][(int)rawcolor]);
                                            break;
                                        case ImageFormat.RGBA8888:
                                            color = ColorFromRGBA8888(psec.Palettes[i][(int)rawcolor]);
                                            break;
                                        default:
                                            throw new Exception("Unexpected palette color type: " + psec.Format);
                                    }
                                    break;
                                default:
                                    throw new Exception("Unexpected image color type: " + psec.Format);
                            }

                            if (pixelPosition.X < w && pixelPosition.Y < h)
                            {
                                bmp.SetPixel(pixelPosition.X, pixelPosition.Y, color);
                            }
                            pixelPosition.Next();
                        }
                        bitmaps.Add(bmp);
                    }
                    return bitmaps;
                }

                public byte[] Serialize()
                {
                    List<byte> serialized = new List<byte>((int)PartSize);
                    serialized.AddRange(BitConverter.GetBytes(Type));
                    serialized.AddRange(BitConverter.GetBytes(Unknown));
                    serialized.AddRange(BitConverter.GetBytes(PartSizeDuplicate));
                    serialized.AddRange(BitConverter.GetBytes(PartSize));
                    serialized.AddRange(BitConverter.GetBytes(Unknown2));

                    serialized.AddRange(BitConverter.GetBytes(DataOffset));
                    serialized.AddRange(BitConverter.GetBytes(Unknown3));
                    serialized.AddRange(BitConverter.GetBytes((ushort)Format));
                    serialized.AddRange(BitConverter.GetBytes((ushort)PxOrder));
                    serialized.AddRange(BitConverter.GetBytes(Width));
                    serialized.AddRange(BitConverter.GetBytes(Height));
                    serialized.AddRange(BitConverter.GetBytes(ColorDepth));
                    serialized.AddRange(BitConverter.GetBytes(Unknown7));

                    serialized.AddRange(BitConverter.GetBytes(Unknown8));
                    serialized.AddRange(BitConverter.GetBytes(Unknown9));
                    serialized.AddRange(BitConverter.GetBytes(Unknown10));
                    serialized.AddRange(BitConverter.GetBytes(Unknown11));
                    serialized.AddRange(BitConverter.GetBytes(Unknown12));
                    serialized.AddRange(BitConverter.GetBytes(Unknown13));

                    serialized.AddRange(BitConverter.GetBytes(PartSizeMinus0x10));
                    serialized.AddRange(BitConverter.GetBytes(Unknown14));
                    serialized.AddRange(BitConverter.GetBytes(Unknown15));
                    serialized.AddRange(BitConverter.GetBytes(LayerCount));
                    serialized.AddRange(BitConverter.GetBytes(Unknown17));
                    serialized.AddRange(BitConverter.GetBytes(FrameCount));

                    for (int i = 0; i < ImageOffsets.Length; ++i)
                    {
                        serialized.AddRange(BitConverter.GetBytes(ImageOffsets[i]));
                    }
                    while (serialized.Count % 16 != 0)
                    {
                        serialized.Add(0x00);
                    }

                    int BitPerPixel = GetBitPerPixel();
                    foreach (List<uint> img in Images)
                    {
                        for (int i = 0; i < img.Count; ++i)
                        {
                            uint col = img[i];
                            switch (BitPerPixel)
                            {
                                case 4:
                                    col = (img[i + 1] << 4) | (img[i]);
                                    serialized.Add((byte)col);
                                    ++i;
                                    break;
                                case 8:
                                    serialized.Add((byte)col);
                                    break;
                                case 16:
                                    serialized.AddRange(BitConverter.GetBytes((ushort)col));
                                    break;
                                case 32:
                                    serialized.AddRange(BitConverter.GetBytes(col));
                                    break;
                            }
                        }
                    }

                    return serialized.ToArray();
                }

                public void ConvertToTruecolor(int imageNumber, List<uint> Palette)
                {
                    for (int i = 0; i < Images[imageNumber].Count; ++i)
                    {
                        uint index = Images[imageNumber][i];
                        Images[imageNumber][i] = Palette[(int)index];
                    }
                }

                public void CovertToPaletted(int imageNumber, uint[] NewPalette)
                {
                    Dictionary<uint, uint> PaletteDict = new Dictionary<uint, uint>(NewPalette.Length);
                    for (uint i = 0; i < NewPalette.Length; ++i)
                    {
                        try
                        {
                            PaletteDict.Add(NewPalette[i], i);
                        }
                        catch (System.ArgumentException)
                        {
                            // if we reach a duplicate we *should* be at the end of our colors
                            break;
                        }
                    }

                    for (int i = 0; i < Images[imageNumber].Count; ++i)
                    {
                        uint color = Images[imageNumber][i];
                        uint index = PaletteDict[color];
                        Images[imageNumber][i] = index;
                    }
                }

                public void DiscardUnusedColorsPaletted(int imageNumber, PaletteSection paletteSection, int paletteNumber)
                {
                    List<uint> pal = paletteSection.Palettes[paletteNumber];
                    List<uint> img = Images[imageNumber];

                    bool[] usedPaletteEntries = new bool[pal.Count];
                    for (int i = 0; i < usedPaletteEntries.Length; ++i)
                    {
                        usedPaletteEntries[i] = false; // initialize array to false
                    }
                    for (int i = 0; i < img.Count; ++i)
                    {
                        usedPaletteEntries[img[i]] = true; // all used palette entries get set to true
                    }

                    // remap old palette entries to new ones by essentially skipping over all unused colors
                    uint[] remapTable = new uint[pal.Count];
                    uint counter = 0;
                    for (int i = 0; i < usedPaletteEntries.Length; ++i)
                    {
                        if (usedPaletteEntries[i])
                        {
                            remapTable[i] = counter;
                            counter++;
                        }
                        else
                        {
                            remapTable[i] = 0xFFFFFFFFu; // just making sure these aren't used
                        }
                    }

                    // remap the image
                    for (int i = 0; i < img.Count; ++i)
                    {
                        img[i] = remapTable[img[i]];
                    }

                    // generate the new palette
                    List<uint> newPal = new List<uint>((int)counter);
                    for (int i = 0; i < usedPaletteEntries.Length; ++i)
                    {
                        if (usedPaletteEntries[i])
                        {
                            newPal.Add(pal[i]);
                        }
                    }

                    paletteSection.Palettes[paletteNumber] = newPal;
                }
            }

            class GimPixelOrderFasterIterator : TiledPixelOrderIterator
            {
                public GimPixelOrderFasterIterator(int width, int height, int bpp) : base(width, height, 0x80 / bpp, 0x08) { }
            }

            interface ISection
            {
                uint GetPartSize();
                void Recalculate(int NewFilesize);
                byte[] Serialize();
            }
            public class GIM
            {

                byte[] File;
                List<ISection> Sections;

                public GIM(byte[] File)
                {
                    Initialize(File);
                }

                public GIM(string Filename)
                {
                    Initialize(System.IO.File.ReadAllBytes(Filename));
                }

                public void Initialize(byte[] File)
                {
                    this.File = File;
                    uint location = 0x10;

                    Sections = new List<ISection>();
                    Sections.Add(new HeaderSection(File, 0));
                    while (location < File.Length)
                    {
                        ushort CurrentType = BitConverter.ToUInt16(File, (int)location);
                        ISection section;
                        switch (CurrentType)
                        {
                            case 0x02:
                                section = new EndOfFileSection(File, (int)location);
                                break;
                            case 0x03:
                                section = new EndOfImageSection(File, (int)location);
                                break;
                            case 0x04:
                                section = new ImageSection(File, (int)location);
                                break;
                            case 0x05:
                                section = new PaletteSection(File, (int)location);
                                break;
                            case 0xFF:
                                section = new FileInfoSection(File, (int)location);
                                break;
                            default:
                                throw new Exception("Invalid Section Type");
                        }

                        Sections.Add(section);
                        location += section.GetPartSize();
                    }
                }

                public uint GetTotalFilesize()
                {
                    uint totalFilesize = 0;
                    foreach (var section in Sections)
                    {
                        totalFilesize += section.GetPartSize();
                    }
                    return totalFilesize;
                }

                public void ReduceToOneImage(int imageNumber)
                {
                    foreach (var section in Sections)
                    {
                        if (section.GetType() == typeof(ImageSection))
                        {
                            ImageSection isec = (ImageSection)section;
                            byte[] img = isec.ImagesRawBytes[imageNumber];
                            isec.ImagesRawBytes = new byte[1][];
                            isec.ImagesRawBytes[0] = img;
                            isec.Width = (ushort)(isec.Width >> imageNumber);
                            isec.Height = (ushort)(isec.Height >> imageNumber);
                        }
                        if (section.GetType() == typeof(PaletteSection))
                        {
                            PaletteSection psec = (PaletteSection)section;
                            byte[] pal = psec.PalettesRawBytes[imageNumber];
                            psec.PalettesRawBytes = new byte[1][];
                            psec.PalettesRawBytes[0] = pal;
                        }
                    }

                    uint fileinfosection = 0;
                    foreach (var section in Sections)
                    {
                        section.Recalculate(0);
                        if (section.GetType() == typeof(FileInfoSection))
                        {
                            fileinfosection = section.GetPartSize();
                        }
                    }
                    uint Filesize = GetTotalFilesize();
                    foreach (var section in Sections)
                    {
                        if (section.GetType() == typeof(EndOfFileSection))
                        {
                            section.Recalculate((int)Filesize - 0x10);
                        }
                        if (section.GetType() == typeof(EndOfImageSection))
                        {
                            section.Recalculate((int)Filesize - 0x20 - (int)fileinfosection);
                        }
                    }
                }

                public List<System.Drawing.Bitmap> ConvertToBitmaps()
                {
                    ImageSection isec = null;
                    PaletteSection psec = null;
                    foreach (var section in Sections)
                    {
                        if (section.GetType() == typeof(ImageSection))
                        {
                            isec = (ImageSection)section;
                        }
                        if (section.GetType() == typeof(PaletteSection))
                        {
                            psec = (PaletteSection)section;
                        }
                    }

                    return isec.ConvertToBitmaps(psec);
                }

                public void HomogenizePalette()
                {
                    ImageSection isec = null;
                    PaletteSection psec = null;
                    foreach (var section in Sections)
                    {
                        if (section.GetType() == typeof(ImageSection))
                        {
                            isec = (ImageSection)section;
                        }
                        if (section.GetType() == typeof(PaletteSection))
                        {
                            psec = (PaletteSection)section;
                        }
                    }

                    for (int i = 0; i < isec.ImageCount; ++i)
                    {
                        isec.DiscardUnusedColorsPaletted(i, psec, i);
                    }

                    List<uint> PaletteList = new List<uint>();
                    foreach (List<uint> pal in psec.Palettes)
                    {
                        PaletteList.AddRange(pal);
                    }
                    List<uint> NewPalette = PaletteList.Distinct().ToList();

                    int maxColors = 1 << isec.ColorDepth;
                    if (NewPalette.Count > maxColors)
                    {
                        string err = "ERROR: Combined Palette over the amount of allowed colors. (" + NewPalette.Count + " > " + maxColors + ")";
                        Console.WriteLine(err);
                        throw new Exception(err);
                    }
                    while (NewPalette.Count < maxColors)
                    {
                        NewPalette.Add(0);
                    }

                    for (int i = 0; i < isec.ImageCount; ++i)
                    {
                        isec.ConvertToTruecolor(i, psec.Palettes[i]);
                        isec.CovertToPaletted(i, NewPalette.ToArray());
                        psec.Palettes[i] = NewPalette.ToList();
                    }
                }


                public byte[] Serialize()
                {
                    List<byte> newfile = new List<byte>(File.Length);
                    foreach (var section in Sections)
                    {
                        newfile.AddRange(section.Serialize());
                    }
                    return newfile.ToArray();
                }
            }

            #endregion << Gim >>
        }

    }


    /// <summary>
    /// RCO File Class
    /// Credits to CFW Prophet for his C# tool
    /// https://github.com/cfwprpht/Simply_Vita_RCO_Extractor
    /// </summary>
    public class RCO
    {
        #region << Vars >>
        private static byte[] gimMagic = new byte[16] { 0x4D, 0x49, 0x47, 0x2E, 0x30, 0x30, 0x2E, 0x31, 0x50, 0x53, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, };
        private static byte[] vagEnd = new byte[16] { 0x00, 0x07, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, 0x77, };
        private static byte[] pngMagic = new byte[16] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, };
        private static byte[] ngrcoMagic = new byte[8] { 0x52, 0x43, 0x4F, 0x46, 0x10, 0x01, 0x00, 0x00, };
        private static byte[] ngCXML = new byte[8] { 0x52, 0x43, 0x53, 0x46, 0x10, 0x01, 0x00, 0x00, };
        private static byte[] vagMagic = new byte[8] { 0x56, 0x41, 0x47, 0x70, 0x00, 0x02, 0x00, 0x01, };
        private static byte[] ddsMagic = new byte[4] { 0x44, 0x44, 0x53, 0x20, };
        private static byte[] wavMagic = new byte[4] { 0x52, 0x49, 0x46, 0x46, };
        private static byte[] gtfMagic = new byte[4] { 0x02, 0x02, 0x00, 0xFF, };
        private static byte[] zlibMagic = new byte[3] { 0x00, 0x78, 0xDA, };
        private static byte[] singlZL = new byte[2] { 0x78, 0xDA, };
        private static byte[] _vag = new byte[0];
        private static byte[] _png = new byte[0];
        private static byte[] _cxml = new byte[0];
        private static byte[] _zlib = new byte[0];
        private static byte[] _wav = new byte[0];
        private static byte[] _gtf = new byte[0];
        private static byte[] _dds = new byte[0];
        private static byte[] zlib = new byte[2];
        private static byte[] vag = new byte[8];
        private static byte[] cxml = new byte[8];
        private static byte[] png = new byte[16];
        private static byte[] temp = new byte[1];
        private static byte[] dds = new byte[4];
        private static byte[] gtf = new byte[4];
        private static byte[] wav = new byte[4];
        private static int i = 0;
        private static int dumped = 0;
        private static int end = 0;
        private static int count = 0;
        private static int countVag = 0;
        private static int countCXML = 0;
        private static int countGim = 0;
        private static int countDDS = 0;
        private static int countPNG = 0;
        private static int countGTF = 0;
        private static int countWAV = 0;
        private static int countZLIB = 0;
        private static string baseDir = "";
        private static string convDir = "";
        private static string corExt = "";
        private static string move = "";
        private static string dest = "";

        #endregion << Vars >>
        /// <summary>
        /// Compare Byte by Byte or Array by Array
        /// </summary>
        /// <param name="bA1">Byte Array 1</param>
        /// <param name="bA2">Byte Array 2</param>
        /// <returns>True if both Byte Array's do match</returns>
        private static bool CompareBytes(byte[] bA1, byte[] bA2)
        {
            int s = 0;
            for (int z = 0; z < bA1.Length; z++)
            {
                if (bA1[z] != bA2[z])
                    s++;
            }

            if (s == 0)
                return true;

            return false;
        }

        public static void DumpRco(string File)
        {
            try
            {
                // Reading Header
                byte[] magic = new byte[8];
                byte[] offset = new byte[4];
                string outFile = "notDefined";
                using (BinaryReader br = new BinaryReader(new FileStream(File, FileMode.Open, FileAccess.Read)))
                {
                    // Check Magic
                    Console.Write("Checking Header....");
                    br.Read(magic, 0, 8);

                    if (!CompareBytes(magic, ngrcoMagic))
                    {
                        Console.WriteLine("ERROR: That is not a valid NextGen RCO!\nExiting now...");
                        Environment.Exit(0);
                    }

                    Console.Write("Magic OK!\nThat's a NextGen RCO :)\n");
                    //create folder in which to extact 
                    //if it exists delete the dam thing xD
                    if (Directory.Exists(PS4_Tools.AppCommonPath() + Path.GetFileNameWithoutExtension(File)))
                    {
                        PS4_Tools.DeleteDirectory(PS4_Tools.AppCommonPath() + Path.GetFileNameWithoutExtension(File));
                    }
                    Directory.CreateDirectory(PS4_Tools.AppCommonPath() + Path.GetFileNameWithoutExtension(File));

                    baseDir = PS4_Tools.AppCommonPath() + Path.GetFileNameWithoutExtension(File) + @"\";

                    // Get Data Table Offset and Length
                    Console.Write("Reading Offset and Length of Data Table...");
                    offset = new byte[4];
                    byte[] eof = new byte[4];
                    br.BaseStream.Seek(0x48, SeekOrigin.Begin);
                    br.Read(offset, 0, 4);
                    br.Read(eof, 0, 4);
                    Array.Reverse(offset);
                    Array.Reverse(eof);
                    Console.Write("done!\n");
                    Console.WriteLine("Readed Hex value of Offset: 0x" + BitConverter.ToString(offset).Replace("-", ""));
                    Console.WriteLine("Readed Hex value of Size: 0x" + BitConverter.ToString(eof).Replace("-", ""));

                    // Check for zlib Header '0x78DA' (compression level=9) or VAG & PNG files and write to file

                    end = Convert.ToInt32(BitConverter.ToString(eof).Replace("-", ""), 16);
                    count = Convert.ToInt32(BitConverter.ToString(offset).Replace("-", ""), 16);
                    Console.WriteLine("Offset to start from: " + count + " bytes");
                    Console.WriteLine("Size to Dump: " + end + " bytes");
                    Console.WriteLine("Searching for ZLib Compressed (Vita) or Non-Compressed (PS4) Files...");
                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                    br.Read(zlib, 0, 2);

                    // main loop
                    if (!CompareBytes(zlib, singlZL))
                    {
                        temp = new byte[16];
                        while ((i = br.Read(temp, 0, 16)) != 0)
                        {
                            // In case of we now also have PS4 RCO's to work down and to not compromise the routine, we swapped the Extraction here 
                            // and placed the search for zlib files under the VAG and PNG file search
                            // For ZLib i removed the second routine that would read after the first Zlib compressed block, adding a 0 byte 0x00 on top of 0x78DA
                            // Instead of that, we simple counted +1 byte on end of dumping process and continue as usually

                            // Now we first fill the buffer's for the header's which we will compare after
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(vag, 0, 8);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(cxml, 0, 8);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(zlib, 0, 2);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(png, 0, 16);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(dds, 0, 4);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(gtf, 0, 4);
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(wav, 0, 4);

                            #region vagExtract
                            if (CompareBytes(vag, vagMagic))
                            {
                                Console.Write("Found a VAG File will start to extract...");
                                outFile = baseDir + countVag + ".vag";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    while (true)
                                    {
                                        if (!CompareBytes(toWrite, vagEnd))
                                        {
                                            if (!CompareBytes(_cxml, ngCXML))
                                            {
                                                if (!CompareBytes(toWrite, pngMagic))
                                                {
                                                    if (!CompareBytes(_zlib, singlZL))
                                                    {
                                                        if (!CompareBytes(_wav, wavMagic))
                                                        {
                                                            if (!CompareBytes(_dds, ddsMagic))
                                                            {
                                                                if (!CompareBytes(_gtf, gtfMagic))
                                                                {
                                                                    bw.Write(toWrite, 0, 16);
                                                                    dumped += 16;
                                                                    count += 16;
                                                                    if (dumped != end)
                                                                    {
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                        {
                                            // We reached the eof and loop was stopped. Now we need to write out the last 16 bytes which build the eof of a VAG file.
                                            bw.Write(toWrite, 0, 16);
                                            dumped += 16;
                                            count += 16;
                                            break;
                                        }
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");

                                // Convert VAG to WAV
                                //TODO :Create C# Converter
                                //ConvertVAG(outFile);
                                countVag++;
                            }
                            #endregion vagExtract
                            #region pngExtract
                            else if (CompareBytes(png, pngMagic))
                            {
                                Console.Write("Found a PNG File will start to extract...");
                                outFile = baseDir + countPNG + ".png";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _vag = new byte[8];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    // Before we Jump into the Loop we need to write out the first readed 16 bytes which are the PNG Magic.
                                    // This is needed cause we need to compare for new Magic's / Header's to know if we reached the eof of current file.
                                    // Otherwise the routine would detect a PNG Magic and stop right after we jumped in, resulting in not extracting the PNG and loosing
                                    // the allready readed 16 bytes.
                                    bw.Write(toWrite, 0, 16);

                                    // count up the readed bytes and read next one before the loop start
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_vag, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    // Now let's start the Loop and extract the PNG
                                    while (true)
                                    {
                                        // Have we reached EOF ?
                                        if (!CompareBytes(toWrite, pngMagic))
                                        {
                                            if (!CompareBytes(_cxml, ngCXML))
                                            {
                                                if (!CompareBytes(_zlib, singlZL))
                                                {
                                                    if (!CompareBytes(_vag, vagMagic))
                                                    {
                                                        if (!CompareBytes(_wav, wavMagic))
                                                        {
                                                            if (!CompareBytes(_dds, ddsMagic))
                                                            {
                                                                if (!CompareBytes(_gtf, gtfMagic))
                                                                {
                                                                    // Write out the readed data
                                                                    bw.Write(toWrite, 0, 16);
                                                                    dumped += 16;
                                                                    count += 16;
                                                                    if (dumped != end)
                                                                    {
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_vag, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");
                                countPNG++;
                            }
                            #endregion pngExtract
                            #region cxmlExtract
                            else if (CompareBytes(cxml, ngCXML))
                            {
                                Console.Write("Found a CXML File will start to extract...");
                                outFile = baseDir + countCXML + ".cxml";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_vag, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    while (true)
                                    {
                                        if (!CompareBytes(_cxml, ngCXML))
                                        {
                                            if (!CompareBytes(toWrite, pngMagic))
                                            {
                                                if (!CompareBytes(_wav, wavMagic))
                                                {
                                                    if (!CompareBytes(_dds, ddsMagic))
                                                    {
                                                        if (!CompareBytes(_gtf, gtfMagic))
                                                        {
                                                            if (!CompareBytes(_zlib, singlZL))
                                                            {
                                                                if (!CompareBytes(_vag, vagMagic))
                                                                {
                                                                    if (dumped != end)
                                                                    {
                                                                        bw.Write(toWrite, 0, 16);
                                                                        dumped += 16;
                                                                        count += 16;
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_vag, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");
                                countCXML++;
                            }
                            #endregion cxmlExtract
                            #region ddsExtract
                            else if (CompareBytes(dds, ddsMagic))
                            {
                                Console.Write("Found a DDS File will start to extract...");
                                outFile = baseDir + countDDS + ".dds";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_vag, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    while (true)
                                    {
                                        if (!CompareBytes(_cxml, ngCXML))
                                        {
                                            if (!CompareBytes(toWrite, pngMagic))
                                            {
                                                if (!CompareBytes(_wav, wavMagic))
                                                {
                                                    if (!CompareBytes(_dds, ddsMagic))
                                                    {
                                                        if (!CompareBytes(_gtf, gtfMagic))
                                                        {
                                                            if (!CompareBytes(_zlib, singlZL))
                                                            {
                                                                if (!CompareBytes(_vag, vagMagic))
                                                                {
                                                                    if (dumped != end)
                                                                    {
                                                                        bw.Write(toWrite, 0, 16);
                                                                        dumped += 16;
                                                                        count += 16;
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_vag, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");
                                countDDS++;
                            }
                            #endregion ddsExtract
                            #region gtfExtract
                            else if (CompareBytes(gtf, gtfMagic))
                            {
                                Console.Write("Found a GTF File will start to extract...");
                                outFile = baseDir + countGTF + ".gtf";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_vag, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    while (true)
                                    {
                                        if (!CompareBytes(_cxml, ngCXML))
                                        {
                                            if (!CompareBytes(toWrite, pngMagic))
                                            {
                                                if (!CompareBytes(_wav, wavMagic))
                                                {
                                                    if (!CompareBytes(_dds, ddsMagic))
                                                    {
                                                        if (!CompareBytes(_gtf, gtfMagic))
                                                        {
                                                            if (!CompareBytes(_zlib, singlZL))
                                                            {
                                                                if (!CompareBytes(_vag, vagMagic))
                                                                {
                                                                    if (dumped != end)
                                                                    {
                                                                        bw.Write(toWrite, 0, 16);
                                                                        dumped += 16;
                                                                        count += 16;
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_vag, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");
                                countGTF++;
                            }
                            #endregion gtfExtract
                            #region wavExtract
                            else if (CompareBytes(wav, wavMagic))
                            {
                                Console.Write("Found a WAV File will start to extract...");
                                outFile = baseDir + countWAV + ".wav";
                                System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    dumped += 16;
                                    count += 16;
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_zlib, 0, 2);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_dds, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_gtf, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_wav, 0, 4);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_vag, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(_cxml, 0, 8);
                                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                                    br.Read(toWrite, 0, 16);

                                    while (true)
                                    {
                                        if (!CompareBytes(_cxml, ngCXML))
                                        {
                                            if (!CompareBytes(toWrite, pngMagic))
                                            {
                                                if (!CompareBytes(_wav, wavMagic))
                                                {
                                                    if (!CompareBytes(_dds, ddsMagic))
                                                    {
                                                        if (!CompareBytes(_gtf, gtfMagic))
                                                        {
                                                            if (!CompareBytes(_zlib, singlZL))
                                                            {
                                                                if (!CompareBytes(_vag, vagMagic))
                                                                {
                                                                    if (dumped != end)
                                                                    {
                                                                        bw.Write(toWrite, 0, 16);
                                                                        dumped += 16;
                                                                        count += 16;
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_zlib, 0, 2);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_dds, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_gtf, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_wav, 0, 4);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_vag, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(_cxml, 0, 8);
                                                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                                                        br.Read(toWrite, 0, 16);
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        else
                                                            break;
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                    break;
                                            }
                                            else
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    bw.Close();
                                }
                                Console.Write("done!\n");
                                countWAV++;
                            }
                            #endregion wavExtract
                            else
                            {
                                Console.WriteLine("\nFound a new File which i don't know what to do with !\nPlease contact the Developer @ www.playstationhax.it");
                                break;
                            }
                            if (dumped == end)
                                break;
                            else
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                        }
                    }
                    else if (CompareBytes(zlib, singlZL))
                    {
                        while ((i = br.Read(temp, 0, 1)) != 0)
                        {
                            #region zlibExtract
                            _zlib = new byte[3];
                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                            br.Read(_zlib, 0, 3);
                            Console.Write("Found a ZLib Compressed File, starting to extract...");
                            byte[] toWrite = new byte[1];
                            outFile = baseDir + countZLIB + ".compressed";
                            corExt = "";
                            System.IO.File.Create(outFile).Close();

                            using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                            {
                                while (true)
                                {
                                    if (!CompareBytes(_zlib, zlibMagic))  // Next Byte is not the start of a Header from a other file ?
                                    {
                                        // write out data to file
                                        br.BaseStream.Seek(count, SeekOrigin.Begin);
                                        br.Read(toWrite, 0, 1);
                                        bw.Write(toWrite, 0, 1);

                                        // Count up 1 and read the next byte(s) before the loop start again
                                        count++;
                                        dumped++;

                                        // Have we reached the end of data table?
                                        if (dumped != end)
                                        {
                                            br.BaseStream.Seek(count, SeekOrigin.Begin);
                                            br.Read(_zlib, 0, 3);
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        break;
                                }

                                // In case of we need to compare zlibHeader with a additional 0x00 on top to know if it is really a zlibHeader and not just a 0x78DA data value
                                // we add that 0x00 on end of the extracted file and count dumped var +1
                                if (CompareBytes(_zlib, zlibMagic))
                                {
                                    toWrite = new byte[1];
                                    bw.Write(toWrite, 0, 1);
                                    dumped++;
                                    count++;
                                    countZLIB++;
                                }
                                bw.Close();
                            }
                            Console.Write("done!\n");

                            // Decompress dumped File
                            if (outFile == "notDefined")
                                Console.WriteLine("Found a Unknowen File!\nPlease contact the Developer on: www.playstationhax.it\n");
                            else
                                //ZLibDeCompress(outFile);

                                // Check Header of Decompressed File and rename
                                Console.Write("Checking Header of Decompressed File...");
                            outFile = outFile + ".decompressed";
                            bool gHeader = false;
                            bool dHeader = false;
                            using (BinaryReader _br = new BinaryReader(new FileStream(outFile, FileMode.Open, FileAccess.Read)))
                            {
                                byte[] xmlHeader = new byte[8];
                                byte[] gimHeader = new byte[16];
                                byte[] ddsHeader = new byte[4];
                                _br.Read(xmlHeader, 0, 8);
                                _br.BaseStream.Seek(0, SeekOrigin.Begin);
                                _br.Read(gimHeader, 0, 16);
                                _br.BaseStream.Seek(0, SeekOrigin.Begin);
                                _br.Read(ddsHeader, 0, 4);

                                if (CompareBytes(xmlHeader, ngCXML))
                                {
                                    countCXML++;
                                    Console.Write("done!\nIt's a CXML Container.\n");
                                    corExt = outFile.Replace(".compressed.decompressed", ".cxml");

                                }
                                else if (CompareBytes(gimHeader, gimMagic))
                                {
                                    countGim++;
                                    gHeader = true;
                                    Console.Write("done!\nIt's a GIM File.\n");
                                    corExt = outFile.Replace(".compressed.decompressed", ".gim");
                                    move = corExt.Replace(".gim", ".png");
                                    dest = convDir + countGim + ".png";
                                }
                                else if (CompareBytes(ddsHeader, ddsMagic))
                                {
                                    countDDS++;
                                    dHeader = true;
                                    Console.Write("done!\nIt's a DDS Container.\n");
                                    corExt = outFile.Replace(".compressed.decompressed", ".dds");
                                    move = corExt.Replace(".dds", ".gtf");
                                    dest = convDir + countDDS + ".gtf";
                                }
                                else
                                    Console.Write("error!\nUnknown Header, please contact the developer...\n");
                                _br.Close();
                            }

                            // Finally Rename to the correct extension
                            Console.Write("Renaming " + "'" + outFile.Replace(baseDir, "") + "'" + " to " + "'" + corExt.Replace(baseDir, "") + "'...");
                            System.IO.File.Move(outFile, corExt);
                            Console.Write("done!\n");

                            // Convert GIM to PNG or DDS to GTF
                            if (gHeader)
                            {
                                //ConvertGIM(corExt);
                            }
                            else if (dHeader)
                            {
                                //  ConvertDDS(corExt);
                                Image.DDS.SavePNGFromDDS(corExt, corExt.Replace(".dds", ".png"));
                            }
                            #endregion ZlibExtract
                            // Have we dumped all data?
                            if (dumped == end)
                            {
                                Console.Write("Moving Converted Files to Extracted Folder...");
                                string fi = "";
                                string final = "";
                                string[] files = Directory.GetFiles(baseDir);

                                foreach (string s in files)
                                {
                                    if (s.Contains(".png") || s.Contains(".gtf"))
                                    {
                                        fi = s.Replace(baseDir, "");
                                        final = convDir + fi;
                                        System.IO.File.Move(s, final);
                                    }
                                }
                                Console.Write("Done!\n");
                                break;
                            }
                            else
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                        }
                        br.Close();
                    }
                    else
                        Console.WriteLine("\nSomthing went wrong!\nPlease contact the developer @ www.playstationhax.it\nERROR: 1");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\nERROR:\n" + e.ToString());
                //Environment.Exit(0);
            }
        }
    }

    public class SaveData
    {
        public struct Sealedkey
        {
            public byte[] MAGIC;/*Magic needs to be validated*/
            public byte[] KeySet;
            public byte[] AlignBytes;
            public byte[] IV;
            public byte[] KEY;
            public byte[] SHA256;
        }
        public static void LoadSaveData(string filelocation, string Sealedkeylocation)
        {
            /*Load the sealed key from the file*/
            Sealedkey sldkey = default(Sealedkey);
            sldkey.MAGIC = new byte[8];
            sldkey.KeySet = new byte[2];
            sldkey.AlignBytes = new byte[6];
            sldkey.IV = new byte[16];
            sldkey.KEY = new byte[32];
            sldkey.SHA256 = new byte[32];

            FileStream fs = new FileStream(Sealedkeylocation, FileMode.Open, FileAccess.Read);

            fs.Read(sldkey.MAGIC, 0, sldkey.MAGIC.Length);
            fs.Read(sldkey.KeySet, 0, sldkey.KeySet.Length);
            fs.Read(sldkey.AlignBytes, 0, sldkey.AlignBytes.Length);
            fs.Read(sldkey.IV, 0, sldkey.IV.Length);
            fs.Read(sldkey.KEY, 0, sldkey.KEY.Length);
            fs.Read(sldkey.SHA256, 0, sldkey.SHA256.Length);

            if (!Util.Utils.CompareBytes(sldkey.MAGIC, new byte[] { 0x70, 0x66, 0x73, 0x53, 0x4B, 0x4B, 0x65, 0x79 }))
            {
                throw new Exception("This is not a valid SealedKey");
            }
            // Declare CspParmeters and RsaCryptoServiceProvider
            // objects with global scope of your Form class.
            CspParameters cspp = new CspParameters();
            RSACryptoServiceProvider rsa;

            // Key container name for
            // private/public key value pair.
            const string keyName = "SonyKey";

            //Create Keys
            Label label1 = new Label();
            // Stores a key pair in the key container.
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            if (rsa.PublicOnly == true)
                label1.Text = "Key: " + cspp.KeyContainerName + " - Public Only";
            else
                label1.Text = "Key: " + cspp.KeyContainerName + " - Full Key Pair";

            if (rsa == null)
                MessageBox.Show("Key not set.");
            else
            {
                // Display a dialog box to select the encrypted file.

                string fName = filelocation;
                if (fName != null)
                {
                    FileInfo fi = new FileInfo(fName);
                    string name = fi.Name;
                    // Create instance of Rijndael for
                    // symetric decryption of the data.
                    RijndaelManaged rjndl = new RijndaelManaged();
                   // rjndl.KeySize = 256;
                   // rjndl.BlockSize = 256;
                    rjndl.Mode = CipherMode.CBC;

                    // Use FileStream objects to read the encrypted
                    // file (inFs) and save the decrypted file (outFs).
                    using (FileStream inFs = new FileStream(fName, FileMode.Open))
                    {

                        // Create the byte arrays for
                        // the encrypted Rijndael key,
                        // the IV, and the cipher text.
                        byte[] KeyEncrypted = sldkey.KEY;
                        byte[] IV = sldkey.IV;

                        /*Read Save File Header*/
                        inFs.Seek(0, SeekOrigin.Begin);
                        byte[] HeaderSave = new byte[116];/*Header info*/
                        /*Encrypted Block 1*/
                        byte[] Block1 = new byte[49154];
                        byte[] Block2 = new byte[9501855];/*This is all testing*/

                        inFs.Read(HeaderSave, 0, 116);
                        inFs.Seek(117, SeekOrigin.Begin);
                        inFs.Read(Block1, 0, Block1.Length);
                        inFs.Seek(Block1.Length + 1, SeekOrigin.Begin);
                        inFs.Read(Block2, 0, Block2.Length);
                        
                        /*The Rest is save info*/

                        // Use RSACryptoServiceProvider
                        // to decrypt the Rijndael key.
                        //byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);
                        /*If im not mistaken the ps4 has already got this key decrypted for us*/

                        // Decrypt the key.
                        ICryptoTransform transform = rjndl.CreateDecryptor(KeyEncrypted, IV);

                        // Decrypt the cipher text from
                        // from the FileSteam of the encrypted
                        // file (inFs) into the FileStream
                        // for the decrypted file (outFs).

                        string outFile = fName + ".dec";
                        using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                        {

                            int count = 0;
                            int offset = 0;

                            // blockSizeBytes can be any arbitrary size.
                            int blockSizeBytes = rjndl.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];


                            // By decrypting a chunk a time,
                            // you can save memory and
                            // accommodate large files.

                            // Start at the beginning
                            // of the cipher text.
                            //inFs.Seek(startC, SeekOrigin.Begin);
                            using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                            {
                                do
                                {
                                    count = inFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamDecrypted.Write(data, 0, count);

                                }
                                while (count > 0);

                                outStreamDecrypted.FlushFinalBlock();
                                outStreamDecrypted.Close();
                            }
                            outFs.Close();
                        }
                        inFs.Close();
                    }
                }
            }
        }
    }
    public class Trophy_File
    {
        /*SHA1*/
        private string SHA1;
        private byte[] Bytes;

        private bool Readbytes;

        TrophyHeader trphy = new TrophyHeader();

        public int FileCount
        {
            get
            {
                return checked((int)Util.Utils.byteArrayToLittleEndianInteger(trphy.files_count));
            }
        }

        public int Version
        {
            get
            {
                return checked((int)Util.Utils.byteArrayToLittleEndianInteger(trphy.version));
            }
        }

        /*Trophy header Structure*/
        public struct TrophyHeader
        {
            public byte[] magic;

            public byte[] version;

            public byte[] file_size;

            public byte[] files_count;

            public byte[] element_size;

            public byte[] dev_flag;

            public byte[] sha1;

            public byte[] padding;
        }
        /*Trophy items*/
        public class TrophyItem
        {
            // Token: 0x0600001D RID: 29 RVA: 0x00002050 File Offset: 0x00000250
            public TrophyItem(int Index, string Name, uint Offset, ulong Size, byte[] TotalBytes)
            {
                this.Index = Index;
                this.Name = Name;
                this.Size = checked((long)Size);
                this.Offset = (long)((ulong)Offset);
                this.TotalBytes = TotalBytes;
            }
            /*Index number of trophy item*/
            public int Index;
            /*Name of trophy item*/
            public string Name;
            /*offset as long*/
            public long Offset;
            /*Size*/
            public long Size;
            /*Total Bytes*/
            public byte[] TotalBytes;
        }

        /*Trophy Files have multiple Items*/
        public List<TrophyItem> trophyItemList = new List<TrophyItem>();

        private TrophyHeader LoadHeader(Stream fs)
        {
            TrophyHeader hdr = default(TrophyHeader);
            hdr.magic = new byte[4];
            hdr.version = new byte[4];
            hdr.file_size = new byte[8];
            hdr.files_count = new byte[4];
            hdr.element_size = new byte[4];
            hdr.dev_flag = new byte[4];
            hdr.sha1 = new byte[20];
            hdr.padding = new byte[36];
            fs.Read(hdr.magic, 0, hdr.magic.Length);
            fs.Read(hdr.version, 0, hdr.version.Length);
            fs.Read(hdr.file_size, 0, hdr.file_size.Length);
            fs.Read(hdr.files_count, 0, hdr.files_count.Length);
            fs.Read(hdr.element_size, 0, hdr.element_size.Length);
            fs.Read(hdr.dev_flag, 0, hdr.dev_flag.Length);
            long num = Util.Utils.byteArrayToLittleEndianInteger(hdr.version);
            if (num <= 3L && num >= 1L)
            {
                switch ((int)(num - 1L))
                {
                    case 0:
                        fs.Read(hdr.padding, 0, hdr.padding.Length);
                        break;
                    case 1:
                        fs.Read(hdr.sha1, 0, hdr.sha1.Length);
                        hdr.padding = new byte[16];
                        fs.Read(hdr.padding, 0, hdr.padding.Length);
                        break;
                    case 2:
                        fs.Read(hdr.sha1, 0, hdr.sha1.Length);
                        hdr.padding = new byte[48];
                        fs.Read(hdr.padding, 0, hdr.padding.Length);
                        break;
                }
            }
            return hdr;
        }

        private void ReadContent(Stream fs)
        {
            byte[] array = new byte[36];
            byte[] array2 = new byte[4];
            byte[] array3 = new byte[8];
            byte[] array4 = new byte[4];
            int num = 0;
            checked
            {
                int num2 = this.FileCount - 1;
                int i = num;
                while (i <= num2)
                {
                    fs.Read(array, 0, array.Length);
                    fs.Read(array2, 0, array2.Length);
                    fs.Read(array3, 0, array3.Length);
                    fs.Read(array4, 0, array4.Length);
                    fs.Seek(12L, SeekOrigin.Current);
                    long position = fs.Position;
                    string name = Util.Utils.byteArrayToUTF8String(array).Replace("\0", null);
                    long num3 = Util.Utils.hexStringToLong(Util.Utils.byteArrayToHexString(array2));
                    long num4 = Util.Utils.hexStringToLong(Util.Utils.byteArrayToHexString(array3));
                    if (this.Readbytes)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(this.Bytes))
                        {
                            byte[] array5 = new byte[(int)(num4 - 1L) + 1];
                            memoryStream.Seek(num3, SeekOrigin.Begin);
                            memoryStream.Read(array5, 0, array5.Length);
                            this.trophyItemList.Add(new TrophyItem(i, name, (uint)num3, (ulong)num4, array5));
                            goto IL_124;
                        }
                        goto IL_10C;
                    }
                    goto IL_10C;
                    IL_124:
                    i++;
                    continue;
                    IL_10C:
                    this.trophyItemList.Add(new TrophyItem(i, name, (uint)num3, (ulong)num4, null));
                    goto IL_124;
                }
            }
        }

        private string CalculateSHA1Hash()
        {
            checked
            {
                if (this.Version > 1)
                {
                    byte[] array = new byte[28];
                    SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();
                    MemoryStream memoryStream = new MemoryStream();
                    using (MemoryStream memoryStream2 = new MemoryStream(this.Bytes))
                    {
                        memoryStream2.Read(array, 0, array.Length);
                        memoryStream.Write(array, 0, array.Length);
                        array = new byte[1];
                        int num = 0;
                        do
                        {
                            memoryStream.Write(array, 0, array.Length);
                            num++;
                        }
                        while (num <= 19);
                        memoryStream2.Seek(48L, SeekOrigin.Begin);
                        array = new byte[(int)(memoryStream2.Length - 48L - 1L) + 1];
                        memoryStream2.Read(array, 0, array.Length);
                        memoryStream.Write(array, 0, array.Length);
                    }
                    byte[] array2 = sha1CryptoServiceProvider.ComputeHash(memoryStream.ToArray());
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (byte b in array2)
                    {
                        stringBuilder.Append(b.ToString("X2"));
                    }
                    return stringBuilder.ToString();
                }
                return null;
            }
        }

        /// <summary>
        /// This method should create a blank trophy file
        /// </summary>
        public Trophy_File()
        {
            /*Load a blank tropy file ?*/
        }

        /// <summary>
        /// Method Will Create a Trohy File From a File Path
        /// </summary>
        /// <param name="FilePath">File Location on disk</param>
        public Trophy_File(string FilePath)
        {
            this.SHA1 = "";
            this.trophyItemList = new List<TrophyItem>();
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStream.Read(Bytes, 0, checked((int)fileStream.Length));
                fileStream.Seek(0L, SeekOrigin.Begin);
                TrophyHeader hdr = LoadHeader(fileStream);
                trphy = hdr;
                if (!Util.Utils.ByteArraysEqual(hdr.magic, new byte[]{220,162,77,0}))
                {
                    throw new Exception("This file is not supported!");
                }
                ReadContent(fileStream);
                if (Version > 1)
                {
                    SHA1 = CalculateSHA1Hash();
                }
            }
            //MessageBox.Show(this._error, "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public Trophy_File Load(byte[] bytes)
        {
            Trophy_File rtn = new Trophy_File();
            try
            {            
                this.trophyItemList = new List<TrophyItem>();
                this.Bytes = bytes;
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    TrophyHeader hdr = LoadHeader(memoryStream);
                    trphy = hdr;
                    if (!Util.Utils.ByteArraysEqual(hdr.magic, new byte[] { 220, 162, 77, 0 }))
                    {
                        throw new Exception("This file is not supported!");
                    }
                    ReadContent(memoryStream);
                    if (Version > 1)
                    {
                        SHA1 = CalculateSHA1Hash();
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
            rtn.Bytes = Bytes;
            rtn.SHA1 = SHA1;
            rtn.trphy = trphy;
            rtn.trophyItemList = trophyItemList;
            return rtn;
        }

        public byte[] ExtractFileToMemory(string filename)
        {
            byte[] result = null;
            TrophyItem archiver = this.trophyItemList.Find((TrophyItem b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(Microsoft.VisualBasic.Strings.Mid(b.Name.ToUpper(), 1, Microsoft.VisualBasic.Strings.Len(filename.ToUpper())), filename.ToUpper(), false) == 0);
            if (archiver != null)
            {
                byte[] array = new byte[checked((int)(archiver.Size - 1L) + 1)];
                using (MemoryStream memoryStream = new MemoryStream(this.Bytes))
                {
                    memoryStream.Seek(archiver.Offset, SeekOrigin.Begin);
                    memoryStream.Read(array, 0, array.Length);
                    using (MemoryStream memoryStream2 = new MemoryStream())
                    {
                        memoryStream2.Write(array, 0, array.Length);
                        result = memoryStream2.ToArray();
                    }
                }
            }
            return result;
        }

    }



    /*********************************************************
     *          PS4 PKG Reader by maxton  
     *          https://github.com/maxton/LibOrbisPkg
     *          
     *          And Official SCE Tools until we can fully 
     *          extract unencrypted file types
     *          
     *********************************************************/

    public class PKG
    {

        #region << Official >>
        public class Official
        {
            /// <summary>
            /// This Uses SCE Tools PLease Try and avoid this
            /// Will be intigrating maxtrons pkg tools
            /// </summary>
            /// <param name="FilePath">PS4 PKG File Path</param>
            /// <returns></returns>
            public static List<string> ReadAllUnprotectedData(string FilePath)
            {
                File.WriteAllBytes(PS4_Tools.AppCommonPath() + "ext.zip", Properties.Resources.ext);
                File.WriteAllBytes(PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);

                if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\ext\"))
                {
                    ZipFile.ExtractToDirectory(PS4_Tools.AppCommonPath() + "ext.zip", PS4_Tools.AppCommonPath());
                }



                List<string> rtnlist = new List<string>();

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
                start.Arguments = "img_file_list  --no_passcode --oformat recursive \"" + FilePath + "\"";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.CreateNoWindow = true;
                using (Process process = Process.Start(start))
                {
                    process.ErrorDataReceived += delegate
                    {

                    };
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        string[] splitresult = result.Split('\n');
                        for (int i = 0; i < splitresult.Length; i++)
                        {
                            rtnlist.Add(splitresult[i]);
                        }

                        // return result;
                    }
                }


                return rtnlist;
            }

            public class Update_Structure
            {
                [XmlRoot(ElementName = "titlepatch")]
                public class Titlepatch
                {

                    [XmlElement(ElementName = "tag")]
                    public Tag Tag { get; set; }
                    [XmlAttribute(AttributeName = "titleid")]
                    public string Titleid { get; set; }
                }

                [XmlRoot(ElementName = "delta_info_set")]
                public class Delta_info_set
                {
                    [XmlAttribute(AttributeName = "url")]
                    public string Url { get; set; }
                }

                [XmlRoot(ElementName = "paramsfo")]
                public class Paramsfo
                {
                    [XmlElement(ElementName = "title")]
                    public string Title { get; set; }
                    [XmlElement(ElementName = "title_00")]
                    public string Title_00 { get; set; }
                }

                public class Piece
                {
                    public string url { get; set; }
                    public int fileOffset { get; set; }
                    public long fileSize { get; set; }
                    public string hashValue { get; set; }
                }

                public class Manifest_Item
                {
                    public long originalFileSize { get; set; }
                    public string packageDigest { get; set; }
                    public int numberOfSplitFiles { get; set; }
                    public List<Piece> pieces { get; set; }
                }

                [XmlRoot(ElementName = "package")]
                public class Package
                {
                    [XmlElement(ElementName = "delta_info_set")]
                    public Delta_info_set Delta_info_set { get; set; }
                    [XmlElement(ElementName = "paramsfo")]
                    public Paramsfo Paramsfo { get; set; }
                    [XmlAttribute(AttributeName = "version")]
                    public string Version { get; set; }
                    [XmlAttribute(AttributeName = "size")]
                    public string Size { get; set; }
                    [XmlAttribute(AttributeName = "digest")]
                    public string Digest { get; set; }
                    [XmlAttribute(AttributeName = "manifest_url")]
                    public string Manifest_url { get; set; }
                    [XmlAttribute(AttributeName = "content_id")]
                    public Manifest_Item Manifest_item { get; set; }
                    public string Content_id { get; set; }
                    [XmlAttribute(AttributeName = "system_ver")]
                    public string System_ver { get; set; }
                    [XmlAttribute(AttributeName = "type")]
                    public string Type { get; set; }
                    [XmlAttribute(AttributeName = "remaster")]
                    public string Remaster { get; set; }
                    [XmlAttribute(AttributeName = "patchgo")]
                    public string Patchgo { get; set; }
                }

                [XmlRoot(ElementName = "latest_playgo_manifest")]
                public class Latest_playgo_manifest
                {
                    [XmlAttribute(AttributeName = "url")]
                    public string Url { get; set; }

                    public PKG.SceneRelated.GP4.Psproject PlayGoPG4 { get; set; }
                }


                [XmlRoot(ElementName = "tag")]
                public class Tag
                {
                    [XmlElement(ElementName = "package")]
                    public Package Package { get; set; }
                    [XmlElement(ElementName = "latest_playgo_manifest")]
                    public Latest_playgo_manifest Latest_playgo_manifest { get; set; }
                    [XmlAttribute(AttributeName = "name")]
                    public string Name { get; set; }
                    [XmlAttribute(AttributeName = "mandatory")]
                    public string Mandatory { get; set; }
                }


            }

            public enum Store_Platform
            {
                PS4 = 0,
                PS3 = 1,
                PSP = 2,
                PSPv2 = 3,
                PS5 = 9,
            }

            public enum Store_Type
            {
                Add_On = 0,
                Game_Video = 1,
                Vehicle = 2,
                Map = 3,
                Bundle = 4,
                Full_Game = 5,
                Avatar = 6,
                Avatars = 7,
                Theme = 8,
                Static_Theme = 9,
                Dynamic_Theme = 10,
                Season_Pass = 11,
                Level = 12,
                Character = 13,
                Other = 14
            }

            public class StoreItems
            {
                public string Store_Content_Title { get; set; }
                public Store_Type Store_Content_Type { get; set; }
                public string Store_Content_Type_Str { get; set; }
                public Store_Platform Store_Content_Platform { get; set; }
                public string Store_Content_Platform_Str { get; set; }
                public string Download_URL { get; set; }
                public Bitmap Store_Content_Image { get; set; }
            }

            public static Update_Structure.Titlepatch CheckForUpdate(string TitleID)
            {
                /*Check for update to ps game*/

                /*HMAC Create new url*/
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                string nptitle = "np_" + TitleID;
                byte[] keyByte = Util.Utils.Hex2Binary("AD62E37F905E06BC19593142281C112CEC0E7EC3E97EFDCAEFCDBAAFA6378D84");
                HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
                byte[] messageBytes = encoding.GetBytes(nptitle);
                byte[] hashnptitle = hmacsha256.ComputeHash(messageBytes);
                string hash = Util.Utils.ByteToString(hashnptitle);

                // return;
                //var test = new SHA256()

                /*Get XML String */
                string urlofupdatexml = "http://gs-sec.ww.np.dl.playstation.net/plo/np/" + TitleID + "/" + hash.ToLower() + "/" + TitleID + "-ver.xml";

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        //add protocols incase sony wants to add them
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                        //add a header cause sometimes they check this setting 
                        //also we can make the Header match the ps4 if need be
                        client.Headers.Add("user-agent", "Only a test!");

                        //downlad string to xml
                        //string xmlheader = "<?xml version='1.0'?>";

                        string xmlfilecontent = client.DownloadString(urlofupdatexml);
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml(xmlfilecontent);

                        /*XML Node List*/
                        XmlNodeList xmlnode;

                        /*we need to load all the info via nodes ext ext */

                        XmlElement root = xmldoc.DocumentElement;

                        Update_Structure updatstruct = new Update_Structure();

                        /*Get TitlePatch*/
                        Update_Structure.Titlepatch titlepatch = new Update_Structure.Titlepatch();
                        XmlNodeList test = xmldoc.GetElementsByTagName("titlepatch");
                        /*get attributes*/
                        titlepatch.Titleid = test[0].Attributes["titleid"].Value.ToString();

                        /*get tag*/
                        Update_Structure.Tag tag = new Update_Structure.Tag();
                        XmlNodeList tagnodelist = xmldoc.GetElementsByTagName("tag");
                        tag.Name = tagnodelist[0].Attributes["name"].Value.ToString();
                        tag.Mandatory = tagnodelist[0].Attributes["mandatory"].Value.ToString();

                        /*get package*/
                        Update_Structure.Package pkg = new Update_Structure.Package();
                        XmlNodeList pkgnodelist = xmldoc.GetElementsByTagName("package");
                        /*get attributes*/
                        pkg.Version = pkgnodelist[0].Attributes["version"].Value.ToString();
                        pkg.Size = pkgnodelist[0].Attributes["size"].Value.ToString();
                        pkg.Digest = pkgnodelist[0].Attributes["digest"].Value.ToString();
                        pkg.Manifest_url = pkgnodelist[0].Attributes["manifest_url"].Value.ToString();

                        /*we add a manaifest item reader and deserliezer here*/
                        Stream stream = client.OpenRead(pkg.Manifest_url);
                        StreamReader sr = new StreamReader(stream);
                        string json = sr.ReadToEnd();
                        pkg.Manifest_item = JsonConvert.DeserializeObject<Update_Structure.Manifest_Item>(json);

                        /*Content ID*/
                        pkg.Content_id = pkgnodelist[0].Attributes["content_id"].Value.ToString();

                        pkg.System_ver = pkgnodelist[0].Attributes["system_ver"].Value.ToString();

                        pkg.Type = pkgnodelist[0].Attributes["type"].Value.ToString();

                        pkg.Remaster = pkgnodelist[0].Attributes["remaster"].Value.ToString();

                        pkg.Patchgo = pkgnodelist[0].Attributes["patchgo"].Value.ToString();

                        /*Delta Info Patch*/

                        Update_Structure.Delta_info_set deltainfo = new Update_Structure.Delta_info_set();
                        XmlNodeList deltanodelist = xmldoc.GetElementsByTagName("delta_info_set");

                        deltainfo.Url = deltanodelist[0].Attributes["url"].Value.ToString();

                        /*Param Sfo*/
                        Update_Structure.Paramsfo paramsfo = new Update_Structure.Paramsfo();
                        XmlNodeList sfotitlenodelist = xmldoc.GetElementsByTagName("title");
                        XmlNodeList sfotitl00enodelist = xmldoc.GetElementsByTagName("title");
                        paramsfo.Title = sfotitlenodelist[0].InnerText.ToString();
                        paramsfo.Title_00 = sfotitl00enodelist[0].InnerText.ToString();

                        /**/
                        Update_Structure.Latest_playgo_manifest playgomani = new Update_Structure.Latest_playgo_manifest();
                        XmlNodeList playgoenodelist = xmldoc.GetElementsByTagName("latest_playgo_manifest");
                        playgomani.Url = playgoenodelist[0].Attributes["url"].Value.ToString();

                        /*we can set the psproject here as well*/
                        stream = client.OpenRead(playgomani.Url);
                        playgomani.PlayGoPG4 = PKG.SceneRelated.GP4.ReadGP4(stream);


                        /*set item to main reverse order */

                        pkg.Paramsfo = paramsfo;
                        pkg.Delta_info_set = deltainfo;
                        tag.Package = pkg;
                        tag.Latest_playgo_manifest = playgomani;
                        titlepatch.Tag = tag;

                        return titlepatch;

                        Regex regex = new Regex("<titlepatch>(.*?)</titlepatch>", RegexOptions.Singleline);

                        /*i have commented this out as for some or other reason I can't get the deserielizer to work */
                        #region << Deserlizers not working >>
                        //var item = DeserializeXMLFileToObject<Update_Structure>(@"C:\Users\3deEchelon\Downloads\CUSA07708-ver.xml");

                        //using (XmlReader reader = new XmlNodeReader(xml))
                        //{
                        //    reader.Read();
                        //    reader.ReadInnerXml();
                        //    var serializer = new XmlSerializer(typeof(Update_Structure), new XmlRootAttribute("titlepatch"));
                        //    var result_Structure = serializer.Deserialize(reader);
                        //}

                        //XmlSerializer serialize2r = new XmlSerializer(typeof(Update_Structure), new XmlRootAttribute("titlepatch"));
                        //using (FileStream fileStream = new FileStream(@"test.file", FileMode.Open))
                        //{
                        //    Update_Structure result = (Update_Structure)serialize2r.Deserialize(fileStream);
                        //    return result;
                        //}


                        //XmlSerializer serializer = new XmlSerializer(typeof(Update_Structure));
                        //using (TextReader reader = new StringReader(xml.OuterXml))
                        //{
                        //    Update_Structure result = (Update_Structure)serializer.Deserialize(reader);
                        //    return result;

                        //}

                        #endregion << Deserlizers not working >>
                    }

                }
                catch (Exception ex)
                {

                }
                return null;
            }

            public static T DeserializeXMLFileToObject<T>(string XmlFilename)
            {
                T returnObject = default(T);
                if (string.IsNullOrEmpty(XmlFilename)) return default(T);

                try
                {
                    StreamReader xmlStream = new StreamReader(XmlFilename);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    returnObject = (T)serializer.Deserialize(xmlStream);
                }
                catch (Exception ex)
                {
                    //ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
                }
                return returnObject;
            }

            private static string DownloadRecersivly(string currentdownload, int pages, WebClient client, string Region, string NPTitle)
            {
                if (currentdownload.Contains("paginator-control__end paginator-control__arrow-navigation internal-app-link ember-view"))
                {
                    try
                    {
                        pages++;
                        currentdownload = currentdownload + client.DownloadString(new Uri("https://store.playstation.com/" + Region + "/grid/" + NPTitle + "_00" + "/" + pages + "?relationship=add-ons"));
                        DownloadRecersivly(currentdownload, pages, client, Region, NPTitle);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return currentdownload;
            }

            private static Bitmap LoadPicture(string url)
            {
                HttpWebRequest wreq;
                HttpWebResponse wresp;
                Stream mystream;
                Bitmap bmp;

                bmp = null;
                mystream = null;
                wresp = null;
                try
                {
                    wreq = (HttpWebRequest)WebRequest.Create("http" + url);
                    wreq.AllowWriteStreamBuffering = true;

                    wresp = (HttpWebResponse)wreq.GetResponse();

                    if ((mystream = wresp.GetResponseStream()) != null)
                        bmp = new Bitmap(mystream);
                }
                finally
                {
                    if (mystream != null)
                        mystream.Close();

                    if (wresp != null)
                        wresp.Close();
                }
                return (bmp);
            }

            /// <summary>
            /// This will return a List of Store Items
            /// </summary>
            /// <param name="NPTitle">NP Title ID e.g. (CUSA07708)</param>
            /// <returns></returns>
            public static List<StoreItems> Get_All_Store_Items(string NPTitle)
            {
                List<StoreItems> storeitems = new List<StoreItems>();

                WebClient client = new WebClient();
                client.Headers.Add("Accept", "text/html");
                client.Headers.Add("Accept-Language", "en-US");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64)");
                client.Headers.Add("Referer", "https://store.playstation.com/");

                string Region = "";
                int Page = 1;
                string Regionstr = NPTitle.Substring(1, 1);
                switch (Regionstr)
                {
                    case "U":
                        Region = "en-gb";
                        break;
                    case "E":
                        Region = "en-gb";
                        break;
                    case "I":
                        Region = "en-us";
                        break;
                    default:
                        Region = "ja-jp";
                        break;
                }

                string download = client.DownloadString(new Uri("https://store.playstation.com/" + Region + "/grid/" + NPTitle + "_00" + "/" + Page + "?relationship=add-ons"));

                string downloadholder = "";

                HtmlAgilityPack.HtmlDocument storepage = new HtmlAgilityPack.HtmlDocument();
                storepage.LoadHtml(download);

                if (download.Contains("cell__title"))
                {
                    string[] Splliters1;


                    /*recersive download*/
                    download = DownloadRecersivly(download, Page, client, Region, NPTitle);
                    string[] splitingcells, splittedfooter, splittedcelltitel, splittedifno;

                    splitingcells = Regex.Split(download, "desktop-presentation__grid-cell__base");
                    for (int i = 1; i < splitingcells.Length; i++)
                    {
                        StoreItems newitem = new StoreItems();

                        splittedfooter = Regex.Split(splitingcells[i], "grid-cell__footer");

                        if (splittedfooter[0].Contains("class=\"grid-cell__title\">"))
                        {
                            splittedcelltitel = Regex.Split(splittedfooter[0], "class=\"grid-cell__title\">");
                            splittedifno = Regex.Split(splittedcelltitel[1], "<");
                        }
                        else
                        {
                            splittedcelltitel = Regex.Split(splittedfooter[0], "<span title=\"");
                            splittedifno = Regex.Split(splittedcelltitel[1], "\"");
                        }
                        newitem.Store_Content_Title = splittedifno[0].Trim();
                        newitem.Store_Content_Title = WebUtility.HtmlDecode(newitem.Store_Content_Title);

                        splittedcelltitel = Regex.Split(splittedfooter[0], "a href=\"");
                        splittedifno = Regex.Split(splittedcelltitel[1], "\"");
                        newitem.Download_URL = "https://store.playstation.com" + splittedifno[0].Trim();

                        splittedcelltitel = Regex.Split(splittedfooter[0], "img src=\"http");
                        splittedifno = Regex.Split(splittedcelltitel[1], "\"");
                        newitem.Store_Content_Image = LoadPicture(splittedifno[0].Trim());

                        splittedcelltitel = Regex.Split(splittedfooter[0], "left-detail--detail-2\">");
                        splittedifno = Regex.Split(splittedcelltitel[1], "<");
                        newitem.Store_Content_Type_Str = splittedifno[0].Trim();
                        newitem.Store_Content_Type_Str = WebUtility.HtmlDecode(newitem.Store_Content_Type_Str);

                        splittedcelltitel = Regex.Split(splittedfooter[0], "left-detail--detail-1\">");
                        splittedifno = Regex.Split(splittedcelltitel[1], "<");
                        newitem.Store_Content_Platform_Str = splittedifno[0].Trim();
                        newitem.Store_Content_Platform_Str = WebUtility.HtmlDecode(newitem.Store_Content_Platform_Str);

                        storeitems.Add(newitem);

                    }
                }



                return storeitems;
            }
        }

        #endregion << Official >>

        #region << Scene Related >>

        public class SceneRelated
        {
            public static void Create_FKPG(string Download_Url, string SaveLocation)
            {
                /*we can download an item */
                File.WriteAllBytes(PS4_Tools.AppCommonPath() + "ext.zip", Properties.Resources.ext);
                File.WriteAllBytes(PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);

                if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\ext\"))
                {
                    ZipFile.ExtractToDirectory(PS4_Tools.AppCommonPath() + "ext.zip", PS4_Tools.AppCommonPath());
                }


            }

            /// <summary>
            /// GP4 Project Class
            /// </summary>
            public class GP4
            {

                [XmlRoot(ElementName = "package")]
                public class Package
                {
                    [XmlAttribute(AttributeName = "content_id")]
                    public string Content_id { get; set; }
                    [XmlAttribute(AttributeName = "passcode")]
                    public string Passcode { get; set; }
                    [XmlAttribute(AttributeName = "storage_type")]
                    public string Storage_type { get; set; }
                    [XmlAttribute(AttributeName = "app_type")]
                    public string App_type { get; set; }
                }

                [XmlRoot(ElementName = "chunk")]
                public class Chunk
                {
                    [XmlAttribute(AttributeName = "id")]
                    public string Id { get; set; }
                    [XmlAttribute(AttributeName = "layer_no")]
                    public string Layer_no { get; set; }
                    [XmlAttribute(AttributeName = "label")]
                    public string Label { get; set; }
                }

                [XmlRoot(ElementName = "chunks")]
                public class Chunks
                {
                    [XmlElement(ElementName = "chunk")]
                    public Chunk Chunk { get; set; }
                }

                [XmlRoot(ElementName = "scenario")]
                public class Scenario
                {
                    [XmlAttribute(AttributeName = "id")]
                    public string Id { get; set; }
                    [XmlAttribute(AttributeName = "type")]
                    public string Type { get; set; }
                    [XmlAttribute(AttributeName = "initial_chunk_count")]
                    public string Initial_chunk_count { get; set; }
                    [XmlAttribute(AttributeName = "label")]
                    public string Label { get; set; }
                    [XmlText]
                    public string Text { get; set; }
                }

                [XmlRoot(ElementName = "scenarios")]
                public class Scenarios
                {
                    [XmlElement(ElementName = "scenario")]
                    public Scenario Scenario { get; set; }
                    [XmlAttribute(AttributeName = "default_id")]
                    public string Default_id { get; set; }
                }

                [XmlRoot(ElementName = "chunk_info")]
                public class Chunk_info
                {
                    [XmlElement(ElementName = "chunks")]
                    public Chunks Chunks { get; set; }
                    [XmlElement(ElementName = "scenarios")]
                    public Scenarios Scenarios { get; set; }
                    [XmlAttribute(AttributeName = "chunk_count")]
                    public string Chunk_count { get; set; }
                    [XmlAttribute(AttributeName = "scenario_count")]
                    public string Scenario_count { get; set; }
                }

                [XmlRoot(ElementName = "volume")]
                public class Volume
                {
                    [XmlElement(ElementName = "volume_type")]
                    public string Volume_type { get; set; }
                    [XmlElement(ElementName = "volume_id")]
                    public string Volume_id { get; set; }
                    [XmlElement(ElementName = "volume_ts")]
                    public string Volume_ts { get; set; }
                    [XmlElement(ElementName = "package")]
                    public Package Package { get; set; }
                    [XmlElement(ElementName = "chunk_info")]
                    public Chunk_info Chunk_info { get; set; }
                }

                [XmlRoot(ElementName = "file")]
                public class File
                {
                    [XmlAttribute(AttributeName = "targ_path")]
                    public string Targ_path { get; set; }
                    [XmlAttribute(AttributeName = "orig_path")]
                    public string Orig_path { get; set; }
                }

                [XmlRoot(ElementName = "files")]
                public class Files
                {
                    [XmlElement(ElementName = "file")]
                    public List<File> File { get; set; }
                    [XmlAttribute(AttributeName = "img_no")]
                    public string Img_no { get; set; }
                }

                [XmlRoot(ElementName = "dir")]
                public class Dir
                {
                    [XmlAttribute(AttributeName = "targ_name")]
                    public string Targ_name { get; set; }
                }

                [XmlRoot(ElementName = "rootdir")]
                public class Rootdir
                {
                    [XmlElement(ElementName = "dir")]
                    public List<Dir> Dir { get; set; }
                }

                [XmlRoot(ElementName = "psproject")]
                public class Psproject
                {
                    [XmlElement(ElementName = "volume")]
                    public Volume Volume { get; set; }
                    [XmlElement(ElementName = "files")]
                    public Files Files { get; set; }
                    [XmlElement(ElementName = "rootdir")]
                    public Rootdir Rootdir { get; set; }
                    [XmlAttribute(AttributeName = "fmt")]
                    public string Fmt { get; set; }
                    [XmlAttribute(AttributeName = "version")]
                    public string Version { get; set; }
                }

                /// <summary>
                /// Read a PS4 GP4 
                /// </summary>
                /// <param name="gp4filelocation">gp4 File Location</param>
                /// <returns></returns>
                public static Psproject ReadGP4(string gp4filelocation)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Psproject));
                    using (FileStream fileStream = new FileStream(gp4filelocation, FileMode.Open))
                    {
                        Psproject result = (Psproject)serializer.Deserialize(fileStream);
                        return result;
                    }
                }

                /// <summary>
                /// Read a PS4 PG4
                /// </summary>
                /// <param name="GP4File">Gp4 File Stream</param>
                /// <returns></returns>
                public static Psproject ReadGP4(Stream GP4File)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Psproject));
                    using (TextReader reader = new StreamReader(GP4File))
                    {
                        Psproject result = (Psproject)serializer.Deserialize(reader);
                        return result;
                    }
                }

                /// <summary>
                /// Save GP4
                /// </summary>
                /// <param name="savelocation">GP4 File Location</param>
                /// <param name="gp4project">gp4 Project</param>
                public static void SaveGP4(string savelocation, Psproject gp4project)
                {
                    try
                    {
                        var xmlserializer = new XmlSerializer(typeof(Psproject));
                        var stringWriter = new StringWriter();
                        using (var writer = XmlWriter.Create(stringWriter))
                        {
                            xmlserializer.Serialize(writer, gp4project);
                            string savestring = stringWriter.ToString();
                            System.IO.File.WriteAllText(savelocation, savestring);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occurred", ex);
                    }
                }

            }

            public class IDS
            {

            }

            public class PARAM_SFO
            {
                public static Param_SFO.PARAM_SFO Get_Param_SFO(string pkgfile)
                {
                    //PARAM_SFO 
                    List<string> lstoffiles = Official.ReadAllUnprotectedData(pkgfile);

                    if (lstoffiles.Contains("Sc0/nptitle.dat\r"))
                    {
                        if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\Working"))
                        {
                            Directory.CreateDirectory(PS4_Tools.AppCommonPath() + @"\Working");
                        }

                        //extract files to temp folder
                        ProcessStartInfo start = new ProcessStartInfo();
                        start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
                        start.Arguments = "img_extract --no_passcode \"" + pkgfile + "\" \"" + PS4_Tools.AppCommonPath() + @"Working" + "\"";
                        start.UseShellExecute = false;
                        start.RedirectStandardOutput = true;
                        start.CreateNoWindow = true;
                        using (Process process = Process.Start(start))
                        {
                            process.ErrorDataReceived += delegate
                            {

                            };
                            using (StreamReader reader = process.StandardOutput)
                            {
                                string result = reader.ReadToEnd();


                                // return result;
                            }
                        }

                        return new Param_SFO.PARAM_SFO(PS4_Tools.AppCommonPath() + @"\Working\Sc0\Param.sfo");

                    }
                    return new Param_SFO.PARAM_SFO();
                }
            }

            public class NP_Data
            {

            }

            public class NP_Title
            {

            }

            #region << UnprotectedPKG >>

            private static string m_pkgfile;
            private static bool m_loaded;
            private static byte[] sfo_byte;
            private static byte[] icon_byte;
            private static byte[] pic_byte;
            private static byte[] trp_byte;
            private static bool m_error;
            public enum PKGType
            {
                Game,
                App,
                Addon_Theme,
                Patch,
                Unknown
            }

            private class Names
            {
                public Names(int m_Index, ulong m_Offset, ulong m_Size, string m_Name)
                {
                    this.Index = m_Index;
                    this.Offset = m_Offset;
                    this.Size = m_Size;
                    this.Name = m_Name;
                }

                public int Index;

                public ulong Offset;

                public ulong Size;

                public string Name;
            }

            public enum PKG_State
            {
               Fake =0,
               Official = 1,
               Officail_DP = 2,
               Unkown = 99 
            }

            private static PKGType GetPkgType(string str)
            {
                if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(str, "gde", false) == 0 || Microsoft.VisualBasic.CompilerServices.Operators.CompareString(str, "gdk", false) == 0)
                {
                    return PKGType.App;
                }
                if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(str, "gd", false) == 0)
                {
                    return PKGType.Game;
                }
                if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(str, "ac", false) == 0)
                {
                    return PKGType.Addon_Theme;
                }
                if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(str, "gp", false) == 0)
                {
                    return PKGType.Patch;
                }
                return PKGType.Unknown;
            }

            public class Unprotected_PKG
            {
                /*Param.SFO*/
                public Param_SFO.PARAM_SFO Param { get; set; }
                /*Trophy File*/
                public Trophy_File Trophy_File { get; set; }
                /*PKG Image*/
                public Bitmap Image { get; set; }
                /*PKG State (Fake ? Offcial */

                public PKG_State PKGState { get; set; }

                public PKGType PKG_Type
                {
                    get
                    {
                        return GetPkgType(Param.Category);
                        //return PKGType.Unknown;
                    }
                }

                public string PS4_Title
                {
                    get
                    {
                        return Param.Title;
                    }
                }
            }


            private static byte[] PKG_Magic = new byte[]{ 0x7F, 0x43, 0x4E, 0x54 };

            /// <summary>
            /// Reads a PS4 PKG Into an Unprotected_PKG Object
            /// </summary>
            /// <param name="pkgfile">PKG File Location</param>
            /// <returns>PKG With Unprotected_PKG Structure</returns>
            public static Unprotected_PKG Read_PKG(string pkgfile)
            {
                Unprotected_PKG pkgreturn = new Unprotected_PKG();
                m_loaded = false;
                sfo_byte = null;
                icon_byte = null;
                pic_byte = null;
                trp_byte = null;
                
                if (!File.Exists(pkgfile))
                {
                    throw new Exception("File not found!");
                }
                List<Names> list = new List<Names>();
                byte[] array = new byte[]
                {
                    0,
                    112,
                    97,
                    114,
                    97,
                    109,
                    46,
                    115,
                    102,
                    111,
                    0
                };
                StringBuilder stringBuilder = new StringBuilder();
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(pkgfile)))
                {
                    /*Check PS4 File Header*/
                    Byte[] PKGFileHeader = binaryReader.ReadBytes(4);
                    if(!Util.Utils.CompareBytes(PKGFileHeader,PKG_Magic))/*If Files Match*/
                    {
                        //fail
                        throw new Exception("This is not a valid ps4 pkg");
                    }
                    binaryReader.BaseStream.Seek(24L, SeekOrigin.Begin);
                    uint num = Util.Utils.ReadUInt32(binaryReader);
                    uint num2 = Util.Utils.ReadUInt32(binaryReader);
                    binaryReader.BaseStream.Seek(44L, SeekOrigin.Begin);
                    uint num3 = Util.Utils.ReadUInt32(binaryReader);
                    binaryReader.BaseStream.Seek(64L, SeekOrigin.Begin);
                    string text = Util.Utils.ReadASCIIString(binaryReader, 36);
                    binaryReader.BaseStream.Seek(119L, SeekOrigin.Begin);
                    ushort num4 = Util.Utils.ReadUInt16(binaryReader);
                    Dictionary<long, long> dictionary;
                    uint num5;
                    uint num6;
                    checked
                    {
                        binaryReader.BaseStream.Seek((long)(unchecked((ulong)num) + 176UL), SeekOrigin.Begin);
                        dictionary = new Dictionary<long, long>();
                        num5 = Util.Utils.ReadUInt32(binaryReader);
                        num6 = Util.Utils.ReadUInt32(binaryReader);
                        binaryReader.BaseStream.Seek(binaryReader.BaseStream.Position + 24L, SeekOrigin.Begin);
                    }
                    do
                    {
                        dictionary.Add((long)((ulong)num5), (long)((ulong)num6));
                        num5 = Util.Utils.ReadUInt32(binaryReader);
                        num6 = Util.Utils.ReadUInt32(binaryReader);
                        binaryReader.BaseStream.Seek(checked(binaryReader.BaseStream.Position + 24L), SeekOrigin.Begin);
                    }
                    while ((ulong)(checked(num5 + num6)) > 0UL);
                    checked
                    {
                        int num8 = 0;
                        try
                        {
                            foreach (KeyValuePair<long, long> keyValuePair in dictionary)
                            {
                                try
                                {
                                    binaryReader.BaseStream.Seek(keyValuePair.Key, SeekOrigin.Begin);
                                    uint num7 = binaryReader.ReadUInt32();
                                    binaryReader.BaseStream.Seek(keyValuePair.Key, SeekOrigin.Begin);
                                    if (unchecked((ulong)num7) == 1179865088UL && sfo_byte == null)
                                    {
                                        sfo_byte = Util.Utils.ReadByte(binaryReader, (int)keyValuePair.Value);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                num8++;
                            }
                        }
                        finally
                        {
                            Dictionary<long, long>.Enumerator enumerator2 = new Dictionary<long, long>.Enumerator();
                            ((IDisposable)enumerator2).Dispose();
                        }
                        try
                        {
                            try
                            {
                                foreach (KeyValuePair<long, long> keyValuePair2 in dictionary)
                                {
                                    binaryReader.BaseStream.Seek(keyValuePair2.Key, SeekOrigin.Begin);
                                    if (Util.Utils.Contain(binaryReader.ReadBytes(array.Length), array))
                                    {
                                        binaryReader.BaseStream.Seek(keyValuePair2.Key, SeekOrigin.Begin);
                                        byte[] array2 = binaryReader.ReadBytes((int)keyValuePair2.Value);
                                        int num9 = 1;
                                        int num10 = array2.Length - 1;
                                        for (int i = num9; i <= num10; i++)
                                        {
                                            if (array2[i] == 0)
                                            {
                                                list.Add(new Names(list.Count, (ulong)dictionary.Keys.ElementAtOrDefault(num8), (ulong)dictionary.Values.ElementAtOrDefault(num8), stringBuilder.ToString()));
                                                num8++;
                                                stringBuilder.Clear();
                                            }
                                            stringBuilder.Append(Util.Utils.HexToString(Microsoft.VisualBasic.Conversion.Hex(array2[i])));
                                        }
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                Dictionary<long, long>.Enumerator enumerator3 = new Dictionary<long, long>.Enumerator();
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        catch (Exception ex2)
                        {
                        }
                        try
                        {
                            foreach (Names names in list)
                            {
                                /*List Of names*/
                            }
                        }
                        finally
                        {
                            List<Names>.Enumerator enumerator4 = new List<Names>.Enumerator();
                            ((IDisposable)enumerator4).Dispose();
                        }
                        Names names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "param.sfo", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            sfo_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "icon0.png", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            icon_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        else
                        {
                            names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "icon1.png", false) == 0);
                            if (names2 != null)
                            {
                                binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                                icon_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                            }
                        }
                        names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "trophy/trophy00.trp", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            trp_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        else
                        {
                            names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "trophy/trophy01.trp", false) == 0);
                            if (names2 != null)
                            {
                                binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                                trp_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                            }
                        }
                        if (sfo_byte != null && sfo_byte.Length > 0)
                        {
                            Param_SFO.PARAM_SFO psfo = new Param_SFO.PARAM_SFO(sfo_byte);
                            pkgreturn.Param = psfo;
                        }
                        if (icon_byte != null && icon_byte.Length > 0)
                        {
                            pkgreturn.Image = Util.Utils.BytesToBitmap(icon_byte);
                        }
                        else if (trp_byte != null && trp_byte.Length > 0)
                        {
                            Trophy_File trpreader = new Trophy_File();
                            trpreader.Load(trp_byte);
                            icon_byte = trpreader.ExtractFileToMemory("ICON0.PNG");
                            if (icon_byte != null && icon_byte.Length > 0)
                            {
                                pkgreturn.Image = Util.Utils.BytesToBitmap(icon_byte);
                            }
                        }
                        if(trp_byte != null && trp_byte.Length > 0)
                        {
                            Trophy_File trpreader = new Trophy_File();
                            //trpreader.Load(trp_byte);
                            pkgreturn.Trophy_File = trpreader.Load(trp_byte);
                        }
                       pkgreturn.PKGState = (num4 == 6666) ? PKG_State.Fake : ((num4 == 7747) ? PKG_State.Officail_DP : PKG_State.Official);
                    }
                }
                m_loaded = true;
                return pkgreturn;
            }

            /// <summary>
            /// Reads a PS4 PKG Into an Unprotected_PKG Object
            /// </summary>
            /// <param name="pkgfile">PKG File Stream</param>
            /// <returns>PKG With Unprotected_PKG Structure</returns>
            public static Unprotected_PKG Read_PKG(Stream pkgfile)
            {
                Unprotected_PKG pkgreturn = new Unprotected_PKG();
                m_loaded = false;
                sfo_byte = null;
                icon_byte = null;
                pic_byte = null;
                trp_byte = null;

                List<Names> list = new List<Names>();
                byte[] array = new byte[]
                {
                    0,
                    112,
                    97,
                    114,
                    97,
                    109,
                    46,
                    115,
                    102,
                    111,
                    0
                };
                StringBuilder stringBuilder = new StringBuilder();
                using (BinaryReader binaryReader = new BinaryReader(pkgfile))
                {
                    /*Check PS4 File Header*/
                    Byte[] PKGFileHeader = binaryReader.ReadBytes(4);
                    if (!Util.Utils.CompareBytes(PKGFileHeader, PKG_Magic))/*If Files Match*/
                    {
                        //fail
                        throw new Exception("This is not a valid ps4 pkg");
                    }
                    binaryReader.BaseStream.Seek(24L, SeekOrigin.Begin);
                    uint num = Util.Utils.ReadUInt32(binaryReader);
                    uint num2 = Util.Utils.ReadUInt32(binaryReader);
                    binaryReader.BaseStream.Seek(44L, SeekOrigin.Begin);
                    uint num3 = Util.Utils.ReadUInt32(binaryReader);
                    binaryReader.BaseStream.Seek(64L, SeekOrigin.Begin);
                    string text = Util.Utils.ReadASCIIString(binaryReader, 36);
                    binaryReader.BaseStream.Seek(119L, SeekOrigin.Begin);
                    ushort num4 = Util.Utils.ReadUInt16(binaryReader);
                    Dictionary<long, long> dictionary;
                    uint num5;
                    uint num6;
                    checked
                    {
                        binaryReader.BaseStream.Seek((long)(unchecked((ulong)num) + 176UL), SeekOrigin.Begin);
                        dictionary = new Dictionary<long, long>();
                        num5 = Util.Utils.ReadUInt32(binaryReader);
                        num6 = Util.Utils.ReadUInt32(binaryReader);
                        binaryReader.BaseStream.Seek(binaryReader.BaseStream.Position + 24L, SeekOrigin.Begin);
                    }
                    do
                    {
                        dictionary.Add((long)((ulong)num5), (long)((ulong)num6));
                        num5 = Util.Utils.ReadUInt32(binaryReader);
                        num6 = Util.Utils.ReadUInt32(binaryReader);
                        binaryReader.BaseStream.Seek(checked(binaryReader.BaseStream.Position + 24L), SeekOrigin.Begin);
                    }
                    while ((ulong)(checked(num5 + num6)) > 0UL);
                    checked
                    {
                        int num8 = 0;
                        try
                        {
                            foreach (KeyValuePair<long, long> keyValuePair in dictionary)
                            {
                                try
                                {
                                    binaryReader.BaseStream.Seek(keyValuePair.Key, SeekOrigin.Begin);
                                    uint num7 = binaryReader.ReadUInt32();
                                    binaryReader.BaseStream.Seek(keyValuePair.Key, SeekOrigin.Begin);
                                    if (unchecked((ulong)num7) == 1179865088UL && sfo_byte == null)
                                    {
                                        sfo_byte = Util.Utils.ReadByte(binaryReader, (int)keyValuePair.Value);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                num8++;
                            }
                        }
                        finally
                        {
                            Dictionary<long, long>.Enumerator enumerator2 = new Dictionary<long, long>.Enumerator();
                            ((IDisposable)enumerator2).Dispose();
                        }
                        try
                        {
                            try
                            {
                                foreach (KeyValuePair<long, long> keyValuePair2 in dictionary)
                                {
                                    binaryReader.BaseStream.Seek(keyValuePair2.Key, SeekOrigin.Begin);
                                    if (Util.Utils.Contain(binaryReader.ReadBytes(array.Length), array))
                                    {
                                        binaryReader.BaseStream.Seek(keyValuePair2.Key, SeekOrigin.Begin);
                                        byte[] array2 = binaryReader.ReadBytes((int)keyValuePair2.Value);
                                        int num9 = 1;
                                        int num10 = array2.Length - 1;
                                        for (int i = num9; i <= num10; i++)
                                        {
                                            if (array2[i] == 0)
                                            {
                                                list.Add(new Names(list.Count, (ulong)dictionary.Keys.ElementAtOrDefault(num8), (ulong)dictionary.Values.ElementAtOrDefault(num8), stringBuilder.ToString()));
                                                num8++;
                                                stringBuilder.Clear();
                                            }
                                            stringBuilder.Append(Util.Utils.HexToString(Microsoft.VisualBasic.Conversion.Hex(array2[i])));
                                        }
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                Dictionary<long, long>.Enumerator enumerator3 = new Dictionary<long, long>.Enumerator();
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        catch (Exception ex2)
                        {
                        }
                        try
                        {
                            foreach (Names names in list)
                            {
                                /*List Of names*/
                            }
                        }
                        finally
                        {
                            List<Names>.Enumerator enumerator4 = new List<Names>.Enumerator();
                            ((IDisposable)enumerator4).Dispose();
                        }
                        Names names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "param.sfo", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            sfo_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "icon0.png", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            icon_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        else
                        {
                            names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "icon1.png", false) == 0);
                            if (names2 != null)
                            {
                                binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                                icon_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                            }
                        }
                        names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "trophy/trophy00.trp", false) == 0);
                        if (names2 != null)
                        {
                            binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                            trp_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                        }
                        else
                        {
                            names2 = list.Find((Names b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(b.Name, "trophy/trophy01.trp", false) == 0);
                            if (names2 != null)
                            {
                                binaryReader.BaseStream.Seek((long)names2.Offset, SeekOrigin.Begin);
                                trp_byte = Util.Utils.ReadByte(binaryReader, (int)names2.Size);
                            }
                        }
                        if (sfo_byte != null && sfo_byte.Length > 0)
                        {
                            Param_SFO.PARAM_SFO psfo = new Param_SFO.PARAM_SFO(sfo_byte);
                            pkgreturn.Param = psfo;
                        }
                        if (icon_byte != null && icon_byte.Length > 0)
                        {
                            pkgreturn.Image = Util.Utils.BytesToBitmap(icon_byte);
                        }
                        else if (trp_byte != null && trp_byte.Length > 0)
                        {
                            Trophy_File trpreader = new Trophy_File();
                            trpreader.Load(trp_byte);
                            icon_byte = trpreader.ExtractFileToMemory("ICON0.PNG");
                            if (icon_byte != null && icon_byte.Length > 0)
                            {
                                pkgreturn.Image = Util.Utils.BytesToBitmap(icon_byte);
                            }
                        }
                        if (trp_byte != null && trp_byte.Length > 0)
                        {
                            Trophy_File trpreader = new Trophy_File();
                            //trpreader.Load(trp_byte);
                            pkgreturn.Trophy_File = trpreader.Load(trp_byte);
                        }
                        pkgreturn.PKGState = (num4 == 6666) ? PKG_State.Fake : ((num4 == 7747) ? PKG_State.Officail_DP : PKG_State.Official);
                    }
                }
                m_loaded = true;
                return pkgreturn;
            }

            #endregion << UnprotectedPKG >>

            public string pkgtable { get; set; }
            private static LibOrbisPkg.PKG.Pkg pkg = null;
            public static List<ListViewItem> lst = new List<ListViewItem>();

            /// <summary>
            /// Reads a pkg file and displays all its info inside the PKG Table 
            /// (Custom to maxtron)
            /// </summary>
            /// <param name="pkgFile"></param>
            public static void ReadPKG(string pkgFile)
            {
                var pkggame = GameArchives.Util.LocalFile(pkgFile);
                using (var stream = pkggame.GetStream())
                    pkg = new LibOrbisPkg.PKG.PkgReader(stream).ReadPkg();
                //LibOrbisPkg.PKG.
                //GameArchives.PackageReader.ReadPackageFromFile(pkg);
                try
                {
                    //var pkggame = GameArchives.Util.LocalFile(pkgFile);
                    var package = PackageReader.ReadPackageFromFile(pkggame);
                    var sfo = PackageReader.ReadPackageFromFile(package.GetFile("/param.sfo"));
                    var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
                    PackageManager.GetInstance();
                }
                catch (Exception ex)
                {

                }

                foreach (var e in pkg.Metas.Metas)
                {
                    var lvi = new ListViewItem(new[] {
          e.id.ToString(),
          string.Format("0x{0:X}", e.DataSize),
          string.Format("0x{0:X}", e.DataOffset),
          e.Encrypted ? "Yes" : "No",
          e.KeyIndex.ToString(),
        });
                    lvi.Tag = e;
                    //entriesListView.Items.Add(lvi);
                    lst.Add(lvi);
                }
            }

            /// <summary>
            /// This one is pretty straight Forward it renames a pkg file to the content id name
            /// </summary>
            /// <param name="pkgfile"></param>
            public static void Rename_pkg_To_ContentID(string pkgfile)
            {

            }

            /// <summary>
            /// This one is pretty straight Forward it renames a pkg file to the Title Of The Package
            /// </summary>
            /// <param name="pkgfile"></param>
            public static void Rename_pkg_To_Title(string pkgfile)
            {

            }
        }

        #endregion << Scene Related >>

        /// <summary>
        /// Class is used for PS2 Classic Building
        /// </summary>
        public class PS2_Classics
        {
            public PS2_Classics()
            {
                Working_Dir = PS4_Tools.AppCommonPath() + @"\Working\";
            }

            /// <summary>
            /// This is the temp directory where the app needs to work if not set by the users the app will set itself
            /// </summary>
            public string Working_Dir { get; set; }

            public string PS2ID { get; set; }

            /// <summary>
            /// Creates a single ISO Game For PS4
            /// </summary>
            /// <param name="PS2_ISO">PS2 ISo Location</param>
            /// <param name="SaveFileLOcation">Preferred pkg save location</param>
            /// <param name="Icon0Location">location of ICON0 if none set default is used</param>
            /// <param name="BackgroundLocation">Location of Background image if none is set default is used</param>
            /// <param name="CustomGP4Location">Use a custom GP4 File For Repacking the ISO</param>
            public void Create_Single_ISO_PKG(string PS2_ISO, string SaveFileLOcation, string Title, string ContentID = "", Bitmap Icon0 = null, string BackgroundLocation = "", string CustomGP4Location = "")
            {
                Console.WriteLine("Reading PS2 ISO");

                #region << CNF Reader >>

                //start off by reading the ISO FIle
                //we need to get the control file info
                //now using the file stream we can read the CNF file
                using (FileStream isoStream = File.OpenRead(PS2_ISO))
                {
                    //use disk utils to read iso file
                    CDReader cd = new CDReader(isoStream, true);
                    //look for the specific file naimly the system config file
                    Stream fileStream = cd.OpenFile(@"SYSTEM.CNF", FileMode.Open);
                    // Use fileStream...
                    TextReader tr = new StreamReader(fileStream);
                    string fullstring = tr.ReadToEnd();//read string to end this will read all the info we need

                    //mine for info
                    string Is = @"\";
                    string Ie = ";";

                    //mine the start and end of the string
                    int start = fullstring.ToString().IndexOf(Is) + Is.Length;
                    int end = fullstring.ToString().IndexOf(Ie, start);
                    if (end > start)
                    {
                        string PS2Id = fullstring.ToString().Substring(start, end - start);

                        if (PS2Id != string.Empty)
                        {
                            PS2ID = PS2Id.Replace(".", "");
                            Console.WriteLine("PS2 ID Found" + PS2Id);
                        }
                        else
                        {
                            Console.WriteLine("Could not load PS2 ID");
                            return;
                        }
                    }
                }

                #endregion << CNF Reader >>

                #region << Set Up Working Directory >>

                if (!Directory.Exists(Working_Dir))
                {
                    Console.WriteLine("Created " + Working_Dir);
                    Directory.CreateDirectory(Working_Dir);
                }
                if (!Directory.Exists(Working_Dir + @"\PS2Emu\"))
                {
                    Console.WriteLine("Created " + Working_Dir + @"\PS2Emu\");
                    Directory.CreateDirectory(Working_Dir + @"\PS2Emu\");
                }

                Console.WriteLine("Writing " + Working_Dir + @"\PS2Emu\" + "param.sfo");
                System.IO.File.WriteAllBytes(Working_Dir + @"\PS2Emu\" + "param.sfo", Properties.Resources.param);

                Console.WriteLine("Writing " + Working_Dir + "PS2Classics.gp4");
                System.IO.File.WriteAllBytes(Working_Dir + "PS2Classics.gp4", Properties.Resources.PS2Classics);

                #endregion << Set Up Wokring Directory >>

                #region << LOad and update gp4 and sfo Project >>
                Console.WriteLine("Loading GP4 Project");
                var project = SceneRelated.GP4.ReadGP4(Working_Dir + "PS2Classics.gp4");
                Console.WriteLine("Loading SFO");
                var sfo = new Param_SFO.PARAM_SFO(Working_Dir + @"\PS2Emu\" + "param.sfo");

                if (ContentID == "")
                {
                    Console.WriteLine("No Content ID Specified Building custom one");
                    //build custom content id
                    ContentID = "UP9000-" + sfo.TitleID.Trim() + "_00-" + PS2ID.Trim() + "0000001";

                    Console.WriteLine("Content ID :" + ContentID);
                }
                //update sfo info

                Console.WriteLine("Updating SFO ");
                for (int i = 0; i < sfo.Tables.Count; i++)
                {

                    if (sfo.Tables[i].Name == "CONTENT_ID")
                    {
                        var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Content ID ( " + tempitem.Value + " -> " + ContentID + ")");
                        tempitem.Value = ContentID;
                        sfo.Tables[i] = tempitem;
                    }
                    if (sfo.Tables[i].Name == "TITLE")
                    {
                        var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Title ( " + tempitem.Value + " -> " + Title + ")");
                        tempitem.Value = Title;
                        sfo.Tables[i] = tempitem;
                    }
                    if (sfo.Tables[i].Name == "TITLE_ID")
                    {
                        var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Title ID ( " + tempitem.Value + " -> " + PS2ID + ")");
                        tempitem.Value = PS2ID;
                        sfo.Tables[i] = tempitem;
                    }
                }

                Console.WriteLine("Saving SFO");
                sfo.SaveSFO(sfo, Working_Dir + @"\PS2Emu\" + "param.sfo");//update sfo info
                                                                          //update GP4

                Console.WriteLine("Upating GP4");
                project.Volume.Package.Content_id = ContentID;//set contentid
                project.Volume.Package.Passcode = "00000000000000000000000000000000";//32 zeros

                //this is single iso building so we shouldn't have to change disc image numbering
                SceneRelated.GP4.SaveGP4(Working_Dir + "PS2Classics.gp4", project);

                Console.WriteLine("Saving GP4");
                #endregion << Load GP4 Project >>


                #region << Save Image Files to corresponding locations and also change to correct format >>

                Console.WriteLine("Saving Images");

                Image.PNG ps4icon0 = new Image.PNG();

                Bitmap icon0 = ps4icon0.Create_PS4_Compatible_PNG(Icon0);
                icon0.Save(PS4_Tools.AppCommonPath() + @"PS2\sce_sys\icon0.png", System.Drawing.Imaging.ImageFormat.Png);

                #endregion  << Save Image Files to corresponding locations and also change to correct format >>
            }



            /// <summary>
            /// Creates a Multi Iso Repacked PS2 Classic
            /// </summary>
            /// <param name="PS2_ISO_s">List of iso locations </param>
            /// <param name="SaveFileLOcation">Preferred pkg save location</param>
            /// <param name="Icon0Location">location of ICON0 if none set default is used</param>
            /// <param name="BackgroundLocation">Location of Background image if none is set default is used</param>
            /// <param name="CustomGP4Location">Use a custom GP4 File For Repacking the ISO</param>
            public void Create_Multi_ISO_PKG(List<string> PS2_ISO_s, string SaveFileLOcation, string Icon0Location = "", string BackgroundLocation = "", string CustomGP4Location = "")
            {
                throw new Exception("Function not built");
            }
        }

        /// <summary>
        /// Class is used for PSP HD Building
        /// </summary>
        public class PSP_HD
        {

        }
    }
    /************************************
     * Credit TO IDC
     ************************************/
    public class PUP
    {
        public void Unpack_PUP(string PUPFile, string SaveToDir, bool SaveTables = false)
        {
            if (!File.Exists(PUPFile))
            {
                throw new Exception("PUP File location does not exist \nLocationSupplied " + PUPFile);
            }

            if (!Directory.Exists(SaveToDir))
            {
                throw new Exception("Save location does not exist \nLocationSupplied " + SaveToDir);
            }


            Unpacker unpacker = new Unpacker();
            unpacker.Unpack(PUPFile, SaveToDir, SaveTables);
        }
    }
}