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
using System.IO.Compression;
using System.Diagnostics;

namespace TrophyUnlocker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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
                try
                {
                    var sfo = new Param_SFO.PARAM_SFO(openFileDialog1.FileName);
                    label5.Text = "Content ID: " +sfo.ContentID;
                    textBox1.Text = openFileDialog1.FileName;
                    label8.Text = "Game Title : " + sfo.Title;
                    txtPreviewTitle.Text = "Unlocker for " + sfo.Title;
                }
                catch(Exception ex)
                {

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
                try
                {
                    var trp = new PS4_Tools.Trophy_File();
                    trp.Load(File.ReadAllBytes(openFileDialog1.FileName));
                    if(trp.FileCount == 0)
                    {
                        MessageBox.Show("Trophy file not valid");
                    }
                    textBox2.Text = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
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
                try
                {
                    PS4_Tools.PKG.SceneRelated.NP_Title titleid = new PS4_Tools.PKG.SceneRelated.NP_Title(openFileDialog1.FileName);
                    textBox3.Text = openFileDialog1.FileName;
                    label6.Text = "NpTitle Id :" + titleid.Nptitle;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
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
                try
                {
                    PS4_Tools.PKG.SceneRelated.NP_Bind bind = new PS4_Tools.PKG.SceneRelated.NP_Bind(openFileDialog1.FileName);
                    textBox4.Text = openFileDialog1.FileName;
                    label7.Text = "Np Bind : " + bind.Nptitle;
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string AppCommonPath()
        {
            string returnstring = "";
            //if (Properties.Settings.Default.OverwriteTemp == true && Properties.Settings.Default.TempPath != string.Empty)
            //{
            //    returnstring = Properties.Settings.Default.TempPath + @"\Ps4Tools\";
            //}
            //else
            {
                returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";
            }
            return returnstring;
        }
        public void UpdateInfo(string info)
        {
            //if (Properties.Settings.Default.EnableAdvancedMode == true)
            //{
            //   // advanced.Invoke(new Action(() => advanced.LabelText += info + Environment.NewLine));
            //}
        }


        /// <summary>
        /// This function will clean out a directory and then delete the directory
        /// </summary>
        /// <param name="target_dir">Supply the directory you want cleaned</param>
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        public void ExtractAllResources()
        {
            UpdateInfo("Checking Directory Paths");
            if (!Directory.Exists(AppCommonPath()))
            {
                UpdateInfo("Created Directory" + AppCommonPath());
                Directory.CreateDirectory(AppCommonPath());
            }
            if (!Directory.Exists(AppCommonPath() + @"\PS2Emu\"))
            {
                UpdateInfo("Created Directory" + AppCommonPath() + @"\PS2Emu\");
                Directory.CreateDirectory(AppCommonPath() + @"\PS2Emu\");
            }

            UpdateInfo("Writing All Binary Files to Temp Path....");

            //copy byte files
            System.IO.File.WriteAllBytes(AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "orbis-pub-cmd.exe");
            System.IO.File.WriteAllBytes(AppCommonPath() + "CONTENTS.zip", Properties.Resources.CONTENTS);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "CONTENTS.zip");
            System.IO.File.WriteAllBytes(AppCommonPath() + "ext.zip", Properties.Resources.ext);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "ext.zip");
            System.IO.File.WriteAllBytes(AppCommonPath() + "Trophy_Unlocker.gp4", Properties.Resources.Trophy_Unlocker);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "Trophy_Unlocker.gp4");


            UpdateInfo("Extracting Zip(s)");
            //extarct zip
            if (Directory.Exists(AppCommonPath() + @"\CONTENTS\"))
            {
                DeleteDirectory(AppCommonPath() + @"\CONTENTS\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "CONTENTS.zip", AppCommonPath()+ @"\CONTENTS\");


            if (Directory.Exists(AppCommonPath() + @"\ext\"))
            {
                DeleteDirectory(AppCommonPath() + @"\ext\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "ext.zip", AppCommonPath());

            File.Delete(AppCommonPath() + "ext.zip");
            File.Delete((AppCommonPath() + "CONTENTS.zip"));
        }
        public string Orbis_Pub__GenCMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-gen.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = false;
            start.CreateNoWindow = false;
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
            }
            return "";
        }
        public string Orbis_CMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-cmd.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                process.ErrorDataReceived += Process_ErrorDataReceived; ;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    if (result.Contains("already converted from elf file to self file"))
                    {
                        DialogResult dlr = MessageBox.Show("Already Converted From Elf Error Found.... will be using Orbis-pub-gen for this pkg\n\n Simply Click Build and select the save folder", "Error with an alternative", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dlr == DialogResult.OK)
                        {
                            //this will open up the GP4 Project inside the Utility
                            Orbis_Pub__GenCMD("", AppCommonPath() + "Trophy_Unlocker.gp4");

                        }
                    }
                    else if (result.Contains("[Error]"))
                    {
                        MessageBox.Show(result);
                    }
                    return result;
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExtractAllResources();
            FolderBrowserDialog saveFileDialog1 = new FolderBrowserDialog();
            //saveFileDialog1.Filter = "PS4 PKG|*.pkg";
            //saveFileDialog1.Title = "Save an PS4 PKG File";
            //saveFileDialog1.ov
            if (DialogResult.OK != saveFileDialog1.ShowDialog())
            {
                return;
            }

            //First We Do Param.Sfo
            File.Copy(textBox1.Text,AppCommonPath() + @"CONTENTS\sce_sys\param.sfo",true);
            //Next Trophy
            File.Copy(textBox2.Text, AppCommonPath() + @"CONTENTS\sce_sys\trophy\trophy00.trp", true);
            //np title
            File.Copy(textBox3.Text, AppCommonPath() + @"CONTENTS\sce_sys\nptitle.dat", true);
            //np bind
            File.Copy(textBox4.Text, AppCommonPath() + @"CONTENTS\sce_sys\npbind.dat", true);
            string tmpcontentid = "EP2165-XDPX30000_00-TROPHYUNLOCKXXXX";
            //string tmpcontentid = "EP2165-CUSA09960_00-F13GAMEDISCXXXXX";
            var sfo = new Param_SFO.PARAM_SFO(AppCommonPath() + @"CONTENTS\sce_sys\param.sfo");
            //save the title as Unlocker For
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                if (sfo.Tables[i].Name == "TITLE")
                {
                    var tempitem = sfo.Tables[i];
                    tempitem.Value = "Unlocker for " + sfo.Title;
                    sfo.Tables[i] = tempitem;
                }
                //CONTENT_ID
                //if (sfo.Tables[i].Name == "CONTENT_ID")
                //{
                //    var tempitem = sfo.Tables[i];
                //    tempitem.Value = tmpcontentid;
                //    sfo.Tables[i] = tempitem;
                //}

                //if (sfo.Tables[i].Name == "TITLE_ID")
                //{
                //    var tempitem = sfo.Tables[i];
                //    tempitem.Value = "XDPX30000";
                //    sfo.Tables[i] = tempitem;
                //}
            }
            sfo.SaveSFO(sfo, AppCommonPath() + @"CONTENTS\sce_sys\param.sfo");//we save it with the new title
            //sfo.Tables = "Unlocker for " + sfo.Title;
            //now build the pkg with the new sfo
            var project = PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(AppCommonPath() + "Trophy_Unlocker.gp4");
            
            project.Volume.Package.Content_id = sfo.ContentID;//this hould be litrally all we need

            PS4_Tools.PKG.SceneRelated.GP4.SaveGP4(AppCommonPath() + "Trophy_Unlocker.gp4", project);


            //now build the pkg
            
            bool BusyCoping = true;
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                Orbis_CMD("", "img_create --oformat pkg \"" + AppCommonPath() + "Trophy_Unlocker.gp4\" \"" + saveFileDialog1.SelectedPath + "\"");
                //orbis_pub_cmd.exe img_create --skip_digest --oformat pkg C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\PS2Classics.gp4 C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\
                BusyCoping = false;
            })).Start();

            while (BusyCoping == true)
            {
                Application.DoEvents();
                //Thread.Sleep(TimeSpan.FromSeconds(5));//sleep for 5 seconds
            }
            //saved
            //get the app version 
            string version = "";
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                if (sfo.Tables[i].Name == "VERSION")
                {
                    var tempitem = sfo.Tables[i];
                    version = tempitem.Value;
                }
          
            }

            PS4_Tools.PKG.SceneRelated.Rename_pkg_To_Title(saveFileDialog1.SelectedPath +@"\"+ sfo.ContentID +"-A" + sfo.APP_VER.Replace(".","") + "-V"+ version.Replace(".","")+ ".pkg", saveFileDialog1.SelectedPath + @"\",true);
            //delete the old one 
            if(File.Exists(saveFileDialog1.SelectedPath + @"\" + sfo.ContentID + "-A" + sfo.APP_VER.Replace(".", "") + "-V" + version.Replace(".", "") + ".pkg"))
            {
                File.Delete(saveFileDialog1.SelectedPath + @"\" + sfo.ContentID + "-A" + sfo.APP_VER.Replace(".", "") + "-V" + version.Replace(".", "") + ".pkg");
            }
            Process.Start(saveFileDialog1.SelectedPath);

        }
    }
}
