using HtmlAgilityPack;
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

                string url = "https://www.google.com/search?q=" + GameName + " PS4 Cover&tbs=iszw:850,iszh:955,isz:lt,islt:qsvga&tbm=isch&source=lnt";//Box Art" //+ "&tbs=iszw:850,iszh:955,isz:lt,islt:qsvga&tbm=isch&source=lnt";// + "&tbm=isch";//&source=lnt&tbs=isz:ex,iszw:850,iszh:955";
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

        private string GetHtmlCodePSX(string GameName)
        {
            try
            {

                string url = "https://www.google.com/search?q=" + GameName + " PSX Box Art" + "&tbs=iszw:850,iszh:955,isz:lt,islt:qsvga&tbm=isch&source=lnt";// + "&tbm=isch";//&source=lnt&tbs=isz:ex,iszw:850,iszh:955";
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
        private List<string> GetUrlsPSX(string html)
        {
            var urls = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                urls.Add(att.Value);

            }
            return urls;
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
                if (cols.Count() > 2 && cols.Count() < 4)
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
                        File.WriteAllLines(Application.StartupPath + @"\ps2dbnew.d", newlist.ToArray());

                        //newlist.Add()




                    }
                }
            }

            //write all items to file
            File.WriteAllLines(Application.StartupPath + @"\ps2dbnew.d", newlist.ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //foreach add closest image 
            string[] lines = File.ReadAllLines(Application.StartupPath + @"\PSXDB.d");
            List<string> newlist = new List<string>();
            foreach (var line in lines)
            {
                var cols = line.Split(';');
                if (cols.Count() > 2 && cols.Count() < 4)
                {
                    //add each line item 
                    //add image
                    string gamename = cols[0].ToString();

                    string html = GetHtmlCodePSX(gamename);
                    List<string> urls = GetUrlsPSX(html);
                    if (urls == null)
                        break;
                    if (urls.Count > 0)
                    {
                        for (int i = 0; i < urls.Count; i++)
                        {
                            if (!urls[i].Contains("google") && urls[i].Contains("https://") /*&& urls[i].Contains(".jpg")*/)
                            {
                               
                                string luckyUrl = urls[i];

                                if (IsUrlImage(luckyUrl))
                                {
                                    newlist.Add(line + ";" + luckyUrl);
                                    File.WriteAllLines(Application.StartupPath + @"\psxdbnew.d", newlist.ToArray());
                                    break;
                                }
                                //}newlist.Add()
                            }



                        }
                    }
                }

                //write all items to file
                File.WriteAllLines(Application.StartupPath + @"\psxdbnew.d", newlist.ToArray());
            }
        }

        public static bool IsUrlImage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (!response.ContentType.Contains("text/html"))
                        {
                            using (var br = new BinaryReader(responseStream))
                            {
                                // e.g. test for a JPEG header here
                                var soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                                var jfif = br.ReadUInt16(); // JFIF marker (FFE0)
                                return soi == 0xd8ff && jfif == 0xe0ff;
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                //Trace.WriteLine(ex);
                return false;
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //foreach add closest image 
            string[] lines = File.ReadAllLines(@"C:\Users\3de Echelon\Pictures\JakAndDaxter\PS4GameNames.txt");
            List<string> newlist = new List<string>();
            foreach (var line in lines)
            {
               if(line != string.Empty)
                
                {
                    //add each line item 
                    //add image
                    string gamename = line;

                    string html = GetHtmlCode(gamename);
                    List<string> urls = GetUrls(html);
                    if (urls == null)
                        break;
                    if (urls.Count > 0)
                    {
                        string luckyUrl = urls[0];

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(luckyUrl, Application.StartupPath + @"\IMAGE\" + gamename);
                        }


                        // newlist.Add(line + ";" + luckyUrl);
                        // File.WriteAllLines(Application.StartupPath + @"\ps2dbnew.d", newlist.ToArray());

                        //newlist.Add()




                    }
                }
            }

            //write all items to file
            File.WriteAllLines(Application.StartupPath + @"\ps2dbnew.d", newlist.ToArray());
        }
    }
}
