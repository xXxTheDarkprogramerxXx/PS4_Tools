using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

/*CREDITS FOR THIS GOES TO https://github.com/darkautism*/

namespace PS4_Tools.TROPHY
{

    public class TROPCONF
    {
        long startByte = 0;
        string path;
        string trophyconf_version;
        public string npcommid;
        public string trophyset_version;
        public string parental_level;
        public string title_name;
        public string title_detail;
        public List<Trophy> trophys;
        public int Count
        {
            get
            {
                return trophys.Count;
            }
        }
        public Trophy this[int index]
        {
            get
            {
                return trophys[index];
            }
        }
        public TROPCONF(string path)
        {
            this.path = path;
            FileStream TROPCONFReader = null;
            if (path == null)
                throw new Exception("Path cannot be null!");

            if (!path.EndsWith(@"\"))
                path += @"\";

            if (!File.Exists(path + "TROPCONF.SFM"))
                throw new Exception("Cannot find TROPCONF.SFM.");

            try
            {
                TROPCONFReader = new FileStream(path + "TROPCONF.SFM", FileMode.Open);
            }
            catch (IOException)
            {
                throw new Exception("Cannot Open TROPCONF.SFM.");
            }

            TROPCONFReader.Position = startByte;
            byte[] data = new byte[TROPCONFReader.Length];
            TROPCONFReader.Read(data, 0, (int)TROPCONFReader.Length);
            string xml = Encoding.UTF8.GetString(data).Trim('\0');
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);
            XmlElement root = xmldoc.DocumentElement;

            // Fixed Data
            trophyconf_version = root.Attributes["version"].Value;
            npcommid = xmldoc.GetElementsByTagName("npcommid")[0].InnerText;
            trophyset_version = xmldoc.GetElementsByTagName("trophyset-version")[0].InnerText;
            parental_level = xmldoc.GetElementsByTagName("parental-level")[0].InnerText;
            title_name = xmldoc.GetElementsByTagName("title-name")[0].InnerText;
            title_detail = xmldoc.GetElementsByTagName("title-detail")[0].InnerText;

            // Trophys
            XmlNodeList trophysXML = xmldoc.GetElementsByTagName("trophy");
            trophys = new List<Trophy>();
            foreach (XmlNode trophy in trophysXML)
            {
                Trophy item = new Trophy(
                    int.Parse(trophy.Attributes["id"].Value),
                    trophy.Attributes["hidden"].Value,
                    trophy.Attributes["ttype"].Value,
                    int.Parse(trophy.Attributes["pid"].Value),
                    trophy["name"].InnerText,
                    trophy["detail"].InnerText
                    );


                trophys.Add(item);
            }


            TROPCONFReader.Close();
        }
        public static string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }
        public TROPCONF(byte[] TROPCONFbytes)
        {

            MemoryStream TROPCONFReader = null;
           
            try
            {
                TROPCONFReader = new MemoryStream(TROPCONFbytes);
            }
            catch (IOException)
            {
                throw new Exception("Cannot Open TROPCONF.SFM.");
            }

            //TROPCONFReader.Position = startByte;
            TROPCONFReader.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[TROPCONFReader.Length];
            TROPCONFReader.Read(data, 0, (int)TROPCONFReader.Length);
            string xml = Encoding.UTF8.GetString(data).Trim('\0');
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(CleanInvalidXmlChars(xml));
            XmlElement root = xmldoc.DocumentElement;

            // Fixed Data
            trophyconf_version = root.Attributes["version"].Value;
            npcommid = xmldoc.GetElementsByTagName("npcommid")[0].InnerText;
            trophyset_version = xmldoc.GetElementsByTagName("trophyset-version")[0].InnerText;
            parental_level = xmldoc.GetElementsByTagName("parental-level")[0].InnerText;
            try
            {
                title_name = xmldoc.GetElementsByTagName("title-name")[0].InnerText;
            }
            catch(Exception ex)
            {
                title_name = "";
            }
            try
            {
                title_detail = xmldoc.GetElementsByTagName("title-detail")[0].InnerText;
            }
            catch(Exception ex)
            {
                title_name = "";
            }

            // Trophys
            XmlNodeList trophysXML = xmldoc.GetElementsByTagName("trophy");
            trophys = new List<Trophy>();
            foreach (XmlNode trophy in trophysXML)
            {
                Trophy item = new Trophy(
                    int.Parse(trophy.Attributes["id"].Value),
                    trophy.Attributes["hidden"].Value,
                    trophy.Attributes["ttype"].Value,
                    int.Parse(trophy.Attributes["pid"].Value),"",""
                    //trophy["name"].InnerText,
                    //trophy["detail"].InnerText
                    );


                trophys.Add(item);
            }


            TROPCONFReader.Close();
        }

        public void PrintState()
        {
            Console.WriteLine(trophyconf_version);
            Console.WriteLine(npcommid);
            Console.WriteLine(trophyset_version);
            Console.WriteLine(parental_level);
            Console.WriteLine(title_name);
            Console.WriteLine(title_detail);

            foreach (Trophy t in trophys)
            {
                Console.WriteLine(t);
            }
        }
        public struct Trophy
        {
            public int id;
            public string hidden;
            /// <summary>
            /// P = 白金 B = 銅 S = 銀 G = 金
            /// </summary>
            public string ttype;
            public int pid;
            public string name;
            public string detail;
            public Trophy(int id, string hidden, string ttype, int pid, string name, string detail)
            {
                this.id = id;
                this.hidden = hidden;
                this.ttype = ttype;
                this.pid = pid;
                this.name = name;
                this.detail = detail;
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[").Append(id).Append(",");
                sb.Append(hidden).Append(",");
                sb.Append(ttype).Append(",");
                sb.Append(pid).Append(",");
                sb.Append(name).Append(",");
                sb.Append(detail).Append("]");
                return sb.ToString();
            }
        }
    }
}
