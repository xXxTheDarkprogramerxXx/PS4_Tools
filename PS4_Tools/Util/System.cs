using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4_Tools
{
    public class Sys
    {
        /// <summary>
        /// Return if false if windows 
        /// </summary>
        public static bool isLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}
