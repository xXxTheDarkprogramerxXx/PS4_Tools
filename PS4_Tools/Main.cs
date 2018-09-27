using GameArchives;
using LibArchiveExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.IO.Compression;
using DiscUtils.Iso9660;

namespace PS4_Tools
{
    public class PS4_Tools
    {
        public static string AppCommonPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("PS4_Tools.dll", "");
        }
    }

    public class SELF
    {

    }

    public class Media
    { 
        
    }

    public class Image
    {
        /// <summary>
        /// PS4 PNG Image Handling
        /// </summary>
        public class PNG
        {
            /// <summary>
            /// Resize the image to the specified width and height.
            /// </summary>
            /// <param name="image">The image to resize.</param>
            /// <param name="width">The width to resize to.</param>
            /// <param name="height">The height to resize to.</param>
            /// <returns>The resized image.</returns>
            private static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
            {
                var destRect = new System.Drawing.Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }

            /// <summary>
            /// this convers images to 24bbp
            /// </summary>
            /// <param name="img"></param>
            /// <returns></returns>
            private static Bitmap ConvertTo24bpp(Bitmap img)
            {
                var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(bmp))
                    gr.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height));
                return bmp;
            }

            /// <summary>
            /// Converts a file location of a bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Original Bitmap Location</param>
            /// <param name="outputfile">Original Bitmap Location</param>
            public void Create_PS4_Compatible_PNG(string inputfile,string outputfile)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                returnbmp.Save(outputfile);//saves the file location
            }

            /// <summary>
            /// Converts a file location of a bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Original Bitmap Location</param>
            /// <param name="outputfile">Original Bitmap Location</param>
            /// <returns>PS4 Comptatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(string inputfile)
            {
                //read input file
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //reutrn new butmap
                return returnbmp;
            }

            /// <summary>
            /// Converts a Bitmap to a ps4 compatible one
            /// </summary>
            /// <param name="inputfile">Normal Bitmap</param>
            /// <returns>PS4 Comptatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(Bitmap inputfile)
            {
                //create new bitmap
                Bitmap returnbmp = inputfile;
                
                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //return ps4 compatible one
                return returnbmp;

            }

            /// <summary>
            /// Converts a stream to a ps4 compatible bitmap
            /// </summary>
            /// <param name="inputfile">Stream</param>
            /// <returns>PS4 COmpatible Bitmap</returns>
            public Bitmap Create_PS4_Compatible_PNG(Stream inputfile)
            {
                //create new bitmap
                Bitmap returnbmp = new Bitmap(inputfile);

                returnbmp = ResizeImage(returnbmp, 512, 512);//converts the image to the correct size
                returnbmp = ConvertTo24bpp(returnbmp);//converts image to 24bpp

                //return ps4 compatible one
                return returnbmp;
            }
        }

        /// <summary>
        /// PS4 DDS Image Handling
        /// </summary>
        public class DDS
        {
            public static void SavePNGFromDDS(string DDSFilePath,string savepath)
            {
                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(savepath);

            }

            public static Stream GetStreamFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();

                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(rtnStream);

                return rtnStream;

            }

            public static Bitmap GetBitmapFromDDS(string DDSFilePath)
            {
                Stream rtnStream = new MemoryStream();

                DDSReader.DDSImage image = new DDSReader.DDSImage(new FileStream(DDSFilePath, FileMode.Open, FileAccess.Read));
                image.Save(rtnStream);

                System.Drawing.Image rtnimage = System.Drawing.Image.FromStream(rtnStream);
                return (Bitmap)rtnimage;
            }
        }

    }

    public class RCO
    {

    }

    public class SaveData
    {

    }


    /*********************************************************
     *          PS4 PKG Reader by maxton  
     *          https://github.com/maxton/LibOrbisPkg
     *          
     *          And Official SCE Tools until we can fully 
     *          extract unencrypted file types
     *          
     *********************************************************/

    public class PKG
    {
        /// <summary>
        /// This Uses SCE Tools PLease Try and avoid this
        /// Will be intigrating maxtrons pkg tools
        /// </summary>
        /// <param name="FilePath">PS4 PKG File Path</param>
        /// <returns></returns>
        public static List<string> ReadAllUnprotectedData(string FilePath)
        {
            File.WriteAllBytes(PS4_Tools.AppCommonPath() + "ext.zip", Properties.Resources.ext);
            File.WriteAllBytes(PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);

            if (!Directory.Exists(PS4_Tools.AppCommonPath() + @"\ext\"))
            {          
               ZipFile.ExtractToDirectory(PS4_Tools.AppCommonPath() + "ext.zip", PS4_Tools.AppCommonPath());
            }
            


            List<string> rtnlist = new List<string>();

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
            start.Arguments = "img_file_list  --no_passcode --oformat recursive \""+FilePath+"\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                process.ErrorDataReceived += delegate {

                };
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    string[] splitresult = result.Split('\n');
                    for (int i = 0; i < splitresult.Length; i++)
                    {
                        rtnlist.Add(splitresult[i]);
                    }
                    
                   // return result;
                }
            }


            return rtnlist;
        }

        /// <summary>
        /// GP4 Project Class
        /// </summary>
        public class GP4
        {

            [XmlRoot(ElementName = "package")]
            public class Package
            {
                [XmlAttribute(AttributeName = "content_id")]
                public string Content_id { get; set; }
                [XmlAttribute(AttributeName = "passcode")]
                public string Passcode { get; set; }
                [XmlAttribute(AttributeName = "storage_type")]
                public string Storage_type { get; set; }
                [XmlAttribute(AttributeName = "app_type")]
                public string App_type { get; set; }
            }

            [XmlRoot(ElementName = "chunk")]
            public class Chunk
            {
                [XmlAttribute(AttributeName = "id")]
                public string Id { get; set; }
                [XmlAttribute(AttributeName = "layer_no")]
                public string Layer_no { get; set; }
                [XmlAttribute(AttributeName = "label")]
                public string Label { get; set; }
            }

            [XmlRoot(ElementName = "chunks")]
            public class Chunks
            {
                [XmlElement(ElementName = "chunk")]
                public Chunk Chunk { get; set; }
            }

            [XmlRoot(ElementName = "scenario")]
            public class Scenario
            {
                [XmlAttribute(AttributeName = "id")]
                public string Id { get; set; }
                [XmlAttribute(AttributeName = "type")]
                public string Type { get; set; }
                [XmlAttribute(AttributeName = "initial_chunk_count")]
                public string Initial_chunk_count { get; set; }
                [XmlAttribute(AttributeName = "label")]
                public string Label { get; set; }
                [XmlText]
                public string Text { get; set; }
            }

            [XmlRoot(ElementName = "scenarios")]
            public class Scenarios
            {
                [XmlElement(ElementName = "scenario")]
                public Scenario Scenario { get; set; }
                [XmlAttribute(AttributeName = "default_id")]
                public string Default_id { get; set; }
            }

            [XmlRoot(ElementName = "chunk_info")]
            public class Chunk_info
            {
                [XmlElement(ElementName = "chunks")]
                public Chunks Chunks { get; set; }
                [XmlElement(ElementName = "scenarios")]
                public Scenarios Scenarios { get; set; }
                [XmlAttribute(AttributeName = "chunk_count")]
                public string Chunk_count { get; set; }
                [XmlAttribute(AttributeName = "scenario_count")]
                public string Scenario_count { get; set; }
            }

            [XmlRoot(ElementName = "volume")]
            public class Volume
            {
                [XmlElement(ElementName = "volume_type")]
                public string Volume_type { get; set; }
                [XmlElement(ElementName = "volume_id")]
                public string Volume_id { get; set; }
                [XmlElement(ElementName = "volume_ts")]
                public string Volume_ts { get; set; }
                [XmlElement(ElementName = "package")]
                public Package Package { get; set; }
                [XmlElement(ElementName = "chunk_info")]
                public Chunk_info Chunk_info { get; set; }
            }

            [XmlRoot(ElementName = "file")]
            public class File
            {
                [XmlAttribute(AttributeName = "targ_path")]
                public string Targ_path { get; set; }
                [XmlAttribute(AttributeName = "orig_path")]
                public string Orig_path { get; set; }
            }

            [XmlRoot(ElementName = "files")]
            public class Files
            {
                [XmlElement(ElementName = "file")]
                public List<File> File { get; set; }
                [XmlAttribute(AttributeName = "img_no")]
                public string Img_no { get; set; }
            }

            [XmlRoot(ElementName = "dir")]
            public class Dir
            {
                [XmlAttribute(AttributeName = "targ_name")]
                public string Targ_name { get; set; }
            }

            [XmlRoot(ElementName = "rootdir")]
            public class Rootdir
            {
                [XmlElement(ElementName = "dir")]
                public List<Dir> Dir { get; set; }
            }

            [XmlRoot(ElementName = "psproject")]
            public class Psproject
            {
                [XmlElement(ElementName = "volume")]
                public Volume Volume { get; set; }
                [XmlElement(ElementName = "files")]
                public Files Files { get; set; }
                [XmlElement(ElementName = "rootdir")]
                public Rootdir Rootdir { get; set; }
                [XmlAttribute(AttributeName = "fmt")]
                public string Fmt { get; set; }
                [XmlAttribute(AttributeName = "version")]
                public string Version { get; set; }
            }

            /// <summary>
            /// Read a PS4 GP4 
            /// </summary>
            /// <param name="gp4filelocation">gp4 File Location</param>
            /// <returns></returns>
            public static Psproject ReadGP4(string gp4filelocation)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Psproject));
                using (FileStream fileStream = new FileStream(gp4filelocation, FileMode.Open))
                {
                    Psproject result = (Psproject)serializer.Deserialize(fileStream);
                    return result;
                }
            }

            /// <summary>
            /// Read a PS4 PG4
            /// </summary>
            /// <param name="GP4File">Gp4 File Stream</param>
            /// <returns></returns>
            public static Psproject ReadGP4(Stream GP4File)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Psproject));
                using (TextReader reader = new StreamReader(GP4File))
                {
                    Psproject result = (Psproject)serializer.Deserialize(reader);
                    return result;
                }
            }

            /// <summary>
            /// Save GP4
            /// </summary>
            /// <param name="savelocation">GP4 File Location</param>
            /// <param name="gp4project">gp4 Project</param>
            public static void SaveGP4(string savelocation,Psproject gp4project)
            {
                try
                {
                    var xmlserializer = new XmlSerializer(typeof(Psproject));
                    var stringWriter = new StringWriter();
                    using (var writer = XmlWriter.Create(stringWriter))
                    {
                        xmlserializer.Serialize(writer, gp4project);
                        string savestring  =stringWriter.ToString();
                        System.IO.File.WriteAllText(savelocation, savestring);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred", ex);
                }
            }

        }

        /// <summary>
        /// Class is used for PS2 Classic Building
        /// </summary>
        public class PS2_Classics
        {
            public PS2_Classics()
            {
                Working_Dir = PS4_Tools.AppCommonPath() + @"\Working\";
            }

            /// <summary>
            /// This is the temp directory where the app needs to work if not set by the users the app will set itself
            /// </summary>
            public string Working_Dir { get; set; }

            public string PS2ID { get; set; }

            /// <summary>
            /// Creates a single ISO Game For PS4
            /// </summary>
            /// <param name="PS2_ISO">PS2 ISo Location</param>
            /// <param name="SaveFileLOcation">Preferred pkg save location</param>
            /// <param name="Icon0Location">location of ICON0 if none set default is used</param>
            /// <param name="BackgroundLocation">Location of Background image if none is set default is used</param>
            /// <param name="CustomGP4Location">Use a custom GP4 File For Repacking the ISO</param>
            public void Create_Single_ISO_PKG(string PS2_ISO,string SaveFileLOcation,string Title,string ContentID ="" ,Bitmap Icon0 = null, string BackgroundLocation = "", string CustomGP4Location = "")
            {
                Console.WriteLine("Reading PS2 ISO");

                #region << CNF Reader >>

                //start off by reading the ISO FIle
                //we need to get the control file info
                //now using the file stream we can read the CNF file
                using (FileStream isoStream = File.OpenRead(PS2_ISO))
                {
                    //use disk utils to read iso file
                    CDReader cd = new CDReader(isoStream, true);
                    //look for the specific file naimly the system config file
                    Stream fileStream = cd.OpenFile(@"SYSTEM.CNF", FileMode.Open);
                    // Use fileStream...
                    TextReader tr = new StreamReader(fileStream);
                    string fullstring = tr.ReadToEnd();//read string to end this will read all the info we need

                    //mine for info
                    string Is = @"\";
                    string Ie = ";";

                    //mine the start and end of the string
                    int start = fullstring.ToString().IndexOf(Is) + Is.Length;
                    int end = fullstring.ToString().IndexOf(Ie, start);
                    if (end > start)
                    {
                        string PS2Id = fullstring.ToString().Substring(start, end - start);

                        if (PS2Id != string.Empty)
                        {
                            PS2ID = PS2Id.Replace(".", "");
                            Console.WriteLine("PS2 ID Found" + PS2Id);
                        }
                        else
                        {
                            Console.WriteLine("Could not load PS2 ID");
                            return;
                        }
                    }
                }

                #endregion << CNF Reader >>

                #region << Set Up Working Directory >>

                if (!Directory.Exists(Working_Dir))
                {
                    Console.WriteLine("Created " + Working_Dir);
                    Directory.CreateDirectory(Working_Dir);
                }
                if (!Directory.Exists(Working_Dir + @"\PS2Emu\"))
                {
                    Console.WriteLine("Created " + Working_Dir + @"\PS2Emu\");
                    Directory.CreateDirectory(Working_Dir + @"\PS2Emu\");
                }

                Console.WriteLine("Writing " + Working_Dir + @"\PS2Emu\" + "param.sfo");
                System.IO.File.WriteAllBytes(Working_Dir + @"\PS2Emu\" + "param.sfo", Properties.Resources.param);

                Console.WriteLine("Writing " + Working_Dir +"PS2Classics.gp4");
                System.IO.File.WriteAllBytes(Working_Dir + "PS2Classics.gp4", Properties.Resources.PS2Classics);

                #endregion << Set Up Wokring Directory >>

                #region << LOad and update gp4 and sfo Project >>
                Console.WriteLine("Loading GP4 Project");
                var project = GP4.ReadGP4(Working_Dir + "PS2Classics.gp4");
                Console.WriteLine("Loading SFO");
                var sfo = new Param_SFO.PARAM_SFO(Working_Dir + @"\PS2Emu\" + "param.sfo");
                
                if(ContentID == "")
                {
                    Console.WriteLine("No Content ID Specified Building custom one");
                    //build custom content id
                    ContentID = "UP9000-" + sfo.TitleID.Trim() + "_00-" + PS2ID.Trim() + "0000001";

                    Console.WriteLine("Content ID :"+ContentID);
                }
                //update sfo info

                Console.WriteLine("Updating SFO ");
                for (int i =0;i< sfo.Tables.Count;i++)
                {

                    if (sfo.Tables[i].Name == "CONTENT_ID")
                    {
                        var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Content ID ( "+tempitem.Value + " -> "+ContentID + ")");
                        tempitem.Value = ContentID;
                        sfo.Tables[i] = tempitem;
                    }
                    if(sfo.Tables[i].Name == "TITLE")
                    {
                         var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Title ( " + tempitem.Value + " -> " + Title + ")");
                        tempitem.Value = Title;
                        sfo.Tables[i] = tempitem;
                    }
                    if(sfo.Tables[i].Name == "TITLE_ID")
                    {
                        var tempitem = sfo.Tables[i];

                        Console.WriteLine("Updating SFO  Title ID ( " + tempitem.Value + " -> " + PS2ID + ")");
                        tempitem.Value = PS2ID;
                        sfo.Tables[i] = tempitem;
                    }
                }

                Console.WriteLine("Saving SFO");
                sfo.SaveSFO(sfo, Working_Dir + @"\PS2Emu\" + "param.sfo");//update sfo info
                //update GP4
                
                Console.WriteLine("Upating GP4");
                project.Volume.Package.Content_id = ContentID;//set contentid
                project.Volume.Package.Passcode = "00000000000000000000000000000000";//32 zeros

                //this is single iso building so we shouldn't have to change disc image numbering
                GP4.SaveGP4(Working_Dir + "PS2Classics.gp4",project);

                Console.WriteLine("Saving GP4");
                #endregion << Load GP4 Project >>


                #region << Save Image Files to corresponding locations and also change to correct format >>

                Console.WriteLine("Saving Images");

                Image.PNG ps4icon0 = new Image.PNG();

                Bitmap icon0 = ps4icon0.Create_PS4_Compatible_PNG(Icon0);
                icon0.Save(PS4_Tools.AppCommonPath() + @"PS2\sce_sys\icon0.png", System.Drawing.Imaging.ImageFormat.Png);

                #endregion  << Save Image Files to corresponding locations and also change to correct format >>
            }



            /// <summary>
            /// Creates a Multi Iso Repacked PS2 Classic
            /// </summary>
            /// <param name="PS2_ISO_s">List of iso locations </param>
            /// <param name="SaveFileLOcation">Preferred pkg save location</param>
            /// <param name="Icon0Location">location of ICON0 if none set default is used</param>
            /// <param name="BackgroundLocation">Location of Background image if none is set default is used</param>
            /// <param name="CustomGP4Location">Use a custom GP4 File For Repacking the ISO</param>
            public void Create_Multi_ISO_PKG(List<string> PS2_ISO_s, string SaveFileLOcation, string Icon0Location = "", string BackgroundLocation = "", string CustomGP4Location = "")
            {
                throw new Exception("Function not built");
            }
        }

        /// <summary>
        /// Class is used for PSP HD Building
        /// </summary>
        public class PSP_HD
        {

        }

        public class IDS
        {

        }

        public class PARAM_SFO
        {
            public static Param_SFO.PARAM_SFO Get_Param_SFO(string pkgfile)
            {
                //PARAM_SFO 
                List<string> lstoffiles = ReadAllUnprotectedData(pkgfile);

                if (lstoffiles.Contains("Sc0/nptitle.dat\r"))
                {
                    if(!Directory.Exists(PS4_Tools.AppCommonPath() + @"\Working"))
                    {
                        Directory.CreateDirectory(PS4_Tools.AppCommonPath() + @"\Working");
                    }

                    //extract files to temp folder
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = PS4_Tools.AppCommonPath() + "orbis-pub-cmd.exe ";
                    start.Arguments = "img_extract --no_passcode \"" + pkgfile + "\" \"" +PS4_Tools.AppCommonPath() + @"Working" +"\"";
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    start.CreateNoWindow = true;
                    using (Process process = Process.Start(start))
                    {
                        process.ErrorDataReceived += delegate {

                        };
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                           

                            // return result;
                        }
                    }

                   return new Param_SFO.PARAM_SFO(PS4_Tools.AppCommonPath() + @"\Working\Sc0\Param.sfo");

                }
                return new Param_SFO.PARAM_SFO();
            }
        }

        public class NP_Data
        {

        }

        public class NP_Title
        {

        }

        public string pkgtable { get; set; }
        private static LibOrbisPkg.PKG.Pkg pkg = null;
        public static List<ListViewItem> lst = new List<ListViewItem>();

        /// <summary>
        /// Reads a pkg file and displays all its info inside the PKG Table 
        /// (Custom to maxtron)
        /// </summary>
        /// <param name="pkgFile"></param>
        public static void ReadPKG(string pkgFile)
        {
            var pkggame = GameArchives.Util.LocalFile(pkgFile);
            using (var stream = pkggame.GetStream())
                pkg = new LibOrbisPkg.PKG.PkgReader(stream).ReadPkg();
            //LibOrbisPkg.PKG.
            //GameArchives.PackageReader.ReadPackageFromFile(pkg);
            try
            {
                //var pkggame = GameArchives.Util.LocalFile(pkgFile);
                var package = PackageReader.ReadPackageFromFile(pkggame);
                var sfo = PackageReader.ReadPackageFromFile(package.GetFile("/param.sfo"));
                var innerPfs = PackageReader.ReadPackageFromFile(package.GetFile("/pfs_image.dat"));
                PackageManager.GetInstance();
            }
            catch (Exception ex)
            {

            }

            foreach (var e in pkg.Metas.Metas)
            {
                var lvi = new ListViewItem(new[] {
          e.id.ToString(),
          string.Format("0x{0:X}", e.DataSize),
          string.Format("0x{0:X}", e.DataOffset),
          e.Encrypted ? "Yes" : "No",
          e.KeyIndex.ToString(),
        });
                lvi.Tag = e;
                //entriesListView.Items.Add(lvi);
                lst.Add(lvi);
            }
        }
        public static void Rename_pkg_To_ContentID(string pkgfile)
        {

        }

    }
    /************************************
     * Credit TO IDC
     ************************************/
    public class PUP
    {
        public void Unpack_PUP(string PUPFile,string SaveToDir,bool SaveTables = false)
        {
            if(!File.Exists(PUPFile))
            {
                throw new Exception("PUP File location does not exist \nLocationSupplied " + PUPFile);
            }

            if(!Directory.Exists(SaveToDir))
            {
                throw new Exception("Save location does not exist \nLocationSupplied " + SaveToDir);
            }


            Unpacker unpacker = new Unpacker();
            unpacker.Unpack(PUPFile, SaveToDir, SaveTables);
        }
    }
}
