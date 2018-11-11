using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.CompilerServices;

namespace PS4_Tools.Util
{
    public class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] Hex2Binary(string hex)
        {
            var chars = hex.ToCharArray();
            var bytes = new List<byte>();
            for (int index = 0; index < chars.Length; index += 2)
            {
                var chunk = new string(chars, index, 2);
                bytes.Add(byte.Parse(chunk, NumberStyles.AllowHexSpecifier));
            }
            return bytes.ToArray();
        }

        public static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }


        /*converts byte to encrypted string*/
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /*VB Basics Method not needed as c# has this function already*/
        public static uint ReadUInt32(object stream)
        {
            byte[] array = new byte[4];
            Type type = null;
            string memberName = "Read";
            object[] array2 = new object[]
            {
                array,
                0,
                4
            };
            object[] arguments = array2;
            string[] argumentNames = null;
            Type[] typeArguments = null;
            bool[] array3 = new bool[]
            {
                true,
                false,
                false
            };
            NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            if (array3[0])
            {
                array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            }
            Array.Reverse(array, 0, 4);
            return BitConverter.ToUInt32(array, 0);
        }
        
        public static ushort ReadUInt16(object stream)
        {
            byte[] array = new byte[4];
            Type type = null;
            string memberName = "Read";
            object[] array2 = new object[]
            {
                array,
                0,
                2
            };
            object[] arguments = array2;
            string[] argumentNames = null;
            Type[] typeArguments = null;
            bool[] array3 = new bool[]
            {
                true,
                false,
                false
            };
            NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            if (array3[0])
            {
                array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            }
            Array.Reverse(array, 0, 2);
            return BitConverter.ToUInt16(array, 0);
        }

        public static string ReadASCIIString(object stream, int legth)
        {
            byte[] array = new byte[checked(legth - 1 + 1)];
            Type type = null;
            string memberName = "Read";
            object[] array2 = new object[]
            {
                array,
                0,
                array.Length
            };
            object[] arguments = array2;
            string[] argumentNames = null;
            Type[] typeArguments = null;
            bool[] array3 = new bool[]
            {
                true,
                false,
                false
            };
            NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            if (array3[0])
            {
                array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            }
            return Encoding.ASCII.GetString(array);
        }

       public static byte[] ReadByte(object stream, int legth)
        {
            byte[] array = new byte[checked(legth - 1 + 1)];
            Type type = null;
            string memberName = "Read";
            object[] array2 = new object[]
            {
                array,
                0,
                array.Length
            };
            object[] arguments = array2;
            string[] argumentNames = null;
            Type[] typeArguments = null;
            bool[] array3 = new bool[]
            {
                true,
                false,
                false
            };
            NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            if (array3[0])
            {
                array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            }
            return array;
        }

         public static System.Drawing.Bitmap BytesToBitmap(byte[] ImgBytes)
        {
            System.Drawing.Bitmap result = null;
            if (ImgBytes != null)
            {
                MemoryStream stream = new MemoryStream(ImgBytes);
                result = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            }
            return result;
        }

        public static bool isLinux()
        {
            return Operators.CompareString(Conversions.ToString(Path.DirectorySeparatorChar), "/", false) == 0;
        }

        public static bool Contain(byte[] a, byte[] b)
        {
            checked
            {
                if (a != null)
                {
                    if (b != null)
                    {
                        if (a.Length > 0 && b.Length > 0)
                        {
                            if (a.Length != b.Length)
                            {
                                return false;
                            }
                            int num = 0;
                            int num2 = a.Length - 1;
                            for (int i = num; i <= num2; i++)
                            {
                                if (a[i] != b[i])
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
        }

         public static string HexToString(string hex)
        {
            StringBuilder stringBuilder = new StringBuilder(hex.Length / 2);
            int num = 0;
            checked
            {
                int num2 = hex.Length - 2;
                for (int i = num; i <= num2; i += 2)
                {
                    stringBuilder.Append(Strings.Chr((int)Convert.ToByte(hex.Substring(i, 2), 16)));
                }
                return stringBuilder.ToString();
            }
        }

        public static long byteArrayToLittleEndianInteger(byte[] bits)
        {
            return (long)((ulong)(bits[0] | (byte)(bits[1] << 0) | (byte)(bits[2] << 0) | (byte)(bits[3] << 0)));
        }

        public static bool ByteArraysEqual(byte[] first, byte[] second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            int num = 0;
            checked
            {
                int num2 = first.Length - 1;
                for (int i = num; i <= num2; i++)
                {
                    if (first[i] != second[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static string byteArrayToUTF8String(byte[] _byte)
        {
            return Encoding.UTF8.GetString(_byte);
        }

        public static string byteArrayToHexString(byte[] bytes_Input)
        {
            StringBuilder stringBuilder = new StringBuilder(checked(bytes_Input.Length * 2));
            foreach (byte number in bytes_Input)
            {
                if (Conversion.Hex(number).Length == 1)
                {
                    stringBuilder.Append("0" + Conversion.Hex(number));
                }
                else
                {
                    stringBuilder.Append("" + Conversion.Hex(number));
                }
            }
            return stringBuilder.ToString();
        }

        public static long hexStringToLong(string strHex)
        {
            checked
            {
                long result;
                try
                {
                    result = (long)Math.Round(Conversion.Val("&H" + strHex + "&"));
                }
                catch (Exception ex)
                {
                    result = (long)Math.Round(Conversion.Val("&H" + strHex));
                }
                return result;
            }
        }
    }
}
