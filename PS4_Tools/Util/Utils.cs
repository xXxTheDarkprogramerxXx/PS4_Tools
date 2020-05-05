using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using Ionic.Zip;
using System.Drawing.Imaging;
using System.Linq;
using static DDSReader.DDSImage;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace PS4_Tools.Util
{
    internal class Utils
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

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets a filesize in a human readble format
        /// </summary>
        /// <param name="FileSize"></param>
        /// <returns></returns>
        public static string GetHumanReadable(double FileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = FileSize;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

        /// <summary>
        /// Converts a Littel Endian Hex Decimal value to a Integer Decimal value
        /// </summary>
        /// <param name="Hex">The byte[] Array to convert from</param>
        /// <param name="reverse">Defines if a array is Little Endian and should be reversed first</param>
        /// <returns>The converted Integer Decimal value</returns>
        public static long HexToDec(byte[] Hex, string reverse = "")
        {
            if (reverse == "reverse")
            {
                Array.Reverse(Hex);
            }

            string bufferString = BitConverter.ToString(Hex).Replace("-", "");
            long bufferInteger = Convert.ToInt32(bufferString, 16);
            return bufferInteger;
        }

        /// <summary>
        /// Kombinated Command for Read or Write Binary or Integer Data
        /// </summary>
        /// <param name="fileToUse">The File that will be used to Read from or to Write to it</param>
        /// <param name="fileToUse2">This is used for the "both" methode. fileToUse will be the file to read from and fileToUse2 will be the file to write to it.</param>
        /// <param name="methodReadOrWriteOrBoth">Defination for Read "r" or Write "w" or if you have big data just use Both "b"</param>
        /// <param name="methodBinaryOrInteger">Defination for Binary Data (bi) or Integer Data (in) when write to a file</param>
        /// <param name="binData">byte array of the binary data to read or write</param>
        /// <param name="binData2">integer array of the integer data to read or write</param>
        /// <param name="offset">Otional, used for the "both" methode to deffine a offset to start to read from a file. If you do not wan't to read from the begin use this var to tell the Routine to jump to your deffined offset.</param>
        /// <param name="count">Optional, also used for the "both" methode to deffine to only to read a specific byte count and not till the end of the file.</param>
        public static void ReadWriteData(string fileToUse, string fileToUse2 = "", string methodReadOrWriteOrBoth = "", string methodBinaryOrInteger = "", byte[] binData = null, int binData2 = 0, long offset = 0, long count = 0)
        {
            byte[] readBuffer;
            string caseSwitch = methodReadOrWriteOrBoth;
            switch (caseSwitch)
            {
                case "r":
                    {
                        FileInfo fileInfo = new FileInfo(fileToUse);
                        readBuffer = new byte[fileInfo.Length];
                        using (BinaryReader b = new BinaryReader(new FileStream(fileToUse, FileMode.Open, FileAccess.Read)))
                        {
                            b.Read(readBuffer, 0, readBuffer.Length);
                            b.Close();
                        }
                    }
                    break;
                case "w":
                    {
                        using (BinaryWriter b = new BinaryWriter(new FileStream(fileToUse, FileMode.Append, FileAccess.Write)))
                        {
                            caseSwitch = methodBinaryOrInteger;
                            switch (caseSwitch)
                            {
                                case "bi":
                                    {
                                        b.Write(binData, 0, binData.Length);
                                        b.Close();
                                    }
                                    break;
                                case "in":
                                    {
                                        b.Write(binData2);
                                        b.Close();
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case "b":
                    {   // For data that will cause a buffer overflow we use this method. We read from a Input File and Write to a Output File with the help of a Buffer till the end of file or the specified length is reached.
                        using (BinaryReader br = new BinaryReader(new FileStream(fileToUse, FileMode.Open, FileAccess.Read)))
                        {
                            using (BinaryWriter bw = new BinaryWriter(new FileStream(fileToUse2, FileMode.Append, FileAccess.Write)))
                            {
                                // this is a variable for the Buffer size. Play arround with it and maybe set a new size to get better result's
                                int workingBufferSize = 4096; // high
                                // int workingBufferSize = 2048; // middle
                                // int workingBufferSize = 1024; // default
                                // int workingBufferSize = 128;  // minimum

                                // Do we read data that is smaller then our working buffer size?
                                if (count < workingBufferSize)
                                {
                                    workingBufferSize = (int)count;
                                }

                                byte[] buffer = new byte[workingBufferSize];
                                int len;

                                // Do we use a specific offset?
                                if (offset != 0)
                                {
                                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                                }

                                // Run the process in a loop
                                while ((len = br.Read(buffer, 0, workingBufferSize)) != 0)
                                {
                                    bw.Write(buffer, 0, len);

                                    // Do we read a specific length?
                                    if (count != 0)
                                    {
                                        // Subtract the working buffer size from the byte count to read/write.
                                        count -= workingBufferSize;

                                        // Stop the loop when the specified byte count to read/write is reached.
                                        if (count == 0)
                                        {
                                            break;
                                        }

                                        // When the count value is lower then the working buffer size we set the working buffer to the value of the count variable to not read more data as wanted
                                        if (count < workingBufferSize)
                                        {
                                            workingBufferSize = (int)count;
                                        }
                                    }
                                }
                                bw.Close();
                            }
                            br.Close();
                        }
                    }
                    break;
            }
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


        public static void ExtractFileToDirectory(string zipFileName, string outputDirectory)
        {
            ZipFile zip = ZipFile.Read(zipFileName);
            Directory.CreateDirectory(outputDirectory);
            foreach (ZipEntry e in zip)
            {
                // check if you want to extract e or not
                if (e.FileName == "TheFileToExtract")
                    e.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
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
        //public static uint NewReadUInt32(object stream)
        //{
        //    byte[] array = new byte[4];
        //    Type type = null;
        //    string memberName = "Read";
        //    object[] array2 = new object[]
        //    {
        //        array,
        //        0,
        //        4
        //    };
        //    object[] arguments = array2;
        //    string[] argumentNames = null;
        //    Type[] typeArguments = null;
        //    bool[] array3 = new bool[]
        //    {
        //        true,
        //        false,
        //        false
        //    };
        //    NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
        //    if (array3[0])
        //    {
        //        array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
        //    }
        //    Array.Reverse(array, 0, 4);
        //    return BitConverter.ToUInt32(array, 0);

        //    //string temp = "";
        //    //BinaryReader reader = (BinaryReader)stream;
        //    ////throw new Exception("sfasda");
        //    //return reader.ReadUInt32();
        //}

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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            //}

            array = ((BinaryReader)stream).ReadBytes(array.Length);

            Array.Reverse(array, 0, 4);
            return BitConverter.ToUInt32(array, 0);

            //string temp = "";
            //BinaryReader reader = (BinaryReader)stream;
            ////throw new Exception("sfasda");
            //return reader.ReadUInt32();
        }

        public static ulong ReadUInt64(object stream)
        {
            byte[] array = new byte[8];
            Type type = null;
            string memberName = "Read";
            object[] array2 = new object[]
            {
                array,
                0,
                8
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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            //}

            array = ((BinaryReader)stream).ReadBytes(array.Length);

            Array.Reverse(array, 0, 8);
            return BitConverter.ToUInt64(array, 0);

            //string temp = "";
            //BinaryReader reader = (BinaryReader)stream;
            ////throw new Exception("sfasda");
            //return reader.ReadUInt32();
        }

        public static ushort ReadUInt16(object stream)
        {
            byte[] array = new byte[2];
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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            //}
            array = ((BinaryReader)stream).ReadBytes(array.Length);
            Array.Reverse(array, 0, 2);
            return BitConverter.ToUInt16(array, 0);

            ////we need to make sure this all works 
            //string temp = "";
            //BinaryReader reader = (BinaryReader)stream;
            ////throw new Exception("sfasda");
            //return reader.ReadUInt16();
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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            //}

            array = ((BinaryReader)stream).ReadBytes(array.Length);
            return Encoding.ASCII.GetString(array);
        }

        public static string ReadUTF8String(object stream, int legth)
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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));
            //}

            array = ((BinaryReader)stream).ReadBytes(array.Length);
            return Encoding.UTF8.GetString(array);
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
            //NewLateBinding.LateCall(stream, type, memberName, arguments, argumentNames, typeArguments, array3, true);
            //if (array3[0])
            //{
            //    array = (byte[])Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2[0]), typeof(byte[]));

            //}
            array = ((BinaryReader)stream).ReadBytes(array.Length);
            //File.WriteAllBytes(@"C:\Temp\Testing\tropy.trp", array);
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

        public static bool isLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
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
            //StringBuilder stringBuilder = new StringBuilder(hex.Length / 2);
            //int num = 0;
            //checked
            //{
            //    int num2 = hex.Length - 2;
            //    for (int i = num; i <= num2; i += 2)
            //    {
            //        stringBuilder.Append(Strings.Chr((int)Convert.ToByte(hex.Substring(i, 2), 16)));
            //    }
            //    return stringBuilder.ToString();
            //}
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return Encoding.ASCII.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"

        }

        public static string Hex(byte Byte)
        {
            return Byte.ToString("X");
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
                if (Util.Utils.Hex(number).Length == 1)
                {
                    stringBuilder.Append("0" + Util.Utils.Hex(number));
                }
                else
                {
                    stringBuilder.Append("" + Util.Utils.Hex(number));
                }
            }
            return stringBuilder.ToString();
            //StringBuilder hex = new StringBuilder(bytes_Input.Length * 2);

            //foreach (byte b in bytes_Input)
            //{
            //    hex.AppendFormat("{0:x2}", b);
            //}
            //return hex.ToString();
        }

        public static long hexStringToLong(string strHex)
        {
            checked
            {
                return (long)HexLiteral2Unsigned(strHex);
            }
        }

        public static ulong HexLiteral2Unsigned(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentException("hex");

            int i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X') ? 2 : 0;
            ulong value = 0;

            while (i < hex.Length)
            {
                uint x = hex[i++];

                if (x >= '0' && x <= '9') x = x - '0';
                else if (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else throw new ArgumentOutOfRangeException("hex");

                value = 16 * value + x;

            }

            return value;
        }


        public static T CreateJaggedArray<T>(params int[] lengths)
        {
            return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
        }
        private static object InitializeJaggedArray(Type type, int index, int[] lengths)
        {
            Array array = Array.CreateInstance(type, lengths[index]);

            Type elementType = type.GetElementType();
            if (elementType == null) return array;

            for (int i = 0; i < lengths[index]; i++)
            {
                array.SetValue(InitializeJaggedArray(elementType, index + 1, lengths), i);
            }

            return array;
        }

        [MethodImpl(256)]
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        [MethodImpl(256)]
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        [MethodImpl(256)]
        public static short Clamp16(int value)
        {
            if (value > short.MaxValue)
                return short.MaxValue;
            if (value < short.MinValue)
                return short.MinValue;
            return (short)value;
        }

        public static sbyte Clamp4(int value)
        {
            if (value > 7)
                return 7;
            if (value < -8)
                return -8;
            return (sbyte)value;
        }


    }

    /// <summary>
    ///     Taken from System.Net in 4.0, useful until we move to .NET 4.0 - needed for Client Profile
    /// </summary>
    internal static class WebUtility
    {
        // Fields
        private static char[] _htmlEntityEndingChars = new char[] { ';', '&' };

        // Methods
        public static string HtmlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            if (value.IndexOf('&') < 0)
            {
                return value;
            }
            StringWriter output = new StringWriter(CultureInfo.InvariantCulture);
            HtmlDecode(value, output);
            return output.ToString();
        }

        public static void HtmlDecode(string value, TextWriter output)
        {
            if (value != null)
            {
                if (output == null)
                {
                    throw new ArgumentNullException("output");
                }
                if (value.IndexOf('&') < 0)
                {
                    output.Write(value);
                }
                else
                {
                    int length = value.Length;
                    for (int i = 0; i < length; i++)
                    {
                        char ch = value[i];
                        if (ch == '&')
                        {
                            int num3 = value.IndexOfAny(_htmlEntityEndingChars, i + 1);
                            if ((num3 > 0) && (value[num3] == ';'))
                            {
                                string entity = value.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#'))
                                {
                                    ushort num4;
                                    if ((entity[1] == 'x') || (entity[1] == 'X'))
                                    {
                                        ushort.TryParse(entity.Substring(2), NumberStyles.AllowHexSpecifier, (IFormatProvider)NumberFormatInfo.InvariantInfo, out num4);
                                    }
                                    else
                                    {
                                        ushort.TryParse(entity.Substring(1), NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out num4);
                                    }
                                    if (num4 != 0)
                                    {
                                        ch = (char)num4;
                                        i = num3;
                                    }
                                }
                                else
                                {
                                    i = num3;
                                    char ch2 = HtmlEntities.Lookup(entity);
                                    if (ch2 != '\0')
                                    {
                                        ch = ch2;
                                    }
                                    else
                                    {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        goto Label_0117;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                        Label_0117:;
                    }
                }
            }
        }

        public static string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            if (IndexOfHtmlEncodingChars(value, 0) == -1)
            {
                return value;
            }
            StringWriter output = new StringWriter(CultureInfo.InvariantCulture);
            HtmlEncode(value, output);
            return output.ToString();
        }

        public static unsafe void HtmlEncode(string value, TextWriter output)
        {
            if (value != null)
            {
                if (output == null)
                {
                    throw new ArgumentNullException("output");
                }
                int num = IndexOfHtmlEncodingChars(value, 0);
                if (num == -1)
                {
                    output.Write(value);
                }
                else
                {
                    int num2 = value.Length - num;
                    fixed (char* str = value)
                    {
                        char* chPtr = str;
                        char* chPtr2 = chPtr;
                        while (num-- > 0)
                        {
                            chPtr2++;
                            output.Write(chPtr2[0]);
                        }
                        while (num2-- > 0)
                        {
                            chPtr2++;
                            char ch = chPtr2[0];
                            if (ch <= '>')
                            {
                                switch (ch)
                                {
                                    case '&':
                                        {
                                            output.Write("&amp;");
                                            continue;
                                        }
                                    case '\'':
                                        {
                                            output.Write("&#39;");
                                            continue;
                                        }
                                    case '"':
                                        {
                                            output.Write("&quot;");
                                            continue;
                                        }
                                    case '<':
                                        {
                                            output.Write("&lt;");
                                            continue;
                                        }
                                    case '>':
                                        {
                                            output.Write("&gt;");
                                            continue;
                                        }
                                }
                                output.Write(ch);
                                continue;
                            }
                            if ((ch >= '\x00a0') && (ch < 'Ā'))
                            {
                                output.Write("&#");
                                output.Write(((int)ch).ToString(NumberFormatInfo.InvariantInfo));
                                output.Write(';');
                            }
                            else
                            {
                                output.Write(ch);
                            }
                        }
                    }
                }
            }
        }

        private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos)
        {
            int num = s.Length - startPos;
            fixed (char* str = s)
            {
                char* chPtr = str;
                char* chPtr2 = chPtr + startPos;
                while (num > 0)
                {
                    char ch = chPtr2[0];
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '&':
                            case '\'':
                            case '"':
                            case '<':
                            case '>':
                                return (s.Length - num);

                            case '=':
                                goto Label_0086;
                        }
                    }
                    else if ((ch >= '\x00a0') && (ch < 'Ā'))
                    {
                        return (s.Length - num);
                    }
                    Label_0086:
                    chPtr2++;
                    num--;
                }
            }
            return -1;
        }

        // Nested Types
        private static class HtmlEntities
        {
            // Fields
            private static string[] _entitiesList = new string[] {
        "\"-quot", "&-amp", "'-apos", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf",
        "\x00ab-laquo", "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm",
        "\x00bb-raquo", "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc",
        "\x00cb-Euml", "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute",
        "\x00db-Ucirc", "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc",
        "\x00eb-euml", "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute",
        "\x00fb-ucirc", "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma",
        "Δ-Delta", "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau",
        "Υ-Upsilon", "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda",
        "μ-mu", "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih",
        "ϖ-piv", " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo",
        "†-dagger", "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade",
        "ℵ-alefsym", "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty",
        "∇-nabla", "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup",
        "∫-int", "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes",
        "⊥-perp", "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
     };
            private static Dictionary<string, char> _lookupTable = GenerateLookupTable();

            // Methods
            private static Dictionary<string, char> GenerateLookupTable()
            {
                Dictionary<string, char> dictionary = new Dictionary<string, char>(StringComparer.Ordinal);
                foreach (string str in _entitiesList)
                {
                    dictionary.Add(str.Substring(2), str[0]);
                }
                return dictionary;
            }

            public static char Lookup(string entity)
            {
                char ch;
                _lookupTable.TryGetValue(entity, out ch);
                return ch;
            }
        }
    }

    public static class StreamExtensions
    {
        public static void WriteUInt16LE(this Stream s, ushort i)
        {
            byte[] tmp = new byte[2];
            tmp[0] = (byte)(i & 0xFF);
            tmp[1] = (byte)((i >> 8) & 0xFF);
            s.Write(tmp, 0, 2);
        }

        public static void WriteUInt32LE(this Stream s, uint i)
        {
            byte[] tmp = new byte[4];
            tmp[0] = (byte)(i & 0xFF);
            tmp[1] = (byte)((i >> 8) & 0xFF);
            tmp[2] = (byte)((i >> 16) & 0xFF);
            tmp[3] = (byte)((i >> 24) & 0xFF);
            s.Write(tmp, 0, 4);
        }

        public static void WriteUInt64LE(this Stream s, ulong i)
        {
            byte[] tmp = new byte[8];
            tmp[0] = (byte)(i & 0xFF);
            tmp[1] = (byte)((i >> 8) & 0xFF);
            tmp[2] = (byte)((i >> 16) & 0xFF);
            tmp[3] = (byte)((i >> 24) & 0xFF);
            i >>= 32;
            tmp[4] = (byte)(i & 0xFF);
            tmp[5] = (byte)((i >> 8) & 0xFF);
            tmp[6] = (byte)((i >> 16) & 0xFF);
            tmp[7] = (byte)((i >> 24) & 0xFF);
            s.Write(tmp, 0, 8);
        }

        public static void WriteInt16LE(this Stream s, short i)
        {
            s.WriteUInt16LE(unchecked((ushort)i));
        }

        public static void WriteInt32LE(this Stream s, int i)
        {
            s.WriteUInt32LE(unchecked((uint)i));
        }

        public static void WriteInt64LE(this Stream s, long i)
        {
            s.WriteUInt64LE(unchecked((ulong)i));
        }

        public static void WriteLE(this Stream s, ushort i) => s.WriteUInt16LE(i);
        public static void WriteLE(this Stream s, uint i) => s.WriteUInt32LE(i);
        public static void WriteLE(this Stream s, ulong i) => s.WriteUInt64LE(i);
        public static void WriteLE(this Stream s, short i) => s.WriteInt16LE(i);
        public static void WriteLE(this Stream s, int i) => s.WriteInt32LE(i);
        public static void WriteLE(this Stream s, long i) => s.WriteInt64LE(i);

        public static uint ReadUInt32LE(this Stream s) => unchecked((uint)s.ReadInt32LE());


        /// <summary>
        /// Read a signed 32-bit little-endian integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ReadInt32LE(this Stream s)
        {
            int ret;
            byte[] tmp = new byte[4];
            s.Read(tmp, 0, 4);
            ret = tmp[0] & 0x000000FF;
            ret |= (tmp[1] << 8) & 0x0000FF00;
            ret |= (tmp[2] << 16) & 0x00FF0000;
            ret |= (tmp[3] << 24);
            return ret;
        }

        /// <summary>
        /// Read a null-terminated ASCII string from the given stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReadASCIINullTerminated(this Stream s, int limit = -1)
        {
            StringBuilder sb = new StringBuilder(255);
            int cur;
            while ((limit == -1 || sb.Length < limit) && (cur = s.ReadByte()) > 0)
            {
                sb.Append((char)cur);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read a given number of bytes from a stream into a new byte array.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count">Number of bytes to read (maximum)</param>
        /// <returns>New byte array of size &lt;=count.</returns>
        public static byte[] ReadBytes(this Stream s, int count)
        {
            // Size of returned array at most count, at least difference between position and length.
            int realCount = (int)((s.Position + count > s.Length) ? (s.Length - s.Position) : count);
            byte[] ret = new byte[realCount];
            s.Read(ret, 0, realCount);
            return ret;
        }

        /// <summary>
        /// Read a signed 64-bit little-endian integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ReadInt64LE(this Stream s)
        {
            long ret;
            byte[] tmp = new byte[8];
            s.Read(tmp, 0, 8);
            ret = tmp[4] & 0x000000FFL;
            ret |= (tmp[5] << 8) & 0x0000FF00L;
            ret |= (tmp[6] << 16) & 0x00FF0000L;
            ret |= (tmp[7] << 24) & 0xFF000000L;
            ret <<= 32;
            ret |= tmp[0] & 0x000000FFL;
            ret |= (tmp[1] << 8) & 0x0000FF00L;
            ret |= (tmp[2] << 16) & 0x00FF0000L;
            ret |= (tmp[3] << 24) & 0xFF000000L;
            return ret;
        }

        /// <summary>
        /// Read an unsigned 64-bit little-endian integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ulong ReadUInt64LE(this Stream s) => unchecked((ulong)s.ReadInt64LE());


        /// <summary>
        /// Read an unsigned 16-bit little-endian integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ushort ReadUInt16LE(this Stream s) => unchecked((ushort)s.ReadInt16LE());

        /// <summary>
        /// Read a signed 16-bit little-endian integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ReadInt16LE(this Stream s)
        {
            int ret;
            byte[] tmp = new byte[2];
            s.Read(tmp, 0, 2);
            ret = tmp[0] & 0x00FF;
            ret |= (tmp[1] << 8) & 0xFF00;
            return (short)ret;
        }

        /// <summary>
        /// Read a signed 8-bit integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static sbyte ReadInt8(this Stream s) => unchecked((sbyte)s.ReadUInt8());

        /// <summary>
        /// Read an unsigned 8-bit integer from the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte ReadUInt8(this Stream s)
        {
            byte ret;
            byte[] tmp = new byte[1];
            s.Read(tmp, 0, 1);
            ret = tmp[0];
            return ret;
        }


    }

    public static class ImageExtensions
    {
        static readonly Byte[] _Data;
        static readonly int _Width;
        static readonly int _Height;
        public static void CopyBlockTo(int x, int y, Byte[] block, out int mask)
        {
            mask = 0;

            int targetPixelIdx = 0;

            for (int py = 0; py < 4; ++py)
            {
                for (int px = 0; px < 4; ++px)
                {
                    // get the source pixel in the image
                    int sx = x + px;
                    int sy = y + py;

                    // enable if we're in the image
                    if (sx < _Width && sy < _Height)
                    {
                        // copy the rgba value
                        int sourcePixelIdx = 4 * (_Width * sy + sx);

                        for (int i = 0; i < 4; ++i) block[targetPixelIdx++] = _Data[sourcePixelIdx++];

                        // enable this pixel
                        mask |= (1 << (4 * py + px));
                    }
                    else
                    {
                        // skip this pixel as its outside the image
                        targetPixelIdx += 4;
                    }
                }
            }
        }
        public static byte[] ToByteArray(this System.Drawing.Image image, ImageFormat format)
        {
            if (image == null)
            {
                return new byte[1];
            }
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
    }

    static class ConstantsExtensions
    {
        public static CompressionOptions FixFlags(this CompressionOptions flags)
        {
            // grab the flag bits            
            var fit = flags & (CompressionOptions.ColourIterativeClusterFit | CompressionOptions.ColourClusterFit | CompressionOptions.ColourRangeFit | CompressionOptions.ColourClusterFitAlt);
            var metric = flags & (CompressionOptions.ColourMetricPerceptual | CompressionOptions.ColourMetricUniform);
            var extra = flags & (CompressionOptions.WeightColourByAlpha | CompressionOptions.UseParallelProcessing);

            // set defaults            
            if (fit == 0) fit = CompressionOptions.ColourClusterFit;
            if (metric == 0) metric = CompressionOptions.ColourMetricPerceptual;

            // done
            return fit | metric | extra;
        }
    }

    public static class HexBinTemp
    {

        public static int DivideByRoundUp(this int value, int divisor)
        {
            return (value + divisor - 1) / divisor;
        }

        public static T[][] DeInterleave<T>(this T[] input, int interleaveSize, int outputCount, int outputSize = -1)
        {
            if (input.Length % outputCount != 0)
                throw new ArgumentOutOfRangeException(nameof(outputCount), outputCount,
                    $"The input array length ({input.Length}) must be divisible by the number of outputs.");

            int inputSize = input.Length / outputCount;
            if (outputSize == -1)
                outputSize = inputSize;

            int inBlockCount = inputSize.DivideByRoundUp(interleaveSize);
            int outBlockCount = outputSize.DivideByRoundUp(interleaveSize);
            int lastInputInterleaveSize = inputSize - (inBlockCount - 1) * interleaveSize;
            int lastOutputInterleaveSize = outputSize - (outBlockCount - 1) * interleaveSize;
            int blocksToCopy = Math.Min(inBlockCount, outBlockCount);

            var outputs = new T[outputCount][];
            for (int i = 0; i < outputCount; i++)
            {
                outputs[i] = new T[outputSize];
            }

            for (int b = 0; b < blocksToCopy; b++)
            {
                int currentInputInterleaveSize = b == inBlockCount - 1 ? lastInputInterleaveSize : interleaveSize;
                int currentOutputInterleaveSize = b == outBlockCount - 1 ? lastOutputInterleaveSize : interleaveSize;
                int bytesToCopy = Math.Min(currentInputInterleaveSize, currentOutputInterleaveSize);

                for (int o = 0; o < outputCount; o++)
                {
                    Array.Copy(input, interleaveSize * b * outputCount + currentInputInterleaveSize * o, outputs[o],
                        interleaveSize * b, bytesToCopy);
                }
            }

            return outputs;
        }

        public static byte[][] DeInterleave(this Stream input, int length, int interleaveSize, int outputCount, int outputSize = -1)
        {
            if (input.CanSeek)
            {
                long remainingLength = input.Length - input.Position;
                if (remainingLength < length)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), length,
                        "Specified length is greater than the number of bytes remaining in the Stream");
                }
            }

            if (length % outputCount != 0)
                throw new ArgumentOutOfRangeException(nameof(outputCount), outputCount,
                    $"The input length ({length}) must be divisible by the number of outputs.");

            int inputSize = length / outputCount;
            if (outputSize == -1)
                outputSize = inputSize;

            int inBlockCount = inputSize.DivideByRoundUp(interleaveSize);
            int outBlockCount = outputSize.DivideByRoundUp(interleaveSize);
            int lastInputInterleaveSize = inputSize - (inBlockCount - 1) * interleaveSize;
            int lastOutputInterleaveSize = outputSize - (outBlockCount - 1) * interleaveSize;
            int blocksToCopy = Math.Min(inBlockCount, outBlockCount);

            var outputs = new byte[outputCount][];
            for (int i = 0; i < outputCount; i++)
            {
                outputs[i] = new byte[outputSize];
            }

            for (int b = 0; b < blocksToCopy; b++)
            {
                int currentInputInterleaveSize = b == inBlockCount - 1 ? lastInputInterleaveSize : interleaveSize;
                int currentOutputInterleaveSize = b == outBlockCount - 1 ? lastOutputInterleaveSize : interleaveSize;
                int bytesToCopy = Math.Min(currentInputInterleaveSize, currentOutputInterleaveSize);

                for (int o = 0; o < outputCount; o++)
                {
                    input.Read(outputs[o], interleaveSize * b, bytesToCopy);
                    if (bytesToCopy < currentInputInterleaveSize)
                    {
                        input.Position += currentInputInterleaveSize - bytesToCopy;
                    }
                }
            }

            return outputs;
        }

        public static short[][] InterleavedByteToShort(this byte[] input, int outputCount)
        {
            int itemCount = input.Length / 2 / outputCount;
            var output = new short[outputCount][];
            for (int i = 0; i < outputCount; i++)
            {
                output[i] = new short[itemCount];
            }

            for (int i = 0; i < itemCount; i++)
            {
                for (int o = 0; o < outputCount; o++)
                {
                    int offset = (i * outputCount + o) * 2;
                    output[o][i] = (short)(input[offset] | (input[offset + 1] << 8));
                }
            }

            return output;
        }

        public static byte[] ShortToInterleavedByte(this short[][] input)
        {
            int inputCount = input.Length;
            int length = input[0].Length;
            var output = new byte[inputCount * length * 2];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    int offset = (i * inputCount + j) * 2;
                    output[offset] = (byte)input[j][i];
                    output[offset + 1] = (byte)(input[j][i] >> 8);
                }
            }

            return output;
        }

        public static void Interleave(this byte[][] inputs, Stream output, int interleaveSize, int outputSize = -1)
        {
            int inputSize = inputs[0].Length;
            if (outputSize == -1)
                outputSize = inputSize;

            if (inputs.Any(x => x.Length != inputSize))
                throw new ArgumentOutOfRangeException(nameof(inputs), "Inputs must be of equal length");

            int inputCount = inputs.Length;
            int inBlockCount = inputSize.DivideByRoundUp(interleaveSize);
            int outBlockCount = outputSize.DivideByRoundUp(interleaveSize);
            int lastInputInterleaveSize = inputSize - (inBlockCount - 1) * interleaveSize;
            int lastOutputInterleaveSize = outputSize - (outBlockCount - 1) * interleaveSize;
            int blocksToCopy = Math.Min(inBlockCount, outBlockCount);

            for (int b = 0; b < blocksToCopy; b++)
            {
                int currentInputInterleaveSize = b == inBlockCount - 1 ? lastInputInterleaveSize : interleaveSize;
                int currentOutputInterleaveSize = b == outBlockCount - 1 ? lastOutputInterleaveSize : interleaveSize;
                int bytesToCopy = Math.Min(currentInputInterleaveSize, currentOutputInterleaveSize);

                for (int i = 0; i < inputCount; i++)
                {
                    output.Write(inputs[i], interleaveSize * b, bytesToCopy);
                    if (bytesToCopy < currentOutputInterleaveSize)
                    {
                        output.Position += currentOutputInterleaveSize - bytesToCopy;
                    }
                }
            }

            //Simply setting the position past the end of the stream doesn't expand the stream,
            //so we do that manually if necessary
            output.SetLength(Math.Max(outputSize * inputCount, output.Length));
        }

        #region << Trophy File >>

        public static byte[] decryptCBC128(this byte[] orginaldata, byte[] key, byte[] iv)
        {
            RijndaelManaged AES = new RijndaelManaged();
            AES.Mode = CipherMode.CBC;
            AES.Padding = PaddingMode.None;
            AES.KeySize = 128;
            AES.Key = key.SubArray(0, 16);
            AES.IV = iv.SubArray(0, 16);
            ICryptoTransform transform = AES.CreateDecryptor();
            byte[] outputData = transform.TransformFinalBlock(orginaldata, 0, orginaldata.Length);
            return outputData;
        }

        public static byte[] encryptCBC128(this byte[] orginaldata, byte[] key, byte[] iv)
        {
            RijndaelManaged AES = new RijndaelManaged();
            AES.Mode = CipherMode.CBC;
            AES.Padding = PaddingMode.None;
            AES.KeySize = 128;
            AES.Key = key.SubArray(0, 16);
            AES.IV = iv.SubArray(0, 16);
            ICryptoTransform transform = AES.CreateEncryptor();
            byte[] outputData = transform.TransformFinalBlock(orginaldata, 0, orginaldata.Length);
            return outputData;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static string ToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }

        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ArrayToString<T>(this T[] array)
        {
            StringBuilder builder = new StringBuilder("[");
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (i != 0) builder.Append(",");
                builder.Append(array[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }

        public static T ToStruct<T>(this byte[] ptr)
        {
            GCHandle handle = GCHandle.Alloc(ptr, GCHandleType.Pinned);
            T ret = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return ret;
        }

        public static byte[] StructToBytes<T>(this T obj)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return buffer;
        }

        public static short ChangeEndian(this short val)
        {
            byte[] arr = BitConverter.GetBytes(val);
            Array.Reverse(arr);
            return BitConverter.ToInt16(arr, 0);
        }

        public static int ChangeEndian(this int val)
        {
            byte[] arr = BitConverter.GetBytes(val);
            Array.Reverse(arr);
            return BitConverter.ToInt32(arr, 0);
        }

        public static uint ChangeEndian(this uint val)
        {
            byte[] arr = BitConverter.GetBytes(val);
            Array.Reverse(arr);
            return BitConverter.ToUInt32(arr, 0);
        }

        public static long ChangeEndian(this long val)
        {
            byte[] arr = BitConverter.GetBytes(val);
            Array.Reverse(arr);
            return BitConverter.ToInt64(arr, 0);
        }

        public static ulong ChangeEndian(this ulong val)
        {
            byte[] arr = BitConverter.GetBytes(val);
            Array.Reverse(arr);
            return BitConverter.ToUInt64(arr, 0);
        }

        #endregion << Trophy File >>
    }

    /// <summary>
    /// This is a placeholder for all ps4 keys
    /// </summary>
    internal class PS4Keys
    {

        //most keys are thanks to the efforts of the community found on the devwiki
        //https://www.psdevwiki.com/ps4/Keys
        /// <summary>
        /// a nice read here
        /// https://en.wikipedia.org/wiki/Extensible_Authentication_Protocol
        /// </summary>
        public class EAPKeys
        {
            public class Kernel
            {
                public static byte[] eap_hmac_key_0 = new byte[16] { 0xBB, 0x6C, 0xD6, 0x6D, 0xDC, 0x67, 0x1F, 0xAC, 0x36, 0x64, 0xF7, 0xBF, 0x50, 0x49, 0xBA, 0xA8 };
                public static byte[] eap_hmac_key = new byte[16] { 0xC4, 0x68, 0x79, 0x04, 0xBC, 0x31, 0xCF, 0x4F, 0x2F, 0x4E, 0x9F, 0x89, 0xFA, 0x45, 0x87, 0x93 };
                public static byte[] eap_internal_seed = new byte[16] { 0x81, 0x17, 0x45, 0xE7, 0xC7, 0xE8, 0x0D, 0x46, 0x0F, 0xAF, 0x23, 0x26, 0x55, 0x0B, 0xD7, 0xE4 };
                public static byte[] eap_internal_seed_2 = new byte[16] { 0xD2, 0xA0, 0xA0, 0xD9, 0x72, 0x9D, 0xE5, 0xD2, 0x11, 0x7D, 0x70, 0x67, 0x6F, 0x1D, 0x55, 0x74 };
                public static byte[] eap_internal_seed_3 = new byte[16] { 0x8D, 0xC1, 0x7C, 0xDF, 0x29, 0xC8, 0x6A, 0x85, 0x5F, 0x2A, 0xE9, 0xA1, 0xAD, 0x3E, 0x91, 0x5F };
            }
        }

        public class SysconKeys
        {
            public class Common
            {
                public static byte[] Security_ID = new byte[10] { 0x3A, 0x4E, 0x6F, 0x74, 0x3A, 0x55, 0x73, 0x65, 0x64, 0x3A };
                public static byte[] AUTH = new byte[256]
                {
                    0xC1 , 0x65 , 0x3D , 0x76 , 0x05 , 0x79 , 0x07 , 0xFB , 0xD2 , 0x8A , 0xFB , 0xC5 , 0x59 , 0xC2 , 0x3C , 0x58 , 0x03 , 0xA0 , 0xCD , 0x50 , 0x56 , 0x13 , 0xC5 , 0x87 , 0x8E , 0x91 , 0xD9 , 0x0B , 0xB6 , 0xB4 , 0xCB , 0xA4 , 0x51 , 0x3F , 0x54 , 0xA0 , 0xF5 , 0x21 , 0x40 , 0xB0 , 0xA3 , 0x8C , 0x15 , 0xC2 , 0x4D , 0xCC , 0x59 , 0xEB , 0x60 , 0x56 , 0x18 , 0xD0 , 0x20 , 0xB4 , 0xA2 , 0x1A , 0x34 , 0xC9 , 0x99 , 0x15 , 0xDA , 0x5A , 0x58 , 0x9D , 0x79 , 0x3F , 0x58 , 0xCB , 0x6E , 0xA8 , 0x26 , 0x66 , 0xD2 , 0x72 , 0x14 , 0x5B , 0x62 , 0xF2 , 0x03 , 0xED , 0x87 , 0x84 , 0x0C , 0x84 , 0x48 , 0x42 , 0xCA , 0x77 , 0x3E , 0xDF , 0xC5 , 0x81 , 0xBC , 0xAA , 0xBF , 0xFB , 0x4A , 0xA4 , 0xEE , 0xED , 0x08 , 0xF6 , 0x69 , 0x5C , 0xAA , 0x2C , 0x13 , 0xEC , 0x30 , 0xFA , 0x1C , 0xE3 , 0x9F , 0xCD , 0xCD , 0xCB , 0xA7 , 0xCD , 0xD9 , 0xC6 , 0x8B , 0xA1 , 0x32 , 0x9D , 0x18 , 0xF8 , 0x98 , 0x42 , 0x46 , 0x22 , 0x8A , 0x1F , 0x1E , 0xB5 , 0x7D , 0x08 , 0xE5 , 0xA5 , 0x2D , 0xE5 , 0x1C , 0xC3 , 0xE3 , 0xD2 , 0xFF , 0x96 , 0xAE , 0x61 , 0xBE , 0x0F , 0x9E , 0x8F , 0x99 , 0x6C , 0xCD , 0xA8 , 0xC7 , 0x6D , 0x66 , 0xA9 , 0x5D , 0xCE , 0x0C , 0x18 , 0xFB , 0xF8 , 0x6B , 0xC2 , 0x70 , 0x50 , 0xDB , 0x65 , 0x65 , 0xF6 , 0x81 , 0xAA , 0x66 , 0x70 , 0xD8 , 0xF4 , 0xE6 , 0x26 , 0x19 , 0x2D , 0xEB , 0x59 , 0x1E , 0x57 , 0xE9 , 0x9B , 0x33 , 0x25 , 0x29 , 0x71 , 0x46 , 0x18 , 0x8E , 0xDB , 0x6D , 0x65 , 0x4E , 0xD7 , 0xF6 , 0x1B , 0x5A , 0x11 , 0x53 , 0x2D , 0x87 , 0xAE , 0x56 , 0x2A , 0x76 , 0xEE , 0xC1 , 0x6F , 0xEC , 0x4B , 0x1E , 0x92 , 0x97 , 0x7F , 0x73 , 0x0D , 0x3D , 0x44 , 0xDB , 0x9F , 0xB1 , 0x1A , 0x4F , 0xF0 , 0x72 , 0x81 , 0x45 , 0x6D , 0x0C , 0xFE , 0xB3 , 0x0A , 0x12 , 0x39 , 0x95 , 0xFE , 0x7C , 0x72 , 0xFB , 0x5F , 0xCC , 0x24 , 0x9F , 0xB0 , 0x95 , 0x70 , 0x4E , 0xCB
                };
                public static byte[] Unknown1 = new byte[128]
                {
                    0x7B , 0xB3 , 0x25 , 0xCE , 0xDD , 0x2F , 0xE9 , 0xC1 , 0xF8 , 0xC9 , 0x87 , 0xE0 , 0xBC , 0x17 , 0x5D , 0x5F , 0xF9 , 0x9F , 0xEB , 0xBB , 0x45 , 0xE9 , 0x67 , 0x93 , 0xAF , 0xBF , 0x57 , 0x27 , 0xE2 , 0x76 , 0xF2 , 0x34 , 0x91 , 0xD8 , 0x2C , 0xAD , 0x48 , 0x3F , 0xA7 , 0x7E , 0x91 , 0x7C , 0x5D , 0xD5 , 0x89 , 0xD2 , 0x80 , 0x49 , 0x6D , 0x24 , 0xA0 , 0xBE , 0xBB , 0xCB , 0xF1 , 0x0D , 0x6B , 0xB7 , 0x5E , 0xC6 , 0x21 , 0x27 , 0x28 , 0x86 , 0xEF , 0xDA , 0x5C , 0x48 , 0x1D , 0xCB , 0xC8 , 0xA9 , 0xAD , 0x6A , 0xD7 , 0x76 , 0xE7 , 0xD6 , 0xDF , 0xE6 , 0xA0 , 0x6A , 0xCD , 0x1B , 0xC0 , 0xD6 , 0x70 , 0x44 , 0x55 , 0xD1 , 0x0D , 0x36 , 0x3D , 0xBD , 0x24 , 0x97 , 0xE6 , 0x72 , 0x3F , 0x94 , 0xA9 , 0x8A , 0xC3 , 0x1F , 0xF7 , 0xDD , 0x7A , 0xE7 , 0x2C , 0xA7 , 0x54 , 0x27 , 0xCF , 0x7A , 0x1E , 0x55 , 0xD1 , 0x45 , 0xE4 , 0xE3 , 0x31 , 0xD7 , 0xCE , 0xDC , 0xC2 , 0x7D , 0xCE , 0xF9
                };
                public static byte[] Unknown2 = new byte[128]
                {
                    0x23 , 0xC4 , 0xF6 , 0x61 , 0x68 , 0xB0 , 0x60 , 0xAB , 0x37 , 0xDC , 0xBE , 0xB2 , 0x60 , 0x12 , 0xD3 , 0xC5 , 0xB1 , 0x93 , 0x2E , 0x9E , 0x7D , 0xCB , 0x4B , 0xC7 , 0xC4 , 0xE5 , 0x66 , 0xBE , 0x5D , 0xC1 , 0xF5 , 0xCA , 0xB1 , 0x85 , 0xF6 , 0x32 , 0x80 , 0xED , 0x4F , 0xB0 , 0x78 , 0x11 , 0x1C , 0x18 , 0x6D , 0xC5 , 0x2F , 0x00 , 0x82 , 0x50 , 0x2D , 0x3D , 0x37 , 0xF3 , 0x66 , 0xC6 , 0x1A , 0x2B , 0x92 , 0xBE , 0x26 , 0x30 , 0x04 , 0x9D , 0xF8 , 0xEC , 0xC3 , 0x3B , 0xDD , 0x6A , 0x21 , 0x38 , 0x0E , 0x53 , 0x50 , 0x5E , 0x3E , 0x56 , 0x43 , 0x89 , 0xFA , 0x8E , 0xE2 , 0x38 , 0x12 , 0x46 , 0x3E , 0x1B , 0xA0 , 0xF6 , 0xA0 , 0x73 , 0x77 , 0x8E , 0x85 , 0x3E , 0xFE , 0x26 , 0x31 , 0x57 , 0xD3 , 0x3F , 0x40 , 0x9A , 0xCE , 0xA6 , 0xBC , 0xF0 , 0xC4 , 0xC8 , 0x7F , 0x1E , 0xFF , 0xB4 , 0x49 , 0xA6 , 0xC0 , 0x5C , 0x06 , 0xBC , 0x11 , 0x54 , 0x65 , 0x35 , 0x79 , 0x92 , 0xC8 , 0xBC
                };
            }

            public class DevKit
            {
                /*Used for:

                40000001 (BLNK)
                40000002 (BASE)
                40000003 (SYST)*/

                public static byte[] Full_Firmware_Key = new byte[16]
                {
                    0x53 , 0x01 , 0xC2 , 0x88 , 0x24 , 0xB5 , 0x71 , 0x37 , 0xA8 , 0x19 , 0xC0 , 0x42 , 0xFC , 0x11 , 0x9E , 0x3F
                };

                /// <summary>
                /// Used to generate the AES-CMAC-128 at the start of decrypted 40000001, 40000002 and 40000003, by digesting the remainder of the decrypted data
                /// </summary>
                public static byte[] Full_Firmware_Key_2 = new byte[16]
                {
                   0x8F , 0x21 , 0x56 , 0x91 , 0xAC , 0x7E , 0xF6 , 0x51 , 0x02 , 0x39 , 0xDD , 0x32 , 0xCC , 0x6A , 0x23 , 0x94
                };

                public static byte[] Full_Firmware_IV = new byte[8]
                {
                    0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
                };
                /// <summary>
                /// SNVS (Devkit)
                /// </summary>
                public static byte[] SNVS_Key1 = new byte[16]
                {
                    0x82 , 0xD4 , 0xEE , 0xE9 , 0xE7 , 0xF6 , 0x8E , 0xFB , 0xC4 , 0x3C , 0x3D , 0x27 , 0x47 , 0xE4 , 0x13 , 0x9F
                };
                public static byte[] SNVS_KIV1 = new byte[8]
               {
                    0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
               };

                /// <summary>
                /// Used to generate the AES-CMAC-128 at the start of decrypted SNVS(Devkit), by digesting the remainder of the decrypted data
                /// </summary>
                public static byte[] SNVS_Key1_Dec = new byte[16]
               {
                   0x4C , 0x49 , 0xDC , 0x8D , 0xF6 , 0xA2 , 0x0E , 0x15 , 0x92 , 0xF9 , 0xE9 , 0xF7 , 0x44 , 0x2B , 0x42 , 0x61
               };

                /// <summary>
                /// SNVS(Devkit #2)
                /// </summary>
                public static byte[] SNVS_Key2 = new byte[16]
                {
                   0xDB , 0x60 , 0x30 , 0x53 , 0xA4 , 0xD3 , 0x11 , 0x91 , 0x49 , 0x99 , 0x6D , 0x0B , 0xA8 , 0x44 , 0x34 , 0xE2 ,
                };
                public static byte[] SNVS_KIV2 = new byte[8]
               {
                    0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
               };

                public static byte[] SNVS_Key2_Dec = new byte[16]
                 {
                       0xB3 , 0xA8 , 0xCB , 0x79 , 0x7D , 0x14 , 0x06 , 0x65 , 0x83 , 0x72 , 0xA9 , 0x2B , 0x6C , 0xFB , 0x34 , 0x90
                 };
            }

            public class Retial_TestKit
            {
                /// <summary>
                /// Used for:
                ///40010001 (Patch #1)
                ///40010002 (Patch #2)
                /// </summary>
                public static byte[] Patch_Firmware_Key_and_CMAC_Key = new byte[16]
                {
                    0xEF , 0x90 , 0xB2 , 0x1B , 0x31 , 0x45 , 0x23 , 0x79 , 0x06 , 0x8E , 0x30 , 0x41 , 0xAA , 0xD8 , 0x28 , 0x1E ,
                };

                public static byte[] Patch_Firmware_Key_and_CMAC_IV = new byte[8]
                {
                    0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
                };

                /// <summary>
                /// Used to generate the AES-CMAC-128 at the start of decrypted 40010001 and 40010002, by digesting the remainder of the decrypted data
                /// </summary>
                public static byte[] Patch_Firmware_Key_and_CMAC_Dec = new byte[16]
               {
                    0x95 , 0xB1 , 0xAA , 0xF2 , 0x0C , 0x16 , 0xD4 , 0x6F , 0xC8 , 0x16 , 0xDF , 0x32 , 0x55 , 0x1D , 0xE0 , 0x32 ,
               };

                public static byte[] SNVS_Key_and_CMAC_Key = new byte[16]
                {
                    0x8B , 0xF0 , 0x74 , 0xCC , 0xA3 , 0xD9 , 0xC3 , 0x98 , 0x14 , 0x22 , 0x56 , 0xD7 , 0xDD , 0x1A , 0x12 , 0x59 ,
                };


                public static byte[] SNVS_Key_and_CMAC_IV = new byte[8]
                {
                    0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00
                };

                public static byte[] SNVS_Key_and_CMAC_Key_Dec = new byte[16]
               {
                   0x40 , 0x65 , 0x91 , 0x8E , 0xB3 , 0x39 , 0x18 , 0x4D , 0xAA , 0xCC , 0xD6 , 0x1B , 0x30 , 0xB5 , 0xFB , 0x59 ,
               };
            }
        }

        public class KernelKeys
        {
            public class Backup_And_Restore_Keys
            {
                public static byte[] Cipher = new byte[16]
                {
                    0x79 , 0xC8 , 0xCC , 0xC8 , 0x89 , 0xA1 , 0x54 , 0x0D , 0x4F , 0x2E , 0x27 , 0xBB , 0x61 , 0x4F , 0xD6 , 0x53 ,
                };

                public static byte[] Hasher = new byte[32]
                {
                    0x1F , 0x18 , 0xC9 , 0x70 , 0xD0 , 0x00 , 0xAC , 0x7E , 0x6F , 0xCC , 0x1A , 0x8C , 0xDD , 0x89 , 0xB4 , 0xFE , 0xCD , 0xA1 , 0x33 , 0xA1 , 0x0E , 0xC8 , 0xF5 , 0x25 , 0x98 , 0x22 , 0x23 , 0xF5 , 0x86 , 0x1F , 0x02 , 0x00 ,
                };
            }

            /// <summary>
            /// Used as suffix to symbol names when hashing with SHA1 to create a NID.
            /// </summary>
            public static byte[] default_suffix_key = new byte[]
            {
                    0x51 , 0x8D , 0x64 , 0xA6 , 0x35 , 0xDE , 0xD8 , 0xC1 , 0xE6 , 0xB0 , 0x39 , 0xB1 , 0xC3 , 0xE5 , 0x52 , 0x30 ,
            };

            //Sealed Key Values or (PFS_EncKey and sealedkey_retail_key) Values

            public class SealedKey
            {
                public class Keyset1
                {
                    public static byte[] Key = new byte[16]
                    {
                        0xB5 , 0xDA , 0xEF , 0xFF , 0x39 , 0xE6 , 0xD9 , 0x0E , 0xCA , 0x7D , 0xC5 , 0xB0 , 0x29 , 0xA8 , 0x15 , 0x3E ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                        0x87 , 0x07 , 0x96 , 0x0A , 0x53 , 0x46 , 0x8D , 0x6C , 0x84 , 0x3B , 0x3D , 0xC9 , 0x62 , 0x4E , 0x22 , 0xAF ,
                    };
                }
                public class Keyset2
                {
                    public static byte[] Key = new byte[16]
                    {
                        0xEC , 0x0D , 0x34 , 0x7E , 0x2A , 0x76 , 0x57 , 0x47 , 0x1F , 0x1F , 0xC3 , 0x3E , 0x9E , 0x91 , 0x6F , 0xD4 ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                        0xA6 , 0xD6 , 0x58 , 0x3D , 0x32 , 0x17 , 0xE8 , 0x7D , 0x9B , 0xE9 , 0xBC , 0xFC , 0x44 , 0x36 , 0xBE , 0x4F ,
                    };
                }
                public class Keyset3
                {
                    public static byte[] Key = new byte[16]
                    {
                        0x51 , 0xD8 , 0xBF , 0xB4 , 0xE3 , 0x87 , 0xFB , 0x41 , 0x20 , 0xF0 , 0x81 , 0xFE , 0x33 , 0xE4 , 0xBE , 0x9A ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                       0xFF , 0xF9 , 0xBD , 0xEA , 0x80 , 0x3B , 0x14 , 0x82 , 0x4C , 0x61 , 0x85 , 0x0E , 0xBB , 0x08 , 0x4E , 0xE9 ,
                    };
                }
                public class Keyset4
                {
                    public static byte[] Key = new byte[16]
                    {
                       0x34 , 0x6B , 0x5D , 0x23 , 0x13 , 0x32 , 0xAC , 0x42 , 0x8A , 0x44 , 0xA7 , 0x08 , 0xB1 , 0x13 , 0x8F , 0x6D ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                      0x5D , 0xC6 , 0xB8 , 0xD1 , 0xA3 , 0xA0 , 0x74 , 0x18 , 0x52 , 0xA7 , 0xD4 , 0x42 , 0x68 , 0x71 , 0x48 , 0x24 ,
                    };
                }
                public class Keyset5
                {
                    public static byte[] Key = new byte[16]
                    {
                      0x20 , 0xD0 , 0x43 , 0x85 , 0x25 , 0x30 , 0xC4 , 0x04 , 0xD1 , 0x68 , 0x69 , 0xE0 , 0x79 , 0x08 , 0xD5 , 0xE6 ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                    0x2D , 0xE8 , 0xDE , 0x4D , 0xE6 , 0x62 , 0x8B , 0xB6 , 0x2D , 0xD5 , 0xC1 , 0x70 , 0xF5 , 0x65 , 0xB6 , 0x2C ,
                    };
                }
                public class Keyset6
                {
                    public static byte[] Key = new byte[16]
                    {
                     0x93 , 0xB7 , 0x27 , 0x0D , 0xF0 , 0xD3 , 0x73 , 0x10 , 0x60 , 0x07 , 0x90 , 0x66 , 0x65 , 0x5D , 0x8D , 0x07 ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                    0xFD , 0x44 , 0xA3 , 0x2D , 0x8B , 0xC8 , 0xAC , 0x18 , 0x9C , 0x1B , 0xD0 , 0x96 , 0x40 , 0x29 , 0x66 , 0xCF ,
                    };
                }
                public class Keyset7
                {
                    public static byte[] Key = new byte[16]
                    {
                        0x4C , 0x78 , 0x44 , 0x83 , 0x69 , 0x37 , 0x50 , 0x8B , 0x92 , 0x33 , 0xDF , 0x7C , 0xD7 , 0xD6 , 0x51 , 0x65 ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                        0xBC , 0x4C , 0x9F , 0x0F , 0xE5 , 0xD3 , 0x56 , 0xA0 , 0x57 , 0x52 , 0x02 , 0x4C , 0xBD , 0xEE , 0xC8 , 0xE4 ,
                    };
                }
                public class Keyset8
                {
                    public static byte[] Key = new byte[16]
                    {
                        0x3A , 0x32 , 0xEE , 0xCF , 0x74 , 0x99 , 0x39 , 0x87 , 0x1C , 0x3D , 0x7B , 0xF8 , 0xC0 , 0x1C , 0x7D , 0x1F ,
                    };

                    public static byte[] Hash = new byte[16]
                    {
                        0xF6 , 0xF9 , 0xD8 , 0x21 , 0x82 , 0xCC , 0xC2 , 0x22 , 0x7B , 0x7D , 0x33 , 0xA3 , 0xB7 , 0x1E , 0xAD , 0xE3 ,
                    };
                }
            }


            public class AuthCode
            {
                public static byte[] KeySet1 = new byte[16]
                {
                    0x2B , 0xCF , 0x69 , 0x8E , 0x79 , 0xCF , 0xDD , 0xFA , 0xC2 , 0x4D , 0x4C , 0x25 , 0xBF , 0x35 , 0x1E , 0x62 ,
                };
            }

            public class Vtrm_Cipher_Init_Keys
            {

                public class hmac_key_seed
                {
                    public static byte[] Key = new byte[32]
                  {
                   0x87 , 0xFB , 0x19 , 0xBB , 0xF3 , 0xD4 , 0xD6 , 0xB1 , 0xB0 , 0xED , 0x22 , 0x6E , 0x39 , 0xCC , 0x62 , 0x1A , 0x37 , 0xFA , 0x4E , 0xD2 , 0xB6 , 0x61 , 0x8B , 0x59 , 0xB3 , 0x4F , 0x77 , 0x0F , 0xBB , 0x92 , 0x94 , 0x7B ,
                  };

                    public static byte[] IV = new byte[16]
                    {
                        0x00 , 0x11 , 0x22 , 0x33 , 0x44 , 0x55 , 0x66 , 0x77 , 0x88 , 0x99 , 0xAA , 0xBB , 0xCC , 0xDD , 0xEE , 0xFF ,
                    };
                }

                public static byte[] aes_key_seed = new byte[112]
                {
                    0xB0 , 0xED , 0x22 , 0x6E , 0x39 , 0xCC , 0x62 , 0x1A , 0x37 , 0xFA , 0x4E , 0xD2 , 0xB6 , 0x61 , 0x8B , 0x59 , 0xDB , 0xF0 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x47 , 0xF1 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x81 , 0xF1 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0xBB , 0xF1 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0xF5 , 0xF1 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x37 , 0xF2 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x79 , 0xF2 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0xA9 , 0xF2 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0xDA , 0xF2 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x07 , 0xF3 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x47 , 0xF3 , 0x7E , 0x82 , 0xFF , 0xFF , 0xFF , 0xFF , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 ,
                };
            }

            public class Keystone_Keys
            {
                public static byte[] keystone_passcode_secret = new byte[16]
                {
                    0xC7 , 0x44 , 0x05 , 0xF6 , 0x74 , 0x24 , 0xBA , 0x34 , 0x2B , 0xC1 , 0x27 , 0x62 , 0x51 , 0xBB , 0xC2 , 0xF5 ,
                };
                public static byte[] passcode_hmac_secret = new byte[16]
                {
                    0x55 , 0xF1 , 0x60 , 0x25 , 0xB6 , 0xA1 , 0xB6 , 0x71 , 0x47 , 0x80 , 0xDB , 0xAE , 0xC8 , 0x52 , 0xFA , 0x2F ,
                };

                public static byte[] keystone_ks_secret = new byte[16]
                {
                    0x78 , 0x3D , 0x6F , 0x3A , 0xE9 , 0x1C , 0x0E , 0x07 , 0x12 , 0xFC , 0xAA , 0xB7 , 0x95 , 0x0B , 0xDE , 0x06 ,
                };
                public static byte[] keystone_hmac_secret = new byte[16]
                {
                    0x85 , 0x5C , 0xF7 , 0xA2 , 0x2D , 0xCD , 0xBD , 0xE1 , 0x27 , 0xE9 , 0xBF , 0xCB , 0xAD , 0x0F , 0xF0 , 0xFE ,
                };
            }

        }

        public class ShellCore_Keys
        {
            /// <summary>
            /// Devkit / Testkit
            /// </summary>
            public class Kits
            {
                /// <summary>
                /// Referenced as Kirk Key in the beginning of the file
                /// </summary>
                public static byte[] Trophy_Key = new byte[16]
                {
                    0x02 , 0xCC , 0xD3 , 0x46 , 0xB4 , 0x59 , 0xCB , 0x83 , 0x50 , 0x5E , 0x8E , 0x76 , 0x0A , 0x44 , 0xD4 , 0x57 ,
                };

            }

            /// <summary>
            /// Retail keys
            /// </summary>
            public class Retail
            {
                public class Trophy
                {
                    public static byte[] Trophy_Key = new byte[16]
                    {
                    0x21 , 0xF4 , 0x1A , 0x6B , 0xAD , 0x8A , 0x1D , 0x3E , 0xCA , 0x7A , 0xD5 , 0x86 , 0xC1 , 0x01 , 0xB7 , 0xA9 ,
                    };
                }
                public class RSA_PKG_Meta
                {
                    public static byte[] P = new byte[128]
                    {
                        0xF9 , 0x67 , 0xAD , 0x99 , 0x12 , 0x31 , 0x0C , 0x56 , 0xA2 , 0x2E , 0x16 , 0x1C , 0x46 , 0xB3 , 0x4D , 0x5B , 0x43 , 0xBE , 0x42 , 0xA2 , 0xF6 , 0x86 , 0x96 , 0x80 , 0x42 , 0xC3 , 0xC7 , 0x3F , 0xC3 , 0x42 , 0xF5 , 0x87 , 0x49 , 0x33 , 0x9F , 0x07 , 0x5D , 0x6E , 0x2C , 0x04 , 0xFD , 0xE3 , 0xE1 , 0xB2 , 0xAE , 0x0A , 0x0C , 0xF0 , 0xC7 , 0xA6 , 0x1C , 0xA1 , 0x63 , 0x50 , 0xC8 , 0x09 , 0x9C , 0x51 , 0x24 , 0x52 , 0x6C , 0x5E , 0x5E , 0xBD , 0x1E , 0x27 , 0x06 , 0xBB , 0xBC , 0x9E , 0x94 , 0xE1 , 0x35 , 0xD4 , 0x6D , 0xB3 , 0xCB , 0x3C , 0x68 , 0xDD , 0x68 , 0xB3 , 0xFE , 0x6C , 0xCB , 0x8D , 0x82 , 0x20 , 0x76 , 0x23 , 0x63 , 0xB7 , 0xE9 , 0x68 , 0x10 , 0x01 , 0x4E , 0xDC , 0xBA , 0x27 , 0x5D , 0x01 , 0xC1 , 0x2D , 0x80 , 0x5E , 0x2B , 0xAF , 0x82 , 0x6B , 0xD8 , 0x84 , 0xB6 , 0x10 , 0x52 , 0x86 , 0xA7 , 0x89 , 0x8E , 0xAE , 0x9A , 0xE2 , 0x89 , 0xC6 , 0xF7 , 0xD5 , 0x87 , 0xFB ,
                    };
                    public static byte[] Q = new byte[128]
                    {
                        0xD7 , 0xA1 , 0x0F , 0x9A , 0x8B , 0xF2 , 0xC9 , 0x11 , 0x95 , 0x32 , 0x9A , 0x8C , 0xF0 , 0xD9 , 0x40 , 0x47 , 0xF5 , 0x68 , 0xA0 , 0x0D , 0xBD , 0xC1 , 0xFC , 0x43 , 0x2F , 0x65 , 0xF9 , 0xC3 , 0x61 , 0x0F , 0x25 , 0x77 , 0x54 , 0xAD , 0xD7 , 0x58 , 0xAC , 0x84 , 0x40 , 0x60 , 0x8D , 0x3F , 0xF3 , 0x65 , 0x89 , 0x75 , 0xB5 , 0xC6 , 0x2C , 0x51 , 0x1A , 0x2F , 0x1F , 0x22 , 0xE4 , 0x43 , 0x11 , 0x54 , 0xBE , 0xC9 , 0xB4 , 0xC7 , 0xB5 , 0x1B , 0x05 , 0x0B , 0xBC , 0x56 , 0x9A , 0xCD , 0x4A , 0xD9 , 0x73 , 0x68 , 0x5E , 0x5C , 0xFB , 0x92 , 0xB7 , 0x8B , 0x0D , 0xFF , 0xF5 , 0x07 , 0xCA , 0xB4 , 0xC8 , 0x9B , 0x96 , 0x3C , 0x07 , 0x9E , 0x3E , 0x6B , 0x2A , 0x11 , 0xF2 , 0x8A , 0xB1 , 0x8A , 0xD7 , 0x2E , 0x1B , 0xA5 , 0x53 , 0x24 , 0x06 , 0xED , 0x50 , 0xB8 , 0x90 , 0x67 , 0xB1 , 0xE2 , 0x41 , 0xC6 , 0x92 , 0x01 , 0xEE , 0x10 , 0xF0 , 0x61 , 0xBB , 0xFB , 0xB2 , 0x7D , 0x4A , 0x73 ,
                    };
                    public static byte[] Modulus = new byte[256]
                    {
                        0xD2 , 0x12 , 0xFC , 0x33 , 0x5F , 0x6D , 0xDB , 0x83 , 0x16 , 0x09 , 0x62 , 0x8B , 0x03 , 0x56 , 0x27 , 0x37 , 0x82 , 0xD4 , 0x77 , 0x85 , 0x35 , 0x29 , 0x39 , 0x2D , 0x52 , 0x6B , 0x8C , 0x4C , 0x8C , 0xFB , 0x06 , 0xC1 , 0x84 , 0x5B , 0xE7 , 0xD4 , 0xF7 , 0xBC , 0xD2 , 0x4E , 0x62 , 0x45 , 0xCD , 0x2A , 0xBB , 0xD7 , 0x77 , 0x76 , 0x45 , 0x36 , 0x55 , 0x27 , 0x3F , 0xB3 , 0xF5 , 0xF9 , 0x8E , 0xDA , 0x4B , 0xEF , 0xAA , 0x59 , 0xAE , 0xB3 , 0x9B , 0xEA , 0x54 , 0x98 , 0xD2 , 0x06 , 0x32 , 0x6A , 0x58 , 0x31 , 0x2A , 0xE0 , 0xD4 , 0x4F , 0x90 , 0xB5 , 0x0A , 0x7D , 0xEC , 0xF4 , 0x3A , 0x9C , 0x52 , 0x67 , 0x2D , 0x99 , 0x31 , 0x8E , 0x0C , 0x43 , 0xE6 , 0x82 , 0xFE , 0x07 , 0x46 , 0xE1 , 0x2E , 0x50 , 0xD4 , 0x1F , 0x2D , 0x2F , 0x7E , 0xD9 , 0x08 , 0xBA , 0x06 , 0xB3 , 0xBF , 0x2E , 0x20 , 0x3F , 0x4E , 0x3F , 0xFE , 0x44 , 0xFF , 0xAA , 0x50 , 0x43 , 0x57 , 0x91 , 0x69 , 0x94 , 0x49 , 0x15 , 0x82 , 0x82 , 0xE4 , 0x0F , 0x4C , 0x8D , 0x9D , 0x2C , 0xC9 , 0x5B , 0x1D , 0x64 , 0xBF , 0x88 , 0x8B , 0xD4 , 0xC5 , 0x94 , 0xE7 , 0x65 , 0x47 , 0x84 , 0x1E , 0xE5 , 0x79 , 0x10 , 0xFB , 0x98 , 0x93 , 0x47 , 0xB9 , 0x7D , 0x85 , 0x12 , 0xA6 , 0x40 , 0x98 , 0x2C , 0xF7 , 0x92 , 0xBC , 0x95 , 0x19 , 0x32 , 0xED , 0xE8 , 0x90 , 0x56 , 0x0D , 0x65 , 0xC1 , 0xAA , 0x78 , 0xC6 , 0x2E , 0x54 , 0xFD , 0x5F , 0x54 , 0xA1 , 0xF6 , 0x7E , 0xE5 , 0xE0 , 0x5F , 0x61 , 0xC1 , 0x20 , 0xB4 , 0xB9 , 0xB4 , 0x33 , 0x08 , 0x70 , 0xE4 , 0xDF , 0x89 , 0x56 , 0xED , 0x01 , 0x29 , 0x46 , 0x77 , 0x5F , 0x8C , 0xB8 , 0xA9 , 0xF5 , 0x1E , 0x2E , 0xB3 , 0xB9 , 0xBF , 0xE0 , 0x09 , 0xB7 , 0x8D , 0x28 , 0xD4 , 0xA6 , 0xC3 , 0xB8 , 0x1E , 0x1F , 0x07 , 0xEB , 0xB4 , 0x12 , 0x0B , 0x95 , 0xB8 , 0x85 , 0x30 , 0xFD , 0xDC , 0x39 , 0x13 , 0xD0 , 0x7C , 0xDC , 0x8F , 0xED , 0xF9 , 0xC9 , 0xA3 , 0xC1 ,
                    };

                    public static byte[] Private_Key = new byte[256]
                    {
                        0x32 , 0xD9 , 0x03 , 0x90 , 0x8F , 0xBD , 0xB0 , 0x8F , 0x57 , 0x2B , 0x28 , 0x5E , 0x0B , 0x8D , 0xB3 , 0xEA , 0x5C , 0xD1 , 0x7E , 0xA8 , 0x90 , 0x88 , 0x8C , 0xDD , 0x6A , 0x80 , 0xBB , 0xB1 , 0xDF , 0xC1 , 0xF7 , 0x0D , 0xAA , 0x32 , 0xF0 , 0xB7 , 0x7C , 0xCB , 0x88 , 0x80 , 0x0E , 0x8B , 0x64 , 0xB0 , 0xBE , 0x4C , 0xD6 , 0x0E , 0x9B , 0x8C , 0x1E , 0x2A , 0x64 , 0xE1 , 0xF3 , 0x5C , 0xD7 , 0x76 , 0x01 , 0x41 , 0x5E , 0x93 , 0x5C , 0x94 , 0xFE , 0xDD , 0x46 , 0x62 , 0xC3 , 0x1B , 0x5A , 0xE2 , 0xA0 , 0xBC , 0x2D , 0xEB , 0xC3 , 0x98 , 0x0A , 0xA7 , 0xB7 , 0x85 , 0x69 , 0x70 , 0x68 , 0x2B , 0x64 , 0x4A , 0xB3 , 0x1F , 0xCC , 0x7D , 0xDC , 0x7C , 0x26 , 0xF4 , 0x77 , 0xF6 , 0x5C , 0xF2 , 0xAE , 0x5A , 0x44 , 0x2D , 0xD3 , 0xAB , 0x16 , 0x62 , 0x04 , 0x19 , 0xBA , 0xFB , 0x90 , 0xFF , 0xE2 , 0x30 , 0x50 , 0x89 , 0x6E , 0xCB , 0x56 , 0xB2 , 0xEB , 0xC0 , 0x91 , 0x16 , 0x92 , 0x5E , 0x30 , 0x8E , 0xAE , 0xC7 , 0x94 , 0x5D , 0xFD , 0x35 , 0xE1 , 0x20 , 0xF8 , 0xAD , 0x3E , 0xBC , 0x08 , 0xBF , 0xC0 , 0x36 , 0x74 , 0x9F , 0xD5 , 0xBB , 0x52 , 0x08 , 0xFD , 0x06 , 0x66 , 0xF3 , 0x7A , 0xB3 , 0x04 , 0xF4 , 0x75 , 0x29 , 0x5D , 0xE9 , 0x5F , 0xAA , 0x10 , 0x30 , 0xB2 , 0x0F , 0x5A , 0x1A , 0xC1 , 0x2A , 0xB3 , 0xFE , 0xCB , 0x21 , 0xAD , 0x80 , 0xEC , 0x8F , 0x20 , 0x09 , 0x1C , 0xDB , 0xC5 , 0x58 , 0x94 , 0xC2 , 0x9C , 0xC6 , 0xCE , 0x82 , 0x65 , 0x3E , 0x57 , 0x90 , 0xBC , 0xA9 , 0x8B , 0x06 , 0xB4 , 0xF0 , 0x72 , 0xF6 , 0x77 , 0xDF , 0x98 , 0x64 , 0xF1 , 0xEC , 0xFE , 0x37 , 0x2D , 0xBC , 0xAE , 0x8C , 0x08 , 0x81 , 0x1F , 0xC3 , 0xC9 , 0x89 , 0x1A , 0xC7 , 0x42 , 0x82 , 0x4B , 0x2E , 0xDC , 0x8E , 0x8D , 0x73 , 0xCE , 0xB1 , 0xCC , 0x01 , 0xD9 , 0x08 , 0x70 , 0x87 , 0x3C , 0x44 , 0x08 , 0xEC , 0x49 , 0x8F , 0x81 , 0x5A , 0xE2 , 0x40 , 0xFF , 0x77 , 0xFC , 0x0D ,
                    };


                    public static byte[] DP = new byte[128]
                    {
                        0x52 , 0xCC , 0x2D , 0xA0 , 0x9C , 0x9E , 0x75 , 0xE7 , 0x28 , 0xEE , 0x3D , 0xDE , 0xE3 , 0x45 , 0xD1 , 0x4F , 0x94 , 0x1C , 0xCC , 0xC8 , 0x87 , 0x29 , 0x45 , 0x3B , 0x8D , 0x6E , 0xAB , 0x6E , 0x2A , 0xA7 , 0xC7 , 0x15 , 0x43 , 0xA3 , 0x04 , 0x8F , 0x90 , 0x5F , 0xEB , 0xF3 , 0x38 , 0x4A , 0x77 , 0xFA , 0x36 , 0xB7 , 0x15 , 0x76 , 0xB6 , 0x01 , 0x1A , 0x8E , 0x25 , 0x87 , 0x82 , 0xF1 , 0x55 , 0xD8 , 0xC6 , 0x43 , 0x2A , 0xC0 , 0xE5 , 0x98 , 0xC9 , 0x32 , 0xD1 , 0x94 , 0x6F , 0xD9 , 0x01 , 0xBA , 0x06 , 0x81 , 0xE0 , 0x6D , 0x88 , 0xF2 , 0x24 , 0x2A , 0x25 , 0x01 , 0x64 , 0x5C , 0xBF , 0xF2 , 0xD9 , 0x99 , 0x67 , 0x3E , 0xF6 , 0x72 , 0xEE , 0xE4 , 0xE2 , 0x33 , 0x5C , 0xF8 , 0x00 , 0x40 , 0xE3 , 0x2A , 0x9A , 0xF4 , 0x3D , 0x22 , 0x86 , 0x44 , 0x3C , 0xFB , 0x0A , 0xA5 , 0x7C , 0x3F , 0xCC , 0xF5 , 0xF1 , 0x16 , 0xC4 , 0xAC , 0x88 , 0xB4 , 0xDE , 0x62 , 0x94 , 0x92 , 0x6A , 0x13 ,
                    };

                    public static byte[] DQ = new byte[128]
                    {
                        0x7C , 0x9D , 0xAD , 0x39 , 0xE0 , 0xD5 , 0x60 , 0x14 , 0x94 , 0x48 , 0x19 , 0x7F , 0x88 , 0x95 , 0xD5 , 0x8B , 0x80 , 0xAD , 0x85 , 0x8A , 0x4B , 0x77 , 0x37 , 0x85 , 0xD0 , 0x77 , 0xBB , 0xBF , 0x89 , 0x71 , 0x4A , 0x72 , 0xCB , 0x72 , 0x68 , 0x38 , 0xEC , 0x02 , 0xC6 , 0x7D , 0xC6 , 0x44 , 0x06 , 0x33 , 0x51 , 0x1C , 0xC0 , 0xFF , 0x95 , 0x8F , 0x0D , 0x75 , 0xDC , 0x25 , 0xBB , 0x0B , 0x73 , 0x91 , 0xA9 , 0x6D , 0x42 , 0xD8 , 0x03 , 0xB7 , 0x68 , 0xD4 , 0x1E , 0x75 , 0x62 , 0xA3 , 0x70 , 0x35 , 0x79 , 0x78 , 0x00 , 0xC8 , 0xF5 , 0xEF , 0x15 , 0xB9 , 0xFC , 0x4E , 0x47 , 0x5A , 0xC8 , 0x70 , 0x70 , 0x5B , 0x52 , 0x98 , 0xC0 , 0xC2 , 0x58 , 0x4A , 0x70 , 0x96 , 0xCC , 0xB8 , 0x10 , 0xE1 , 0x2F , 0x78 , 0x8B , 0x2B , 0xA1 , 0x7F , 0xF9 , 0xAC , 0xDE , 0xF0 , 0xBB , 0x2B , 0xE2 , 0x66 , 0xE3 , 0x22 , 0x92 , 0x31 , 0x21 , 0x57 , 0x92 , 0xC4 , 0xB8 , 0xF2 , 0x3E , 0x76 , 0x20 , 0x37 ,
                    };

                    public static byte[] QP = new byte[128]
                    {
                        0x45 , 0x97 , 0x55 , 0xD4 , 0x22 , 0x08 , 0x5E , 0xF3 , 0x5C , 0xB4 , 0x05 , 0x7A , 0xFD , 0xAA , 0x42 , 0x42 , 0xAD , 0x9A , 0x8C , 0xA0 , 0x6C , 0xBB , 0x1D , 0x68 , 0x54 , 0x54 , 0x6E , 0x3E , 0x32 , 0xE3 , 0x53 , 0x73 , 0x76 , 0xF1 , 0x3E , 0x01 , 0xEA , 0xD3 , 0xCF , 0xEB , 0xEB , 0x23 , 0x3E , 0xC0 , 0xBE , 0xCE , 0xEC , 0x2C , 0x89 , 0x5F , 0xA8 , 0x27 , 0x3A , 0x4C , 0xB7 , 0xE6 , 0x74 , 0xBC , 0x45 , 0x4C , 0x26 , 0xC8 , 0x25 , 0xFF , 0x34 , 0x63 , 0x25 , 0x37 , 0xE1 , 0x48 , 0x10 , 0xC1 , 0x93 , 0xA6 , 0xAF , 0xEB , 0xBA , 0xE3 , 0xA2 , 0xF1 , 0x3D , 0xEF , 0x63 , 0xD8 , 0xF4 , 0xFD , 0xD3 , 0xEE , 0xE2 , 0x5D , 0xE9 , 0x33 , 0xCC , 0xAD , 0xBA , 0x75 , 0x5C , 0x85 , 0xAF , 0xCE , 0xA9 , 0x3D , 0xD1 , 0xA2 , 0x17 , 0xF3 , 0xF6 , 0x98 , 0xB3 , 0x50 , 0x8E , 0x5E , 0xF6 , 0xEB , 0x02 , 0x8E , 0xA1 , 0x62 , 0xA7 , 0xD6 , 0x2C , 0xEC , 0x91 , 0xFF , 0x15 , 0x40 , 0xD2 , 0xE3 ,
                    };
                }
                public class Index_Dat
                {
                    public static byte[] Key = new byte[32]
                    {
                        0xEE , 0xD5 , 0xA4 , 0xFF , 0xE8 , 0xA3 , 0xC9 , 0x10 , 0xDC , 0x1B , 0xFD , 0x6A , 0xAF , 0x13 , 0x82 , 0x25 , 0x0B , 0x38 , 0x0D , 0xBA , 0xE5 , 0x04 , 0x5D , 0x23 , 0x05 , 0x69 , 0x47 , 0x3F , 0x46 , 0xB0 , 0x7B , 0x1F ,
                    };

                    public static byte[] IV = new byte[16]
                    {
                        0x3A , 0xCB , 0x38 , 0xC1 , 0xEC , 0x12 , 0x11 , 0x9D , 0x56 , 0x92 , 0x9F , 0x49 , 0xF7 , 0x04 , 0x15 , 0xFF ,
                    };
                    public int Flag = 8;
                }
                public class HMAC_SHA256_Patch_Pkg_URL_Key
                {
                    public static byte[] Key = new byte[32]
                    {
                        0xAD , 0x62 , 0xE3 , 0x7F , 0x90 , 0x5E , 0x06 , 0xBC , 0x19 , 0x59 , 0x31 , 0x42 , 0x28 , 0x1C , 0x11 , 0x2C , 0xEC , 0x0E , 0x7E , 0xC3 , 0xE9 , 0x7E , 0xFD , 0xCA , 0xEF , 0xCD , 0xBA , 0xAF , 0xA6 , 0x37 , 0x8D , 0x84 ,
                    };
                }
                public class RSA_2048_HID_Config_Service_Signature_Verification_Public_Key
                {
                    public static byte[] Key = new byte[256]
                    {
                        0xEF , 0x27 , 0x69 , 0x15 , 0xB7 , 0x82 , 0x2A , 0xDF , 0x5D , 0x8E , 0xA7 , 0xDF , 0x90 , 0x94 , 0xAD , 0x0E , 0xF2 , 0xC7 , 0x2B , 0xB9 , 0xC0 , 0x8F , 0xFA , 0xC5 , 0x8F , 0xEA , 0x3A , 0x07 , 0x50 , 0x5A , 0x4B , 0x2D , 0x61 , 0x0E , 0xEE , 0x58 , 0x9D , 0xBA , 0xC9 , 0x67 , 0xD0 , 0x8B , 0x96 , 0xFB , 0xC0 , 0x5A , 0xC8 , 0x11 , 0x1F , 0x38 , 0x88 , 0x6D , 0xA9 , 0x94 , 0x09 , 0x94 , 0x0B , 0x78 , 0x64 , 0x91 , 0xFE , 0xCF , 0x0E , 0xA6 , 0x4C , 0x7F , 0x0F , 0x1E , 0x41 , 0x9B , 0x5B , 0xA4 , 0xD6 , 0x70 , 0x1F , 0x2E , 0x00 , 0x69 , 0xA0 , 0xE0 , 0xFF , 0xCB , 0x48 , 0x84 , 0x33 , 0x98 , 0x27 , 0xD4 , 0x4A , 0x78 , 0xCD , 0xC5 , 0x9E , 0x28 , 0x7A , 0x40 , 0xAB , 0xB2 , 0xA3 , 0xD2 , 0x6B , 0xAF , 0x99 , 0x69 , 0x3F , 0x8E , 0x23 , 0x76 , 0xA3 , 0x09 , 0xCF , 0xC5 , 0x2D , 0x2F , 0x11 , 0x67 , 0xF8 , 0xCD , 0x12 , 0x04 , 0xC6 , 0x6C , 0x94 , 0xDC , 0x54 , 0xC0 , 0x93 , 0x32 , 0x82 , 0xB1 , 0x2D , 0x03 , 0x62 , 0xA9 , 0x93 , 0xBF , 0xE9 , 0x95 , 0xD4 , 0x77 , 0x61 , 0x1B , 0xB7 , 0xB2 , 0x6F , 0xB3 , 0x4A , 0xC4 , 0xED , 0x47 , 0xC3 , 0xEF , 0xC6 , 0x2D , 0x8A , 0x93 , 0xB3 , 0x56 , 0x12 , 0x55 , 0x30 , 0x7D , 0xFA , 0xA1 , 0xDC , 0xA9 , 0x5E , 0xBA , 0x61 , 0x24 , 0x68 , 0xF3 , 0x47 , 0x1C , 0xEF , 0x85 , 0x48 , 0x68 , 0xDC , 0x03 , 0x5C , 0x74 , 0x44 , 0x2F , 0x21 , 0xF7 , 0x37 , 0x4A , 0xA6 , 0x2C , 0xEE , 0xAE , 0x5B , 0x9D , 0xD5 , 0xC1 , 0x04 , 0x9A , 0xDC , 0x70 , 0xB7 , 0xF3 , 0x67 , 0x8B , 0x37 , 0x34 , 0x0C , 0xD5 , 0xF8 , 0xBC , 0xC5 , 0x7B , 0x1C , 0x34 , 0x3D , 0x17 , 0x1C , 0x51 , 0x07 , 0x40 , 0xAD , 0x79 , 0x2C , 0x5C , 0x5C , 0x72 , 0x16 , 0x7E , 0xE2 , 0x95 , 0xF7 , 0x3E , 0xD2 , 0x37 , 0xF9 , 0xC9 , 0xD2 , 0xD0 , 0x6E , 0xD6 , 0x6F , 0x50 , 0x2F , 0x64 , 0x34 , 0xFE , 0xAD , 0x5B , 0x64 , 0xB1 , 0x06 , 0x23 , 0xC3 , 0xE7 , 0xE2 , 0x7B ,
                    };
                }
            };
        }

        public class Dualshock_4_Keys
        {
            //Speck ?
            public byte[] Bootloader = new byte[48]
            {
                0x39 , 0xFF , 0x1A , 0x67 , 0x2B , 0x4F , 0x99 , 0xA6 , 0xA1 , 0xCA , 0x65 , 0xC2 , 0x99 , 0xD6 , 0x27 , 0x0C , 0x7D , 0x4E , 0x1A , 0xF9 , 0x10 , 0x36 , 0xAD , 0x6C , 0x8D , 0x20 , 0xEA , 0xD1 , 0xFF , 0x33 , 0xD9 , 0x03 , 0x94 , 0xFD , 0x44 , 0x15 , 0xB5 , 0x40 , 0x72 , 0xD9 , 0xC8 , 0x3B , 0x94 , 0x99 , 0x43 , 0x04 , 0xFD , 0x49 ,
            };
            public byte[] App0 = new byte[64]
            {
                0x3E , 0x5C , 0x05 , 0xC6 , 0xAF , 0xAF , 0xAB , 0x02 , 0x20 , 0x3B , 0x3D , 0x18 , 0x17 , 0x33 , 0xDD , 0xCB , 0xA9 , 0x65 , 0x40 , 0x0F , 0xD5 , 0x3A , 0x6F , 0x50 , 0x17 , 0x31 , 0xF3 , 0x86 , 0x55 , 0xB2 , 0x08 , 0x08 , 0xCF , 0xB8 , 0xE6 , 0x18 , 0x1C , 0xC9 , 0x1D , 0x64 , 0xC4 , 0x99 , 0x3B , 0x04 , 0x0B , 0xEC , 0xC7 , 0xB5 , 0xED , 0x18 , 0xA5 , 0x68 , 0x3A , 0x95 , 0xA3 , 0x38 , 0xF3 , 0xCA , 0x32 , 0x55 , 0x28 , 0xA9 , 0x6F , 0xCB ,
            };

            public byte[] App1 = new byte[64]
            {
                0x7F , 0x81 , 0x48 , 0x8F , 0x32 , 0x02 , 0x4C , 0x6B , 0xF5 , 0xD9 , 0x99 , 0x92 , 0x87 , 0x98 , 0xAE , 0xC0 , 0x78 , 0x5F , 0xC3 , 0xE6 , 0x1B , 0xAF , 0x32 , 0xDF , 0xA5 , 0x83 , 0x3F , 0x43 , 0x49 , 0x64 , 0xCD , 0x53 , 0x37 , 0x52 , 0x52 , 0x39 , 0xB1 , 0x0B , 0xF8 , 0x38 , 0xEF , 0x29 , 0xB3 , 0x7E , 0xBD , 0x73 , 0xD9 , 0x51 , 0x1E , 0xC4 , 0xDF , 0xFB , 0x97 , 0x25 , 0xA1 , 0xE9 , 0xD2 , 0x67 , 0x89 , 0x90 , 0xA0 , 0x3C , 0x28 , 0x32 ,
            };

            public byte[] Certificate_Authority_Modulus = new byte[256]
            {
                0x8E , 0xD7 , 0xF9 , 0xE4 , 0xAA , 0x5C , 0xC5 , 0xD2 , 0x31 , 0x96 , 0xF0 , 0xDE , 0x79 , 0x7D , 0xFE , 0xAC , 0xF6 , 0x3E , 0xDE , 0x7B , 0xC9 , 0x67 , 0x16 , 0xF1 , 0x3C , 0xF5 , 0x2A , 0xDE , 0xF8 , 0xDA , 0xCF , 0xA8 , 0xE2 , 0x33 , 0xDC , 0x65 , 0x57 , 0x17 , 0x34 , 0x7D , 0x4C , 0x8C , 0x82 , 0x6E , 0xAB , 0x90 , 0x36 , 0x16 , 0xFF , 0x9F , 0xB8 , 0xF9 , 0x73 , 0x36 , 0x17 , 0xFB , 0xD4 , 0x4E , 0xC8 , 0x10 , 0x78 , 0xAD , 0x6E , 0x24 , 0xB0 , 0x62 , 0x61 , 0x9F , 0x5A , 0x17 , 0xEE , 0x2F , 0x55 , 0x72 , 0xB4 , 0x27 , 0xC0 , 0x34 , 0xA9 , 0x49 , 0x36 , 0x3E , 0x86 , 0xD3 , 0xB2 , 0x13 , 0x35 , 0x1F , 0x89 , 0x04 , 0xA4 , 0x99 , 0xF8 , 0x62 , 0x40 , 0x1F , 0x4E , 0x60 , 0xAC , 0x21 , 0x31 , 0xCD , 0x4B , 0xB9 , 0xFD , 0xDF , 0xD5 , 0x90 , 0xC8 , 0xE2 , 0x2B , 0x7D , 0xF9 , 0x6D , 0x01 , 0x5A , 0x41 , 0xC5 , 0x49 , 0xF3 , 0xEA , 0x0D , 0xED , 0xFC , 0x32 , 0xCE , 0xC3 , 0x2D , 0x72 , 0xC5 , 0x34 , 0x93 , 0x4A , 0xEF , 0x3D , 0xD1 , 0x2B , 0x58 , 0xDB , 0x35 , 0x7D , 0xD0 , 0x4D , 0x9A , 0x93 , 0x11 , 0xA3 , 0x83 , 0x3F , 0xF8 , 0x55 , 0x7A , 0x0B , 0x85 , 0xB4 , 0x54 , 0xCD , 0x21 , 0xDA , 0xB9 , 0x0D , 0x71 , 0x4A , 0xEA , 0x2D , 0xEC , 0x42 , 0xE6 , 0xF4 , 0xEF , 0x20 , 0x45 , 0x3C , 0xF6 , 0xDB , 0xF3 , 0x95 , 0x4E , 0x73 , 0xA8 , 0x76 , 0x91 , 0xCF , 0xA0 , 0x3F , 0x47 , 0x59 , 0x45 , 0x5C , 0x8B , 0x96 , 0xF1 , 0xD0 , 0xB6 , 0x9D , 0xD3 , 0xDD , 0x62 , 0x62 , 0xE9 , 0x43 , 0x8D , 0xCC , 0x26 , 0x96 , 0xCF , 0xE6 , 0x4B , 0x93 , 0x0C , 0x6E , 0x7D , 0x4E , 0x01 , 0x51 , 0xF6 , 0xD1 , 0xB1 , 0x5D , 0x1A , 0x4B , 0xE2 , 0xE6 , 0x0F , 0x0B , 0x36 , 0x11 , 0x8C , 0x60 , 0xF2 , 0x53 , 0xFD , 0xBC , 0xE2 , 0x27 , 0xA8 , 0xA4 , 0xC9 , 0xCD , 0xF2 , 0x26 , 0x08 , 0x58 , 0x58 , 0x4A , 0xB8 , 0xD7 , 0x1C , 0x62 , 0x9C , 0xD4 , 0x21 , 0xEC , 0x66 , 0x60 , 0x59 ,
            };
            public Int32 Exponent = 0x10001;

        }


    }

}
    


