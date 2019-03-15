using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PS4_Tools.LibOrbis.Util
{
    public class ReaderBase
    {
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        private unsafe struct storage
        {
            [FieldOffset(0)]
            public byte u8;
            [FieldOffset(0)]
            public sbyte s8;
            [FieldOffset(0)]
            public ushort u16;
            [FieldOffset(0)]
            public short s16;
            [FieldOffset(0)]
            public uint u32;
            [FieldOffset(0)]
            public int s32;
            [FieldOffset(0)]
            public ulong u64;
            [FieldOffset(0)]
            public long s64;
            [FieldOffset(0)]
            public float f32;
            [FieldOffset(0)]
            public double f64;
            [FieldOffset(0)]
              public fixed byte buf[8];
        }

        private storage buffer;
        protected Stream s;
        protected bool flipEndian;
        protected ReaderBase(bool flipEndian, Stream stream)
        {
            s = stream;
            this.flipEndian = flipEndian;
        }
        private storage ReadEndian(int count)
        {
            unsafe
            {
                fixed (byte* bytePtr = buffer.buf)
                    if (flipEndian)
                        for (int i = count - 1; i >= 0; i--)
                        {


                            bytePtr[i] = (byte)s.ReadByte();
                        }
                    else

                        for (int i = 0; i < count; i++)
                        {


                            bytePtr[i] = (byte)s.ReadByte();
                        }
            }
            return buffer;
        }
        protected byte Byte() => ReadEndian(1).u8;
        protected sbyte SByte() => ReadEndian(1).s8;
        protected ushort UShort() => ReadEndian(2).u16;
        protected short Short() => ReadEndian(2).s16;
        protected uint UInt() => ReadEndian(4).u32;
        protected int Int() => ReadEndian(4).s32;
        protected ulong ULong() => ReadEndian(8).u64;
        protected long Long() => ReadEndian(8).s64;
        //protected unsafe void ReadBytes(byte* arr, int count)
        //{
        //  for (var i = 0; i < count; i++)
        //  {
        //    arr[i] = (byte)s.ReadByte();
        //  }
        //}
        protected byte[] ReadBytes(int count)
        {
            var ret = new byte[count];
            s.Read(ret, 0, count);
            return ret;
        }
    }
}
