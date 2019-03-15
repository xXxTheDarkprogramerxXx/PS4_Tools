using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4_Tools.LibOrbis.Util
{

    public static class ArrayExtensions
    {
        public static T[] Fill<T>(this T[] arr, T val)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = val;
            }
            return arr;
        }
    }

}
