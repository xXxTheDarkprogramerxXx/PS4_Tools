using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS2_DB_Controler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string GetHtmlCode(string GameName)
        {
            try
            {

                string url = "https://www.google.com/search?q=" + GameName + " PS2 Box Art" + "&tbs=iszw:850,iszh:955,isz:lt,islt:qsvga&tbm=isch&source=lnt";// + "&tbm=isch";//&source=lnt&tbs=isz:ex,iszw:850,iszh:955";
                string data = "";

                Uri uri = new Uri(url);

                var request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

                var response = (HttpWebResponse)request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return "";
                    using (var sr = new StreamReader(dataStream))
                    {
                        data = sr.ReadToEnd();
                    }
                }
                return data;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private List<string> GetUrls(string html)
        {
            try
            {
                var urls = new List<string>();

                int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

                while (ndx >= 0)
                {
                    ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                    ndx++;
                    int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                    string url = html.Substring(ndx, ndx2 - ndx);
                    urls.Add(url);
                    ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
                }
                return urls;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private byte[] GetImage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return null;
                    using (var sr = new BinaryReader(dataStream))
                    {
                        byte[] bytes = sr.ReadBytes(100000000);

                        return bytes;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private static byte[] HexString2Bytes(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        private static string Bytes2HexString(byte[] buffer)
        {
            var hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //foreach add closest image 
            string[] lines = File.ReadAllLines(Application.StartupPath + @"\PS2DB.d");
            List<string> newlist = new List<string>();
            foreach (var line in lines)
            {
                var cols = line.Split(';');
                if (cols.Count() > 2)
                {
                    //add each line item 
                    //add image
                    string gamename = cols[0].ToString();

                    string html = GetHtmlCode(gamename);
                    List<string> urls = GetUrls(html);
                    if (urls == null)
                        break;
                    if (urls.Count > 0)
                    {
                        string luckyUrl = urls[0];


                        newlist.Add(line + ";" + luckyUrl);


                        //newlist.Add()




                    }
                }
            }

            //write all items to file
            File.WriteAllLines(Application.StartupPath + @"\ps2dbnew.d", newlist.ToArray());
        }
    }
}
