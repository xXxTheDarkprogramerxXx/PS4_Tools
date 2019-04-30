#region License
/*
SquishNET is licensed under the MIT license.
Copyright © 2013 Matt Stevens

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.Runtime.InteropServices;

namespace SquishNET
{
    /// <summary>
    /// Wrapper for libsquish DXT compression/decompression library.
    /// </summary>
    public static class Squish
    {
        // Function delegates
        private delegate int GetStorageRequirementsDelegate(int width, int height, int flags);
        private delegate void CompressFunctionDelegate(IntPtr rgba, IntPtr block, int flags);
        private delegate void CompressMaskedDelegate(IntPtr rgba, int mask, IntPtr block, int flags);
        private delegate void DecompressDelegate(IntPtr rgba, IntPtr block, int flags);
        private delegate void CompressImageDelegate(IntPtr rgba, int width, int height, IntPtr blocks, int flags);
        private delegate void DecompressImageDelegate(IntPtr rgba, int width, int height, IntPtr blocks, int flags);

        // Function delegates
        private static GetStorageRequirementsDelegate GetStorageRequirementsFunction;
        private static CompressFunctionDelegate CompressFunction;
        private static CompressMaskedDelegate CompressMaskedFunction;
        private static DecompressDelegate DecompressFunction;
        private static CompressImageDelegate CompressImageFunction;
        private static DecompressImageDelegate DecompressImageFunction;

        static Squish()
        {
            if (IntPtr.Size == 8)
                Getx64Delegates();
            else
                Getx86Delegates();
        }

        // Get function pointers from x86 assembly
        private static void Getx86Delegates()
        {
            //GetStorageRequirementsFunction = NativeSquish_x86.Squish.GetStorageRequirements;
            //CompressFunction = NativeSquish_x86.Squish.Compress;
            //CompressMaskedFunction = NativeSquish_x86.Squish.CompressMasked;
            //DecompressFunction = NativeSquish_x86.Squish.Decompress;
            //CompressImageFunction = NativeSquish_x86.Squish.CompressImage;
            //DecompressImageFunction = NativeSquish_x86.Squish.DecompressImage;
        }

        // Get function pointers from x64 assembly
        private static void Getx64Delegates()
        {
            //GetStorageRequirementsFunction = NativeSquish_x64.Squish.GetStorageRequirements;
            //CompressFunction = NativeSquish_x64.Squish.Compress;
            //CompressMaskedFunction = NativeSquish_x64.Squish.CompressMasked;
            //DecompressFunction = NativeSquish_x64.Squish.Decompress;
            //CompressImageFunction = NativeSquish_x64.Squish.CompressImage;
            //DecompressImageFunction = NativeSquish_x64.Squish.DecompressImage;
        }

        /// <summary>
        /// Returns the final size in bytes of DXT data compressed with the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="width">Source image width.</param>
        /// <param name="height">Source image height.</param>
        /// <param name="flags">Compression parameters.</param>
        /// <returns>Size in bytes of the DXT data.</returns>
        public static int GetStorageRequirements(int width, int height, SquishFlags flags)
        {
            return GetStorageRequirementsFunction(width, height, (int)flags);
        }

        /// <summary>
        /// Compress a 4x4 pixel block using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Source RGBA block.</param>
        /// <param name="block">Output DXT compressed block.</param>
        /// <param name="flags">Compression flags.</param>
        public static void Compress(IntPtr rgba, IntPtr block, SquishFlags flags)
        {
            CompressFunction(rgba, block, (int)flags);
        }

        /// <summary>
        /// Compress a 4x4 pixel block using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Source RGBA block.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>Output DXT compressed block.</returns>
        public static byte[] Compress(byte[] rgba, SquishFlags flags)
        {
            byte[] compressedData = new byte[GetStorageRequirements(4, 4, flags)];

            GCHandle pinnedData = GCHandle.Alloc(compressedData, GCHandleType.Pinned);
            GCHandle pinnedRgba = GCHandle.Alloc(rgba, GCHandleType.Pinned);

            CompressFunction(pinnedRgba.AddrOfPinnedObject(), pinnedData.AddrOfPinnedObject(), (int)flags);

            pinnedRgba.Free();
            pinnedData.Free();

            return compressedData;
        }

        /// <summary>
        /// Compress a 4x4 pixel block using the parameters specified in <paramref name="flags"/>. The <paramref name="mask"/> parameter is a used as 
        /// a bit mask to specifify what pixels are valid for compression, corresponding the lowest bit to the first pixel.
        /// </summary>
        /// <param name="rgba">Source RGBA block.</param>
        /// <param name="mask">Pixel bit mask.</param>
        /// <param name="block">Output DXT compressed block.</param>
        /// <param name="flags">Compression flags.</param>
        public static unsafe void CompressMasked(IntPtr rgba, int mask, IntPtr block, SquishFlags flags)
        {
            CompressMaskedFunction(rgba, mask, block, (int)flags);
        }

        /// <summary>
        /// Compress a 4x4 pixel block using the parameters specified in <paramref name="flags"/>. The <paramref name="mask"/> parameter is a used as 
        /// a bit mask to specifify what pixels are valid for compression, corresponding the lowest bit to the first pixel.
        /// </summary>
        /// <param name="rgba">Source RGBA block.</param>
        /// <param name="mask">Pixel bit mask.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>Output DXT compressed block.</returns>
        public static byte[] CompressMasked(byte[] rgba, int mask, SquishFlags flags)
        {
            byte[] compressedData = new byte[GetStorageRequirements(4, 4, flags)];

            GCHandle pinnedData = GCHandle.Alloc(compressedData, GCHandleType.Pinned);
            GCHandle pinnedRgba = GCHandle.Alloc(rgba, GCHandleType.Pinned);

            CompressMaskedFunction(pinnedRgba.AddrOfPinnedObject(), mask, pinnedData.AddrOfPinnedObject(), (int)flags);

            pinnedRgba.Free();
            pinnedData.Free();

            return compressedData;
        }

        /// <summary>
        /// Decompresses a 4x4 pixel block using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Output RGBA decompressed block.</param>
        /// <param name="block">Source DXT block.</param>
        /// <param name="flags">Decompression flags.</param>
        public static unsafe void Decompress(IntPtr rgba, IntPtr block, SquishFlags flags)
        {
            DecompressFunction(rgba, block, (int)flags);
        }

        /// <summary>
        /// Decompresses a 4x4 pixel block using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="block">Source DXT block.</param>
        /// <param name="flags">Decompression flags.</param>
        /// <returns>Output RGBA decompressed block.</returns>
        public static byte[] Decompress(byte[] block, SquishFlags flags)
        {
            byte[] decompressedData = new byte[4 * 4 * 4];

            GCHandle pinnedData = GCHandle.Alloc(decompressedData, GCHandleType.Pinned);
            GCHandle pinnedBlock = GCHandle.Alloc(block, GCHandleType.Pinned);

            DecompressFunction(pinnedData.AddrOfPinnedObject(), pinnedBlock.AddrOfPinnedObject(), (int)flags);

            pinnedBlock.Free();
            pinnedData.Free();

            return decompressedData;
        }

        /// <summary>
        /// Compresses an image using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Source RGBA image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="blocks">Output DXT compressed image.</param>
        /// <param name="flags">Compression flags.</param>
        public static unsafe void CompressImage(IntPtr rgba, int width, int height, IntPtr blocks, SquishFlags flags)
        {
            CompressImageFunction(rgba, width, height, blocks, (int)flags);
        }

        /// <summary>
        /// Compresses an image using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Source RGBA image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>Output DXT compressed image.</returns>
        public static byte[] CompressImage(byte[] rgba, int width, int height, SquishFlags flags)
        {
            byte[] compressedData = new byte[GetStorageRequirements(width, height, flags)];

            GCHandle pinnedData = GCHandle.Alloc(compressedData, GCHandleType.Pinned);
            GCHandle pinnedRgba = GCHandle.Alloc(rgba, GCHandleType.Pinned);

            CompressImageFunction(pinnedRgba.AddrOfPinnedObject(), width, height, pinnedData.AddrOfPinnedObject(), (int)flags);

            pinnedRgba.Free();
            pinnedData.Free();

            return compressedData;
        }

        /// <summary>
        /// Decompresses an image using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="rgba">Output RGBA decompressed image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="blocks">Source DXT compressed image.</param>
        /// <param name="flags">Decompression flags.</param>
        public static unsafe void DecompressImage(IntPtr rgba, int width, int height, IntPtr blocks, SquishFlags flags)
        {
            DecompressImageFunction(rgba, width, height, blocks, (int)flags);
        }

        /// <summary>
        /// Decompresses an image using the parameters specified in <paramref name="flags"/>.
        /// </summary>
        /// <param name="blocks">Source DXT compressed image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="flags">Decompression flags.</param>
        /// <returns>Output RGBA decompressed image.</returns>
        public static byte[] DecompressImage(byte[] blocks, int width, int height, SquishFlags flags)
        {
            byte[] decompressedData = new byte[width * height * 4];

            GCHandle pinnedData = GCHandle.Alloc(decompressedData, GCHandleType.Pinned);
            GCHandle pinnedBlocks = GCHandle.Alloc(blocks, GCHandleType.Pinned);

            DecompressImageFunction(pinnedData.AddrOfPinnedObject(), width, height, pinnedBlocks.AddrOfPinnedObject(), (int)flags);

            pinnedBlocks.Free();
            pinnedData.Free();

            return decompressedData;
        }
    }
}