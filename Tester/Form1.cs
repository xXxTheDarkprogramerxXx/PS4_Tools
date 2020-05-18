using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPUPExtract_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 PUP File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "pup";

            openFileDialog1.Filter = "PS4 PUP File (*.pup)|*.pup";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;



            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                //folderDlg.ShowNewFolderButton = true;
                //// Show the FolderBrowserDialog.  
                //if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    //PS4_Tools.PUP pupfunction = new PS4_Tools.PUP();
                    //pupfunction.Unpack_PUP(openFileDialog1.FileName, folderDlg.SelectedPath);
                    PS4_Tools.PUP pupfile = new PS4_Tools.PUP();
                    PS4_Tools.PUP.PlaystationUpdateFile pup = pupfile.Read_Pup(openFileDialog1.FileName);

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PS4_Tools.PKG.ReadPKG(@"C:\Users\3deEchelon\Desktop\IV0002-HXHB00111_00-DUMPFUN000000000-A0100-V0100.pkg");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button1.PerformClick();
            MessageBox.Show("This is a community project all items in this toolset was made by a lot of talented developers throughout the scene\n\nCredits are on the Github repo");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string path 

            //set base directory for PS4 project
            PS4_Tools.PKG.SceneRelated.GP4.Psproject project = PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(@"C:\Users\3deEchelon\Documents\Sony\Crash Bandcioot Twinsanity.gp4");
            if (project.Fmt != "gp4")
            {
                MessageBox.Show("This is not a valid PS4 Project");
                return;
            }

            //lets read the pkg content info 
            if (project.Volume.Package.Passcode.Length != 32)
            {
                MessageBox.Show("Passcode Lentgh is not valid");
            }




            //to save a gp4 

            PS4_Tools.PKG.SceneRelated.GP4.SaveGP4(@"C:\Users\3deEchelon\Documents\Sony\tempworking.gp4", project);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            /*Intigration with Maxtron's LibOrbis has begun */

            var temp = PS4_Tools.PKG.SceneRelated.ReadPKG(@"C:\Users\3deEchelon\Desktop\PS4\Batman.RETURN.TO.ARKHAM.ARKHAM.ASYLUM.PS4-DUPLEX\Batman.Return.to.Arkham.Arkham.Asylum.PS4-DUPLEX\duplex-batman.return.to.arkham.arkham.asylum\Batman.Return.to.Arkham.Arkham.Asylum.PS4-DUPLEX.pkg");


        }

        public static System.Drawing.Bitmap BytesToBitmap(byte[] ImgBytes)
        {
            System.Drawing.Bitmap result = null;
            if (ImgBytes != null)
            {
                MemoryStream stream = new MemoryStream(ImgBytes);
                result = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            }
            return result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            //var byteimage = PS4_Tools.Image.DDS.GetBytesFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            var byteimage = PS4_Tools.Image.DDS.GetBytesFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            var item = BytesToBitmap(byteimage);
            pictureBox1.Image = item;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //PS4_Tools.RCO.DumpRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");
            var file = PS4_Tools.RCO.ReadRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");
            //testing time 
            //write a file to server or whatever
            //we can dump them all if we want 2
            var image = file.FileTable.PNGFiles[0].FileBytes;
            System.IO.File.WriteAllBytes(@"C: \Users\3deEchelon\Desktop\PS4\RCO\img0.png", image);

            //here we can view spesific files and there file bytes are there if the need arrises
        }

        #region << Atrac9 player >>

        System.Media.SoundPlayer player;
        bool Playing = false;
        private void button6_Click(object sender, EventArgs e)
        {

            if (Playing == false)
            {
                var bytes = PS4_Tools.Media.Atrac9.LoadAt9(@"C:\Users\3deEchelon\Desktop\PS4\AT9\prelude1.at9");
                player = new System.Media.SoundPlayer(new MemoryStream(bytes));
                player.Play();

                button6.Text = "Stop Playing";
                Playing = true;
            }
            else
            {
                player.Stop();

                button6.Text = "Play At9";
                Playing = false;
            }

        }


        #endregion << Atrac9 player >>

        private void button7_Click(object sender, EventArgs e)
        {
            var item = PS4_Tools.PKG.Official.CheckForUpdate(textBox1.Text);

            /*TitleID Patch Data Is Avaiavle Here*/

            /*Build some string*/
            string update = " Update Info :";
            update += "\n Title : " + item.Tag.Package.Paramsfo.Title;
            update += "\n Version : " + item.Tag.Package.Version;
            int ver = Convert.ToInt32(item.Tag.Package.System_ver);
            update += "\n System Version : " + ver.ToString("X");
            update += "\n Remaster : " + item.Tag.Package.Remaster;
            update += "\n Manifest File Number of Pieces : " + item.Tag.Package.Manifest_item.pieces.Count;

            label1.Text = update;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var storeitems = PS4_Tools.PKG.Official.Get_All_Store_Items(textBox1.Text);
            GridWithDisplay grid = new GridWithDisplay(storeitems);
            grid.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PS4_PKG_Viewer.Form1 formmain = new PS4_PKG_Viewer.Form1();
            formmain.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /*Lets try and work with some savedata*/
            //PS4_Tools.SaveData.Doit(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\SAVEDATA00.bin", @"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\sdimg_SAVEDATA00");

            PS4_Tools.SaveData.LoadSaveData(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA00135\sdimg_BAK1Save0x0sgd", @"C: \Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA00135\BAK1Save0x0sgd.bin");

        }

        private void button11_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.SceneRelated.Rename_pkg_To_ContentID(@"E:\Euro.Fishing.Collectors.Edition.PS4-DUPLEX.pkg", @"E:\", true);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // PS4_Tools.PKG.Official.RIF rifitem = PS4_Tools.PKG.Official.ReadRif(@"C:\Users\3deEchelon\Downloads\RifTest.rif");
            PS4_Tools.Licensing.RIF rifitem = PS4_Tools.Licensing.ReadRif(@"C:\Users\3deEchelon\Desktop\PS4\LM\Sc0\license.dat");
            /*Rif Loaded*/
            // string Content_ID = System.Text.Encoding.ASCII.GetString(rifitem.Content_ID);

            lblExtrInfo.Text = "Rif information" + "\r\n";

            lblExtrInfo.Text += @"Content ID : " + rifitem.Content_ID + "\r\n" + "\r\n";
            //lblExtrInfo.Text += @"Content Type : " + System.Text.Encoding.UTF8.GetString(rifitem.DRM_Type) + "\r\n" + "\r\n";
            lblExtrInfo.Text += @"Encrypted Secret : " + rifitem.Encrypted_Secret.Entitlement_Key + "\r\n";
            lblExtrInfo.Text += @"Secret Encryption IV" + rifitem.Secret_Encryption_IV + "\r\n";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //Open File Items
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var ps4filetype = PS4_Tools.Tools.Get_PS4_File_Type(openFileDialog1.FileName);
                MessageBox.Show("File is a " + ps4filetype.ToString());
                switch (ps4filetype)
                {
                    case PS4_Tools.Tools.File_Type.PARAM_SFO:
                        var sfo = new Param_SFO.PARAM_SFO(openFileDialog1.FileName);
                        break;
                    case PS4_Tools.Tools.File_Type.PS4_DDS:
                        var dd = PS4_Tools.Image.DDS.GetBytesFromDDS(openFileDialog1.FileName);
                        break;
                    case PS4_Tools.Tools.File_Type.PS4_PKG:
                        var pkg = PS4_Tools.PKG.SceneRelated.Read_PKG(openFileDialog1.FileName);
                        break;
                    case PS4_Tools.Tools.File_Type.UpdateFile:
                        var update = new PS4_Tools.PUP();
                        var tempfile = update.Read_Pup(openFileDialog1.FileName);
                        break;
                    case PS4_Tools.Tools.File_Type.ATRAC9:
                        var bytes = PS4_Tools.Media.Atrac9.LoadAt9(openFileDialog1.FileName);
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(new MemoryStream(bytes));
                        player.Play();
                        break;
                }
            }
        }

        //02 CC D3 46 B4 59 CB 83 50 5E 8E 76 0A 44 D4 57
        private byte[] devtrophykey = new byte[16] {
            2, 204, 211, 70, 180, 89, 203, 131, 80, 94, 142, 118, 10, 68, 212, 87};

        private byte[] devtrophykey1 = new byte[16] {
            0x02, 0xCC, 0xD3, 0x46, 0xB4, 0x59, 0xCB, 0x83, 0x50, 0x5E, 0x8E, 0x76, 0x0A, 0x44, 0xD4, 0x57};

        private byte[] retailtrophykey = new byte[16]
        {
            0x21,0xF4,0x1A,0x6B,0xAD,0x8A,0x1D,0x3E,0xCA,0x7A,0xD5,0x86,0xC1,0x01,0xB7,0xA9
        };

        private void button14_Click(object sender, EventArgs e)
        {
            //Create a RIF File


        }
        public static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            Bitmap btimap = new Bitmap(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\pic0.png");
            Stream sm = new FileStream(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\pic0.png", FileMode.Open, FileAccess.Read);

            //PS4_Tools.Image.DDS.PS4.CreateDDSFromBitmap(btimap, @"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\test.dds");
            // pictureBox1.Image = item;

            //test if dds is corectly saved

            var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\test.dds");
            pictureBox1.Image = item;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.Remastered.PS2_Classics ps2classics = new PS4_Tools.PKG.Remastered.PS2_Classics();
            Bitmap bit = new Bitmap(@"C:\Users\3deEchelon\Desktop\PS4\LibHomebrew Compiler\ps2emu\Fake PKG Tools\ps2emu\sce_sys\icon0.png");

            ps2classics.Create_Single_ISO_PKG(@"C:\Users\3deEchelon\Desktop\PS2\X2 - Wolverine's Revenge (USA).iso", @"C:\Users\3deEchelon\Desktop\PS2\X2 - Wolverine's Revenge (USA).pkg", "X2 - Wolverine's Revenge", bit, @"C:\Users\3deEchelon\Desktop\PS4\LibHomebrew Compiler\ps2emu\Fake PKG Tools\ps2emu\sce_sys\pic1.png");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ImageUtil.Form1 imgutil = new ImageUtil.Form1();
            imgutil.ShowDialog();

            //OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //openFileDialog1.Title = "Select PS4 File(icon0)";

            //openFileDialog1.CheckFileExists = true;

            //openFileDialog1.CheckPathExists = true;

            //openFileDialog1.Filter = "PS4 File (*.*)|*.*";

            //openFileDialog1.RestoreDirectory = true;

            //openFileDialog1.Multiselect = false;

            //openFileDialog1.ReadOnlyChecked = true;

            //openFileDialog1.ShowReadOnly = true;
            //if(openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //    saveFileDialog1.Filter = "PS4 PNG Image|*.png";
            //    saveFileDialog1.Title = "Save an PS4 Image File (icon0)";
            //    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //    {
            //        PS4_Tools.Image.PNG png = new PS4_Tools.Image.PNG();
            //        png.Create_PS4_Compatible_PNG(openFileDialog1.FileName,saveFileDialog1.FileName);
            //    }
            //}
            //openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //openFileDialog1.Title = "Select PS4 File(pic1)";

            //openFileDialog1.CheckFileExists = true;

            //openFileDialog1.CheckPathExists = true;

            //openFileDialog1.Filter = "PS4 File (*.*)|*.*";

            //openFileDialog1.RestoreDirectory = true;

            //openFileDialog1.Multiselect = false;

            //openFileDialog1.ReadOnlyChecked = true;

            //openFileDialog1.ShowReadOnly = true;
            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //    saveFileDialog1.Filter = "PS4 PNG Image|*.png";
            //    saveFileDialog1.Title = "Save an PS4 Image File (pic1)";
            //    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //    {
            //        PS4_Tools.Image.PNG png = new PS4_Tools.Image.PNG();
            //        png.Create_PS4_Compatible_PNG(openFileDialog1.FileName, saveFileDialog1.FileName,1080, 1920);
            //    }
            //}
        }

        private void button19_Click(object sender, EventArgs e)
        {
            PS4_Tools.Trophy_File tphy = new PS4_Tools.Trophy_File();
            tphy.Load(File.ReadAllBytes(@"C:\Users\3deEchelon\Downloads\PlayStation 4 Trophies\PlayStation 4 Trophies\Adventures of Pip\data\NPWR09053_00\trophy.img"));
        }

        private void groupBox15_Enter(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            var file = PS4_Tools.RCO.ReadRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");

            //we write out all files

            for (int i = 0; i < file.FileTable.CXMLFiles.Count; i++)
            {

            }


            PS4_Tools.RCO.DumpRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 Trophy File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 Retail File (TROPHY.TRP)|TROPHY.TRP";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                PS4_Tools.Trophy_File trophy = new PS4_Tools.Trophy_File();
                Stream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                var trophyfile = trophy.Load(stream);

                PS4_Tools.TROPHY.TROPCONF tconf;
                PS4_Tools.TROPHY.TROPTRNS tpsn;
                PS4_Tools.TROPHY.TROPUSR tusr;

                DateTime ps3Time = new DateTime(2008, 1, 1);
                DateTime randomEndTime = DateTime.Now;

                string NPCOMID = "";
                string Diroftrpy = Path.GetDirectoryName(openFileDialog1.FileName);
                if (File.Exists(Diroftrpy + @"\TRPPARAM.INI"))
                {
                    string inifile = File.ReadAllText(Diroftrpy + @"\TRPPARAM.INI");
                    NPCOMID = getBetween(inifile, "NPCOMMID=", "TROPAPPVER=");
                    if (NPCOMID == "")
                    {
                        MessageBox.Show("Can not find NPCOMMID");
                        return;
                    }
                    NPCOMID = NPCOMID.TrimEnd();
                }
                for (int i = 0; i < trophyfile.trophyItemList.Count; i++)
                {
                    var itembytes = trophyfile.ExtractFileToMemory(trophyfile.trophyItemList[i].Name);
                    byte[] itemcontainer = null;
                    try
                    {

                        itemcontainer = PS4_Tools.Trophy_File.ESFM.LoadAndDecrypt(itembytes, NPCOMID);
                        File.WriteAllBytes(Application.StartupPath + @"\TropyFiles\" + trophyfile.trophyItemList[i].Name.Replace(".ESFM", ".SFM"), itemcontainer);

                    }
                    catch
                    {
                        File.WriteAllBytes(Application.StartupPath + @"\TropyFiles\" + trophyfile.trophyItemList[i].Name, itembytes);
                    }
                    //now we implement unlock all !
                    //okay lets begin

                    if (trophy.trophyItemList[i].Name == "TROPCONF.ESFM")
                    {
                        tconf = new PS4_Tools.TROPHY.TROPCONF(itemcontainer);
                    }
                    if (trophy.trophyItemList[i].Name == "TROPTRNS.DAT")
                    {
                        tpsn = new PS4_Tools.TROPHY.TROPTRNS(itemcontainer);
                    }
                    if (trophy.trophyItemList[i].Name == "TROPUSR.DAT")
                    {
                        tusr = new PS4_Tools.TROPHY.TROPUSR(itemcontainer);
                    }
                }
                MessageBox.Show("Trophy File Extraction and decryption completed");
            }


        }

        private void button23_Click(object sender, EventArgs e)
        {
            string myspitstring = txtspitter.Text.Replace(" ","").Replace("\r\n","");


            string str = myspitstring;
            int chunkSize = 2;
            int stringLength = str.Length;
            txtspitout.Text = "";
            int count = 0;
            for (int i = 0; i < stringLength; i += chunkSize)
            {
                if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                //Console.WriteLine(str.Substring(i, chunkSize));
                txtspitout.Text += "0x" + str.Substring(i, chunkSize) + " , ";
                count++;
            }
            txtspitout.Text =txtspitout.Text.ToUpper();
            txtspitout.Text = txtspitout.Text.Replace("X", "x");
            txtspitter.Text = count.ToString();


        }

        private void button22_Click(object sender, EventArgs e)
        {
            PS4_Tools.Trophy_File trphy = new PS4_Tools.Trophy_File();
            var item= trphy.SealedTrophy(File.ReadAllBytes(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\trophy\data\NPWR04914_00\trophy.img"), File.ReadAllBytes(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\trophy\data\NPWR04914_00\sealedkey"));
            File.WriteAllBytes("testdecypt.dat", item);

        }

        private void button24_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.SceneRelated.PBM.PBMStruct pbmfile = PS4_Tools.PKG.SceneRelated.PBM.Read(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\CUSA00265\app.pbm");
            var displayinfo = pbmfile.DisplayInfo();
            for (int i = 0; i < displayinfo.Count ; i++)
            {
                Console.WriteLine(displayinfo[i]);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.SceneRelated.App.JSON thejson = new PS4_Tools.PKG.SceneRelated.App.JSON(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\CUSA00265\app.json");
            Console.WriteLine("Number of Split Files " + thejson.NumberOfSplitFiles);
            Console.WriteLine("Package Digest " + thejson.PackageDigest);
            Console.WriteLine("Pieces[1] Url " + thejson.Pieces[0].Url);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            //string path 

            //set base directory for PS4 project
            PS4_Tools.PKG.SceneRelated.GP4.Psproject project = PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(@"C: \Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\CUSA00265\app.xml");
            if (project.Fmt != "playgo-status")
            {
                MessageBox.Show("This is not a valid PS4 PlayGoXML");
                return;
            }

            //lets read the pkg content info 
            //that should be that

        }
    }
}
