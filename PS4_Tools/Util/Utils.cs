using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.CompilerServices;
using Ionic.Zip;

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


}
