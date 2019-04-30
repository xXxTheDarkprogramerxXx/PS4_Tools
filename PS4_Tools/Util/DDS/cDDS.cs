using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using PS4_Tools.LibOrbis.Util;
using SquishNET;

namespace PS4_Tools.Util.DDS
{
    public enum D3DFormat : uint
    {
        // Historic (occur in x360 files)
        X360_A8R8G8B8 = 6,
        X360_DXT1 = 18,
        X360_DXT2 = 19,

        A8R8G8B8 = 21,
        A8 = 28,
        ATI2 = 0x32495441,  // MakeFourCC('A', 'T', 'I', '2')
        DXT1 = 0x31545844,  // MakeFourCC('D', 'X', 'T', '1')
        DXT3 = 0x33545844,  // MakeFourCC('D', 'X', 'T', '3')
        DXT5 = 0x35545844,   // MakeFourCC('D', 'X', 'T', '5')
        DDS = 0x44445320 // D D S 
    }

    public abstract class Texture
    {
        string name;
        protected string extension;
        D3DFormat format;
        protected List<MipMap> mipMaps = new List<MipMap>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual D3DFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        public string ShortFormat
        {
            get
            {
                switch (format)
                {
                    case D3DFormat.A8R8G8B8:
                        return "8888";

                    default:
                        return format.ToString().Substring(0, 4);
                }
            }
        }

        public List<MipMap> MipMaps => mipMaps;
    }

    public class MipMap
    {
        int width;
        int height;
        byte[] data;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
    }


        public enum PixelFormatFourCC
    {
        DXT1 = 0x31545844,  // MakeFourCC('D', 'X', 'T', '1')
        DXT3 = 0x33545844,  // MakeFourCC('D', 'X', 'T', '3')
        DXT5 = 0x35545844   // MakeFourCC('D', 'X', 'T', '5')
    }

    [Flags]
    public enum PixelFormatFlags : uint
    {
        DDPF_ALPHAPIXELS = 0x1,
        DDPF_ALPHA = 0x2,
        DDPF_FOURCC = 0x4,
        DDPF_RGB = 0x40,
        DDPF_YUV = 0x200,
        DDPF_LUMINANCE = 0x20000
    }

    [Flags]
    public enum DDSCaps : uint
    {
        DDSCAPS_COMPLEX = 0x8,
        DDSCAPS_TEXTURE = 0x1000,
        DDSCAPS_MIPMAP = 0x400000
    }

    [Flags]
    public enum DDSCaps2 : uint
    {
        DDSCAPS2_CUBEMAP = 0x200,
        DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
        DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
        DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
        DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
        DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
        DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
        DDSCAPS2_VOLUME = 0x200000
    }

    public class DDSPixelFormat
    {
        PixelFormatFlags flags;
        PixelFormatFourCC fourCC;
        int rgbBitCount;
        uint rBitMask;
        uint gBitMask;
        uint bBitMask;
        uint aBitMask;

        public PixelFormatFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public PixelFormatFourCC FourCC
        {
            get { return fourCC; }
            set { fourCC = value; }
        }

        public int RGBBitCount
        {
            get { return rgbBitCount; }
            set { rgbBitCount = value; }
        }

        public uint RBitMask
        {
            get { return rBitMask; }
            set { rBitMask = value; }
        }

        public uint GBitMask
        {
            get { return gBitMask; }
            set { gBitMask = value; }
        }

        public uint BBitMask
        {
            get { return bBitMask; }
            set { bBitMask = value; }
        }

        public uint ABitMask
        {
            get { return aBitMask; }
            set { aBitMask = value; }
        }
    }

        public class DDS : Texture
        {
            [Flags]
            public enum Flags
            {
                Caps = 1,
                Height = 2,
                Width = 4,
                Pitch = 8,
                PixelFormat = 4096,
                MipMapCount = 131072,
                LinearSize = 524288,
                DepthTexture = 8388608
            }

            Flags flags;
            int height;
            int width;
            int pitch;
            int depth = 0;
            DDSPixelFormat pixelFormat;
            DDSCaps caps;
            DDSCaps2 caps2;

            D3DFormat format;

            public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
            {
                 get { return width; }
                set { width = value; }
                }

        public int Depth => depth;

            public DDS() { }

        public DDS(D3DFormat Format, Bitmap bitmap)
        {
            SquishFlags flags = SquishFlags.Dxt1;
            bool bCompressed = true;

            switch (Format)
            {
                case D3DFormat.DXT1:
                    flags = SquishFlags.Dxt1;
                    break;

                case D3DFormat.DXT3:
                    flags = SquishFlags.Dxt3;
                    break;

                case D3DFormat.DXT5:
                    flags = SquishFlags.Dxt5;
                    break;

                default:
                    bCompressed = false;
                    break;
            }

            format = Format;
            width = bitmap.Width;
            height = bitmap.Height;

            MipMap mip = new MipMap()
            {
                Width = width,
                Height = height
            };

            byte[] data = new byte[mip.Width * mip.Height * 4];

            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, mip.Width, mip.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpdata.Scan0, data, 0, bmpdata.Stride * bmpdata.Height);
            bitmap.UnlockBits(bmpdata);

            if (bCompressed)
            {
                for (uint i = 0; i < data.Length - 4; i += 4)
                {
                    byte r = data[i + 0];
                    data[i + 0] = data[i + 2];
                    data[i + 2] = r;
                }

                //byte[] dest = new byte[Squish.GetStorageRequirements(mip.Width, mip.Height, flags | SquishFlags.ColourIterativeClusterFit)];
                //dest = Squish.CompressImage(data, mip.Width, mip.Height, flags | SquishFlags.ColourIterativeClusterFit);
                //mip.Data = dest;
            }
            else
            {
                mip.Data = data;
            }

            mipMaps.Add(mip);

        }


        public static DDS Load(string path)
            {
                return DDS.Load(File.ReadAllBytes(path));
            }

            public static DDS Load(byte[] data)
            {
                DDS dds = new DDS();

                using (MemoryStream ms = new MemoryStream(data))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    if (!IsDDS(br)) { return null; }

                    dds.pixelFormat = new DDSPixelFormat();

                    br.ReadUInt32();    // header length
                    dds.flags = (Flags)br.ReadUInt32();
                    dds.height = (int)br.ReadUInt32();
                    dds.width = (int)br.ReadUInt32();
                    dds.pitch = (int)br.ReadUInt32();
                    dds.depth = (int)br.ReadUInt32();
                    int mipCount = (int)br.ReadUInt32();
                    for (int i = 0; i < 11; i++) { br.ReadUInt32(); }
                    br.ReadUInt32();    // pixel format length
                    dds.pixelFormat.Flags = (PixelFormatFlags)br.ReadUInt32();
                    dds.pixelFormat.FourCC = (PixelFormatFourCC)br.ReadUInt32();
                    dds.pixelFormat.RGBBitCount = (int)br.ReadUInt32();
                    dds.pixelFormat.RBitMask = br.ReadUInt32();
                    dds.pixelFormat.GBitMask = br.ReadUInt32();
                    dds.pixelFormat.BBitMask = br.ReadUInt32();
                    dds.pixelFormat.ABitMask = br.ReadUInt32();
                    dds.caps = (DDSCaps)br.ReadUInt32();
                    dds.caps2 = (DDSCaps2)br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();

                    if (dds.pixelFormat.Flags.HasFlag(PixelFormatFlags.DDPF_FOURCC))
                    {
                        dds.format = (D3DFormat)dds.pixelFormat.FourCC;
                    }
                    else if (dds.pixelFormat.Flags.HasFlag(PixelFormatFlags.DDPF_RGB) & dds.pixelFormat.Flags.HasFlag(PixelFormatFlags.DDPF_ALPHAPIXELS))
                    {
                        dds.format = D3DFormat.A8R8G8B8;
                    }

                    for (int i = 0; i < Math.Max(1, mipCount); i++)
                    {
                        MipMap mip = new MipMap
                        {
                            Width = dds.width >> i,
                            Height = dds.height >> i
                        };

                        switch (dds.format)
                        {
                            case D3DFormat.A8R8G8B8:
                                mip.Data = br.ReadBytes(mip.Width * mip.Height * 4);
                                break;

                            case D3DFormat.DXT1:
                                mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 8);
                                break;

                            case D3DFormat.DXT3:
                            case D3DFormat.DXT5:
                                mip.Data = br.ReadBytes((((mip.Width + 3) / 4) * ((mip.Height + 3) / 4)) * 16);
                                break;
                        }

                        dds.mipMaps.Add(mip);
                    }
                }

                return dds;
            }

            public static bool IsDDS(BinaryReader br)
            {
                return (
                    br.ReadByte() == 0x44 && // D
                    br.ReadByte() == 0x44 && // D
                    br.ReadByte() == 0x53 && // S
                    br.ReadByte() == 0x20    //  
                );
            }

            public void Save(string path)
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(path , FileMode.Create)))
                {
                    Save(bw, this);
                }
            }

            public static void Save(BinaryWriter bw, DDS dds)
            {
                Flags flags = (Flags.Caps | Flags.Height | Flags.Width | Flags.PixelFormat | Flags.MipMapCount);
                flags |= (dds.format == D3DFormat.A8R8G8B8 ? Flags.Pitch : Flags.LinearSize);

                bw.Write(new byte[] { 0x44, 0x44, 0x53, 0x20 });    // 'DDS '
                bw.Write(124);
                bw.Write((int)flags);
                bw.Write(dds.Height);
                bw.Write(dds.Width);
                bw.Write((flags.HasFlag(Flags.Pitch) ? dds.width * 4 : dds.MipMaps[0].Data.Length));
                bw.Write(dds.Depth);
                bw.Write(dds.MipMaps.Count);

                for (int i = 0; i < 11; i++) { bw.Write(0); }

                // PixelFormat
                bw.Write(32);

                switch (dds.Format)
                {
                    case D3DFormat.DXT1:
                    case D3DFormat.DXT3:
                    case D3DFormat.DXT5:
                        bw.Write(4);        // fourCC length
                        bw.Write(dds.Format.ToString().ToCharArray());
                        bw.Write(0);
                        bw.Write(0);
                        bw.Write(0);
                        bw.Write(0);
                        bw.Write(0);
                        break;

                    default:
                        bw.Write(0);    // fourCC length
                        bw.Write(0);
                        bw.Write(32);   //  RGB bit count
                        bw.Write(255 << 16);    // R mask
                        bw.Write(255 << 8);     // G mask
                        bw.Write(255 << 0);     // B mask
                        bw.Write(255 << 24);    // A mask
                        break;
                }

                bw.Write((int)DDSCaps.DDSCAPS_TEXTURE);
                bw.Write(0);    // Caps 2
                bw.Write(0);    // Caps 3
                bw.Write(0);    // Caps 4
                bw.Write(0);    // Reserved

                for (int i = 0; i < dds.mipMaps.Count; i++)
                {
                    bw.Write(dds.mipMaps[i].Data);
                }
            }

            public Bitmap Decompress(int mipLevel = 0, bool bSuppressAlpha = false)
            {
                MipMap mip = MipMaps[mipLevel];

                Bitmap b = new Bitmap(mip.Width, mip.Height, PixelFormat.Format32bppArgb);
                //SquishFlags flags = 0;
                bool bNotCompressed = false;

                switch (format)
                {
                    case D3DFormat.DXT1:
                        //flags = SquishFlags.Dxt1;
                        break;

                    case D3DFormat.DXT5:
                        //flags = SquishFlags.Dxt5;
                        break;

                    case D3DFormat.A8R8G8B8:
                        bNotCompressed = true;
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Can't decompress: {0}", format));
                }

                byte[] dest = new byte[mip.Width * mip.Height * 4];
                byte[] data = mip.Data;

                if (bNotCompressed)
                {
                    for (uint i = 0; i < data.Length - 4; i += 4)
                    {
                        uint colour = (uint)((data[i + 3] << 24) | (data[i + 2] << 16) | (data[i + 1] << 8) | (data[i + 0] << 0));

                        dest[i + 0] = (byte)((colour & pixelFormat.BBitMask) >> 0);
                        dest[i + 1] = (byte)((colour & pixelFormat.GBitMask) >> 8);
                        dest[i + 2] = (byte)((colour & pixelFormat.RBitMask) >> 16);
                        dest[i + 3] = (byte)((colour & pixelFormat.ABitMask) >> 24);
                    }
                }
                else
                {
                    //data = Squish.DecompressImage(dest, mip.Width, mip.Height, flags);

                    //for (uint i = 0; i < dest.Length - 4; i += 4)
                    //{
                    //    byte r = dest[i + 0];
                    //    dest[i + 0] = dest[i + 2];
                    //    dest[i + 2] = r;
                    //}
                }

                BitmapData bmpdata = b.LockBits(new Rectangle(0, 0, mip.Width, mip.Height), ImageLockMode.ReadWrite, (bSuppressAlpha ? PixelFormat.Format32bppRgb : b.PixelFormat));
                Marshal.Copy(dest, 0, bmpdata.Scan0, dest.Length);
                b.UnlockBits(bmpdata);

                return b;
            }
        }
    }