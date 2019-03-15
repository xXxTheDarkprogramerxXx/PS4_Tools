using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PS4_Tools.LibOrbis.Util
{
    /***********************************************
      * 
      * Similar to netfix.cs this class will be used 
      * to add extensions methods needed that are in
      * .net4+ and not in .net3.5
      * 
      * Date         ||      Extension        || Username
      * 2019-03-11        StreamExtensions       XDPX 
      * 2019-03-14        EnumExtensions         XDPX
      * 2019-03-15        BigIntigerExtension    XDPX
      ************************************************/
    static class StreamExtensions2
    {
        const int DefaultBufferSize = 4096;

        public static long CopyTo(this Stream source, Stream destination)
        {
            return CopyTo(source, destination, DefaultBufferSize, _ => { });
        }

        public static long CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            return CopyTo(source, destination, bufferSize, _ => { });
        }

        public static long CopyTo(this Stream source, Stream destination, Action<long> reportProgress)
        {
            return CopyTo(source, destination, DefaultBufferSize, reportProgress);
        }

        public static long CopyTo(this Stream source, Stream destination, int bufferSize, Action<long> reportProgress)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (reportProgress == null) throw new ArgumentNullException("reportProgress");

            var buffer = new byte[bufferSize];

            var transferredBytes = 0L;

            for (var bytesRead = source.Read(buffer, 0, buffer.Length); bytesRead > 0; bytesRead = source.Read(buffer, 0, buffer.Length))
            {
                transferredBytes += bytesRead;
                reportProgress(transferredBytes);

                destination.Write(buffer, 0, bytesRead);
            }

            destination.Flush();

            return transferredBytes;
        }
    }


    /// <summary>
    /// Extentions for enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// A FX 3.5 way to mimic the FX4 "HasFlag" method.
        /// </summary>
        /// <param name="variable">The tested enum.</param>
        /// <param name="value">The value to test.</param>
        /// <returns>True if the flag is set. Otherwise false.</returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            // check if from the same type.
            if (variable.GetType() != value.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            Convert.ToUInt64(value);
            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);

            return (num2 & num) == num;
        }
    }


    public static class BigIntigerExtensions
    {
        public static byte[] ToByteArray(this BigIntegerLibrary.BigInteger bi)
        {
            return new byte[90];
        }
    }
}
