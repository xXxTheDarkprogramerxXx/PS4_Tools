using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.IO;

namespace DesignLibrary_Tutorial
{
    /// <summary>
    /// This Class is used to keep all our conversion code nice and tidy
    /// </summary>
    public class Converter
    {
        #region << From URL To >>

        /// <summary>
        /// Use this function when you want to get a drawable from a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Drawable</returns>
        public static Drawable drawableFromUrl(string url)
        {
            Bitmap x;

            Stream input = GetStreamFromUrl(url);

            x = BitmapFactory.DecodeStream(input);
            return new BitmapDrawable(x);
        }

        /// <summary>
        /// Use this function when you want to get a stream from a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>System.IO.Stream</returns>
        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData("http://" + url);

            return new MemoryStream(imageData);
        }

        #endregion << From URL To >>
    }
}