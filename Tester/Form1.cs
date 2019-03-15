using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.  
                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    PS4_Tools.PUP pupfunction = new PS4_Tools.PUP();
                    pupfunction.Unpack_PUP(openFileDialog1.FileName, folderDlg.SelectedPath);
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
            MessageBox.Show("This is a community project all items in this toolset was made buy a lot of talented developers throughout the scene\n\nCredits are on the Github repo");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string path 

            //set base directory for PS4 project
            PS4_Tools.PKG.SceneRelated.GP4.Psproject project =   PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(@"C:\Users\3deEchelon\Documents\Sony\Crash Bandcioot Twinsanity.gp4");
            if(project.Fmt != "gp4")
            {
                MessageBox.Show("This is not a valid PS4 Project");
                return;
            }

            //lets read the pkg content info 
            if(project.Volume.Package.Passcode.Length != 32)
            {
                MessageBox.Show("Passcode Lentgh is not valid");
            }


            

            //to save a gp4 

            PS4_Tools.PKG.SceneRelated.GP4.SaveGP4(@"C:\Users\3deEchelon\Documents\Sony\tempworking.gp4", project);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Official Method has been removed");

            /*Intigration with Maxtron's LibOrbis has begun */

            var temp = PS4_Tools.PKG.SceneRelated.ReadPKG(@"C:\Users\3deEchelon\Downloads\PS4-Xplorer BETA 1.05 - Lapy\ED1234-NPXX20001_00-0000000000000000-A0100-V0105.pkg");


            //var lstitem = PS4_Tools.PKG.Official.ReadAllUnprotectedData(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
            //listBox1.Items.Clear();
            //listBox2.Items.Clear();
            //for (int i = 0; i < lstitem.Count; i++)
            //{
            //    listBox1.Items.Add(lstitem[i]);
            //}
            ////Extarct SFO

            //Param_SFO.PARAM_SFO sfo = PS4_Tools.PKG.SceneRelated.PARAM_SFO.Get_Param_SFO(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
            //for (int i = 0; i < sfo.Tables.Count; i++)
            //{
            //    listBox2.Items.Add( sfo.Tables[i].Name + " : " + sfo.Tables[i].Value);
            //}
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
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

        private void button6_Click(object sender, EventArgs e)
        {


            PS4_Tools.Media.Atrac9.LoadAt9(@"C:\Users\3deEchelon\Desktop\PS4\AT9\prelude1.at9");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var item =PS4_Tools.PKG.Official.CheckForUpdate(textBox1.Text);

            /*TitleID Patch Data Is Avaiavle Here*/

            /*Build some string*/
            string update = label1.Text;
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

            PS4_Tools.SaveData.LoadSaveData(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\sdimg_SAVEDATA00", @"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\SAVEDATA00.bin");

        }

        private void button11_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.SceneRelated.Rename_pkg_To_ContentID(@"E:\Euro.Fishing.Collectors.Edition.PS4-DUPLEX.pkg", @"E:\", true);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // PS4_Tools.PKG.Official.RIF rifitem = PS4_Tools.PKG.Official.ReadRif(@"C:\Users\3deEchelon\Downloads\RifTest.rif");
            PS4_Tools.PKG.Official.RIF rifitem = PS4_Tools.PKG.Official.ReadRif(@"C:\Users\3deEchelon\Desktop\PS4\LM\Sc0\license.dat");
            /*Rif Loaded*/
            // string Content_ID = System.Text.Encoding.ASCII.GetString(rifitem.Content_ID);

            lblExtrInfo.Text = "Rif information" + "\r\n"; 

            lblExtrInfo.Text += @"Content ID : " + rifitem.Content_ID + "\r\n" + "\r\n";

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
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //Create a RIF File


        }

        private void button15_Click(object sender, EventArgs e)
        {
            Bitmap btimap = new Bitmap(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.png");
            PS4_Tools.Image.DDS.CreateDDSFromBitmap(btimap, @"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\test.dds");
           // pictureBox1.Image = item;
        }
    }
}
