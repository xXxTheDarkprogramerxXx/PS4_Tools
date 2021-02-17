/* Copyright (c)  2018 TheDarkporgramer
*
*
* 
*/

#region << System >>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

#endregion << System >>

#region << Disc Utils >>

#if !PS4_UNITY
#if !Android_Mono
using DiscUtils.Iso9660;
#endif
#endif
#endregion << Disc Utils >>

#region << VGAudio >>
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
#endregion << VGAudio >>

#region << Json >>

using Newtonsoft.Json;

#endregion << Json >>

#region << Graphics >>

using System.Drawing;
using System.Drawing.Drawing2D;

#endregion << Graphics >>

#region << PS4 Tools >>

using PS4_Tools.LibOrbis.PKG;
using PS4_Tools.LibOrbis.Util;
using PS4_Tools.Util;


#endregion << PS4 Tools >>

#region << Unity Engine >>

#if UNITY_EDITOR || UNITY_PS4 || PS4_UNITY || DEBUG

using UnityEngine;

#endif
using System.Runtime.InteropServices;

#endregion << Unity Engine >>


#region << SQL Lite >>

using System.Data.SQLite;
using System.Data;

#endregion << SQL Lite >>

namespace PS4_Tools
{

    /// <summary>
    /// Reserved class for some PS4 Tools helpers
    /// </summary>
    internal class PS4_Tools
    {
        /// <summary>
        /// App Common Path will be used to store some items locally 
        /// </summary>
        /// <returns>PS4_Tools execution directory</returns>
        internal static string AppCommonPath()
        {
            string rtn = "";
            if (Sys.isLinux == false)
            {
                rtn = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("PS4_Tools.dll", ""); // Get Location of the execution directory
            }
            else
            {
                rtn = "/PS4Tools";
                if (!Directory.Exists(rtn))
                {
                    Directory.CreateDirectory(rtn);
                }
                else
                {
                    DeleteDirectory(rtn);
                    Directory.CreateDirectory(rtn);
                }

            }
            return rtn;
        }

        /// <summary>
        /// Deletes all files within a Directory and also deletes a Directory
        /// </summary>
        /// <param name="target_dir"></param>
        internal static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir); //get all files
                string[] dirs = Directory.GetDirectories(target_dir); //get all directories

                //get all files and delete file where needed
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                //get all dirextoies and delete directories
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
        internal static void CopyDir(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Get Files & Copy
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);

                // ADD Unique File Name Check to Below!!!!
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            // Get dirs recursively and copy files
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyDir(folder, dest);
            }
        }
        internal static void MoveDirectory(string sourceFolder, string destFolder)
        {
            CopyDir(sourceFolder, destFolder);
            DeleteDirectory(sourceFolder);
        }
    }

    /// <summary>
    /// PS4 Self file handeling
    /// </summary>
    public class SELF
    {
        #region SelfStruct

        /// <summary>
        /// Self Header 
        /// </summary>
        public class Self_Header
        {
            public const UInt32 signature = 0x1D3D154Fu;

            public UInt32 magic = new UInt32();
            public UInt32 unknown04 = new UInt32();
            public UInt32 unknown08 = new UInt32();
            public UInt16 header_size = new UInt16();
            public UInt16 unknown_size = new UInt16();
            public UInt32 file_size = new UInt32();
            public UInt32 unknown14 = new UInt32();
            public UInt16 segment_count = new UInt16();
            public UInt16 unknown1A = new UInt16();
            public UInt32 unknown1C = new UInt32();
        }

        //public class self_info
        //{
        //    public hilo64_t id = new hilo64_t();
        //    public hilo64_t unknown08 = new hilo64_t();
        //    public hilo64_t system_version_1 = new hilo64_t();
        //    public hilo64_t system_version_2 = new hilo64_t();
        //    public uint8_t[] content_id = Arrays.InitializeWithDefaultInstances<uint8_t>(32);
        //}

        //public class Self_Segment_Header
        //{
        //    public UInt32 flags = new uint();
        //    public uint unknown04 = new uint();
        //    public hilo64_t offset = new hilo64_t();
        //    public hilo64_t compressed_size = new hilo64_t();
        //    public hilo64_t uncompressed_size = new hilo64_t();
        //}

        #endregion
    }

    /// <summary>
    /// PS4 Media Handler 
    /// </summary>
    public class Media
    {
        /// <summary>
        /// ATRAC9 Reserved class
        /// Credits for the Atract9 work goes to Thealexbarney he did some amazing work on it for VGAduio and LibAtrac9
        /// https://github.com/Thealexbarney/LibAtrac9 
        /// </summary>
        public class Atrac9
        {
            /// <summary>
            /// Atract 9 Structure class
            /// </summary>
            public class At9Structure
            {
                public LibAtrac9.Atrac9Config Config { get; set; }
                public byte[][] AudioData { get; set; }
                public int SampleCount { get; set; }
                public int Version { get; set; }
                public int EncoderDelay { get; set; }
                public int SuperframeCount { get; set; }

                public bool Looping { get; set; }
                public int LoopStart { get; set; }
                public int LoopEnd { get; set; }
            }

            /// <summary>
            /// Allows users to load an at9 for decoding and returns the wav as a byte array
            /// </summary>
            /// <param name="at9file">Location of at9 file</param>
            /// <returns>Byte Array of Wav file</returns>
            public static byte[] LoadAt9(string at9file)
            {
                //Byte array holder for return vars
                byte[] array;

                using (Stream stream = new FileStream(at9file, FileMode.Open, FileAccess.Read))
                using (BinaryReader read = new BinaryReader(stream))
                {
                    At9Reader reader = new At9Reader();
                    At9Structure structure = reader.ReadFile(stream);
                    IAudioFormat format = new Atrac9FormatBuilder(structure.AudioData, structure.Config, structure.SampleCount, structure.EncoderDelay)
                .WithLoop(structure.Looping, structure.LoopStart, structure.LoopEnd)
                .Build();
                    //now we have the atrac9 format now we need to play it somehow
                    AudioData AudioData = new AudioData(format);
                    MemoryStream SongStream = new MemoryStream(0);
                    AudioInfo.Containers[FileType.Wave].GetWriter().WriteToStream(AudioData, SongStream, null);

#if DEBUG
                    /*Uncomment this if you need to test but this definitely works*/
                    //System.IO.File.WriteAllBytes(at9file + ".wav", SongStream.ToArray());

#endif

                    array = SongStream.ToArray();



                }
                return array;
            }

            /// <summary>
            /// Allows user to load an at9 to At9Structure
            /// </summary>
            /// <param name="at9file"></param>
            /// <returns></returns>
            public static At9Structure Load_At9(string at9file)
            {
                At9Structure at9 = new At9Structure();

                using (Stream stream = new FileStream(at9file, FileMode.Open, FileAccess.Read))
                using (BinaryReader read = new BinaryReader(stream))
                {
                    At9Reader reader = new At9Reader();
                    At9Structure structure = reader.ReadFile(stream);
                    at9 = structure;
                }


                return at9;
            }
        }
    }

    /// <summary>
    /// PS4 Tools Image Reserved class all Image files will be referenced in here
    /// </summary>
    public class Image
    {
        /// <summary>
        /// PS4 PNG Image Handling
        /// </summary>
        public class PNG
        {
#if !PS4_UNITY

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

                using (var graphics = System.Drawing.Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
#endif

#if !PS4_UNITY

            /// <summary>
            /// this convers images to 24bbp
            /// </summary>
            /// <param name="img"></param>
            /// <returns></returns>
            private static Bitmap ConvertTo24bpp(Bitmap img)
            {
                var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = System.Drawing.Graphics.FromImage(bmp))
                    gr.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height));
                return bmp;
            }
#endif
#if !PS4_UNITY
            #region << Create_PS4_Compatible_PNG >>

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
            public void Create_PS4_Compatible_PNG(string inputfile, string outputfile, int Height = 512, int Width = 512)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, Width, Height);//converts the image to the correct size
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

            /// <summary>
            /// Converts a file location of a bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Original Bitmap Location</param>
            /// <param name="outputfile">Original Bitmap Location</param>
            /// <returns>PS4 Comptatible Bitmap as Byte[]</returns>
            public byte[] Create_PS4_Compatible_PNG_As_Bytes(string InputFile)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(InputFile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //reutrn new butmap
                return returnbmp.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
            }

            #endregion << Create_PS4_Compatible_PNG >>
#endif
        }

        /// <summary>
        /// PS4 DDS Image Handling
        /// </summary>
        public class DDS
        {
            #region << Returns >>

            /// <summary>
            /// Directly save a PNG from a DDS File
            /// </summary>
            /// <param name="DDSFilePath">.dds file</param>
            /// <param name="savepath">Location of png</param>
            public static void SavePNGFromDDS(string DDSFilePath, string savepath)
            {
                /*Migrating to .Net3.5 this class might not work right now*/
                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.BitmapImage.Save(savepath);

            }


#if !PS4_UNITY

            /// <summary>
            /// Gets a stream from a DDS
            /// </summary>
            /// <param name="DDSFilePath"></param>
            /// <returns>Stream of PNG from DDS</returns>
            public static Stream GetStreamFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();

                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                Bitmap temp = image.BitmapImage;
                temp.Save(rtnStream, System.Drawing.Imaging.ImageFormat.Png);
                return rtnStream;

            }

#endif

#if !PS4_UNITY

            /// <summary>
            /// Get a Bitmap from a DDS
            /// </summary>
            /// <param name="DDSFilePath"></param>
            /// <returns>Returns a Bitmap</returns>
            public static Bitmap GetBitmapFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();
                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                return (Bitmap)image.BitmapImage;
            }

            /// <summary>
            /// Gets a PNG as byte[] from a dds file
            /// </summary>
            /// <param name="DDSFilePath"></param>
            /// <returns></returns>
            public static byte[] GetBytesFromDDS(string DDSFilePath)
            {
                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                Bitmap temp = image.BitmapImage;
                return temp.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
            }
#endif
            #endregion << Returns >>

            #region << Creations >>

#if !PS4_UNITY
            public class PS4
            {

#if PS4_UNITY || DEBUG

                /// <summary>
                /// Still not ready for release
                /// </summary>
                /// <param name="Bitmap"></param>
                /// <param name="SavePath"></param>
                public static void CreateDDSFromBitmap(Bitmap Bitmap, string SavePath)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    Bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);

                    //DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    //img.Save(Bitmap, SavePath);
                    //img;
                    UnityEngine.Texture2D dd = LoadTexture(memoryStream);
                    dd.Compress(true);
                    byte[] data = dd.GetRawTextureData();
                    //img.Save(Bitmap, SavePath);
                    File.WriteAllBytes(SavePath, data);
                }

                public static void CreateDDSFromStream(Stream pngstream, string SavePath)
                {
                    MemoryStream ms = new MemoryStream();
                    CopyStream(pngstream, ms);
                    Bitmap Bitmap = new Bitmap(ms);
                    DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    UnityEngine.Texture2D dd = LoadTexture(ms);
                    dd.Compress(true);
                    byte[] data = dd.GetRawTextureData();
                    //img.Save(Bitmap, SavePath);
                    File.WriteAllBytes(SavePath, data);
                    img.Save(Bitmap, SavePath);
                    //img;
                }


                public static void CreateDDSFromStream(byte[] png, string SavePath)
                {
                    MemoryStream ms = new MemoryStream(png);
                    // Bitmap Bitmap = new Bitmap(ms);
                    // DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    UnityEngine.Texture2D dd = LoadTexture(ms);
                    dd.Compress(true);


                    UnityEditor.EditorUtility.CompressTexture(dd, TextureFormat.DXT1, TextureCompressionQuality.Best);
                    byte[] data = dd.GetRawTextureData();
                    // dd.GetRawTextureData()
                    //img.Save(Bitmap, SavePath);
                    File.WriteAllBytes(SavePath, data);
                    //img;
                }

                public static UnityEngine.Texture2D LoadTexture(string FilePath)
                {

                    // Load a PNG or JPG file from disk to a Texture2D
                    // Returns null if load fails

                    UnityEngine.Texture2D Tex2D;
                    byte[] FileData;

                    if (File.Exists(FilePath))
                    {
                        FileData = File.ReadAllBytes(FilePath);
                        Tex2D = new UnityEngine.Texture2D(2, 2);           // Create new "empty" texture
                        if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                            return Tex2D;                 // If data = readable -> return texture
                    }
                    return null;                     // Return null if load failed
                }


                public static UnityEngine.Texture2D LoadTexture(MemoryStream ms)
                {

                    // Load a PNG or JPG file from disk to a Texture2D
                    // Returns null if load fails

                    UnityEngine.Texture2D Tex2D;
                    byte[] FileData;

                    if (ms.Length == 0)
                    {
                        return null;
                    }

                    FileData = ms.ToArray();
                    Tex2D = new UnityEngine.Texture2D(2, 2);           // Create new "empty" texture
                    if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                        return Tex2D;                 // If data = readable -> return texture

                    return null;                     // Return null if load failed
                }
#endif
                // Merged From linked CopyStream below and Jon Skeet's ReadFully example
                public static void CopyStream(Stream input, Stream output)
                {
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
#endif
#if !PS4_UNITY
            public class Windows
            {
                /// <summary>
                /// Still not ready for release
                /// </summary>
                /// <param name="Bitmap"></param>
                /// <param name="SavePath"></param>
                public static void CreateDDSFromBitmap(Bitmap Bitmap, string SavePath)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    Bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);

                    DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    img.Save(Bitmap, SavePath);
                    //img;
                    //UnityEngine.Texture2D dd = LoadTexture(memoryStream);
                    //dd.Compress(true);
                    //byte[] data = dd.GetRawTextureData();
                    ////img.Save(Bitmap, SavePath);
                    //File.WriteAllBytes(SavePath, data);
                }

                public static void CreateDDSFromStream(Stream pngstream, string SavePath)
                {
                    MemoryStream ms = new MemoryStream();
                    CopyStream(pngstream, ms);
                    Bitmap Bitmap = new Bitmap(ms);
                    DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    img.Save(Bitmap, SavePath);
                    //img;
                }

                public static void CreateDDSFromStream(byte[] png, string SavePath)
                {
                    MemoryStream ms = new MemoryStream(png);
                    Bitmap Bitmap = new Bitmap(ms);
                    DDSReader.DDSImage img = new DDSReader.DDSImage(Bitmap);
                    //UnityEngine.Texture2D dd = LoadTexture(ms);
                    //dd.Compress(true);
                    //byte[] data = dd.GetRawTextureData();
                    img.Save(Bitmap, SavePath);
                    //File.WriteAllBytes(SavePath, data);
                    //img;
                }


                // Merged From linked CopyStream below and Jon Skeet's ReadFully example
                public static void CopyStream(Stream input, Stream output)
                {
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
#endif
            #endregion << Creations >>
        }
#if !PS4_UNITY
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

                private static System.Drawing.Color ColorFromRGBA5650(uint color)
                {
                    int r = (int)(((color & 0x0000001F)) << 3);
                    int g = (int)(((color & 0x000007E0) >> 5) << 2);
                    int b = (int)(((color & 0x0000F800) >> 11) << 3);
                    return System.Drawing.Color.FromArgb(0, r, g, b);
                }
                private static System.Drawing.Color ColorFromRGBA5551(uint color)
                {
                    int r = (int)(((color & 0x0000001F)) << 3);
                    int g = (int)(((color & 0x000003E0) >> 5) << 3);
                    int b = (int)(((color & 0x00007C00) >> 10) << 3);
                    int a = (int)(((color & 0x00008000) >> 15) << 7);
                    return System.Drawing.Color.FromArgb(a, r, g, b);
                }
                private static System.Drawing.Color ColorFromRGBA4444(uint color)
                {
                    int r = (int)(((color & 0x0000000F)) << 4);
                    int g = (int)(((color & 0x000000F0) >> 4) << 4);
                    int b = (int)(((color & 0x00000F00) >> 8) << 4);
                    int a = (int)(((color & 0x0000F000) >> 12) << 4);
                    return System.Drawing.Color.FromArgb(a, r, g, b);
                }
                private static System.Drawing.Color ColorFromRGBA8888(uint color)
                {
                    int r = (int)((color & 0x000000FF));
                    int g = (int)((color & 0x0000FF00) >> 8);
                    int b = (int)((color & 0x00FF0000) >> 16);
                    int a = (int)((color & 0xFF000000) >> 24);
                    return System.Drawing.Color.FromArgb(a, r, g, b);
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
                            System.Drawing.Color color;

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
#endif
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
        private static byte[] rcoMagic = new byte[4] { 0x52, 0x43, 0x4F, 0x46 };
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


        /// <summary>
        /// Reads a RCO file into a RCOFile Container
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static RCOFile ReadRco(string File)
        {
            RCOFile rco = new RCOFile();
            rco.FileTable = new FileTable();

            try
            {
                // Reading Header
                byte[] magic = new byte[4];
                byte[] Tree_Table_Offset = new byte[4];
                string outFile = "notDefined";
                using (BinaryReader br = new BinaryReader(new FileStream(File, FileMode.Open, FileAccess.Read)))
                {
                    //Read the header with all needed offsets
                    rco.Header = ReadHeader(br);
                    //Tree Table
                    //Array reversed
                    //Start Offset 
                    br.BaseStream.Position = (long)rco.Header.Tree_Table_Offset;//go to start of table
                    var treetable = br.ReadBytes((int)rco.Header.Tree_Table_Size);//read size

                    //now reverse 
                    Array.Reverse(treetable);

                    // Get Data Table Offset and Length
                    //Console.Write("Reading Offset and Length of Data Table...");
                    Tree_Table_Offset = new byte[4];
                    byte[] eof = new byte[4];
                    br.BaseStream.Seek(0x48, SeekOrigin.Begin);
                    Tree_Table_Offset = br.ReadBytes(4);
                    br.Read(eof, 0, 4);
                    Array.Reverse(Tree_Table_Offset);
                    Array.Reverse(eof);
                    //Console.Write("done!\n");
                    //Console.WriteLine("Readed Hex value of Offset: 0x" + BitConverter.ToString(offset).Replace("-", ""));
                    //Console.WriteLine("Readed Hex value of Size: 0x" + BitConverter.ToString(eof).Replace("-", ""));

                    // Check for zlib Header '0x78DA' (compression level=9) or VAG & PNG files and write to file

                    end = Convert.ToInt32(BitConverter.ToString(eof).Replace("-", ""), 16);
                    count = Convert.ToInt32(BitConverter.ToString(Tree_Table_Offset).Replace("-", ""), 16);
                    //Console.WriteLine("Offset to start from: " + count + " bytes");
                    //Console.WriteLine("Size to Dump: " + end + " bytes");
                    //Console.WriteLine("Searching for ZLib Compressed (Vita) or Non-Compressed (PS4) Files...");
                    br.BaseStream.Seek(count, SeekOrigin.Begin);
                    br.Read(zlib, 0, 2);


                    ////somehow we need to get the name of the file
                    //BinaryReader br2 = br;
                    //byte[] array = new byte[br.BaseStream.Length];

                    //br2.Read(array, 0, (int)br.BaseStream.Length);
                    //string filenametest = System.Text.Encoding.UTF8.GetString(array);
                    //string tempstr = Util.Utils.byteArrayToHexString(array);
                    //string idkanymore = Util.Utils.HexToString(tempstr);

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
                                //Console.Write("Found a VAG File will start to extract...");
                                //outFile = baseDir + countVag + ".vag";
                                //System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                MemoryStream memset = new MemoryStream();

                                using (BinaryWriter bw = new BinaryWriter(memset))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    memset.Read(toWrite, 0, 16);
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
                                                                    memset.Read(toWrite, 0, 16);
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
                                            memset.Read(toWrite, 0, 16);
                                            Vag vag = new Vag();
                                            vag.FileBytes = memset.ToArray();
                                            vag.VagFile = outFile;
                                            rco.FileTable.VagFiles.Add(vag);
                                            dumped += 16;
                                            count += 16;
                                            break;
                                        }
                                    }
                                    bw.Close();

                                    memset.Close();
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
                                //Console.Write("Found a PNG File will start to extract...");
                                outFile = baseDir + countPNG + ".png";
                                //System.IO.File.Create(outFile).Close();

                                byte[] toWrite = new byte[16];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _vag = new byte[8];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);
                                MemoryStream memset = new MemoryStream();
                                using (BinaryWriter bw = new BinaryWriter(memset))
                                {
                                    // Before we Jump into the Loop we need to write out the first readed 16 bytes which are the PNG Magic.
                                    // This is needed cause we need to compare for new Magic's / Header's to know if we reached the eof of current file.
                                    // Otherwise the routine would detect a PNG Magic and stop right after we jumped in, resulting in not extracting the PNG and loosing
                                    // the allready readed 16 bytes.
                                    bw.Write(toWrite, 0, 16);

                                    memset.Read(toWrite, 0, 16);
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
                                                                    memset.Read(toWrite, 0, 16);
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
                                    PNG png = new PNG();
                                    png.PNGFile = outFile;

                                    png.FileBytes = memset.ToArray();
                                    rco.FileTable.PNGFiles.Add(png);
                                    bw.Close();

                                    memset.Close();
                                }
                                Console.Write("done!\n");
                                countPNG++;
                            }
                            #endregion pngExtract
                            #region cxmlExtract
                            else if (CompareBytes(cxml, ngCXML))
                            {
                                //Console.Write("Found a CXML File will start to extract...");
                                outFile = baseDir + countCXML + ".cxml";
                                //System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                MemoryStream memset = new MemoryStream();

                                using (BinaryWriter bw = new BinaryWriter(memset))
                                {
                                    bw.Write(toWrite, 0, 16);
                                    memset.Read(toWrite, 0, 16);
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
                                                                        memset.Read(toWrite, 0, 16);
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

                                    CXML cxml = new CXML();
                                    cxml.CXMLFile = outFile;
                                    cxml.FileBytes = memset.ToArray();
                                    bw.Close();
                                    memset.Close();
                                }
                                Console.Write("done!\n");
                                countCXML++;
                            }
                            #endregion cxmlExtract
                            #region ddsExtract
                            else if (CompareBytes(dds, ddsMagic))
                            {
                                //   Console.Write("Found a DDS File will start to extract...");
                                outFile = baseDir + countDDS + ".dds";
                                // System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                //using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    //bw.Write(toWrite, 0, 16);
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
                                                                        //                                  bw.Write(toWrite, 0, 16);
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
                                    DDS dds = new DDS();
                                    //Image.DDS dds = new Image.DDS();
                                    dds.DDS_Name = outFile;
                                    //we need to still convert these images
                                    dds.DDS_File = new Image.DDS();
                                    rco.FileTable.DDSFiles.Add(dds);
                                    //bw.Close();
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
                                // Console.Write("Found a WAV File will start to extract...");
                                outFile = baseDir + countWAV + ".wav";
                                //System.IO.File.Create(outFile).Close();
                                byte[] toWrite = new byte[16];
                                _vag = new byte[8];
                                _zlib = new byte[2];
                                _wav = new byte[4];
                                _gtf = new byte[4];
                                _dds = new byte[4];
                                _cxml = new byte[8];
                                br.BaseStream.Seek(count, SeekOrigin.Begin);
                                br.Read(toWrite, 0, 16);

                                //using (BinaryWriter bw = new BinaryWriter(new FileStream(outFile, FileMode.Append, FileAccess.Write)))
                                {
                                    // bw.Write(toWrite, 0, 16);
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
                                                                        //                                   bw.Write(toWrite, 0, 16);
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
                                    Wave wav = new Wave();
                                    wav.WaveFile = outFile;
                                    wav.FileBytes = toWrite;
                                    rco.FileTable.WaveFiles.Add(wav);
                                    //bw.Close();
                                }
                                Console.Write("done!\n");
                                countWAV++;
                            }
                            #endregion wavExtract
                            else
                            {
                                //Console.WriteLine("\nFound a new File which i don't know what to do with !\nPlease contact the Developer @ www.playstationhax.it");
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
            return rco;
        }

        //stil working on the rco file archive type

        public struct RCOFileHeader
        {
            public byte[] MAGIC;
            public byte[] Version;
            public ulong Tree_Table_Offset;
            public ulong Tree_Table_Size;
            public ulong ID_String_Table_Offset;
            public ulong ID_String_Table_Size;
            public ulong ID_Integer_Table_Offset;
            public ulong ID_Intiger_Table_Size;
            public ulong String_Table_Offset;
            public ulong String_Table_Size;
            public uint Overlapping;
            public uint empty; //???
            public ulong Styles_ID_Intiger_Table_Offset;
            public ulong Styles_ID_Initger_Table_Size;
            public ulong Intiger_Array_Table_Offset;
            public ulong Intiger_Array_Table_Size;
            public ulong Float_Array_Table_Offset;
            public ulong Float_Array_Table_Size;
            public ulong File_Table_Offset;
            public ulong File_Table_Size;

            public List<string> ToList()
            {
                List<string> items = new List<string>();

                items.Add("MAGIC:" + Encoding.ASCII.GetString(MAGIC));
                items.Add("Version:" + Util.Utils.byteArrayToHexString(Version));
                items.Add("Tree Table Offset:" + Tree_Table_Offset.ToString("X"));
                items.Add("Tree Table Size:" + Tree_Table_Size.ToString("X"));
                items.Add("ID_String_Table_Offset:" + ID_String_Table_Offset.ToString("X"));
                items.Add("ID_String_Table_Size:" + ID_String_Table_Size.ToString("X"));
                items.Add("ID_Integer_Table_Offset:" + ID_Integer_Table_Offset.ToString("X"));
                items.Add("ID_Intiger_Table_Size:" + ID_Intiger_Table_Size.ToString("X"));
                items.Add("String_Table_Offset:" + String_Table_Offset.ToString("X"));
                items.Add("String_Table_Size:" + String_Table_Size.ToString("X"));
                items.Add("Overlapping:" + Overlapping.ToString("X"));
                items.Add("empty:" + empty.ToString("X"));
                items.Add("Styles_ID_Intiger_Table_Offset:" + Styles_ID_Intiger_Table_Offset.ToString("X"));
                items.Add("Styles_ID_Initger_Table_Size:" + Styles_ID_Initger_Table_Size.ToString("X"));
                items.Add("Intiger_Array_Table_Offset:" + Intiger_Array_Table_Offset.ToString("X"));
                items.Add("Intiger_Array_Table_Size:" + Intiger_Array_Table_Size.ToString("X"));
                items.Add("Float_Array_Table_Offset:" + Float_Array_Table_Offset.ToString("X"));
                items.Add("Float_Array_Table_Size:" + Float_Array_Table_Size.ToString("X"));
                items.Add("File_Table_Offset:" + File_Table_Offset.ToString("X"));
                items.Add("File_Table_Size:" + File_Table_Size.ToString("X"));
                return items;
            }

        }

        public struct Tree_Table
        {
            public byte[] Root_Element; //size of 4 (offset within String Table in this case <resource>
            public uint Attribute_Counter;
            public uint Parent;
            public uint Prevoius_Borhter;
            public uint Next_Brother;
            public uint First_Child;
            public uint Last_Child;
            public uint String_Pointer;
            public uint Type; //(2 == FLOAT)
            public ulong Float_value;
            public byte[] empty_value;
            public uint String_Pointer2;
            public uint Type_descriptor1;
            //public uint Offset_
        }

        private static RCOFileHeader ReadHeader(BinaryReader br)
        {
            // Check Magic
            //Console.Write("Checking Header....");
            RCOFileHeader rco = new RCOFileHeader();
            byte[] magic = br.ReadBytes(4);//just want the header not the version incase sony changes this down the line



            if (!Util.Utils.CompareBytes(magic, rcoMagic))
            {
                //Console.WriteLine("ERROR: That is not a valid NextGen RCO!\nExiting now...");
                //Environment.Exit(0);
                throw new Exception("ERROR: That is not a valid NextGen RCO!");
            }
            rco.MAGIC = magic;//else we set the magic

            //read version information 
            rco.Version = br.ReadBytes(4);
            Array.Reverse(rco.Version);//just reverse the array so its human readable

            rco.Tree_Table_Offset = br.ReadUInt32();
            rco.Tree_Table_Size = br.ReadUInt32();
            rco.ID_String_Table_Offset = br.ReadUInt32();
            rco.ID_String_Table_Size = br.ReadUInt32();
            rco.ID_Integer_Table_Offset = br.ReadUInt32();
            rco.ID_Intiger_Table_Size = br.ReadUInt32();
            rco.String_Table_Offset = br.ReadUInt32();
            rco.String_Table_Size = br.ReadUInt32();
            rco.Overlapping = br.ReadUInt32();
            rco.empty = br.ReadUInt32();
            rco.Styles_ID_Intiger_Table_Offset = br.ReadUInt32();
            rco.Styles_ID_Initger_Table_Size = br.ReadUInt32();
            rco.Intiger_Array_Table_Offset = br.ReadUInt32();
            rco.Intiger_Array_Table_Size = br.ReadUInt32();
            rco.Float_Array_Table_Offset = br.ReadUInt32();
            rco.Float_Array_Table_Size = br.ReadUInt32();
            rco.File_Table_Offset = br.ReadUInt32();
            rco.File_Table_Size = br.ReadUInt32();

            return rco;
        }

        public struct RCOFile
        {
            internal byte[] MAGIC;/*Magic needs to be validated*/
            internal byte[] Version;/*Version	0x04	0x04	00 00 01 10*	CXML version '1.10'*/
            internal byte[] Offset;/*Offset information*/

            public RCOFileHeader Header;

            public FileTable FileTable;
        }

        public class Wave
        {
            public string WaveFile { get; set; }
            public byte[] FileBytes { get; set; }
        }

        public class Vag
        {
            public string VagFile { get; set; }
            public byte[] FileBytes { get; set; }
        }

        public class CXML
        {
            public string CXMLFile { get; set; }
            public byte[] FileBytes { get; set; }
        }

        public class PNG
        {
            public string PNGFile { get; set; }
            public byte[] FileBytes { get; set; }
        }

        public class DDS
        {
            public string DDS_Name { get; set; }
            public Image.DDS DDS_File { get; set; }
        }

        public class GTF
        {
            public string FileName { get; set; }
            public byte[] FileBytes { get; set; }
        }

        public class FileTable
        {
            public List<Wave> WaveFiles = new List<Wave>();
            public List<Vag> VagFiles = new List<Vag>();
            public List<PNG> PNGFiles = new List<PNG>();
            public List<CXML> CXMLFiles = new List<CXML>();
            public List<DDS> DDSFiles = new List<DDS>();
            public List<GTF> GTFList = new List<GTF>();
        }
    }

    /// <summary>
    /// Save Data Class That Needs To Be Finished (Encrypted saves should be included once samu is released)
    /// </summary>
    public class SaveData
    {

        #region << Load Save File Class>>



        /// <summary>
        /// Load a Save File
        /// </summary>
        /// <param name="filelocation">Save File Location on disk</param>
        /// <param name="Sealedkeylocation">Selaed Key Location On Disk</param>
        public static void LoadSaveData(string filelocation, string Sealedkeylocation)
        {
            /*Load the sealed key from the file*/
            Licensing.Sealedkey sldkey = Licensing.LoadSealedKey(Sealedkeylocation);

            Doit(Sealedkeylocation, filelocation, filelocation + "_Decrypt");
        }

        public static byte[] GetSaveDataPFSKey(byte[] SealedKeyLocation)
        {
            var bytes = SealedKeyLocation;
            byte[] dec = new byte[32];
            SCEUtil.sceSblSsDecryptSealedKey(bytes, dec);
            return dec;
        }

        #endregion << Load Save File Class>>

        public static void Doit(string SealedKey, string SaveFile, string FileDecrypt)
        {
            var bytes = File.ReadAllBytes(SealedKey);
            byte[] dec = new byte[32];
            SCEUtil.sceSblSsDecryptSealedKey(bytes, dec);

            Console.WriteLine("Your PFS Key is {0}", BitConverter.ToString(dec).Replace("-", string.Empty));

            var save = File.ReadAllBytes(SaveFile);
            byte[] iv = new byte[16];
            Buffer.BlockCopy(bytes, 16, iv, 0, iv.Length);

            using (AesManaged aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.IV = iv;
                aes.KeySize = 256;
                aes.Key = dec;
                aes.Padding = PaddingMode.None;
                var stream = new MemoryStream();
                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
                    {
                        using (var writer = new BinaryWriter(cryptoStream))
                        {
                            writer.Write(save);
                        }
                    }
                }

                byte[] cipherBytes = stream.ToArray();
                Console.WriteLine("PFS Save Content:");
                //Console.WriteLine(UTF8Encoding.UTF8.GetString(cipherBytes));
                File.WriteAllBytes(FileDecrypt, cipherBytes);
            }
            Console.ReadLine();
        }

        public static void ReadPFS(string Path)
        {
        }

        public static byte[] GetSealedKey_Bytes(string SealedKey)
        {
            var bytes = File.ReadAllBytes(SealedKey);
            byte[] dec = new byte[32];
            SCEUtil.sceSblSsDecryptSealedKey(bytes, dec);

            Console.WriteLine("Your PFS Key is {0}", BitConverter.ToString(dec).Replace("-", string.Empty));
            return dec;
        }

        public static string GetSealedKey(string SealedKey, string SaveFile, string FileDecrypt)
        {
            var bytes = File.ReadAllBytes(SealedKey);
            byte[] dec = new byte[32];
            SCEUtil.sceSblSsDecryptSealedKey(bytes, dec);

            Console.WriteLine("Your PFS Key is {0}", BitConverter.ToString(dec).Replace("-", string.Empty));
            return BitConverter.ToString(dec).Replace("-", string.Empty);
        }



    }


    public class PFS
    {
        //thanks to liborbispkg for this bit of into
        [Flags]
        public enum PfsMode : ushort
        {
            None = 0,
            Signed = 0x1,
            Is64Bit = 0x2,
            Encrypted = 0x4,
            UnknownFlagAlwaysSet = 0x8,
        }


        public class Header
        {
            public int Version = 1; // 1
            public int Magic = 20130315; // 20130315 (march 15 2013???)
            public int Id = 0;
            public byte Fmode = 0;
            public byte Clean = 0;
            public byte ReadOnly = 0;
            public byte Rsv = 0;
            public PfsMode Mode = PfsMode.UnknownFlagAlwaysSet;
            public ushort Unk1 = 0;
            public uint BlockSize = 0x10000;
            public uint NBackup = 0;
            /// <summary>
            /// This is always 1 for some reason.
            /// </summary>
            public long NBlock = 1;
            public long DinodeCount = 0;
            public long Ndblock = 0;
            public long DinodeBlockCount = 0;

        }
    }


    /// <summary>
    /// Tropy File Class
    /// </summary>
    public class Trophy_File
    {

        /*SHA1*/
        private string SHA1;//SHA1 PlaceHolder
        private byte[] Bytes;//Bytes Placeholder

        private bool Readbytes = true; //Bool Read Bytes

        TrophyHeader trphy = new TrophyHeader(); //Trophy Header Object

        /// <summary>
        /// How Many Files Are In The Trophy File
        /// </summary>
        public int FileCount
        {
            get
            {
                return checked((int)Util.Utils.byteArrayToLittleEndianInteger(trphy.files_count));
            }
        }

        /// <summary>
        /// Version of the Trophy File
        /// </summary>
        public int Version
        {
            get
            {
                return checked((int)Util.Utils.byteArrayToLittleEndianInteger(trphy.version));
            }
        }

        /// <summary>
        /// Trophy header Structure
        /// </summary>
        public struct TrophyHeader
        {
            public byte[] magic;//Magic

            public byte[] version;//Version Of Trophy Header File

            public byte[] file_size;//File Size

            public byte[] files_count;//File Counts

            public byte[] element_size;//Elements Size

            public byte[] dev_flag;//Is a Dev Trophy File

            public byte[] sha1;//SHA1 Hash

            public byte[] padding;//Padding 
        }
        /*Trophy items*/
        public class TrophyItem
        {
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
                if (!Util.Utils.ByteArraysEqual(hdr.magic, new byte[] { 220, 162, 77, 0 }))
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
            catch (Exception ex)
            {

            }
            rtn.Bytes = Bytes;
            rtn.SHA1 = SHA1;
            rtn.trphy = trphy;
            rtn.trophyItemList = trophyItemList;
            return rtn;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public Trophy_File Load(Stream File)
        {
            return Load(ReadFully(File));
        }

        public byte[] ExtractFileToMemory(string filename)
        {
            byte[] result = null;
            //TrophyItem archiver = this.trophyItemList.Find((TrophyItem b) => Microsoft.VisualBasic.CompilerServices.Operators.CompareString(Microsoft.VisualBasic.Strings.Mid(b.Name.ToUpper(), 1, Microsoft.VisualBasic.Strings.Len(filename.ToUpper())), filename.ToUpper(), false) == 0);
            TrophyItem archiver = this.trophyItemList.Find((TrophyItem b) => b.Name == filename);

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

        /// <summary>
        /// Reads a sealed trophy file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        public byte[] SealedTrophy(byte[] file, byte[] SealedKey)
        {
            //first we need to decrypt the trophy 
            //i think these keys should allow a decrypt
            Licensing.Sealedkey keyload = Licensing.LoadSealedKey(SealedKey);


            var bytes = SealedKey;
            byte[] dec = new byte[32];
            //this should be how sony does it 
            SCEUtil.sceSblSsDecryptSealedKey(bytes, dec);

            Console.WriteLine("Your PFS Key is {0}", BitConverter.ToString(dec).Replace("-", string.Empty));

            var save = file;
            byte[] iv = new byte[16];
            Buffer.BlockCopy(bytes, 16, iv, 0, iv.Length);

            byte[] decryptionkey = PS4PkgUtil.DecryptAes(PS4Keys.KernelKeys.SealedKey.Keyset1.Key, keyload.IV, keyload.KEY);
            Console.WriteLine("Your PFS Key is {0}", BitConverter.ToString(decryptionkey).Replace("-", string.Empty));
            return PS4PkgUtil.DecryptAes(keyload.KEY, keyload.IV, file);


            return null;
        }

        /// <summary>
        /// This is the class that will hold the tbl info 
        /// </summary>
        private class tbl_trophy_flag
        {
            public int id { get; set; }
            public int title_id { get; set; }
            public string revision { get; set; }
            public string trophy_title_id { get; set; }
            public int trophyid { get; set; }
            public int groupid { get; set; }
            public bool visible { get; set; }
            public bool unlocked { get; set; }
            public int unlock_attribute { get; set; }
            public string time_unlocked { get; set; }
            public string time_unlocked_uc { get; set; }
            public int grade { get; set; }
            public bool hidden { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class EarnedTrophies
        {
            [JsonProperty("platinum")]
            public int Platinum { get; set; }

            [JsonProperty("gold")]
            public int Gold { get; set; }

            [JsonProperty("silver")]
            public int Silver { get; set; }

            [JsonProperty("bronze")]
            public int Bronze { get; set; }
        }
        public class trpsummary
        {
            [JsonProperty("format")]
            public int Format { get; set; }

            [JsonProperty("earnedTrophies")]
            public EarnedTrophies EarnedTrophies { get; set; }
            [JsonConstructor]
            public trpsummary() { }
        }

        public static void Unlock_All_Title_Id(string title_id, string dbFileLocation = @"C:\Publish\Sony\trophy_local.db", string trpsummaryFile = @"C:\Publish\Sony\trpsummary.dat")
        {

            Console.WriteLine("Create connection to trophy db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load tropy db");


            //create a backup 
            if (File.Exists(dbFilename + ".backup"))
            {
                File.Delete(dbFilename + ".backup");
            }
            File.Copy(dbFilename, dbFilename + ".backup", true);//just give it an overwrite incase

            //we just want to do the update here now 
            string SQL = @"UPDATE  tbl_trophy_flag set time_unlocked='" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.00Z") + "', time_unlocked_uc ='" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.00Z") + @"',unlocked=1
                WHERE trophy_title_id='" + title_id + "' AND unlocked = 0";

            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Could not update fields");
            }


            //now do the title file so it updates percentages ext

            SQL = @"UPDATE tbl_trophy_title SET  progress=100,unlocked_trophy_num=trophy_num,unlocked_platinum_num=platinum_num,unlocked_gold_num=gold_num,unlocked_silver_num=silver_num,unlocked_bronze_num=bronze_num Where trophy_title_id = '" + title_id + "'";
            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Could not update header fields");
            }

            //load up current trophies
            //we need to rebuild the summary trophy file cause we are not using the sony method we are doing it manual
            //todo do screenshots somewhere
            //trpsummary myDeserializedClass = new trpsummary();

            //File.ReadAllText = 

            trpsummary myDeserializedClass = JsonConvert.DeserializeObject<trpsummary>(File.ReadAllText(trpsummaryFile));

            SQL = "SELECT SUM(unlocked_platinum_num) FROM tbl_trophy_title";

            int TotalPlatNum = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Platinum = TotalPlatNum;

            SQL = "SELECT SUM(unlocked_gold_num) FROM tbl_trophy_title";

            int TotalGoldNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Gold = TotalGoldNUm;

            SQL = "SELECT SUM(unlocked_silver_num) FROM tbl_trophy_title";

            int TotalSilverNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Silver = TotalSilverNUm;

            SQL = "SELECT SUM(unlocked_bronze_num) FROM tbl_trophy_title";

            int TotalBronzeNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Bronze = TotalBronzeNUm;

            File.WriteAllText(trpsummaryFile, JsonConvert.SerializeObject(myDeserializedClass));

            return;
            //            string SQL = @"SELECT * FROM tbl_trophy_flag
            //Where trophy_title_id='"+ title_id + "'";

            var dttemp = SQLLite.SqlHelper.GetDataTable(SQL, cs);
            List<tbl_trophy_flag> item = new List<tbl_trophy_flag>();

            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                //here we have some data !
                tbl_trophy_flag newitem = new tbl_trophy_flag();

                newitem.id = Convert.ToInt32(dttemp.Rows[i]["id"].ToString());
                newitem.title_id = Convert.ToInt32(dttemp.Rows[i]["title_id"].ToString());
                newitem.revision = dttemp.Rows[i]["title_id"].ToString();
                newitem.trophy_title_id = dttemp.Rows[i]["trophy_title_id"].ToString();
                newitem.trophyid = Convert.ToInt32(dttemp.Rows[i]["trophyid"].ToString());
                newitem.groupid = Convert.ToInt32(dttemp.Rows[i]["groupid"].ToString());
                newitem.visible = Convert.ToBoolean(Convert.ToInt32(dttemp.Rows[i]["visible"].ToString()));
                newitem.unlocked = Convert.ToBoolean(Convert.ToInt32(dttemp.Rows[i]["unlocked"].ToString()));
                newitem.unlock_attribute = Convert.ToInt32(dttemp.Rows[i]["unlock_attribute"].ToString());
                newitem.time_unlocked = dttemp.Rows[i]["time_unlocked"].ToString();
                newitem.time_unlocked_uc = dttemp.Rows[i]["time_unlocked_uc"].ToString();
                newitem.grade = Convert.ToInt32(dttemp.Rows[i]["grade"].ToString());
                newitem.hidden = Convert.ToBoolean(Convert.ToInt32(dttemp.Rows[i]["hidden"].ToString()));
                newitem.title = dttemp.Rows[i]["title"].ToString();
                newitem.description = dttemp.Rows[i]["description"].ToString();
                item.Add(newitem);
            }

            for (int i = 0; i < item.Count; i++)
            {
                if (item[i].unlocked == false)
                {
                    //set the new item to true 
                }
            }
        }

        public static void Unlock_All_Trophies(string dbFileLocation = @"C:\Publish\Sony\trophy_local.db", string trpsummaryFile = @"C:\Publish\Sony\trpsummary.dat")
        {
            Console.WriteLine("Create connection to trophy db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load tropy db");

            //create a backup 
            if (File.Exists(dbFilename + ".backup"))
            {
                File.Delete(dbFilename + ".backup");
            }
            File.Copy(dbFilename, dbFilename + ".backup", true);//just give it an overwrite incase

            //we just want to do the update here now this is for all
            string SQL = @"UPDATE  tbl_trophy_flag set time_unlocked='" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.00Z") + "', time_unlocked_uc ='" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.00Z") + @"',unlocked=1
                WHERE unlocked = 0";

            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Could not update fields");
            }


            //now do the title file so it updates percentages ext

            SQL = @"UPDATE tbl_trophy_title SET  progress=100,unlocked_trophy_num=trophy_num,unlocked_platinum_num=platinum_num,unlocked_gold_num=gold_num,unlocked_silver_num=silver_num,unlocked_bronze_num=bronze_num";
            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Could not update header fields");
            }

            //load up current trophies
            //we need to rebuild the summary trophy file cause we are not using the sony method we are doing it manual
            //todo do screenshots somewhere
            trpsummary myDeserializedClass = JsonConvert.DeserializeObject<trpsummary>(File.ReadAllText(trpsummaryFile));

            SQL = "SELECT SUM(unlocked_platinum_num) FROM tbl_trophy_title";

            int TotalPlatNum = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Platinum = TotalPlatNum;

            SQL = "SELECT SUM(unlocked_gold_num) FROM tbl_trophy_title";

            int TotalGoldNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Gold = TotalGoldNUm;

            SQL = "SELECT SUM(unlocked_silver_num) FROM tbl_trophy_title";

            int TotalSilverNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Silver = TotalSilverNUm;

            SQL = "SELECT SUM(unlocked_bronze_num) FROM tbl_trophy_title";

            int TotalBronzeNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Bronze = TotalBronzeNUm;

            File.WriteAllText(trpsummaryFile, JsonConvert.SerializeObject(myDeserializedClass));

            return;
        }

        /// <summary>
        /// Class used for Encrypted SFM
        /// </summary>
        public class ESFM
        {
            //so Huge thanks to RedEyeX32 (https://twitter.com/RedEyeX32)
            //for his huge help on this

            #region << All From RedEyeX32 >>       
            public static byte[] data_erk, data_riv, keygen_riv = new byte[16];

            private static byte[] aes_encrypt_cbc(byte[] key, byte[] iv, string input)
            {
                AesManaged aes = new AesManaged();
                aes.Key = key;
                aes.IV = iv;
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                if (iv.Length != 16)
                    Array.Resize(ref iv, 16);

                return aes.CreateEncryptor(key, iv).TransformFinalBlock(Encoding.ASCII.GetBytes(input), 0, input.Length);
            }

            private static byte[] aes_decrypt_cbc(byte[] key, byte[] iv, byte[] input)
            {
                try
                {
                    AesManaged aes = new AesManaged();
                    aes.Key = key;
                    aes.IV = iv;
                    aes.KeySize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.None;

                    if (iv.Length != 16)
                        Array.Resize(ref iv, 16);

                    return aes.CreateDecryptor(key, iv).TransformFinalBlock(input, 0, input.Length);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            #endregion << All From RedEyeX32 >>

            /// <summary>
            /// Load and Decrypts an encrypt trophy file
            /// </summary>
            public static void LoadAndDecrypt(string ESFMFile, string NpCommId)
            {
                try
                {
                    BinaryWriter IO = new BinaryWriter(new FileStream(ESFMFile, FileMode.Open, FileAccess.Read), Encoding.BigEndianUnicode);

                    IO.Seek(0, SeekOrigin.Begin);

                    string np_comm_id = NpCommId.PadRight(16, '\0');
                    data_erk = aes_encrypt_cbc(PS4Keys.ShellCore_Keys.Retail.Trophy.Trophy_Key, keygen_riv, np_comm_id);
                    Stream basestream = IO.BaseStream;
                    data_riv = Util.StreamExtensions.ReadBytes(basestream, 16);

                    byte[] data = aes_decrypt_cbc(data_erk, data_riv, Util.StreamExtensions.ReadBytes(IO.BaseStream, (int)(IO.BaseStream.Length - 16)));

                    IO.Close();

                    File.WriteAllBytes(ESFMFile + ".DECRYPTED", data);
                }
                catch (Exception ex)
                {

                }
            }

            /// <summary>
            /// Load and Decrypts an encrypt trophy file
            /// </summary>
            public static byte[] LoadAndDecrypt(byte[] ESFMFile, string NpCommId)
            {
                BinaryWriter IO = new BinaryWriter(new MemoryStream(ESFMFile), Encoding.BigEndianUnicode);

                IO.Seek(0, SeekOrigin.Begin);

                string np_comm_id = NpCommId.PadRight(16, '\0');
                data_erk = aes_encrypt_cbc(PS4Keys.ShellCore_Keys.Retail.Trophy.Trophy_Key, keygen_riv, np_comm_id);
                Stream basestream = IO.BaseStream;
                data_riv = Util.StreamExtensions.ReadBytes(basestream, 16);

                byte[] data = aes_decrypt_cbc(data_erk, data_riv, Util.StreamExtensions.ReadBytes(IO.BaseStream, (int)(IO.BaseStream.Length - 16)));

                IO.Close();

                //File.WriteAllBytes(ESFMFile + ".DECRYPTED", data);
                return data;
            }
        }

    }

    /// <summary>
    /// PS4 Tools Recovery System
    /// </summary>
    public class Recovery
    {
        /// <summary>
        /// This is made possible by kemalsanli
        /// Credit for the orginal python script goes to him
        /// </summary>
        public static void TrophyTimeStampFix(string dbFileLocation = @"C:\Publish\Sony\trophy_local.db")
        {
            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load tropy db");
            Console.WriteLine("Updating... \n");
            string SQL = @"UPDATE tbl_trophy_title_entry SET time_last_update=time_last_update_uc where time_last_update=='0001-01-01T00:00:00.00Z'";
            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Failed to update records in SQL");
            }
            SQL = @"UPDATE tbl_trophy_title SET time_last_update=time_last_update_uc, time_last_unlocked=time_last_update_uc WHERE time_last_update == '0001-01-01T00:00:00.00Z' or time_last_unlocked == '0001-01-01T00:00:00.00Z'";
            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Failed to update records in SQL");
            }
            SQL = @"UPDATE tbl_trophy_flag SET time_unlocked=time_unlocked_uc where time_unlocked=='0001-01-01T00:00:00.00Z'";
            if (SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs) == false)
            {
                throw new Exception("Failed to update records in SQL");
            }



        }

        /// <summary>
        /// This should fix the trophy summary issues when a user rebuilds the database
        /// </summary>
        /// <param name="trpsummaryFile"></param>
        public static void FixTrophySummary(string dbFileLocation = @"C:\Publish\Sony\trophy_local.db", string trpsummaryFile = @"C:\Publish\Sony\trpsummary.dat")
        {
            Console.WriteLine("Create connection to trophy db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load tropy db");

            //create a backup 
            if (File.Exists(trpsummaryFile + ".backup"))
            {
                File.Delete(trpsummaryFile + ".backup");
            }

            File.Copy(trpsummaryFile, trpsummaryFile + ".backup", true);

            //load up current trophies
            //we need to rebuild the summary trophy file cause we are not using the sony method we are doing it manual
            //todo do screenshots somewhere
            Trophy_File.trpsummary myDeserializedClass = JsonConvert.DeserializeObject<Trophy_File.trpsummary>(File.ReadAllText(trpsummaryFile));

            string SQL = "SELECT SUM(unlocked_platinum_num) FROM tbl_trophy_title";

            int TotalPlatNum = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Platinum = TotalPlatNum;

            SQL = "SELECT SUM(unlocked_gold_num) FROM tbl_trophy_title";

            int TotalGoldNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Gold = TotalGoldNUm;

            SQL = "SELECT SUM(unlocked_silver_num) FROM tbl_trophy_title";

            int TotalSilverNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Silver = TotalSilverNUm;

            SQL = "SELECT SUM(unlocked_bronze_num) FROM tbl_trophy_title";

            int TotalBronzeNUm = Convert.ToInt32(SQLLite.SqlHelper.GetSingleValue(SQL, cs));

            myDeserializedClass.EarnedTrophies.Bronze = TotalBronzeNUm;

            File.WriteAllText(trpsummaryFile, JsonConvert.SerializeObject(myDeserializedClass));

        }

        /// <summary>
        /// Rebuilds your App.db Credits for this goes to Zer0xFF
        /// </summary>
        /// <param name="dbFileLocation"></param>
        public static void RebuildAppDb(string dbFileLocation = @"C:\Publish\Sony\app.db", string apploc = @"/user/app/", string metaloc = @"/system_data/priv/appmeta/")
        {
            Processed = new List<CUSA>();
            Console.WriteLine("Create connection to app.db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load tropy db");


            //create a backup 
            if (File.Exists(dbFilename + ".backup"))
            {
                File.Delete(dbFilename + ".backup");
            }
            File.Copy(dbFilename, dbFilename + ".backup", true);//just give it an overwrite incase


            List<string> CusaList = new List<string>();

            string[] allfiles = Directory.GetDirectories(apploc);
            for (int i = 0; i < allfiles.Length; i++)
            {
                if (allfiles[i].Contains("CUSA"))
                {
                    CusaList.Add(Path.GetFileName(allfiles[i]));
                }
            }
            //create the in List
            string titleidlist = "";
            for (int i = 0; i < CusaList.Count; i++)
            {
                titleidlist += "'" + CusaList[i] + "',";
            }


            titleidlist = titleidlist.Remove(titleidlist.Length - 1, 1);//remove the last,

            string insertlist = "";

            string SQL = @"SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'tbl_appbrowse_%%';";
            DataTable dttemp = SQLLite.SqlHelper.GetDataTable(SQL, cs);
            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                SQL = "SELECT T.titleid FROM " + dttemp.Rows[i][0].ToString() + " T WHERE T.titleid NOT IN (" + titleidlist + @")";
                DataTable dtMissing = SQLLite.SqlHelper.GetDataTable(SQL, cs);
                for (int ix = 0; ix < dtMissing.Rows.Count; ix++)
                {
                    //title id needs to be checked
                    string GameId = dtMissing.Rows[i][0].ToString();
                    Console.WriteLine("Processing GameID:" + GameId);
                    var cusa = get_game_info_by_id(GameId, metaloc, apploc);
                    if (cusa.is_usable == true)
                    {
                        insertlist = "('" + cusa.sfo.TitleID + "','" + cusa.sfo.ContentID + "','" + cusa.sfo.Title + "','/user/appmeta/" + cusa.sfo.TitleID + "', '2018-07-27 15:06:46.822', '0', '0', '5', '1', '100', '0', '151', '5', '1', 'gd', '0', '0', '0', '0', NULL, NULL, NULL, '" + cusa.size.ToString() + "', '2018-07-27 15:06:46.802', '0', 'game', NULL, '0', '0', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '0', NULL, NULL, NULL, NULL, NULL, '0', '0', NULL, '2018-07-27 15:06:46.757')";
                        //do the insert here and just get it over and done with 
                        SQL = "INSERT INTO " + dttemp.Rows[i][0].ToString() + " VALUES " + insertlist;
                        SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs);
                        Console.WriteLine("Completed");
                    }
                    else
                    {
                        Console.WriteLine("Skipped");
                    }
                }

                Console.WriteLine("Processing table: tbl_appinfo");

                SQL = @"SELECT DISTINCT T.titleid FROM (SELECT titleid FROM " + dttemp.Rows[i][0].ToString() + ") T WHERE T.titleid LIKE 'CUSA%%' AND T.titleid NOT IN (SELECT DISTINCT titleid FROM tbl_appinfo);";
                DataTable dtmissing_appinfo_cusa_id = SQLLite.SqlHelper.GetDataTable(SQL, cs);
                for (int ix = 0; ix < dtmissing_appinfo_cusa_id.Rows.Count; ix++)
                {
                    string game_id = dtmissing_appinfo_cusa_id.Rows[i][0].ToString();
                    Console.WriteLine("Processing GameID:" + game_id);
                    var cusa = get_game_info_by_id(game_id, metaloc, apploc);
                    if (cusa.is_usable == true)
                    {
                        List<AppInfo> sqlitems = get_pseudo_appinfo(cusa.sfo, cusa.size);
                        for (int sq = 0; sq < sqlitems.Count; sq++)
                        {
                            SQL = "INSERT INTO tbl_appinfo (titleid, key, val) VALUES ('" + game_id + "','" + sqlitems[i].Key + "','" + sqlitems[i].Value + "')";
                            SQLLite.SqlHelper.ExecuteNonQueryBL1(SQL, cs);
                        }

                        Console.WriteLine("Completed");
                    }
                    else
                    {
                        Console.WriteLine("Skipped");
                    }
                }
            }


        }

        private static List<CUSA> Processed = new List<CUSA>();



        private class CUSA
        {
            public string GameId = "";
            public Param_SFO.PARAM_SFO sfo = null;

            public long size = 999999;

            public bool is_usable = false;

        }

        private static CUSA get_game_info_by_id(string GameID, string AppMetaLocation, string apploc)
        {
            bool exits = false;
            for (int i = 0; i < Processed.Count; i++)
            {
                if (Processed[i].GameId == GameID)
                {
                    return Processed[i];
                }
            }
            try
            {
                var sfo = new Param_SFO.PARAM_SFO(AppMetaLocation + GameID + @"/param.sfo");
                if (sfo.Category == "")
                {
                    var holder = new CUSA();
                    holder.GameId = GameID;
                    holder.is_usable = false;
                    holder.size = 0;
                    Processed.Add(holder);
                    return holder;
                }
                else
                {
                    var holder = new CUSA();
                    holder.GameId = GameID;
                    holder.sfo = sfo;
                    holder.is_usable = true;
                    try
                    {
                        holder.size = new System.IO.FileInfo(apploc + GameID + "/app.pkg").Length;
                    }
                    catch (Exception ex)
                    {
                        //for testing on a local disk i did not copy all pkg files over
                        holder.size = 0;
                    }
                    Processed.Add(holder);
                    return holder;
                }
            }
            catch (Exception ex)
            {
                var holder = new CUSA();
                holder.GameId = GameID;
                holder.is_usable = false;
                holder.size = 0;
                Processed.Add(holder);
                return holder;
            }

        }


        private class AppInfo
        {
            public string Key = "";
            public string Value = "";
        }



        private static List<AppInfo> get_pseudo_appinfo(Param_SFO.PARAM_SFO sfo, long size)
        {
            //build the first few items by default 

            List<AppInfo> appinfo = new List<AppInfo>();
            AppInfo info = new AppInfo();
            info.Key = "#_access_index";
            info.Value = "67";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#_last_access_time";
            info.Value = "2018-07-27 15:04:39.822";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#_contents_status";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#_mtime";
            info.Value = "2018-07-27 15:04:40.635";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#_size";
            info.Value = size.ToString();
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#_update_index";
            info.Value = "74";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "#exit_type";
            info.Value = "0";
            appinfo.Add(info);

            //dump all SFO info here 
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                info = new AppInfo();
                info.Key = sfo.Tables[i].Name.ToString();
                info.Value = sfo.Tables[i].Value.ToString();
                appinfo.Add(info);
            }

            info = new AppInfo();
            info.Key = "_contents_ext_type";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_contents_location";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_current_slot";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_disable_live_detail";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_hdd_location";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_path_info";
            info.Value = "3113537756987392";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_path_info_2";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_size_other_hdd";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_sort_priority";
            info.Value = "100";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_uninstallable";
            info.Value = "1";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_view_category";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_working_status";
            info.Value = "0";
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_org_path";
            info.Value = "/user/app/" + sfo.TitleID;
            appinfo.Add(info);

            info = new AppInfo();
            info.Key = "_metadata_path";
            info.Value = "/user/appmeta/" + sfo.TitleID;
            appinfo.Add(info);


            return appinfo;

        }


    }

    /// <summary>
    /// PS4 Licesing reserved class
    /// </summary>
    public class Licensing
    {

        #region << ACF >>
        //https://www.psdevwiki.com/ps4/Activation_ACF
        /*
         *  From	To	Description
            1C9080	1C9083	MAGIC acf
            1C9084	1C9087	Constant1 Constant 4 bytes always the same.
            1C9088	1C9097	Unique1 Unique 16 bytes.
            1C9098	1C909B	Constant2 Constant 4 bytes always the same.
            1C909C	1C90A3	Unique2 Unique 8 bytes.
            1C90A4	1C90A7	Const3 Constant 4 bytes always the same.
            1C90A8	1C90E7	Unique3 Unique 64 bytes.*/

        /// <summary>
        /// This seems to be a type of activation file for devkits/ testkits
        /// I did not have a file so this code should work
        /// </summary>
        protected internal struct ACF
        {
            internal static byte[] MAGIC = new byte[4];
            internal static int CONST1;
            internal static byte[] BYTE16 = new byte[16];
            internal static int CONST2;
            internal static byte[] BYTE8 = new byte[8];
            internal static int CONST3;
            internal static byte[] BYTE64 = new byte[64];

            public void LoadACFFile(byte[] Acffile)
            {
                MemoryStream fs = new MemoryStream(Acffile);
                BinaryReader br = new BinaryReader(fs);
                br.BaseStream.Position = 0x1C9080;//go to the start of the file

                MAGIC = br.ReadBytes(4);
                CONST1 = br.ReadInt32();
                BYTE16 = br.ReadBytes(16);
                CONST2 = br.ReadInt32();
                BYTE8 = br.ReadBytes(8);
                CONST3 = br.ReadInt32();
                BYTE64 = br.ReadBytes(64);

                if (!Util.Utils.CompareBytes(MAGIC, new byte[] { 0x61, 0x63, 0x66, 0x00 }))
                {
                    throw new Exception("This is not a valid ACF");
                }
            }
        }

        #endregion << ACF >>

        #region << SealedKey >>



        /// <summary>
        /// Everything Inside the PS4/ from PKG's To Save Data Uses a sealed key
        /// </summary>
        public struct Sealedkey
        {
            public byte[] MAGIC;/*Magic needs to be validated*/
            public byte[] KeySet;/*Sony uses this to tell the system which one of the key sets to use in case one needs to be scrapped I'm guessing*/
            public byte[] AlignBytes;/*Padding to IV*/
            public byte[] IV;/*IV Key For SHA256 at this point im guessing the system uses CBC but i could be wrong*/
            public byte[] KEY;/*Key Fpr SHA256*/
            public byte[] SHA256;//SHA256 HASH?
        }

        /// <summary>
        /// Loads a SealedKey File Into the SealedKey Obejct
        /// </summary>
        /// <param name="Sealedkeylocation">Sealed Key Location On Disk</param>
        /// <returns></returns>
        public static Sealedkey LoadSealedKey(string Sealedkeylocation)
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
            return sldkey;
        }


        /// <summary>
        /// Loads a SealedKey File Into the SealedKey Obejct
        /// </summary>
        /// <param name="SealedKey">Sealed Key Location as bytearray</param>
        /// <returns></returns>
        public static Sealedkey LoadSealedKey(byte[] SealedKey)
        {
            /*Load the sealed key from the file*/
            Sealedkey sldkey = default(Sealedkey);
            sldkey.MAGIC = new byte[8];
            sldkey.KeySet = new byte[2];
            sldkey.AlignBytes = new byte[6];
            sldkey.IV = new byte[16];
            sldkey.KEY = new byte[32];
            sldkey.SHA256 = new byte[32];

            MemoryStream fs = new MemoryStream(SealedKey);

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
            return sldkey;
        }

        #endregion << SealedKey >>

        #region << Rif >>

        #region << Riff Information >>
        public enum RiffType
        {
            KDS,
            Isolated,
            Disc,
            Debug,
            Retial,
            Unknown1,
            DebugUnknown,
        }

        private static RiffType GetRiffType(byte[] TypeAddress)
        {
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 0, 0 }))
            {
                return RiffType.KDS;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 0, 1 }))
            {
                return RiffType.KDS;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 0, 3 }))
            {
                return RiffType.KDS;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 1, 1 }))
            {
                return RiffType.Isolated;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 3, 2 }))
            {
                return RiffType.Isolated;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 1, 2 }))
            {
                return RiffType.Disc;
            }
            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 2, 1 }) || Util.Utils.CompareBytes(TypeAddress, new byte[] { 2, 2 }) || Util.Utils.CompareBytes(TypeAddress, new byte[] { 2, 3 }))
            {
                return RiffType.Debug;
            }

            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 3, 3 }))
            {
                return RiffType.Retial;
            }

            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 3, 4 }))
            {
                return RiffType.Unknown1;
            }

            if (Util.Utils.CompareBytes(TypeAddress, new byte[] { 3, 0, 5 }))
            {
                return RiffType.DebugUnknown;
            }
            return RiffType.Unknown1;
        }

        public class RIF
        {
            /// <summary>
            /// Magic should always be RIF\0
            /// </summary>
            public byte[] Magic { get; set; }
            public byte[] Version { get; set; }
            public byte[] Unknown { get; set; }
            public byte[] PSN_Account_ID { get; set; }
            public byte[] Start_Timestamp { get; set; }
            public byte[] End_Timestamp { get; set; }
            public byte[] Content_ID_Bytes { get; set; }

            public string Content_ID
            { get { return System.Text.Encoding.Default.GetString(Content_ID_Bytes); } }
            public RiffType Type { get; set; }
            public byte[] DRM_Type { get; set; }
            public byte[] Content_Type { get; set; }
            public byte[] SKU_Flag { get; set; }
            public byte[] Extra_Flags { get; set; }

            public byte[] Unknown1 = new byte[4];

            public byte[] Unknown2 = new byte[4];

            public byte[] Unknown3 = new byte[3];

            public byte[] Unknown4 = new byte[1];

            public byte[] Unknown5 = new byte[468];

            public byte[] Disc_key = new byte[32];

            public byte[] Secret_Encryption_IV_Bytes = new byte[16];
            public string Secret_Encryption_IV { get { return ByteArrayToString(Secret_Encryption_IV_Bytes); } }

            public byte[] Encrypted_Secret_Bytes = new byte[144];

            public Secret Encrypted_Secret { get { return Get_Secret(Encrypted_Secret_Bytes); } }

            public byte[] RSA_Signature_bytes = new byte[256];

            public string RSA_Signature { get { return ByteArrayToString(RSA_Signature_bytes); } }

        }

        public class Secret
        {
            public byte[] Unknown1 = new byte[16];

            public byte[] Unknown2 = new byte[16];

            public byte[] Unknown3 = new byte[16];

            public byte[] Content_Key_Seed = new byte[16]; //Used to generate PFS key

            public byte[] SELF_Key_Seed = new byte[16];//Used to generate SELF key

            public byte[] Unknown4 = new byte[16];

            public byte[] Unknown5 = new byte[16];

            public byte[] Entitlement_Key = new byte[16];

            public byte[] Unknown6 = new byte[16];

        }

        public static Secret Get_Secret(byte[] bytes)
        {
            Secret secret = new Secret();

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
            {
                secret.Unknown1 = binaryReader.ReadBytes(16);
                secret.Unknown2 = binaryReader.ReadBytes(16);
                secret.Unknown3 = binaryReader.ReadBytes(16);
                secret.Content_Key_Seed = binaryReader.ReadBytes(16);
                secret.SELF_Key_Seed = binaryReader.ReadBytes(16);
                secret.Unknown4 = binaryReader.ReadBytes(16);
                secret.Unknown5 = binaryReader.ReadBytes(16);
                secret.Entitlement_Key = binaryReader.ReadBytes(16);
                secret.Unknown6 = binaryReader.ReadBytes(16);
            }

            return secret;
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static RIF ReadRif(string RifLocation)
        {
            RIF rif = new RIF();
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(RifLocation)))
            {
                /*Check PS4 File Header*/
                Byte[] RifFileHeader = binaryReader.ReadBytes(4);
                if (!Util.Utils.CompareBytes(RifFileHeader, new byte[] { 0x52, 0x49, 0x46, 0x00 }))/*If Files Match*/
                {
                    //fail
                    /*Lets be Honest id actually want a universal solution ps3/psp2/psp rif's all in one spot 
                     This will also be used in my other project*/

                    throw new Exception("This is not a valid ps4 Rif File");
                }

                rif.Magic = RifFileHeader;
                rif.Version = binaryReader.ReadBytes(2);
                rif.Unknown = binaryReader.ReadBytes(2);
                rif.PSN_Account_ID = binaryReader.ReadBytes(8);
                rif.Start_Timestamp = binaryReader.ReadBytes(8);
                rif.End_Timestamp = binaryReader.ReadBytes(8);
                rif.Content_ID_Bytes = binaryReader.ReadBytes(48);
                rif.Type = GetRiffType(binaryReader.ReadBytes(2));
                rif.DRM_Type = binaryReader.ReadBytes(2);
                rif.Content_Type = binaryReader.ReadBytes(2);
                rif.SKU_Flag = binaryReader.ReadBytes(2);
                rif.Extra_Flags = binaryReader.ReadBytes(4);
                rif.Unknown1 = binaryReader.ReadBytes(4);
                rif.Unknown2 = binaryReader.ReadBytes(4);
                rif.Unknown3 = binaryReader.ReadBytes(3);
                rif.Unknown4 = binaryReader.ReadBytes(1);
                rif.Unknown5 = binaryReader.ReadBytes(468);
                rif.Disc_key = binaryReader.ReadBytes(32);
                rif.Secret_Encryption_IV_Bytes = binaryReader.ReadBytes(16);
                rif.Encrypted_Secret_Bytes = binaryReader.ReadBytes(144);
                rif.RSA_Signature_bytes = binaryReader.ReadBytes(256);
            }


            return rif;
        }


        public static RIF CreateNewRif(string COntentID, string PSN_Account_ID)
        {
            RIF rif = new RIF();
            //we need act.dat ? 

            return rif;
        }

        //public static RIF CreateNewRi
        #endregion < Riff Informtaiton >>

        #endregion << Rif >> 

        #region << Act.Dat >>

        public class Act_Dat
        {
            /// <summary>
            /// Magic should always be ACT\0
            /// </summary>
            public byte[] Magic = new byte[4];

            public byte[] Version = new byte[2];

            public byte[] Type = new byte[2];
            public byte[] PSN_Account_ID = new byte[8];
            public byte[] Start_Timestamp { get; set; }
            public byte[] End_Timestamp { get; set; }

            public byte[] Unknown1 = new byte[64];

            public byte[] DeviceId = new byte[32];
            public byte[] Unknown2 = new byte[32];

            public byte[] RIF_Secret_Encryption_IV_Bytes = new byte[16];
            public string RIF_Secret_Encryption_IV { get { return ByteArrayToString(RIF_Secret_Encryption_IV_Bytes); } }

            public byte[] RIF_Secret_Encryption_Key_Seed_Bytes = new byte[16];

            public string RIF_Secret_Encryption_Key_Seed { get { return ByteArrayToString(RIF_Secret_Encryption_Key_Seed_Bytes); } }

            public byte[] Unknown3 = new byte[64];

            public byte[] RSA_Signature_bytes = new byte[256];

            public string RSA_Signature { get { return ByteArrayToString(RSA_Signature_bytes); } }

        }

        public static Act_Dat Read_Act(string act_Location)
        {
            Act_Dat act = new Act_Dat();

            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(act_Location)))
            {
                /*Check PS4 File Header*/
                Byte[] ActFileHeader = binaryReader.ReadBytes(4);
                if (!Util.Utils.CompareBytes(ActFileHeader, new byte[] { 0x41, 0x43, 0x54, 0x00 }))/*If Files Match*/
                {
                    //fail
                    /*Lets be Honest id actually want a universal solution ps3/psp2/psp rif's all in one spot 
                     This will also be used in my other project*/

                    throw new Exception("This is not a valid ps4 Act File");
                }

                act.Magic = ActFileHeader;
                act.Version = binaryReader.ReadBytes(2);
                act.Type = binaryReader.ReadBytes(2);
                act.PSN_Account_ID = binaryReader.ReadBytes(8);
                act.Start_Timestamp = binaryReader.ReadBytes(8);
                act.End_Timestamp = binaryReader.ReadBytes(8);
                act.Unknown1 = binaryReader.ReadBytes(64);
                act.DeviceId = binaryReader.ReadBytes(32);
                act.Unknown2 = binaryReader.ReadBytes(32);
                act.RIF_Secret_Encryption_IV_Bytes = binaryReader.ReadBytes(16);
                act.RIF_Secret_Encryption_Key_Seed_Bytes = binaryReader.ReadBytes(16);
                act.Unknown3 = binaryReader.ReadBytes(64);
                act.RSA_Signature_bytes = binaryReader.ReadBytes(256);
            }


            return act;
        }

        #endregion << Act.Dat>>
    }

    #region << Files On The PS4 >>

    /// <summary>
    /// Content Information Files
    /// Content Information Files are Multimedia files used to display the content in the XMB.
    ///Only ICON0.PNG is mandatory.The other information related with the content (inputs/outputs) is stored in PARAM.SFO.
    /// </summary>
    public class Content_Information_Files
    {
        public class Changeinfo_File
        {
            long FileSize { get; set; }
            Changeinfo File { get; set; }
        }

        public Changeinfo Load_ChangeInfo(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Changeinfo));
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                Changeinfo result = (Changeinfo)serializer.Deserialize(fileStream);
                return result;
            }
        }

        [XmlRoot(ElementName = "changes")]
        public class Changes
        {
            [XmlAttribute(AttributeName = "app_ver")]
            public string App_ver { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "changeinfo")]
        public class Changeinfo
        {
            [XmlElement(ElementName = "changes")]
            public Changes Changes { get; set; }
        }
    }

    /// <summary>
    /// This multilingual XML document is used as a content information file containing voice recognition definitions for a title.
    ///It is always accompanied by pronunciation.sig, which is used to authenticate the pronunciation.xml
    /// The phoneme element provides a phonemic/phonetic pronunciation for the contained text to enable starting the application by the user.
    ///Depending on the licence region where a title is sold, these are always at least provided:
    /// </summary>
    public class Pronunciation_Files
    {
        [XmlRoot(ElementName = "speechRecognitionWords")]
        public class SpeechRecognitionWords
        {
            [XmlElement(ElementName = "text")]
            public string Text { get; set; }
            [XmlElement(ElementName = "pronunciation")]
            public string Pronunciation { get; set; }
        }

        [XmlRoot(ElementName = "language")]
        public class Language
        {
            [XmlElement(ElementName = "speechRecognitionWords")]
            public List<SpeechRecognitionWords> SpeechRecognitionWords { get; set; }
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName = "modified")]
            public string Modified { get; set; }
        }

        [XmlRoot(ElementName = "gamePackage")]
        public class GamePackage
        {
            [XmlElement(ElementName = "language")]
            public List<Language> Language { get; set; }
        }

        /// <summary>
        /// Use this to Load a Pronunciation.xml file
        /// </summary>
        /// <param name="path">Path of Pronunciation.xml file</param>
        /// <returns>GamePackage File</returns>
        public GamePackage Load_GamePackage(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GamePackage));
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                GamePackage result = (GamePackage)serializer.Deserialize(fileStream);
                return result;
            }
        }
    }


    #endregion << Files On The PS4 >>

    /*********************************************************
     *          PS4 PKG Reader by maxton  
     *          https://github.com/maxton/LibOrbisPkg
     *          
     *          And Official SCE Tools until we can fully 
     *          extract unencrypted file types
     *          
     *          SCE tools was removed in v1.0
     *          Maxtron's liborbis is our main handler 
     *          Still needs some .net 3.5 conversions
     *          especially memory mapped files
     *********************************************************/

    public class PKG
    {
        #region << For PS4 >>

        public static void LockGame(string TitleID, string dbFileLocation = @"C:\Publish\Sony\app.db")
        {
            Console.WriteLine("Create connection to app.db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load app.db");

            string SQL = @"SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'tbl_appbrowse_%%';";
            DataTable dttemp = SQLLite.SqlHelper.GetDataTable(SQL, cs);
            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                string Update = @"UPDATE "+dttemp.Rows[i][0].ToString()+@"
SET visible = 0
Where titleId = '"+TitleID+"'";
                SQLLite.SqlHelper.ExecuteNonQueryBL1(Update, cs);
            }

        }

        public static void UnlockGame(string TitleID,string dbFileLocation = @"C:\Publish\Sony\app.db")
        {
            Console.WriteLine("Create connection to app.db...");
            //SQLiteConnection con = new SQLiteConnection();

            string dbFilename = dbFileLocation;
            //build the connection string
            string cs = string.Format("Version=3;uri=file:{0}", dbFilename);//sony is format 3
            if (!File.Exists(dbFilename))
                throw new Exception("Could not load app.db");
            string SQL = @"SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'tbl_appbrowse_%%';";
            DataTable dttemp = SQLLite.SqlHelper.GetDataTable(SQL, cs);
            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                string Update = @"UPDATE " + dttemp.Rows[i][0].ToString() + @"
SET visible = 1
Where titleId = '" + TitleID + "'";
                SQLLite.SqlHelper.ExecuteNonQueryBL1(Update, cs);
            }
        }

        #endregion << For PS4 >>

        #region << Official >>
        public class Official
        {
            /// <summary>
            /// This Uses SCE Tools PLease Try and avoid this
            /// Will be intigrating maxtrons pkg tools
            /// </summary>
            /// <param name="FilePath">PS4 PKG File Path</param>
            /// <returns></returns>
            //public static List<string> ReadAllUnprotectedData(string FilePath)
            //{
            //    File.WriteAllBytes(PS4_Tools.AppCommonPath() + "ext.zip", Properties.Resources.ext);
            //    File.WriteAllBytes(PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);

            //    if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\ext\"))
            //    {
            //        ZipFile.ExtractToDirectory(PS4_Tools.AppCommonPath() + "ext.zip", PS4_Tools.AppCommonPath());
            //    }



            //    List<string> rtnlist = new List<string>();

            //    ProcessStartInfo start = new ProcessStartInfo();
            //    start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
            //    start.Arguments = "img_file_list  --no_passcode --oformat recursive \"" + FilePath + "\"";
            //    start.UseShellExecute = false;
            //    start.RedirectStandardOutput = true;
            //    start.CreateNoWindow = true;
            //    using (Process process = Process.Start(start))
            //    {
            //        process.ErrorDataReceived += delegate
            //        {

            //        };
            //        using (StreamReader reader = process.StandardOutput)
            //        {
            //            string result = reader.ReadToEnd();
            //            string[] splitresult = result.Split('\n');
            //            for (int i = 0; i < splitresult.Length; i++)
            //            {
            //                rtnlist.Add(splitresult[i]);
            //            }

            //            // return result;
            //        }
            //    }


            //    return rtnlist;
            //}

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
                    public long fileOffset { get; set; }
                    public long fileSize { get; set; }
                    public string hashValue { get; set; }
                }

                public class Manifest_Item
                {
                    public long originalFileSize { get; set; }
                    public string packageDigest { get; set; }
                    public long numberOfSplitFiles { get; set; }
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
                /// <summary>
                /// This use to be Download_URL but im adding changes so you can read content from the psn store
                /// </summary>
                public string Store_URL { get; set; }
                public byte[] Store_Content_Image { get; set; }//swithing to bytearry for Lapy 
            }

            public static Update_Structure.Titlepatch CheckForUpdate(string TitleID)
            {
                /*Check for update to ps game*/

                /*HMAC Create new url*/
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                string nptitle = "np_" + TitleID;
                byte[] keyByte = PS4Keys.ShellCore_Keys.Retail.HMAC_SHA256_Patch_Pkg_URL_Key.Key;  //Util.Utils.Hex2Binary("AD62E37F905E06BC19593142281C112CEC0E7EC3E97EFDCAEFCDBAAFA6378D84");
                HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
                byte[] messageBytes = encoding.GetBytes(nptitle);
                byte[] hashnptitle = hmacsha256.ComputeHash(messageBytes);
                string hash = Util.Utils.ByteToString(hashnptitle);

                // return;
                //var test = new SHA256()

                /*Get XML String */
                string urlofupdatexml = "http://gs-sec.ww.np.dl.playstation.net/plo/np/" + TitleID + "/" + hash.ToLower() + "/" + TitleID + "-ver.xml";

                nptitle = "" + TitleID;
                keyByte = PS4Keys.ShellCore_Keys.Retail.HMAC_SHA256_Patch_Pkg_URL_Key.Key;  //Util.Utils.Hex2Binary("AD62E37F905E06BC19593142281C112CEC0E7EC3E97EFDCAEFCDBAAFA6378D84");
                hmacsha256 = new HMACSHA256(keyByte);
                messageBytes = encoding.GetBytes(nptitle);
                hashnptitle = hmacsha256.ComputeHash(messageBytes);
                hash = Util.Utils.ByteToString(hashnptitle);

                string checkforappver = "http://gs2-sec.ww.prod.dl.playstation.net/gs2-sec/appkgo//prod/" + TitleID + "/" + hash.ToLower() + "/" + TitleID + "/" + "icon0.png";
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        //add protocols incase sony wants to add them
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
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
                        try
                        {
                            string json = sr.ReadToEnd();
                            pkg.Manifest_item = JsonConvert.DeserializeObject<Update_Structure.Manifest_Item>(json);
                        }
                        catch (Exception ex)
                        {
                            //json changed ???
                        }
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
#if !PS4_UNITY
            private static Bitmap LoadPicture(string url)
            {
                try
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
                catch (Exception ex)
                {
                    return null;
                }
            }
#endif

#if PS4_UNITY
            private static Stream LoadPicture(string url)
            {
                try
                {
                    HttpWebRequest wreq;
                    HttpWebResponse wresp;
                    Stream mystream;
                    

                   
                    mystream = null;
                    wresp = null;
                    try
                    {
                        wreq = (HttpWebRequest)WebRequest.Create("http" + url);
                        wreq.AllowWriteStreamBuffering = true;

                        wresp = (HttpWebResponse)wreq.GetResponse();

                        if ((mystream = wresp.GetResponseStream()) != null)
                            return mystream;
                    }
                    finally
                    {
                        if (mystream != null)
                            mystream.Close();

                        if (wresp != null)
                            wresp.Close();
                    }
                    return (mystream);
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
#endif
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
                        try
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
                            newitem.Store_Content_Title = Util.WebUtility.HtmlDecode(newitem.Store_Content_Title);

                            splittedcelltitel = Regex.Split(splittedfooter[0], "a href=\"");
                            splittedifno = Regex.Split(splittedcelltitel[1], "\"");
                            newitem.Store_URL = "https://store.playstation.com" + splittedifno[0].Trim();

                            splittedcelltitel = Regex.Split(splittedfooter[0], "img src=\"http");
                            splittedifno = Regex.Split(splittedcelltitel[1], "\"");
#if !PS4_UNITY
                            newitem.Store_Content_Image = LoadPicture(splittedifno[0].Trim()).ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
#endif
#if PS4_UNITY
                            newitem.Store_Content_Image = LoadPicture(splittedifno[0].Trim()).ToByteArray();
#endif
                            splittedcelltitel = Regex.Split(splittedfooter[0], "left-detail--detail-2\">");
                            splittedifno = Regex.Split(splittedcelltitel[1], "<");
                            newitem.Store_Content_Type_Str = splittedifno[0].Trim();
                            newitem.Store_Content_Type_Str = Util.WebUtility.HtmlDecode(newitem.Store_Content_Type_Str);

                            splittedcelltitel = Regex.Split(splittedfooter[0], "left-detail--detail-1\">");
                            splittedifno = Regex.Split(splittedcelltitel[1], "<");
                            newitem.Store_Content_Platform_Str = splittedifno[0].Trim();
                            newitem.Store_Content_Platform_Str = Util.WebUtility.HtmlDecode(newitem.Store_Content_Platform_Str);

                            storeitems.Add(newitem);
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }



                return storeitems;
            }
        }

        #endregion << Official >>

        #region << Scene Related >>

        public class SceneRelated
        {

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
                [XmlRoot(ElementName = "chunk_status")]
                public class Chunk_status
                {
                    [XmlElement(ElementName = "chunks")]
                    public Chunks Chunks { get; set; }
                    [XmlElement(ElementName = "scenarios")]
                    public Scenarios Scenarios { get; set; }
                    [XmlAttribute(AttributeName = "chunk_count")]
                    public string Chunk_count { get; set; }
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
                    [XmlElement(ElementName = "chunk_status")]
                    public Chunk_status Chunk_status { get; set; }
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
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Encoding = Encoding.UTF8;
                        //XmlWriter writtest = new 

                        var xmlserializer = new XmlSerializer(typeof(Psproject));
                        var stringWriter = new StringWriter();
                        using (var writer = XmlWriter.Create(stringWriter, settings))
                        {
                            writer.WriteProcessingInstruction("xml", @"version=""1.0"" encoding=""UTF-8"" standalone=""yes""");
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
                /*TODO::*/
                /*Console ID*/

            }

            public class PARAM_SFO
            {
                public static Param_SFO.PARAM_SFO Get_Param_SFO(string pkgfile)
                {
                    //PARAM_SFO 

                    var ps4_pkg = PKG.SceneRelated.Read_PKG(pkgfile);
                    return ps4_pkg.Param;

                    //List<string> lstoffiles = 

                    //if (lstoffiles.Contains("Sc0/nptitle.dat\r"))
                    //{
                    //    if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\Working"))
                    //    {
                    //        Directory.CreateDirectory(PS4_Tools.AppCommonPath() + @"\Working");
                    //    }

                    //    //extract files to temp folder
                    //    ProcessStartInfo start = new ProcessStartInfo();
                    //    start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
                    //    start.Arguments = "img_extract --no_passcode \"" + pkgfile + "\" \"" + PS4_Tools.AppCommonPath() + @"Working" + "\"";
                    //    start.UseShellExecute = false;
                    //    start.RedirectStandardOutput = true;
                    //    start.CreateNoWindow = true;
                    //    using (Process process = Process.Start(start))
                    //    {
                    //        process.ErrorDataReceived += delegate
                    //        {

                    //        };
                    //        using (StreamReader reader = process.StandardOutput)
                    //        {
                    //            string result = reader.ReadToEnd();


                    //            // return result;
                    //        }
                    //    }

                    //    return new Param_SFO.PARAM_SFO(PS4_Tools.AppCommonPath() + @"\Working\Sc0\Param.sfo");

                    //}
                    //return new Param_SFO.PARAM_SFO();
                }
            }

            public class NP_Bind
            {



                //Read NP_Bind
                public byte[] magic;                        //D2 94 A0 18
                public byte[] version;                      //00 00 00 01
                public byte[] unk1;                         //00 00 00 00 00 00 02 14
                public byte[] unk2;                         //length is 0x70 they seem to be all the same with all versions
                public byte[] revions;                      //00 10 00 0C ? revisoin ?
                public string Nptitle;                      //4E 50 57 52 31 35 30 39 31 5F 30 30
                public byte[] unk3;                         //00 10 00 0C ? revisoin again ?
                public byte[] unk4;                         //30 00 00 00 00 00 00 00 00 00 00 00 first part might be the key hash used ?
                public byte[] unk5;                         //00 12 00 B0 ? not sure another random holder ?
                public byte[] NpTitleSecret;                //This is from here to end of file 64 bit enc key
                public byte[] emptyspace;                   //random empty space ?
                public byte[] Footer;                       //Footer encryption ?            

                public NP_Bind()
                {

                }
                //Read NP_Title
                public NP_Bind(byte[] filebytes)
                {
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(filebytes)))
                    {
                        Byte[] FileMagic = binaryReader.ReadBytes(4);
                        if (!Util.Utils.CompareBytes(FileMagic, new byte[] { 0xD2, 0x94, 0xA0, 0x18 }))/*If Files Match*/
                        {
                            throw new Exception("This is not a valid NP_Bind file");
                        }
                        //read ProductCode
                        binaryReader.BaseStream.Position = 0;
                        magic = binaryReader.ReadBytes(4);
                        version = binaryReader.ReadBytes(4);
                        unk1 = binaryReader.ReadBytes(8);
                        unk2 = binaryReader.ReadBytes(0x70);
                        revions = binaryReader.ReadBytes(4);
                        Nptitle = System.Text.Encoding.UTF8.GetString(binaryReader.ReadBytes(0xC));//4E 50 57 52 31 35 30 39 31 5F 30 30
                        unk3 = binaryReader.ReadBytes(4);
                        unk4 = binaryReader.ReadBytes(0xC);
                        unk5 = binaryReader.ReadBytes(4);
                        NpTitleSecret = binaryReader.ReadBytes(0xC4);
                        emptyspace = binaryReader.ReadBytes(0x98);
                        Footer = binaryReader.ReadBytes(0x14);
                    }
                }

                public NP_Bind(string FileLocation)
                {
                    byte[] filebytes = File.ReadAllBytes(FileLocation);
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(filebytes)))
                    {
                        Byte[] FileMagic = binaryReader.ReadBytes(4);
                        if (!Util.Utils.CompareBytes(FileMagic, new byte[] { 0xD2, 0x94, 0xA0, 0x18 }))/*If Files Match*/
                        {
                            throw new Exception("This is not a valid NP_Title file");
                        }
                        //read ProductCode
                        binaryReader.BaseStream.Position = 0;
                        magic = binaryReader.ReadBytes(4);
                        version = binaryReader.ReadBytes(4);
                        unk1 = binaryReader.ReadBytes(8);
                        unk2 = binaryReader.ReadBytes(0x70);
                        revions = binaryReader.ReadBytes(4);
                        Nptitle = System.Text.Encoding.UTF8.GetString(binaryReader.ReadBytes(0xC));//4E 50 57 52 31 35 30 39 31 5F 30 30
                        unk3 = binaryReader.ReadBytes(4);
                        unk4 = binaryReader.ReadBytes(0xC);
                        unk5 = binaryReader.ReadBytes(4);
                        NpTitleSecret = binaryReader.ReadBytes(0xC4);
                        emptyspace = binaryReader.ReadBytes(0x98);
                        Footer = binaryReader.ReadBytes(0x14);
                    }

                }



                //Save NP_Bind
            }

            public class NP_Title
            {
                public byte[] magic;                        //4E 50 54 44
                public byte[] flag;                         //00 00 00 80
                public byte[] emptyval;                     //00 00 00 00 00 00 00 00
                public string Nptitle;                      //43 55 53 41 31 30 38 35 39 5F 30 30 00 00 00 00
                public byte[] NpTitleSecret;                //This is from here to end of file 64 bit enc key

                public NP_Title()
                {
                    
                }
                //Read NP_Title
                public NP_Title(byte[] filebytes)
                {
                    NP_Title rtnstruct = new NP_Title();
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(filebytes)))
                    {
                        Byte[] FileMagic = binaryReader.ReadBytes(4);
                        if (!Util.Utils.CompareBytes(FileMagic, new byte[] { 0x4E, 0x50, 0x54, 0x44 }))/*If Files Match*/
                        {
                            throw new Exception("This is not a valid NP_Title file");
                        }
                        //read ProductCode
                        binaryReader.BaseStream.Position = 0;
                        magic = binaryReader.ReadBytes(4);
                        flag = binaryReader.ReadBytes(4);//00 00 00 80 debug
                        emptyval = binaryReader.ReadBytes(8);
                        Nptitle = System.Text.Encoding.UTF8.GetString(binaryReader.ReadBytes(16));
                        NpTitleSecret = binaryReader.ReadBytes(0x80);
                    }
                }

                public NP_Title(string FileLocation)
                {
                    byte[] filebytes = File.ReadAllBytes(FileLocation);
                    NP_Title rtnstruct = new NP_Title();
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(filebytes)))
                    {
                        Byte[] FileMagic = binaryReader.ReadBytes(4);
                        if (!Util.Utils.CompareBytes(FileMagic, new byte[] { 0x4E, 0x50, 0x54, 0x44 }))/*If Files Match*/
                        {
                            throw new Exception("This is not a valid NP_Title file");
                        }
                        //read ProductCode
                        binaryReader.BaseStream.Position = 0;
                        magic = binaryReader.ReadBytes(4);
                        flag = binaryReader.ReadBytes(4);//00 00 00 80 debug
                        emptyval = binaryReader.ReadBytes(8);
                        Nptitle = System.Text.Encoding.UTF8.GetString(binaryReader.ReadBytes(16));
                        NpTitleSecret = binaryReader.ReadBytes(0x80);
                    }

                }
                //Save NP_Title
            }

            public class PBM
            {
                public class PBMStruct
                {
                    public byte[] pbm_magic;                      // 0x000 - 0x7064626D //should be the same everywhere
                    public byte[] Productcode;                  // 0x004 - 0x435553413030323635 = "CUSA00265" remember to grab the extra bytes at the end 12
                    public uint emptyval;                       // 0x010 - not sure this just empty for some reason
                    public uint Unk1;                           // 0x014 - seems to always be 90 09 00 00
                    public uint Verion;                         // 0x018 - seems to always be 00 00 01 00
                    public uint emptyval2;                       // 0x01C - not sure this just empty for some reason
                    public uint Unk2;                           // 0x020 - examples 00 00 6C CE
                    public byte[] Padding;                      // 0x024 - seems to be padding length 3C
                    public byte[] Enc;                          // 0x060 - enc? hash ? length 20
                    public byte[] blankval;                     // 0x080 - blank values length 20
                    public byte[] UnkA0;                        // 0x0A0 - seems to always be 01 00 00 00 
                    public byte[] blankval2;                    // 0x0A4 - seems to always be 00 00 00 00 
                    public uint UnkA8;                          // 0x0A8 - Again not sure seems to always be 00 01 00 00
                    public uint Lenght;                         // 0x0AC - Length of file FF holder 5B 01 00 00
                    public byte[] BlockB0_BF;                   // 0x0B0 - this seems to be padding again B0- BF length 50
                    public byte[] FFStartHolder;                // 0x100 - Start of the FF holder - read the Length from the length attribute
                    public byte[] EndHash;                      // EOF for 20 bytes might be a hash? FC 50 65 C4 27 B0 7C 49 98 E8 26 3B 1D 85 E2 87 93 EB 38 F4 A3 F0 50 BF 7A 4A 77 0E 28 07 E8 E9 

                    public List<string> DisplayInfo()
                    {
                        List<string> rtnstr = new List<string>();
                        rtnstr.Add("pkg_magic:" + Encoding.ASCII.GetString(pbm_magic));
                        rtnstr.Add("Productcode:" + Encoding.ASCII.GetString(Productcode));
                        rtnstr.Add("emptyval:" + emptyval.ToString("X"));
                        rtnstr.Add("Unk1:" + Unk1.ToString("X"));
                        rtnstr.Add("Verion:" + Verion.ToString("X"));
                        rtnstr.Add("emptyval2:" + emptyval2.ToString("X"));
                        rtnstr.Add("Unk2:" + Unk2.ToString("X"));
                        rtnstr.Add("Padding:" + Padding.ToHexString());
                        rtnstr.Add("Enc:" + Enc.ToHexString());
                        rtnstr.Add("blankval:" + blankval.ToHexString());
                        rtnstr.Add("UnkA0:" + UnkA0.ToHexString());
                        rtnstr.Add("blankval2:" + blankval2.ToHexString());
                        rtnstr.Add("UnkA8:" + UnkA8.ToString("X"));
                        rtnstr.Add("Lenght:" + Lenght.ToString("X"));
                        rtnstr.Add("BlockB0_BF:" + BlockB0_BF.ToHexString());
                        rtnstr.Add("FFStartHolder:" + FFStartHolder.ToHexString());
                        rtnstr.Add("EndHash:" + EndHash.ToHexString());
                        //.ToString("X")
                        return rtnstr;
                    }

                }

                public static PBMStruct Read(byte[] filebytes)
                {
                    PBMStruct rtnstruct = new PBMStruct();
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(filebytes)))
                    {
                        Byte[] FileMagic = binaryReader.ReadBytes(4);
                        if (!Util.Utils.CompareBytes(FileMagic, new byte[] { 0x70, 0x64, 0x62, 0x6D }))/*If Files Match*/
                        {
                            throw new Exception("This is not a valid PBM file");
                        }
                        //read ProductCode
                        binaryReader.BaseStream.Position = 0;
                        rtnstruct.pbm_magic = binaryReader.ReadBytes(4);
                        rtnstruct.Productcode = binaryReader.ReadBytes(12);//0x0C
                        rtnstruct.emptyval = binaryReader.ReadUInt32();
                        rtnstruct.Unk1 = binaryReader.ReadUInt32();
                        rtnstruct.Verion = binaryReader.ReadUInt32();
                        rtnstruct.emptyval2 = binaryReader.ReadUInt32();
                        rtnstruct.Unk2 = binaryReader.ReadUInt32();
                        rtnstruct.Padding = binaryReader.ReadBytes(0x3C);
                        rtnstruct.Enc = binaryReader.ReadBytes(0x20);
                        rtnstruct.blankval = binaryReader.ReadBytes(0x20);
                        rtnstruct.UnkA0 = binaryReader.ReadBytes(4);
                        rtnstruct.blankval2 = binaryReader.ReadBytes(4);
                        rtnstruct.UnkA8 = binaryReader.ReadUInt32();//could be another version not sure
                        rtnstruct.Lenght = binaryReader.ReadUInt32();
                        rtnstruct.BlockB0_BF = binaryReader.ReadBytes(0x50);
                        rtnstruct.FFStartHolder = binaryReader.ReadBytes((int)rtnstruct.Lenght);
                        rtnstruct.EndHash = binaryReader.ReadBytes(0x20);
                    }


                    return rtnstruct;
                }

                public static PBMStruct Read(string filelocation)
                {
                    return Read(File.ReadAllBytes(filelocation));
                }
            }

            public class App
            {
                public class JSON
                {
                    // Generated by Xamasoft JSON Class Generator
                    public class Piece
                    {

                        [JsonProperty("fileOffset")]
                        public int FileOffset { get; set; }

                        [JsonProperty("fileSize")]
                        public int FileSize { get; set; }

                        [JsonProperty("url")]
                        public string Url { get; set; }
                    }

                    [JsonProperty("numberOfSplitFiles")]
                    public int NumberOfSplitFiles { get; set; }

                    [JsonProperty("packageDigest")]
                    public string PackageDigest { get; set; }

                    [JsonProperty("pieces")]
                    public IList<Piece> Pieces { get; set; }

                    public JSON()
                    {

                    }

                    public JSON(string ifile)
                    {
                        JSON ofile = new JSON();
                        StreamReader sr = new StreamReader(ifile);
                        string json = sr.ReadToEnd();
                        ofile = JsonConvert.DeserializeObject<JSON>(json);
                        this.NumberOfSplitFiles = ofile.NumberOfSplitFiles;
                        this.PackageDigest = ofile.PackageDigest;
                        this.Pieces = ofile.Pieces;
                    }
                }



            }





            /// <summary>
            /// Creates a PS4 Fake DLC Package
            /// </summary>
            /// <param name="Download_Url">PKG.Official.StoreItems().Store_URL</param>
            /// <param name="SaveLocation">Save Location on Local Disc</param>
            public static void Create_DLC_FKPG(string Download_Url, string SaveLocation)
            {
                ///*we can download an item */
                //File.WriteAllBytes(PS4_Tools.AppCommonPath() + "ext.zip", Properties.Resources.ext);
                //File.WriteAllBytes(PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);

                //if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\ext\"))
                //{
                //    //*Untill Maxtron has his method avaialble we still need some tools*//
                //    Util.Utils.ExtractFileToDirectory(PS4_Tools.AppCommonPath() + "ext.zip", PS4_Tools.AppCommonPath());
                //}

                //We are switching to Maxtron's Liborbis

                //we have maxtron's pkg tools now solets see what liborbis can do for us 
                // LibOrbis.PKG.PkgBuilder pkg = new PkgBuilder();


            }

            #region << UnprotectedPKG >>

            private static string m_pkgfile;
            private static bool m_loaded;
            private static byte[] sfo_byte;
            private static byte[] icon_byte;
            private static byte[] pic_byte;
            private static byte[] trp_byte;
            private static bool m_error;
            private static byte[] image_byte;
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
                Fake = 0,
                Official = 1,
                Officail_DP = 2,
                Unkown = 99
            }

            private static PKGType GetPkgType(string str)
            {
                if (str == "gde" || str == "gdk")
                {
                    return PKGType.App;
                }
                if (str == "gd")
                {
                    return PKGType.Game;
                }
                if (str == "ac")
                {
                    return PKGType.Addon_Theme;
                }
                if (str == "gp")
                {
                    return PKGType.Patch;
                }
                return PKGType.Unknown;
            }

            public class Unprotected_PKG
            {
                /// <summary>
                /// PKG Header Info 
                /// </summary>
                public PS4_Struct Header = new PS4_Struct();

                /// <summary>
                /// PKG Entires
                /// </summary>
                public PS4PkgUtil.PackageEntry[] Entires { get; set; }

                /*Param.SFO*/
                public Param_SFO.PARAM_SFO Param { get; set; }
                /*Trophy File*/
                public Trophy_File Trophy_File { get; set; }
                /*PKG Image*/
                public byte[] Image { get; set; }
                /// <summary>
                /// PS4 Icon Image
                /// </summary>
                public byte[] Icon { get; set; }

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

                public string Content_ID
                {
                    get
                    {
                        return Header.pkg_content_id;
                    }
                }

                public string Size
                {
                    get
                    {
                        return Util.Utils.GetHumanReadable(Header.pkg_size);
                    }
                }


                public string BuildDate
                {
                    get
                    {
                        return Util.Utils.FromUnixTime(Header.pkg_version_date).ToString();
                    }
                }

            }




            public class PS4_Struct
            {
                public uint pkg_magic;                      // 0x000 - 0x7F434E54
                public uint pkg_type;                       // 0x004
                public uint pkg_0x008;                      // 0x008 - unknown field
                public uint pkg_file_count;                 // 0x00C
                public uint pkg_entry_count;                // 0x010
                public uint pkg_sc_entry_count;             // 0x014
                public uint pkg_entry_count_2;              // 0x016 - same as pkg_entry_count
                public uint pkg_table_offset;               // 0x018 - file table offset
                public uint pkg_entry_data_size;            // 0x01C
                public ulong pkg_body_offset;                // 0x020 - offset of PKG entries
                public ulong pkg_body_size;                  // 0x028 - length of all PKG entries
                public ulong pkg_content_offset;             // 0x030
                public ulong pkg_content_size;               // 0x038
                public string pkg_content_id;               // 0x040 - packages' content ID as a 36-byte string
                public string pkg_padding;                  // 0x064 - padding
                public uint pkg_drm_type;                   // 0x070 - DRM type
                public uint pkg_content_type;               // 0x074 - Content type
                public uint pkg_content_flags;              // 0x078 - Content flags
                public uint pkg_promote_size;               // 0x07C
                public uint pkg_version_date;               // 0x080
                public uint pkg_version_hash;               // 0x084
                public uint pkg_0x088;                      // 0x088
                public uint pkg_0x08C;                      // 0x08C
                public uint pkg_0x090;                      // 0x090
                public uint pkg_0x094;                      // 0x094
                public uint pkg_iro_tag;                    // 0x098
                public uint pkg_drm_type_version;           // 0x09C
                public Digest_Table digesttable = new Digest_Table();
                public byte[] unk_0x400;
                public uint pfs_image_count;                // 0x404 - count of PFS images
                public ulong pfs_image_flags;                // 0x408 - PFS flags
                public ulong pfs_image_offset;               // 0x410 - offset to start of external PFS image
                public ulong pfs_image_size;                 // 0x418 - size of external PFS image
                public ulong mount_image_offset;             // 0x420
                public ulong mount_image_size;               // 0x428
                public ulong pkg_size;                       // 0x430
                public uint pfs_signed_size;                // 0x438
                public uint pfs_cache_size;                 // 0x43C
                public byte[] pfs_image_digest;//[0x20];    // 0x440
                public byte[] pfs_signed_digest;//[0x20];   // 0x460
                public ulong pfs_split_size_nth_0;           // 0x480
                public ulong pfs_split_size_nth_1;           // 0x488
                public byte[] pkg_digest;//[0x20];          // 0xFE0

                public List<string> DisplayInfo()
                {
                    List<string> rtnstr = new List<string>();
                    rtnstr.Add("pkg_magic:" + Encoding.ASCII.GetString(PKG_Magic));
                    rtnstr.Add("pkg_type:" + pkg_type);
                    rtnstr.Add("pkg_0x008:" + pkg_0x008.ToString("X"));
                    rtnstr.Add("pkg_file_count:" + pkg_file_count);
                    rtnstr.Add("pkg_entry_count:" + pkg_entry_count);
                    rtnstr.Add("pkg_sc_entry_count:" + pkg_sc_entry_count);
                    rtnstr.Add("pkg_entry_count_2:" + pkg_entry_count_2);
                    rtnstr.Add("pkg_table_offset: 0x" + pkg_table_offset.ToString("X"));
                    rtnstr.Add("pkg_entry_data_size: 0x" + pkg_entry_data_size.ToString("X"));
                    rtnstr.Add("pkg_body_offset: 0x" + pkg_body_offset.ToString("X"));
                    rtnstr.Add("pkg_body_size: 0x" + pkg_body_size.ToString("X"));
                    rtnstr.Add("pkg_content_offset: 0x" + pkg_content_offset.ToString("X"));
                    rtnstr.Add("pkg_content_size : 0x" + pkg_content_size.ToString("X"));
                    rtnstr.Add("pkg_content_id :" + pkg_content_id);
                    rtnstr.Add("pkg_padding :" + pkg_padding);
                    rtnstr.Add("pkg_drm_type :" + ((pkg_drm_type == 15) ? "PS4" : "Unkown"));
                    rtnstr.Add("pkg_content_type :" + pkg_content_type);
                    rtnstr.Add("pkg_content_flags :" + pkg_content_flags);
                    rtnstr.Add("pkg_promote_size :" + pkg_promote_size.ToString("X"));
                    rtnstr.Add("pkg_version_date :" + pkg_version_date.ToString("X"));
                    rtnstr.Add("pkg_version_hash :" + pkg_version_hash.ToString("X"));
                    rtnstr.Add("pkg_0x088 :" + pkg_0x088.ToString("X"));
                    rtnstr.Add("pkg_0x08C :" + pkg_0x08C.ToString("X"));
                    rtnstr.Add("pkg_0x090 :" + pkg_0x090.ToString("X"));
                    rtnstr.Add("pkg_0x094 :" + pkg_0x094.ToString("X"));
                    rtnstr.Add("pkg_iro_tag :" + ((pkg_iro_tag == 0) ? "None" : pkg_iro_tag.ToString()));
                    rtnstr.Add("ekc_version :" + pkg_drm_type_version.ToString("X"));
                    rtnstr.Add("digest_entries1 :" + digesttable.digest_entries1.ToHexString());
                    rtnstr.Add("digest_entries2 :" + digesttable.digest_entries2.ToHexString());
                    rtnstr.Add("digest_table_digest :" + digesttable.digest_table_digest.ToHexString());
                    rtnstr.Add("digest_body_digest :" + digesttable.digest_body_digest.ToHexString());
                    rtnstr.Add("unk_0x400 :" + Encoding.ASCII.GetString(unk_0x400));
                    rtnstr.Add("pfs_image_count :" + pfs_image_count);
                    rtnstr.Add("pfs_image_flags : 0x" + pfs_image_flags.ToString("X"));
                    rtnstr.Add("pfs_image_offset : 0x" + pfs_image_offset.ToString("X"));
                    rtnstr.Add("pfs_image_size : 0x" + pfs_image_size.ToString("X"));
                    rtnstr.Add("mount_image_offset : 0x" + mount_image_offset.ToString("X"));
                    rtnstr.Add("mount_image_size : 0x" + mount_image_size.ToString("X"));
                    rtnstr.Add("pkg_size : 0x" + pkg_size.ToString("X"));
                    rtnstr.Add("pfs_signed_size : 0x" + pfs_signed_size.ToString("X"));
                    rtnstr.Add("pfs_cache_size : 0x" + pfs_cache_size.ToString("X"));
                    rtnstr.Add("pfs_image_digest :" + pfs_image_digest.ToHexString());
                    rtnstr.Add("pfs_signed_digest :" + pfs_signed_digest.ToHexString());
                    rtnstr.Add("pfs_split_size_nth_0 : 0x" + pfs_split_size_nth_0.ToString("X"));
                    rtnstr.Add("pfs_split_size_nth_1 : 0x" + pfs_split_size_nth_1.ToString("X"));
                    rtnstr.Add("pkg_digest :" + pkg_digest.ToHexString());

                    return rtnstr;
                }
            }

            public class Digest_Table
            {
                public byte[] digest_entries1;//[0x20];     // 0x100 - sha256 digest for main entry 1
                public byte[] digest_entries2;//[0x20];     // 0x120 - sha256 digest for main entry 2
                public byte[] digest_table_digest;//[0x20]; // 0x140 - sha256 digest for digest table
                public byte[] digest_body_digest;//[0x20];  // 0x160 - sha256 digest for main table
            }

            /// <summary>
            /// Just to match PKG Magic
            /// </summary>
            private static byte[] PKG_Magic = new byte[] { 0x7F, 0x43, 0x4E, 0x54 };

            /// <summary>
            /// Reads a PS4 PKG Into an Unprotected_PKG Object
            /// </summary>
            /// <param name="pkgfile">PKG File Location</param>
            /// <returns>PKG With Unprotected_PKG Structure</returns>
            public static Unprotected_PKG Read_PKG(string pkgfile)
            {
                FileStream fs = new FileStream(pkgfile, FileMode.Open, FileAccess.Read);
                return Read_PKG(fs);
            }

            private static PS4_Struct ReadHeader(BinaryReader binaryReader)
            {
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                PS4_Struct ps4struct = new PS4_Struct();
                ps4struct.pkg_magic = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_type = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_0x008 = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_file_count = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_entry_count = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_sc_entry_count = Util.Utils.ReadUInt16(binaryReader);
                ps4struct.pkg_entry_count_2 = Util.Utils.ReadUInt16(binaryReader);
                ps4struct.pkg_table_offset = Util.Utils.ReadUInt32(binaryReader); //10880        
                ps4struct.pkg_entry_data_size = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_body_offset = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pkg_body_size = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pkg_content_offset = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pkg_content_size = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pkg_content_id = Util.Utils.ReadASCIIString(binaryReader, 0x24);
                ps4struct.pkg_padding = Util.Utils.ReadASCIIString(binaryReader, 0xC);
                ps4struct.pkg_drm_type = Util.Utils.ReadUInt32(binaryReader);//PS4 only ? Maybe ps5 soon since they support the same file system
                ps4struct.pkg_content_type = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_content_flags = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_promote_size = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_version_date = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_version_hash = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_0x088 = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_0x08C = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_0x090 = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_0x094 = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_iro_tag = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pkg_drm_type_version = Util.Utils.ReadUInt32(binaryReader);
                /*Digest Table*/

                binaryReader.BaseStream.Position = 0x100;

                ps4struct.digesttable.digest_entries1 = binaryReader.ReadBytes(0x20);
                ps4struct.digesttable.digest_entries2 = binaryReader.ReadBytes(0x20);
                ps4struct.digesttable.digest_table_digest = binaryReader.ReadBytes(0x20);
                ps4struct.digesttable.digest_body_digest = binaryReader.ReadBytes(0x20);

                //no idea
                binaryReader.BaseStream.Position = 0x400;
                ps4struct.unk_0x400 = binaryReader.ReadBytes(4);

                //pfs items
                ps4struct.pfs_image_count = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pfs_image_flags = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pfs_image_offset = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pfs_image_size = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.mount_image_offset = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.mount_image_size = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pkg_size = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pfs_signed_size = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pfs_cache_size = Util.Utils.ReadUInt32(binaryReader);
                ps4struct.pfs_image_digest = binaryReader.ReadBytes(0x20);
                ps4struct.pfs_signed_digest = binaryReader.ReadBytes(0x20);
                ps4struct.pfs_split_size_nth_0 = Util.Utils.ReadUInt64(binaryReader);
                ps4struct.pfs_split_size_nth_1 = Util.Utils.ReadUInt64(binaryReader);

                binaryReader.BaseStream.Position = 0xFE0;
                ps4struct.pkg_digest = binaryReader.ReadBytes(0x20);

                return ps4struct;
            }

            /*
             
                 */
            /// <summary>
            /// Extracts a PKG to a folder location this will only work with fake PKG's
            /// </summary>
            /// <param name="pkgfile"></param>
            public static void ExtarctPKG(string pkgfile)
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(pkgfile)))
                {
                    /*Check PS4 File Header*/
                    Byte[] PKGFileHeader = binaryReader.ReadBytes(4);
                    if (!Util.Utils.CompareBytes(PKGFileHeader, PKG_Magic))/*If Files Match*/
                    {
                        //fail
                        /*Lets be Honnest id actually want a universal solution ps3/psp2/psp pkg's all in one spot 
                         This will also be used in my other project*/

                        throw new Exception("This is not a valid ps4 pkg");
                    }

                    PS4_Struct ps4struct = ReadHeader(binaryReader);

                    binaryReader.BaseStream.Seek(9216, SeekOrigin.Begin);//go to a specific offset
                    byte[] data = PS4PkgUtil.Decrypt(binaryReader.ReadBytes(256));//simple decrypt


                    binaryReader.BaseStream.Seek(ps4struct.pkg_table_offset, SeekOrigin.Begin);

                    //This came from Red-EyeX32
                    //made some adjustments

                    PS4PkgUtil.PackageEntry[] entry = new PS4PkgUtil.PackageEntry[ps4struct.pkg_entry_count];
                    for (int i = 0; i < ps4struct.pkg_entry_count; ++i)
                    {
                        entry[i].id = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].filename_offset = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].flags1 = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].flags2 = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].offset = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].size = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].padding = binaryReader.ReadBytes(8);

                        entry[i].key_index = ((entry[i].flags2 & 0xF000) >> 12);
                        entry[i].is_encrypted = ((entry[i].flags1 & 0x80000000) != 0) ? true : false;
                    }

                    //create extacted directory
                    if (!Directory.Exists(Path.GetDirectoryName(pkgfile) + "\\Extracted\\"))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(pkgfile) + "\\Extracted\\");
                    }

                    for (int i = 0; i < ps4struct.pkg_entry_count; i++)
                    {
                        try
                        {
                            bool is_encrypted = entry[i].is_encrypted;
                            if (is_encrypted)
                            {
                                //testing image key stuff here
                                if (entry[i].CustomName == PS4PkgUtil.EntryId.IMAGE_KEY.ToString())
                                {
                                    //var file_data = PS4PkgUtil.Decrypt(entry[i].file_data,entry[i].);
                                    //string CustomName = entry[i].CustomName;
                                    //var lastComma = CustomName.LastIndexOf('_');
                                    //if (lastComma != -1) CustomName = CustomName.Remove(lastComma, 1).Insert(lastComma, ".");

                                    //File.WriteAllBytes(Path.GetDirectoryName(pkgfile) + "\\Extracted\\" + CustomName, file_data);
                                }
                                else if (entry[i].CustomName == PS4PkgUtil.EntryId.NPTITLE_DAT.ToString())
                                {
                                    var test = "";
                                    byte[] entry_data = new byte[64];
                                    Array.Copy(entry[i].ToArray(), entry_data, 28);
                                    Array.Copy(data, 0, entry_data, 28, 28);

                                    byte[] iv = new byte[16];
                                    byte[] key = new byte[16];
                                    byte[] hash = PS4PkgUtil.Sha256(entry_data, 0, entry_data.Length);
                                    Array.Copy(hash, 0, iv, 0, 16);
                                    Array.Copy(hash, 16, key, 0, 16);
                                    binaryReader.BaseStream.Position = (long)((ulong)entry[i].offset);
                                    byte[] file_data = PS4PkgUtil.DecryptAes(key, iv, binaryReader.ReadBytes((int)entry[i].size));
                                    entry[i].file_data = file_data;
                                    //File.WriteAllBytes(pkgfile + "_" + entry[i].CustomName, file_data);
                                    string CustomName = entry[i].CustomName;
                                    var lastComma = CustomName.LastIndexOf('_');
                                    if (lastComma != -1) CustomName = CustomName.Remove(lastComma, 1).Insert(lastComma, ".");
                                    File.WriteAllBytes(Path.GetDirectoryName(pkgfile) + "\\Extracted\\" + CustomName + "entrydata", entry_data);
                                    File.WriteAllBytes(Path.GetDirectoryName(pkgfile) + "\\Extracted\\" + CustomName, file_data);
                                }
                                else
                                {

                                    /*byte[] EnteryHolder = entry[i].ToArray();
                                    byte[] entry_data = new byte[EnteryHolder.Length];*/
                                    byte[] entry_data = new byte[64];
                                    Array.Copy(entry[i].ToArray(), entry_data, 32);
                                    Array.Copy(data, 0, entry_data, 32, 32);
                                    byte[] iv = new byte[16];
                                    byte[] key = new byte[16];
                                    byte[] hash = PS4PkgUtil.Sha256(entry_data, 0, entry_data.Length);
                                    Array.Copy(hash, 0, iv, 0, 16);
                                    Array.Copy(hash, 16, key, 0, 16);
                                    binaryReader.BaseStream.Position = (long)((ulong)entry[i].offset);
                                    byte[] file_data = PS4PkgUtil.DecryptAes(key, iv, binaryReader.ReadBytes((int)entry[i].size));
                                    entry[i].file_data = file_data;
                                    //File.WriteAllBytes(pkgfile + "_" + entry[i].CustomName, file_data);
                                    string CustomName = entry[i].CustomName;
                                    var lastComma = CustomName.LastIndexOf('_');
                                    if (lastComma != -1) CustomName = CustomName.Remove(lastComma, 1).Insert(lastComma, ".");

                                    File.WriteAllBytes(Path.GetDirectoryName(pkgfile) + "\\Extracted\\" + CustomName, file_data);
                                }
                            }
                            else
                            {
                                //else it should just be a simple read
                                binaryReader.BaseStream.Position = (long)((ulong)entry[i].offset);
                                byte[] file_data = binaryReader.ReadBytes((int)entry[i].size);
                                entry[i].file_data = file_data;
                                string CustomName = entry[i].CustomName;
                                var lastComma = CustomName.LastIndexOf('_');
                                if (lastComma != -1) CustomName = CustomName.Remove(lastComma, 1).Insert(lastComma, ".");

                                File.WriteAllBytes(Path.GetDirectoryName(pkgfile) + "\\Extracted\\" + CustomName, file_data);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
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
                image_byte = null;

                using (BinaryReader binaryReader = new BinaryReader(pkgfile))
                {
                    /*Check PS4 File Header*/
                    Byte[] PKGFileHeader = binaryReader.ReadBytes(4);
                    if (!Util.Utils.CompareBytes(PKGFileHeader, PKG_Magic))/*If Files Match*/
                    {
                        //fail
                        /*Lets be Honnest id actually want a universal solution ps3/psp2/psp pkg's all in one spot 
                         This will also be used in my other project*/

                        throw new Exception("This is not a valid ps4 pkg");
                    }

                    binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    pkgreturn.Header = ReadHeader(binaryReader);
                    PS4_Struct ps4struct = pkgreturn.Header;

                    binaryReader.BaseStream.Seek(9216, SeekOrigin.Begin);//go to a specific offset
                    byte[] data = PS4PkgUtil.Decrypt(binaryReader.ReadBytes(256));//simple decrypt


                    binaryReader.BaseStream.Seek(ps4struct.pkg_table_offset, SeekOrigin.Begin);

                    //This came from Red-EyeX32
                    //made some adjustments

                    PS4PkgUtil.PackageEntry[] entry = new PS4PkgUtil.PackageEntry[ps4struct.pkg_entry_count];
                    for (int i = 0; i < ps4struct.pkg_entry_count; ++i)
                    {
                        entry[i].id = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].filename_offset = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].flags1 = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].flags2 = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].offset = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].size = Util.Utils.ReadUInt32(binaryReader);
                        entry[i].padding = binaryReader.ReadBytes(8);

                        entry[i].key_index = ((entry[i].flags2 & 0xF000) >> 12);
                        entry[i].is_encrypted = ((entry[i].flags1 & 0x80000000) != 0) ? true : false;
                    }

                    pkgreturn.Entires = entry;
                    string temp = Encoding.ASCII.GetString(data);

                    binaryReader.BaseStream.Seek(0x077, SeekOrigin.Begin);
                    ushort pkgtype = Util.Utils.ReadUInt16(binaryReader);//custom read offset 119 this will tll us if its debug or retail

                    //from the offset table we need to read the name

                    for (int i = 0; i < ps4struct.pkg_entry_count; i++)
                    {
                        try
                        {
                            bool is_encrypted = entry[i].is_encrypted;
                            if (is_encrypted)
                            {
                                byte[] entry_data = new byte[64];
                                Array.Copy(entry[i].ToArray(), entry_data, 32);
                                Array.Copy(data, 0, entry_data, 32, 32);
                                byte[] iv = new byte[16];
                                byte[] key = new byte[16];
                                byte[] hash = PS4PkgUtil.Sha256(entry_data, 0, entry_data.Length);
                                Array.Copy(hash, 0, iv, 0, 16);
                                Array.Copy(hash, 16, key, 0, 16);
                                binaryReader.BaseStream.Position = (long)((ulong)entry[i].offset);
                                byte[] file_data = PS4PkgUtil.DecryptAes(key, iv, binaryReader.ReadBytes((int)entry[i].size));
                                entry[i].file_data = file_data;
                                // File.WriteAllBytes(pkgfile + "_" + entry[j].CustomName, file_data);
                            }
                            else
                            {
                                //else it should just be a simple read
                                binaryReader.BaseStream.Position = (long)((ulong)entry[i].offset);
                                byte[] file_data = binaryReader.ReadBytes((int)entry[i].size);
                                entry[i].file_data = file_data;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                    for (int i = 0; i < entry.Length; i++)
                    {
                        if (entry[i].CustomName == PS4PkgUtil.EntryId.PARAM_SFO.ToString())
                        {
                            //we have param
                            sfo_byte = entry[i].file_data;
                        }
                        if (entry[i].CustomName == PS4PkgUtil.EntryId.ICON0_PNG.ToString())
                        {
                            icon_byte = entry[i].file_data;
                        }
                        if (entry[i].CustomName == PS4PkgUtil.EntryId.PIC0_PNG.ToString())
                        {
                            image_byte = entry[i].file_data;
                        }
                        if (entry[i].CustomName == PS4PkgUtil.EntryId.PIC1_PNG.ToString())
                        {
                            image_byte = entry[i].file_data;
                        }

                        if (entry[i].CustomName == PS4PkgUtil.EntryId.TROPHY__TROPHY00_TRP.ToString())
                        {
                            trp_byte = entry[i].file_data;
                        }
                    }

                    if (sfo_byte != null && sfo_byte.Length > 0)
                    {
                        Param_SFO.PARAM_SFO psfo = new Param_SFO.PARAM_SFO(sfo_byte);
                        pkgreturn.Param = psfo;
                    }
                    if (icon_byte != null && icon_byte.Length > 0)
                    {
                        pkgreturn.Icon = icon_byte;
                    }
                    else if (trp_byte != null && trp_byte.Length > 0)
                    {
                        Trophy_File trpreader = new Trophy_File();
                        trpreader.Load(trp_byte);
                        icon_byte = trpreader.ExtractFileToMemory("ICON0.PNG");
                        if (icon_byte != null && icon_byte.Length > 0)
                        {
                            pkgreturn.Icon = icon_byte;
                        }
                    }
                    if (image_byte != null && image_byte.Length > 0)
                    {
                        pkgreturn.Image = image_byte;
                    }

                    if (trp_byte != null && trp_byte.Length > 0)
                    {
                        Trophy_File trpreader = new Trophy_File();
                        //trpreader.Load(trp_byte);
                        pkgreturn.Trophy_File = trpreader.Load(trp_byte);
                    }
                    pkgreturn.PKGState = (pkgtype == 6666) ? PKG_State.Fake : ((pkgtype == 7747) ? PKG_State.Officail_DP : PKG_State.Official);

                }
                m_loaded = true;
                return pkgreturn;
            }

            #endregion << UnprotectedPKG >>

            /// <summary>
            /// POWERED BY MAXTRON
            /// </summary>
#if (!PS4_UNITY)
            #region << PKG File >>
            public class PS4PKGFile
            {
                public Pkg Package { get; set; }
                public Header PackageHeader { get; set; }
                public Param_SFO.PARAM_SFO PackageSFO
                {
                    get;
                    set;
                }


                public byte[] Ekpfs { get; set; }

                public GameArchives.AbstractPackage PackageInside { get; set; }

                public GameArchives.AbstractPackage InnerPFS { get; set; }

                public PKGEncryption Encryption { get; set; }
            }

            public class PKGEncryption
            {
                public byte[] dk3 { get; set; }

                public byte[] iv { get; set; }

                public byte[] imageKeyDecrypted { get; set; }
            }

            #endregion << PKG File >>

            public string pkgtable { get; set; }
            //private static PS4_Tools.LibOrbis.PKG.Pkg pkg = null;
            public static List<string> lst = new List<string>();

            /// <summary>
            /// Reads a pkg file and displays all its info inside the PKG Table 
            /// (Custom to maxtron)
            /// </summary>
            /// <param name="pkgFile"></param>
            public static PS4PKGFile ReadPKG(string pkgFile)
            {

                PS4PKGFile rtnfile = new PS4PKGFile();
                //throw new Exception("LibOrbisPkg needs to be ported to .net3.5 this is noted for a future release");
                var pkgfileloc = GameArchives.Util.LocalFile(pkgFile);

                Pkg pkg = new Pkg();

                //lets see what happens
                Stream ms = new FileStream(pkgFile, FileMode.Open, FileAccess.Read);

                using (Stream s = new FileStream(pkgFile, FileMode.Open, FileAccess.Read))
                {
                    var header = new PkgReader(s).ReadHeader();
                    rtnfile.PackageHeader = header;
                }
                using (Stream s = new FileStream(pkgFile, FileMode.Open, FileAccess.Read))
                {
                    var pkginside = new PkgReader(s).ReadPkg();
                    rtnfile.Package = pkginside;
                    var sfoEditor = pkginside.ParamSfo.ParamSfo;
                    /*Need to convert the 2*/
                    //rtnfile.PackageSFO = ConvertToParam(pkginside.ParamSfo);
                    pkg = pkginside;
                }

                try
                {
                    var dk3 = Crypto.RSA2048Decrypt(pkg.EntryKeys.Keys[3].key, RSAKeyset.PkgDerivedKey3Keyset);
                    var iv_key = Crypto.Sha256(
                      pkg.ImageKey.meta.GetBytes()
                      .Concat(dk3)
                      .ToArray());
                    var imageKeyDecrypted = pkg.ImageKey.FileData.Clone() as byte[];
                    Crypto.AesCbcCfb128Decrypt(
                      imageKeyDecrypted,
                      imageKeyDecrypted,
                      imageKeyDecrypted.Length,
                      iv_key.Skip(16).Take(16).ToArray(),
                      iv_key.Take(16).ToArray());

                    rtnfile.Encryption.dk3 = dk3;
                    rtnfile.Encryption.iv = iv_key;
                    rtnfile.Encryption.imageKeyDecrypted = imageKeyDecrypted;

                    var ekpfs = Crypto.RSA2048Decrypt(imageKeyDecrypted, RSAKeyset.FakeKeyset);
                    rtnfile.Ekpfs = ekpfs;
                    var package = GameArchives.PackageReader.ReadPackageFromFile(pkgfileloc, new string(ekpfs.Select(b => (char)b).ToArray()));
                    rtnfile.PackageInside = package;
                    var innerPfs = GameArchives.PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
                    rtnfile.InnerPFS = innerPfs;
                }
                catch (Exception)
                {
                }
                //var sfoEditor = new SFOView(pkginside.ParamSfo.ParamSfo, true);
                //        var pkggame = GameArchives.Util.LocalFile(pkgFile);
                //        using (var stream = pkggame.GetStream())
                //            pkg = new PS4_Tools.LibOrbis.PKG.PkgReader(stream).ReadPkg();
                //        //PS4_Tools.LibOrbis.PKG.
                //        //GameArchives.PackageReader.ReadPackageFromFile(pkg);
                //        try
                //        {
                //            //var pkggame = GameArchives.Util.LocalFile(pkgFile);
                //            var package = PackageReader.ReadPackageFromFile(pkggame);
                //            var sfo = PackageReader.ReadPackageFromFile(package.GetFile("/param.sfo"));
                //            var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
                //            PackageManager.GetInstance();
                //        }
                //        catch (Exception ex)
                //        {

                //        }

                //        foreach (var e in pkg.Metas.Metas)
                //        {
                //            var lvi = new ListViewItem(new[] {
                //  e.id.ToString(),
                //  string.Format("0x{0:X}", e.DataSize),
                //  string.Format("0x{0:X}", e.DataOffset),
                //  e.Encrypted ? "Yes" : "No",
                //  e.KeyIndex.ToString(),
                //});
                //            lvi.Tag = e;
                //            //entriesListView.Items.Add(lvi);
                //            lst.Add(lvi);
                //        }

                return rtnfile;

            }
#endif
            /// <summary>
            /// This one is pretty straight Forward it renames a pkg file to the content id name
            /// </summary>
            /// <param name="pkgfile"></param>
            public static void Rename_pkg_To_ContentID(string pkgfile, string outputpkgfolder, bool overwrite = false)
            {
                Unprotected_PKG ps4pkg = Read_PKG(pkgfile);
                //get paramsfo 
                Param_SFO.PARAM_SFO psfo = ps4pkg.Param;
                string FolderName = new FileInfo(pkgfile).DirectoryName;
                string FileName = new FileInfo(pkgfile).Name;

                string outputfile = outputpkgfolder + psfo.ContentID + ".pkg";

                if (!File.Exists(outputfile) && overwrite == false)
                {
                    File.Copy(pkgfile, outputfile, overwrite);
                }
                else
                {
                    File.Copy(pkgfile, outputfile, overwrite);
                }


            }

            /// <summary>
            /// This one is pretty straight Forward it renames a pkg file to the Title Of The Package
            /// </summary>
            /// <param name="pkgfile"></param>
            public static void Rename_pkg_To_Title(string pkgfile, string outputpkgfolder, bool overwrite = false)
            {
                Unprotected_PKG ps4pkg = Read_PKG(pkgfile);
                //get paramsfo 
                Param_SFO.PARAM_SFO psfo = ps4pkg.Param;
                string FolderName = new FileInfo(pkgfile).DirectoryName;
                string FileName = new FileInfo(pkgfile).Name;

                string outputfile = outputpkgfolder + psfo.Title + ".pkg";

                if (!File.Exists(outputfile) && overwrite == false)
                {
                    File.Copy(pkgfile, RemoveSpecialCharacters(outputfile), overwrite);
                }
                else
                {
                    File.Copy(pkgfile, RemoveSpecialCharacters(outputfile), overwrite);
                }
            }
            public static string RemoveSpecialCharacters(string str)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            private static List<Param_SFO.PARAM_SFO.Table> AddNewItem(int Index, string Name, string Value, Param_SFO.PARAM_SFO.FMT format, int lenght, int maxlength, List<Param_SFO.PARAM_SFO.Table> xtable)
            {
                Param_SFO.PARAM_SFO.index_table indextable = new Param_SFO.PARAM_SFO.index_table();

                Param_SFO.PARAM_SFO.Table tableitem = new Param_SFO.PARAM_SFO.Table();

                indextable.param_data_fmt = format;
                indextable.param_data_len = Convert.ToUInt32(lenght);
                indextable.param_data_max_len = Convert.ToUInt32(maxlength);
                tableitem.index = Index;
                tableitem.Indextable = indextable;
                tableitem.Name = Name;
                tableitem.Value = Value;
                xtable.Add(tableitem);

                return xtable;
            }


            private static Param_SFO.PARAM_SFO ConvertToParam(SfoEntry maxtronsfo)
            {
                Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO();

                for (int i = 0; i < maxtronsfo.ParamSfo.Values.Count; i++)
                {

                    Param_SFO.PARAM_SFO.FMT format = Param_SFO.PARAM_SFO.FMT.Utf8Null;
                    var item = maxtronsfo.ParamSfo.Values[i];
                    string value = "";
                    switch (item.Type)
                    {
                        case LibOrbis.SFO.SfoEntryType.Integer:
                            format = Param_SFO.PARAM_SFO.FMT.UINT32;
                            //value = Convert.ToInt32(Util.Utils.(item.ToString())).ToString();

                            break;
                        case LibOrbis.SFO.SfoEntryType.Utf8:
                            format = Param_SFO.PARAM_SFO.FMT.Utf8Null;
                            value = item.ToString();
                            break;
                        case LibOrbis.SFO.SfoEntryType.Utf8Special:
                            format = Param_SFO.PARAM_SFO.FMT.ASCII;
                            value = item.ToString();
                            break;
                    }

                    AddNewItem(i, maxtronsfo.ParamSfo.Values[i].Name, value, format, item.Length, item.MaxLength, sfo.Tables);
                }

                return sfo;
            }
        }

        #endregion << Scene Related >>

#if (!PS4_UNITY)
#if !Android_Mono

        /// <summary>
        /// Remastered support class can build psp hd's and ps2 classics
        /// </summary>
        public class Remastered
        {
            /// <summary>
            /// Class is used for PS2 Classic Building
            /// This class will repack PS2 Games Into PS4
            /// </summary>
            public class PS2_Classics
            {
                /// <summary>
                /// PS2 Classics Main Working Direcotry
                /// </summary>
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
                public void Create_Single_ISO_PKG(string PS2_ISO, string SaveFileLOcation, string Title, Bitmap Icon0 = null, string BackgroundLocation = "", string ContentID = "", string CustomGP4Location = "")
                {
                    try
                    {
                        //checks files that are required 
                        if (PS2_ISO == "")
                        {
                            throw new Exception("PS2 ISO is required");
                        }
                        if (SaveFileLOcation == "")
                        {
                            throw new Exception("SaveFileLOcation is required");
                        }
                        if (Title == "")
                        {
                            throw new Exception("Title is required");
                        }
                        if (Icon0 == null)
                        {
                            throw new Exception("Icon0 is required");
                        }
                        if (BackgroundLocation == null)
                        {
                            throw new Exception("BackgroundLocation is required");
                        }

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
                                    throw new Exception("Could not load PS2 ID");
                                }
                            }
                        }

                        #endregion << CNF Reader >>

                        #region << Set Up Working Directory >>

                        Console.WriteLine("Creating working directory");

                        if (Directory.Exists(Working_Dir + @"PS2Emu"))
                        {
                            PS4_Tools.DeleteDirectory(Working_Dir + @"PS2Emu");
                        }

                        if (!Directory.Exists(Working_Dir))
                        {
                            Directory.CreateDirectory(Working_Dir);
                            Console.WriteLine("Created " + Working_Dir);
                        }
                        if (!Directory.Exists(Working_Dir + @"\PS2Emu\"))
                        {
                            Directory.CreateDirectory(Working_Dir + @"\PS2Emu\");
                            Console.WriteLine("Created " + Working_Dir + @"\PS2Emu\");
                        }

                        System.IO.File.WriteAllBytes(Working_Dir + "PS2.zip", Properties.Resources.PS2);
                        Console.WriteLine("Writing " + Working_Dir + "PS2.zip");

                        Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(Working_Dir + "PS2.zip");
                        zip.ExtractAll(Working_Dir + @"\PS2Emu\");

                        //System.IO.Directory.Move(Working_Dir + @"\PS2Emu\PS2", Working_Dir + @"\PS2Emu\");
                        PS4_Tools.MoveDirectory(Working_Dir + @"\PS2Emu\PS2", Working_Dir + @"\PS2Emu\");
                        System.IO.File.WriteAllBytes(Working_Dir + @"\PS2Emu\sce_sys\" + "param.sfo", Properties.Resources.param);
                        Console.WriteLine("Writing " + Working_Dir + @"\PS2Emu\sce_sys\" + "param.sfo");

                        System.IO.File.WriteAllBytes(Working_Dir + "PS2Classics.gp4", Properties.Resources.PS2Classics);
                        Console.WriteLine("Writing " + Working_Dir + "PS2Classics.gp4");



                        #endregion << Set Up Wokring Directory >>

                        #region << Load and update gp4 and sfo Project >>
                        Console.WriteLine("Loading GP4 Project");
                        var project = SceneRelated.GP4.ReadGP4(Working_Dir + "PS2Classics.gp4");
                        Console.WriteLine("Loading SFO");
                        var sfo = new Param_SFO.PARAM_SFO(Working_Dir + @"\PS2Emu\sce_sys\" + "param.sfo");

                        if (ContentID == "")
                        {
                            Console.WriteLine("No Content ID Specified Building custom one");
                            //build custom content id
                            ContentID = "UP9000-" + sfo.TitleID.Trim() + "_00-" + PS2ID.Trim().Replace("_", "") + "0000001";

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
                        sfo.SaveSFO(sfo, Working_Dir + @"\PS2Emu\sce_sys\" + "param.sfo");//update sfo info
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
                        icon0.Save(Working_Dir + @"PS2Emu\sce_sys\icon0.png");

                        Bitmap bitmap = new Bitmap(BackgroundLocation);

                        Bitmap icon1 = ps4icon0.Create_PS4_Compatible_PNG(bitmap);
                        icon1.Save(Working_Dir + @"PS2Emu\sce_sys\pic1.png");

                        #endregion  << Save Image Files to corresponding locations and also change to correct format >>

                        #region << PS2 Config >>

                        Console.WriteLine("Creating Custom PS2 LUA And Config");
                        var textfile = File.ReadAllText(Working_Dir + @"PS2Emu\config-emu-ps4.txt");
                        if (textfile.Contains("--max-disc-num="))
                        {
                            //read the nesasary info
                            string Is = @"--max-disc-num=";

                            int start = textfile.ToString().IndexOf(Is) + Is.Length;
                            int end = start + 1;//cause we know its one char more
                            if (end > start)
                            {
                                string texttoreplace = textfile.ToString().Substring(start, end - start);
                                textfile = textfile.Replace(Is + texttoreplace, @"--max-disc-num=" + 1);//single iso
                            }
                        }

                        textfile = textfile.Replace(@"#--path-patches=""/app0/patches""", @"--path-patches=""/app0/patches""");//add patches
                        textfile = textfile.Replace(@"#--path-featuredata=""/app0/patches""", @"--path-featuredata=""/app0/patches""");//add featuredata
                        textfile = textfile.Replace(@"#--path-toolingscript=""/app0/patches""", @"--path-toolingscript=""/app0/patches""");//#--path-toolingscript=""/app0/patches"""
                        File.WriteAllText(Working_Dir + @"PS2Emu\config-emu-ps4.txt", textfile);

                        #endregion << Copy over the images >>

                        #region << PS2 ISO copy >>
                        Console.WriteLine("Moving ISO File This May Take Some Time");

                        File.Delete(Working_Dir + @"\PS2Emu\image\disc01.iso");
                        //CopyFileWithProgress(txtPath.Text.Trim(), AppCommonPath() + @"\PS2\image\disc01.iso");
                        string currentimage = PS2_ISO;

                        //Copy File 
                        File.Copy(currentimage, Working_Dir + @"\PS2Emu\image\disc" + String.Format("{0:D2}", 1) + ".iso", true);


                        //now build the ps4 pkg
                        //still needed is a way to include memory mapped files inside the ps4 
                        var gp4 = LibOrbis.GP4.Gp4Project.ReadFrom(new FileStream(Working_Dir + "PS2Classics.gp4", FileMode.Open, FileAccess.Read));

                        //LibOrbis.PKG.PkgBuilder builder = new PkgBuilder(PkgProperties.FromGp4(gp4, Working_Dir));
                        //LibOrbis.GP4.Gp4Project.WriteTo(gp4,new FileStream(SaveFileLOcation, FileMode.OpenOrCreate,FileAccess.ReadWrite));
                        //idk what the hell the issue is this works in the sce toolset
                        for (int i = 0; i < gp4.files.Items.Count; i++)
                        {
                            gp4.files.Items[i].OrigPath = Working_Dir + gp4.files.Items[i].OrigPath;
                        }

                        new PkgBuilder(PkgProperties.FromGp4(gp4, Working_Dir + "\\")).Write(SaveFileLOcation);


                        #endregion << PS2 ISO copy >>
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
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
                /*PSP Tools should work on this without to much of a hastle*/

                /// <summary>
                /// This will validate a UMD dump from the PSP 
                /// </summary>
                /// <param name="path"></param>
                public void ValidateUMD(string path)
                {

                }


                /*Most of the PBP Tools will need to be referenced for this to work 100%*/

            }
        }
#endif
#endif
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

        /// <summary>
        /// SLB2 Class to handle the PS4'S SLB2 container
        /// </summary>
        public class SLB2
        {
            #region Variables
            static long slb2BaseOffset = 0x200;
            static long containerSize;
            static long slb2Version;
            static long fileCount;
            static int blockSize = 512;
            static long containerCount;
            static int blockCountOffset = 0x20;
            static int byteCountOffset = 0x24;
            static int fileNameOffset = 0x30;
            static long byteCount;
            static long blockCount;
            static string fileName;
            #endregion Variables

            /// <summary>
            /// Reset the major Variables for the next SLB2 Container
            /// </summary>
            private static void ResetVars()
            {
                slb2BaseOffset = 0x200;
                blockCountOffset = 0x20;
                byteCountOffset = 0x24;
                fileNameOffset = 0x30;
            }

            /// <summary>
            /// Check SLB2 Magic of a SLB2 Container
            /// </summary>
            /// <param name="slb2">The SLB2 Container to check</param>
            /// <returns>True if the File Magic do match the SLB2 Magic</returns>
            public static bool CheckHeader(string slb2)
            {
                using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open)))
                {
                    byte[] buffer = new byte[4];
                    byte[] slb2Magic = new byte[5] { 0x53, 0x4C, 0x42, 0x32, 0x01, };
                    b.Read(buffer, 0, 4);
                    if (Util.Utils.CompareBytes(buffer, slb2Magic))
                    {
                        return true;
                    }
                    b.Close();
                }
                return false;
            }

            /// <summary>
            /// Read the TOC of the SLB2 Container
            /// </summary>
            /// <param name="slb2">The SLB2 Container to read the TOC from</param>
            internal static PlaystationUpdateFile Read(string slb2)
            {
                PlaystationUpdateFile file = new PlaystationUpdateFile();
                using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open)))
                {
                    byte[] bufferA = new byte[4];
                    byte[] bufferB = new byte[4];
                    byte[] bufferC = new byte[4];
                    byte[] bufferPup = new byte[20];
                    slb2Version = 0;
                    fileCount = 0;
                    containerCount = 0;
                    containerSize = 0;

                    file.Magic = b.ReadBytes(4);//should be SLB2
                    if (!Util.Utils.CompareBytes(file.Magic, new byte[] { 0x53, 0x4C, 0x42, 0x32 }))
                    {
                        throw new Exception("File is not a SLB2 container");
                    }

                    FileInfo fileInfo = new FileInfo(slb2);
                    containerSize = fileInfo.Length;
                    file.FileSize = containerSize;

                    b.BaseStream.Seek(0x04, SeekOrigin.Begin);
                    b.Read(bufferA, 0, 4);
                    slb2Version = Util.Utils.HexToDec(bufferA, "reverse");

                    file.Version = slb2Version;

                    b.BaseStream.Seek(0x0C, SeekOrigin.Begin);
                    b.Read(bufferB, 0, 4);
                    fileCount = Util.Utils.HexToDec(bufferB, "reverse");

                    file.File_Counter = fileCount;

                    b.BaseStream.Seek(0x10, SeekOrigin.Begin);
                    b.Read(bufferC, 0, 4);
                    containerCount = Util.Utils.HexToDec(bufferC, "reverse");

                    file.Container_Counter = containerCount;

                    //start reading from 20 
                    b.BaseStream.Seek(0x20, SeekOrigin.Begin);
                    for (int i = 0; i < fileCount; i++)
                    {
                        InnerPUP pup = new InnerPUP();
                        pup.OffsetOfDecryptedBlocks = b.ReadBytes(4);//read 4 bytes should state the start offset
                        pup.CryptContentSize = Util.Utils.HexToDec(b.ReadBytes(4), "reverse");
                        pup.Reserved = b.ReadBytes(8);
                        pup.CryptContentName = Encoding.ASCII.GetString(b.ReadBytes(14));
                        b.ReadBytes(18);//just skiep the next 4 bytes
                        file.ListOfInnerPup.Add(pup);
                    }

                    //now we can read the actual file info if we need
                    b.Close();
                }
                return file;
            }

            /// <summary>
            /// Get the Version of the SLB2 Container (Need's Read() to be called once before)
            /// </summary>
            /// <returns>The version of the SLB2 Container</returns>
            public static long GetVersion()
            {
                return slb2Version;
            }

            /// <summary>
            /// Get File Count of SLB2 Container (Need's Read() to be called once before)
            /// </summary>
            /// <returns>File Count of the input SLB2 Container</returns>
            public static long GetFileCount()
            {
                return fileCount;
            }

            /// <summary>
            /// Check the Size of the input SLB2 Container against the saved size in the header (Need's Read() to be called once before)
            /// </summary>
            /// <returns>True if the saved size in header do match the reall file size</returns>
            public static bool CheckSize()
            {
                if ((containerCount * blockSize) == containerSize)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Extract a SLB2 Container
            /// </summary>
            /// <param name="slb2">The SLB2 Container to use</param>
            /// <param name="path">The path where to save the extracted files</param>
            public static void Extract(string slb2, string path)
            {
                int flag = 0;
                ASCIIEncoding enc = new ASCIIEncoding();

                while (fileCount != 0)
                {
                    byte[] bufferA = new byte[4];
                    byte[] bufferB = new byte[4];
                    byte[] bufferC = new byte[16];

                    using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open, FileAccess.Read)))
                    {
                        b.BaseStream.Seek(blockCountOffset, SeekOrigin.Begin);
                        b.Read(bufferA, 0, 4);
                        b.BaseStream.Seek(byteCountOffset, SeekOrigin.Begin);
                        b.Read(bufferB, 0, 4);
                        b.BaseStream.Seek(fileNameOffset, SeekOrigin.Begin);
                        b.Read(bufferC, 0, 16);

                        b.Close();
                    }

                    blockCount = Util.Utils.HexToDec(bufferA, "reverse");
                    byteCount = Util.Utils.HexToDec(bufferB, "reverse");

                    for (int i = 15; i > 0; i--)
                    {
                        if (bufferC[i] != 00)
                        {
                            byte[] newByte = new byte[i + 1];
                            Array.Copy(bufferC, 0, newByte, 0, i + 1);
                            fileName = enc.GetString(newByte);
                            break;
                        }
                    }

                    if (blockCount > 1)
                    {
                        slb2BaseOffset = (blockCount * blockSize);
                    }

                    if (fileName == "C0000001")
                    {
                        if (!File.Exists(path + fileName + "_stage1.bin") == true)
                        {
                            File.Create(path + fileName + "_stage1.bin").Close();
                            flag = 1;
                        }
                        else
                        {
                            File.Create(path + fileName + "_stage2.bin").Close();
                            flag = 2;
                        }
                    }
                    else if (fileName == "C0008001")
                    {
                        if (!File.Exists(path + fileName + "_stage1.bin") == true)
                        {
                            File.Create(path + fileName + "_stage1.bin").Close();
                            flag = 1;
                        }
                        else
                        {
                            File.Create(path + fileName + "_stage2.bin").Close();
                            flag = 2;
                        }
                    }
                    else if (fileName == "C0010001" ||
                             fileName == "eap_kbl" ||
                             fileName == "C0018001" ||
                             fileName == "C0020001" ||
                             fileName == "C0028001")
                    {
                        File.Create(path + fileName + ".bin").Close();
                        flag = 3;
                    }
                    else
                    {
                        File.Create(path + fileName).Close();
                    }

                    if (flag == 1)
                    {
                        Util.Utils.ReadWriteData(slb2, (path + fileName + "_stage1.bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                    }
                    else if (flag == 2)
                    {
                        Util.Utils.ReadWriteData(slb2, (path + fileName + "_stage2.bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                    }
                    else if (flag == 3)
                    {
                        Util.Utils.ReadWriteData(slb2, (path + fileName + ".bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                    }
                    else
                    {
                        Util.Utils.ReadWriteData(slb2, (path + fileName), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                    }

                    flag = 0;
                    fileCount -= 1;
                    blockCountOffset += 0x30;
                    byteCountOffset += 0x30;
                    fileNameOffset += 0x30;
                }

                // Reseting the main vars to the standart sart value
                ResetVars();
            }
        }

        public PlaystationUpdateFile Read_Pup(string PUPFile)
        {
            return SLB2.Read(PUPFile);
        }

        /// <summary>
        /// Playstation Update File Holder
        /// </summary>
        public class PlaystationUpdateFile
        {
            /// <summary>
            /// Magic of file
            /// </summary>
            public byte[] Magic { get; set; }

            /// <summary>
            /// Version of file
            /// </summary>
            public long Version { get; set; }

            /// <summary>
            /// Unknown Padding ?
            /// </summary>
            public long File_Counter { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long Container_Counter { get; set; }
            /// <summary>
            /// Flag
            /// </summary>
            public byte[] BlockCounter { get; set; }

            /// <summary>
            /// Unknown Padding ?
            /// </summary>
            public byte[] Reserved { get; set; }

            /// <summary>
            /// Unknown Padding ?
            /// </summary>
            public byte[] DecryptedBlocks { get; set; }

            /// <summary>
            /// Lists all inner PUP files
            /// </summary>
            public List<InnerPUP> ListOfInnerPup = new List<InnerPUP>();

            /// <summary>
            /// File Container Size
            /// </summary>
            public long FileSize { get; set; }
        }

        /// <summary>
        /// Inner PUP
        /// </summary>
        public class InnerPUP
        {
            /// <summary>
            /// Offset of DerytpedBlocks
            /// </summary>
            public byte[] OffsetOfDecryptedBlocks { get; set; }

            /// <summary>
            /// Crypt Content Size
            /// </summary>
            public long CryptContentSize { get; set; }

            /// <summary>
            /// Crypt Content Name
            /// </summary>
            public string CryptContentName { get; set; }

            /// <summary>
            /// Reserveed
            /// </summary>
            public byte[] Reserved { get; set; }

            /// <summary>
            /// Inner Pup Magic
            /// </summary>
            public byte[] InnerPupMagic { get; set; }
        }


    }

    /// <summary>
    /// PS4 tools reserved tools class
    /// </summary>
    public class Tools
    {
        public enum File_Type
        {
            PS4_PKG,
            PS4_ICON,
            PS4_DDS,
            PS4_RIF,
            PS4_ACT,
            PARAM_SFO,
            PLAYGO,
            ATRAC9,
            UpdateFile,
            RCOFile,
            Unkown,
        }

        public static File_Type Get_PS4_File_Type(string FileLocation)
        {
            //Check PS4 File Type
            File_Type ps4type = File_Type.Unkown;
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(FileLocation)))
            {
                //Check FIle Info
                byte[] FileHeader = binaryReader.ReadBytes(4);//most of sony's magic's are 4 bytes
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x41, 0x43, 0x54, 0x00 }))/*ACT.DAT*/
                {
                    ps4type = File_Type.PS4_ACT;
                }
                if (Utils.CompareBytes(FileHeader, new byte[] { 0x52, 0x49, 0x46, 0x46 }))/*RIFF*/
                {
                    if (Path.GetExtension(FileLocation).ToUpper() == ".AT9")
                    {
                        ps4type = File_Type.ATRAC9;
                    }
                    else
                    {
                        ps4type = File_Type.PS4_RIF;
                    }
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x52, 0x49, 0x46, 0x00 }))/*RIF*/
                {
                    if (Path.GetExtension(FileLocation).ToUpper() == ".AT9")
                    {
                        ps4type = File_Type.ATRAC9;
                    }
                    else
                    {
                        ps4type = File_Type.PS4_RIF;
                    }
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x7F, 0x43, 0x4E, 0x54 }))/*PKG*/
                {
                    ps4type = File_Type.PS4_PKG;
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x00, 0x50, 0x53, 0x46 }))/*PARAM.SFO*/
                {
                    ps4type = File_Type.PARAM_SFO;
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x89, 0x50, 0x4E, 0x47 }))/*PNG*/ //89 50 4E 47
                {
                    ps4type = File_Type.PS4_ICON;
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x44, 0x44, 0x53, 0x20 }))/*DDS*/ //44 44 53 20
                {
                    ps4type = File_Type.PS4_DDS;
                }
                if (Util.Utils.CompareBytes(FileHeader, new byte[] { 0x70, 0x6C, 0x67, 0x6F }))/*Play Go*/ //70 6C 67 6F
                {
                    ps4type = File_Type.PLAYGO;
                }
                if (Utils.CompareBytes(FileHeader, new byte[] { 0x53, 0x4C, 0x42, 0x32, }))//SLB2
                {
                    ps4type = File_Type.UpdateFile;
                }
            }


            return ps4type;
        }
    }
}