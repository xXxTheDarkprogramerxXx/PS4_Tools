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

namespace SquishNET
{
    /// <summary>
    /// Flags used when compressing/decompressing DXT data. These are the same values as defined in squish.h.
    /// </summary>
    [Flags]
    public enum SquishFlags
    {
        /// <summary>
        /// Use DXT1 compression.
        /// </summary>
        Dxt1 = (1 << 0),

        /// <summary>
        /// Use DXT3 compression.
        /// </summary>
        Dxt3 = (1 << 1),

        /// <summary>
        /// Use DXT5 compression.
        /// </summary>
        Dxt5 = (1 << 2),

        /// <summary>
        /// Use a very slow but very high quality colour compressor.
        /// </summary>
        ColourIterativeClusterFit = (1 << 8),

        /// <summary>
        /// Use a slow but high quality colour compressor (the default).
        /// </summary>
        ColourClusterFit = (1 << 3),

        /// <summary>
        /// Use a fast but low quality colour compressor.
        /// </summary>
        ColourRangeFit = (1 << 4),

        /// <summary>
        /// Use a perceptual metric for colour error (the default).
        /// </summary>
        ColourMetricPerceptual = (1 << 5),

        /// <summary>
        /// Use a uniform metric for colour error.
        /// </summary>
        ColourMetricUniform = (1 << 6),

        /// <summary>
        /// Weight the colour by alpha during cluster fit (disabled by default).
        /// </summary>
        WeightColourByAlpha = (1 << 7)
    }
}