using PS4_Tools.LibOrbis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4_Tools.Util.DDS
{
    public class BC5Unorm
    {

        public static byte[] Compress(byte[] buffer, ushort width, ushort height, int byteCount)
        {
            List<byte> compressed = new List<byte>();

            for (int pixY = 0, i = 0; pixY < height; pixY += 4)
            {
                for (int pixX = 0; pixX < width; pixX++)
                {

                    byte[] redPixels = new byte[16];
                    byte[] greenPixels = new byte[16];
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            redPixels[4 * y + x] = buffer[i + 4 * x + y * width];
                            greenPixels[4 * y + x] = buffer[i + 1 + 4 * x + y * width];
                        }
                    }
                    byte minRedColour = 255;
                    byte maxRedColour = 0;
                    byte minGreenColour = 255;
                    byte maxGreenColour = 0;

                    for (int j = 0; j < 16; j++)
                    {
                        minRedColour = Math.Min(minRedColour, redPixels[j]);
                        minGreenColour = Math.Min(minRedColour, greenPixels[j]);
                        maxRedColour = Math.Max(maxRedColour, redPixels[j]);
                        maxGreenColour = Math.Max(maxRedColour, greenPixels[j]);
                    }
                    redPixels = FlipBlockRows(redPixels);
                    greenPixels = FlipBlockRows(greenPixels);

                    byte[] redIndices = ConvertIndicesToBytes(CalculateIndices(redPixels, minRedColour, maxRedColour));
                    byte[] greenIndices = ConvertIndicesToBytes(CalculateIndices(greenPixels, minGreenColour, maxGreenColour));

                    compressed.Add(minRedColour);
                    compressed.Add(maxRedColour);
                    compressed.Add(redIndices[5]);
                    compressed.Add(redIndices[4]);
                    compressed.Add(redIndices[3]);
                    compressed.Add(redIndices[2]);
                    compressed.Add(redIndices[1]);
                    compressed.Add(redIndices[0]);

                    compressed.Add(minGreenColour);
                    compressed.Add(maxGreenColour);
                    compressed.Add(greenIndices[5]);
                    compressed.Add(greenIndices[4]);
                    compressed.Add(greenIndices[3]);
                    compressed.Add(greenIndices[2]);
                    compressed.Add(greenIndices[1]);
                    compressed.Add(greenIndices[0]);

                    i += 4 * 4;
                }
                i += width * 4 * 3;
            }
            return compressed.ToArray();
        }
        public static byte[] FlipBlockRows(byte[] pixels)
        {
            byte[] output = new byte[16];


            output[0] = pixels[8];
            output[1] = pixels[9];
            output[2] = pixels[10];
            output[3] = pixels[11];


            output[4] = pixels[12];
            output[5] = pixels[13];
            output[6] = pixels[14];
            output[7] = pixels[15];

            output[8] = pixels[0];
            output[9] = pixels[1];
            output[10] = pixels[2];
            output[11] = pixels[3];

            output[12] = pixels[4];
            output[13] = pixels[5];
            output[14] = pixels[6];
            output[15] = pixels[7];


            return output;
        }
        public static int[] CalculateIndices(byte[] pixels, byte minColour, byte maxColour)
        {

            byte[] colours = CalcColours(minColour, maxColour);
            int[] indices = new int[16];

            for (int i = 0; i < 16; i++)
            {
                int index = 0;
                int closestDiff = 255;
                for (int colour = 0; colour < 8; colour++)
                {
                    int diff = 0;
                    if (pixels[i] == colours[colour])
                    {
                        index = colour;
                        break;
                    }
                    if (pixels[i] < colours[colour])
                    {
                        diff = colours[colour] - pixels[i];
                    }
                    else if (pixels[i] > colours[colour])
                    {
                        diff = pixels[i] - colours[colour];
                    }
                    if (diff < closestDiff)
                    {
                        closestDiff = diff;
                        index = colour;
                    }
                }
                indices[i] = index;
            }
            return indices;

        }
        public static byte[] ConvertIndicesToBytes(int[] indices)
        {

            byte[] output = new byte[6];

            bool[] index7bits = GetBitsForIndex(indices[7]);
            output[0] = (byte)((index7bits[2] ? (byte)(1 << 7) : (byte)0) +
                        (index7bits[1] ? (byte)(1 << 6) : (byte)0) +
                        (index7bits[0] ? (byte)(1 << 5) : (byte)0));

            bool[] index6bits = GetBitsForIndex(indices[6]);
            output[0] += (byte)((index6bits[2] ? (byte)(1 << 4) : (byte)0) +
                        (index6bits[1] ? (byte)(1 << 3) : (byte)0) +
                        (index6bits[0] ? (byte)(1 << 2) : (byte)0));

            bool[] index5bits = GetBitsForIndex(indices[5]);
            output[0] += (byte)((index5bits[2] ? (byte)(1 << 1) : (byte)0) +
                        (index5bits[1] ? (byte)(1) : (byte)0));

            bool[] index4bits = GetBitsForIndex(indices[4]);
            output[1] = (byte)((index5bits[0] ? (byte)(1 << 7) : (byte)0) +
                        (index4bits[2] ? (byte)(1 << 6) : (byte)0) +
                        (index4bits[1] ? (byte)(1 << 5) : (byte)0) +
                        (index4bits[0] ? (byte)(1 << 4) : (byte)0));

            bool[] index3bits = GetBitsForIndex(indices[3]);
            output[1] += (byte)((index3bits[2] ? (byte)(1 << 3) : (byte)0) +
                        (index3bits[1] ? (byte)(1 << 2) : (byte)0) +
                        (index3bits[0] ? (byte)(1 << 1) : (byte)0));

            bool[] index2bits = GetBitsForIndex(indices[2]);
            output[1] += (byte)((index2bits[2] ? (byte)(1) : (byte)0));

            bool[] index1bits = GetBitsForIndex(indices[1]);
            bool[] index0bits = GetBitsForIndex(indices[0]);
            output[2] += (byte)((index2bits[1] ? (byte)(1 << 7) : (byte)0) +
                (index2bits[0] ? (byte)(1 << 6) : (byte)0) +
                (index1bits[2] ? (byte)(1 << 5) : (byte)0) +
                (index1bits[1] ? (byte)(1 << 4) : (byte)0) +
                (index1bits[0] ? (byte)(1 << 3) : (byte)0) +
                (index0bits[2] ? (byte)(1 << 2) : (byte)0) +
                (index0bits[1] ? (byte)(1 << 1) : (byte)0) +
                (index0bits[0] ? (byte)(1) : (byte)0));


            bool[] index15bits = GetBitsForIndex(indices[15]);
            output[3] = (byte)((index15bits[2] ? (byte)(1 << 7) : (byte)0) +
                        (index15bits[1] ? (byte)(1 << 6) : (byte)0) +
                        (index15bits[0] ? (byte)(1 << 5) : (byte)0));

            bool[] index14bits = GetBitsForIndex(indices[14]);
            output[3] += (byte)((index14bits[2] ? (byte)(1 << 4) : (byte)0) +
                        (index14bits[1] ? (byte)(1 << 3) : (byte)0) +
                        (index14bits[0] ? (byte)(1 << 2) : (byte)0));

            bool[] index13bits = GetBitsForIndex(indices[13]);
            output[3] += (byte)((index13bits[2] ? (byte)(1 << 1) : (byte)0) +
                        (index13bits[1] ? (byte)(1) : (byte)0));

            bool[] index12bits = GetBitsForIndex(indices[12]);
            output[4] = (byte)((index13bits[0] ? (byte)(1 << 7) : (byte)0) +
                        (index12bits[2] ? (byte)(1 << 6) : (byte)0) +
                        (index12bits[1] ? (byte)(1 << 5) : (byte)0) +
                        (index12bits[0] ? (byte)(1 << 4) : (byte)0));

            bool[] index11bits = GetBitsForIndex(indices[11]);
            output[4] += (byte)((index11bits[2] ? (byte)(1 << 3) : (byte)0) +
                        (index11bits[1] ? (byte)(1 << 2) : (byte)0) +
                        (index11bits[0] ? (byte)(1 << 1) : (byte)0));

            bool[] index10bits = GetBitsForIndex(indices[10]);
            output[4] += (byte)((index10bits[2] ? (byte)(1) : (byte)0));

            bool[] index9bits = GetBitsForIndex(indices[9]);
            bool[] index8bits = GetBitsForIndex(indices[8]);
            output[5] += (byte)((index10bits[1] ? (byte)(1 << 7) : (byte)0) +
                (index10bits[0] ? (byte)(1 << 6) : (byte)0) +
                (index9bits[2] ? (byte)(1 << 5) : (byte)0) +
                (index9bits[1] ? (byte)(1 << 4) : (byte)0) +
                (index9bits[0] ? (byte)(1 << 3) : (byte)0) +
                (index8bits[2] ? (byte)(1 << 2) : (byte)0) +
                (index8bits[1] ? (byte)(1 << 1) : (byte)0) +
                (index8bits[0] ? (byte)(1) : (byte)0));

            return output;

        }

        public static bool[] GetBitsForIndex(int index)
        {
            bool[] output = new bool[3];

            switch (index)
            {
                case 0:
                    output[0] = false;
                    output[1] = false;
                    output[2] = false;
                    break;
                case 1:
                    output[0] = true;
                    output[1] = false;
                    output[2] = false;
                    break;
                case 2:
                    output[0] = false;
                    output[1] = true;
                    output[2] = false;
                    break;
                case 3:
                    output[0] = true;
                    output[1] = true;
                    output[2] = false;
                    break;
                case 4:
                    output[0] = false;
                    output[1] = false;
                    output[2] = true;
                    break;
                case 5:
                    output[0] = true;
                    output[1] = false;
                    output[2] = true;
                    break;
                case 6:
                    output[0] = false;
                    output[1] = true;
                    output[2] = true;
                    break;
                case 7:
                    output[0] = true;
                    output[1] = true;
                    output[2] = true;
                    break;
            }

            return output;
        }
        public static byte[] CalcColours(byte minColour, byte maxColour)
        {
            byte[] output = new byte[8];

            output[0] = minColour;
            output[1] = maxColour;

            float colour0 = minColour;// / 255;
            float colour1 = maxColour;// / 255;

            if (minColour > maxColour)
            {
                output[2] = (byte)((6 * colour0 + 1 * colour1) / 7.0f);
                output[3] = (byte)((5 * colour0 + 2 * colour1) / 7.0f);
                output[4] = (byte)((4 * colour0 + 3 * colour1) / 7.0f);
                output[5] = (byte)((3 * colour0 + 4 * colour1) / 7.0f);
                output[6] = (byte)((2 * colour0 + 5 * colour1) / 7.0f);
                output[7] = (byte)((1 * colour0 + 6 * colour1) / 7.0f);
            }
            else
            {
                output[2] = (byte)((4 * colour0 + 1 * colour1) / 5.0f);
                output[3] = (byte)((3 * colour0 + 2 * colour1) / 5.0f);
                output[4] = (byte)((2 * colour0 + 3 * colour1) / 5.0f);
                output[5] = (byte)((1 * colour0 + 4 * colour1) / 5.0f);
                output[6] = 0;
                output[7] = 255;
            }

            return output;
        }
        public static uint[] GetIndices(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6)
        {
            int value1 = b3;
            value1 = value1 << 8;
            value1 += b2;
            value1 = value1 << 8;
            value1 += b1;
            int value2 = b6;
            value2 = value2 << 8;
            value2 += b5;
            value2 = value2 << 8;
            value2 += b4;
            value2 = value2 << 8;

            uint[] indices = new uint[16];
            /*
            indices[0] = MakeIndexFrom3Bits(GetBit(b3, 3), GetBit(b3, 2), GetBit(b3, 1)); //GetIndex(value1, 1); //(uint)(value1 & ((1 << 4) - 1));
            indices[1] = MakeIndexFrom3Bits(GetBit(b3, 6), GetBit(b3, 5), GetBit(b3, 4)); //GetIndex(value1, 2); //(uint)((value1 >> 3) & ((1 << 4) - 1));
            indices[2] = MakeIndexFrom3Bits(GetBit(b2, 1), GetBit(b3, 8), GetBit(b3, 7)); //GetIndex(value1, 3); //(uint)((value1 >> 6) & ((1 << 4) - 1));
            indices[3] = MakeIndexFrom3Bits(GetBit(b2, 4), GetBit(b2, 3), GetBit(b2, 2)); //GetIndex(value1, 4); //(uint)((value1 >> 9) & ((1 << 4) - 1));
            indices[4] = MakeIndexFrom3Bits(GetBit(b2, 7), GetBit(b2, 6), GetBit(b2, 5)); //GetIndex(value1, 5); //(uint)((value1 >> 12) & ((1 << 4) - 1));
            indices[5] = MakeIndexFrom3Bits(GetBit(b1, 2), GetBit(b1, 1), GetBit(b2, 8)); //GetIndex(value1, 6); //(uint)((value1 >> 15) & ((1 << 4) - 1));
            indices[6] = MakeIndexFrom3Bits(GetBit(b1, 5), GetBit(b1, 4), GetBit(b1, 3)); //GetIndex(value1, 7); //(uint)((value1 >> 18) & ((1 << 4) - 1));
            indices[7] = MakeIndexFrom3Bits(GetBit(b1, 8), GetBit(b1, 7), GetBit(b1, 6)); //GetIndex(value1, 8); //(uint)((value1 >> 21) & ((1 << 4) - 1));

            indices[8] = MakeIndexFrom3Bits(GetBit(b6, 3), GetBit(b6, 2), GetBit(b6, 1)); //GetIndex(value1, 1); // (uint)(value2 & ((1 << 4) - 1));
            indices[9] = MakeIndexFrom3Bits(GetBit(b6, 6), GetBit(b6, 5), GetBit(b6, 4)); //GetIndex(value1, 2); // (uint)((value2 >> 3) & ((1 << 4) - 1));
            indices[10] = MakeIndexFrom3Bits(GetBit(b5, 1), GetBit(b6, 8), GetBit(b6, 7)); //GetIndex(value1, 3); // (uint)((value2 >> 6) & ((1 << 4) - 1));
            indices[11] = MakeIndexFrom3Bits(GetBit(b5, 4), GetBit(b5, 3), GetBit(b5, 2)); //GetIndex(value1, 4); // (uint)((value2 >> 9) & ((1 << 4) - 1));
            indices[12] = MakeIndexFrom3Bits(GetBit(b5, 7), GetBit(b5, 6), GetBit(b5, 5)); //GetIndex(value1, 5); // (uint)((value2 >> 12) & ((1 << 4) - 1));
            indices[13] = MakeIndexFrom3Bits(GetBit(b4, 2), GetBit(b4, 1), GetBit(b5, 8)); //GetIndex(value1, 6); // (uint)((value2 >> 15) & ((1 << 4) - 1));
            indices[14] = MakeIndexFrom3Bits(GetBit(b4, 5), GetBit(b4, 4), GetBit(b4, 3)); //GetIndex(value1, 7); // (uint)((value2 >> 18) & ((1 << 4) - 1));
            indices[15] = MakeIndexFrom3Bits(GetBit(b4, 8), GetBit(b4, 7), GetBit(b4, 6)); //GetIndex(value1, 8); // (uint)((value2 >> 21) & ((1 << 4) - 1));
            */
            indices[0] = MakeIndexFrom3Bits(GetBit(b3, 3), GetBit(b3, 2), GetBit(b3, 1)); //GetIndex(value1, 1); //(uint)(value1 & ((1 << 4) - 1));
            indices[1] = MakeIndexFrom3Bits(GetBit(b3, 6), GetBit(b3, 5), GetBit(b3, 4)); //GetIndex(value1, 2); //(uint)((value1 >> 3) & ((1 << 4) - 1));
            indices[2] = MakeIndexFrom3Bits(GetBit(b2, 1), GetBit(b3, 8), GetBit(b3, 7)); //GetIndex(value1, 3); //(uint)((value1 >> 6) & ((1 << 4) - 1));
            indices[3] = MakeIndexFrom3Bits(GetBit(b2, 4), GetBit(b2, 3), GetBit(b2, 2)); //GetIndex(value1, 4); //(uint)((value1 >> 9) & ((1 << 4) - 1));
            indices[4] = MakeIndexFrom3Bits(GetBit(b2, 7), GetBit(b2, 6), GetBit(b2, 5)); //GetIndex(value1, 5); //(uint)((value1 >> 12) & ((1 << 4) - 1));
            indices[5] = MakeIndexFrom3Bits(GetBit(b1, 2), GetBit(b1, 1), GetBit(b2, 8)); //GetIndex(value1, 6); //(uint)((value1 >> 15) & ((1 << 4) - 1));
            indices[6] = MakeIndexFrom3Bits(GetBit(b1, 5), GetBit(b1, 4), GetBit(b1, 3)); //GetIndex(value1, 7); //(uint)((value1 >> 18) & ((1 << 4) - 1));
            indices[7] = MakeIndexFrom3Bits(GetBit(b1, 8), GetBit(b1, 7), GetBit(b1, 6)); //GetIndex(value1, 8); //(uint)((value1 >> 21) & ((1 << 4) - 1));

            indices[8] = MakeIndexFrom3Bits(GetBit(b6, 3), GetBit(b6, 2), GetBit(b6, 1)); //GetIndex(value1, 1); // (uint)(value2 & ((1 << 4) - 1));
            indices[9] = MakeIndexFrom3Bits(GetBit(b6, 6), GetBit(b6, 5), GetBit(b6, 4)); //GetIndex(value1, 2); // (uint)((value2 >> 3) & ((1 << 4) - 1));
            indices[10] = MakeIndexFrom3Bits(GetBit(b5, 1), GetBit(b6, 8), GetBit(b6, 7)); //GetIndex(value1, 3); // (uint)((value2 >> 6) & ((1 << 4) - 1));
            indices[11] = MakeIndexFrom3Bits(GetBit(b5, 4), GetBit(b5, 3), GetBit(b5, 2)); //GetIndex(value1, 4); // (uint)((value2 >> 9) & ((1 << 4) - 1));
            indices[12] = MakeIndexFrom3Bits(GetBit(b5, 7), GetBit(b5, 6), GetBit(b5, 5)); //GetIndex(value1, 5); // (uint)((value2 >> 12) & ((1 << 4) - 1));
            indices[13] = MakeIndexFrom3Bits(GetBit(b4, 2), GetBit(b4, 1), GetBit(b5, 8)); //GetIndex(value1, 6); // (uint)((value2 >> 15) & ((1 << 4) - 1));
            indices[14] = MakeIndexFrom3Bits(GetBit(b4, 5), GetBit(b4, 4), GetBit(b4, 3)); //GetIndex(value1, 7); // (uint)((value2 >> 18) & ((1 << 4) - 1));
            indices[15] = MakeIndexFrom3Bits(GetBit(b4, 8), GetBit(b4, 7), GetBit(b4, 6)); //GetIndex(value1, 8); // (uint)((value2 >> 21) & ((1 << 4) - 1));

            return indices;

        }

        public static uint MakeIndexFrom3Bits(bool mostSigBit, bool midBit, bool leastSigBit)
        {
            return MakeIndexFrom3Bits(mostSigBit, midBit, leastSigBit, false);
        }
        public static uint MakeIndexFrom3Bits(bool mostSigBit, bool midBit, bool leastSigBit, bool flip)
        {
            if (flip) return (uint)((leastSigBit ? 1 << 2 : 0) + (midBit ? 1 << 1 : 0) + (mostSigBit ? 1 : 0));
            return (uint)((mostSigBit ? 1 << 2 : 0) + (midBit ? 1 << 1 : 0) + (leastSigBit ? 1 : 0));
        }
        public static bool GetBit(byte b, int n)
        {
            return (b & (1 << n - 1)) != 0;
        }

        public static uint GetIndex(int packedIndex, int IndexNum)
        {
            uint value = 0;
            int index3 = IndexNum + 2;
            int index2 = IndexNum + 1;
            int index1 = IndexNum;
            value = (uint)(((packedIndex & (1 << index1 - 1)) != 0 ? 1 << 2 : 0) +
                    ((packedIndex & (1 << index2 - 1)) != 0 ? 1 << 1 : 0) +
                    ((packedIndex & (1 << index3 - 1)) != 0 ? 1 : 0));

            return value;

        }
    }

}
