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
            button1.PerformClick();
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
            var lstitem = PS4_Tools.PKG.Official.ReadAllUnprotectedData(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            for (int i = 0; i < lstitem.Count; i++)
            {
                listBox1.Items.Add(lstitem[i]);
            }
            //Extarct SFO

            Param_SFO.PARAM_SFO sfo = PS4_Tools.PKG.SceneRelated.PARAM_SFO.Get_Param_SFO(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                listBox2.Items.Add( sfo.Tables[i].Name + " : " + sfo.Tables[i].Value);
            }
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            pictureBox1.Image = item;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            PS4_Tools.RCO.DumpRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");
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
            /*This was mainly powered by leecherman's version 1.1 code i do see he removed that code in future release
             (Not sure why he ended up using official sce tools over this)*/
            PS4_Tools.PKG.SceneRelated.Unprotected_PKG ps4pkg = PS4_Tools.PKG.SceneRelated.Read_PKG(@"C:\Users\3deEchelon\Desktop\PS4\Euro.FISHING.COLLECTORS.EDITION.PS4-DUPLEX\Euro.Fishing.Collectors.Edition.PS4-DUPLEX\duplex-euro.fishing.collectors.ed.ps4\Euro.Fishing.Collectors.Edition.PS4-DUPLEX.pkg");

            /*Lets work with the data shall we*/
            /*Display the PSFO in some type of info format*/
            var item = ps4pkg.Param;
           
            for (int i = 0; i < item.Tables.Count; i++)
            {
                listBox3.Items.Add(item.Tables[i].Name + ":" + item.Tables[i].Value);
            }
            /*Display Image*/
            pictureBox2.Image = ps4pkg.Image;

            var trphy = ps4pkg.Trophy_File;

            for (int i = 0; i < trphy.FileCount; i++)
            {
                listBox4.Items.Add(trphy.trophyItemList[i].Name);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /*Lets try and work with some savedata*/
            PS4_Tools.SaveData.LoadSaveData(@"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\sdimg_SAVEDATA00", @"C:\Users\3deEchelon\Desktop\PS4\RE\Ps4 Save Data Backup\10000000\savedata\CUSA01656\SAVEDATA00.bin");

        }
    }
}
