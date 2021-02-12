using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Windows.Forms;
using LibOrbisPkg.Util;
using System.Runtime.InteropServices;

namespace PS4_PKG_Tool
{
    public partial class Form1 : Form
    {

        public static PS4_Tools.PKG.SceneRelated.GP4.Psproject project = new PS4_Tools.PKG.SceneRelated.GP4.Psproject();

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            project = new PS4_Tools.PKG.SceneRelated.GP4.Psproject();//start a new project
        }

        public static void Read<T>(IMemoryReader reader, long pos, out T value) where T : struct
        {
            int sizeOfT = Marshal.SizeOf(typeof(T));
            var buf = new byte[sizeOfT];
            reader.Read(pos, buf, 0, sizeOfT);
            value = ByteArrayToStructure<T>(buf, 0);
        }


        // https://stackoverflow.com/a/2887
        private static unsafe T ByteArrayToStructure<T>(byte[] bytes, int offset) where T : struct
        {
            fixed (byte* ptr = &bytes[offset])
            {
                return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(@"E:\PS4\SAVEDATA\781ce6549a5d099d\CUSA00135\BAK1Save0x0sgd");




            byte[] enckey = PS4_Tools.SaveData.GetSealedKey_Bytes(@"E:\PS4\SAVEDATA\781ce6549a5d099d\CUSA00135\BAK1Save0x0sgd.bin");
            var r = file.CreateViewAccessor();

            LibOrbisPkg.PFS.PfsHeader PfsHeader = new LibOrbisPkg.PFS.PfsHeader();
            var reader = r;
            var buf = new byte[0x400];
            //reader.Read(0, buf, 0, 0x400);
            //Read(reader,)
            reader.ReadArray(0, buf, 0, 0x400);
            var header=  LibOrbisPkg.PFS.PfsHeader.ReadFromStream(new MemoryStream(buf));
            //byte[] passcode = new byte[] { 0xB5, 0xDA, 0xEF, 0xFF, 0x39, 0xE6, 0xD9, 0x0E, 0xCA, 0x7D, 0xC5, 0xB0, 0x29, 0xA8, 0x15, 0x3E, 0x87, 0x07, 0x96, 0x0A, 0x53, 0x46, 0x8D, 0x6C, 0x84, 0x3B, 0x3D, 0xC9, 0x62, 0x4E, 0x22, 0xAF, };
            byte[] Data = new byte[] { 0xA2, 0xC0, 0x13, 0x20, 0x30, 0x0B, 0x26, 0x01, 0x6D, 0x7C, 0x72, 0x0C, 0x22, 0xC3, 0x25, 0xE0 };
            byte[] Tweak = new byte[] { 0xF2, 0x80, 0x83, 0x30, 0xE8, 0xDE, 0x69, 0x8F, 0x91, 0x6C, 0x45, 0x3D, 0xBF, 0xF0, 0x55, 0xF3 };
            LibOrbisPkg.PFS.PfsReader pfsreader = new LibOrbisPkg.PFS.PfsReader(r,tweak :Tweak,data : Data);
            //just testing something here quickly 
            

            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.CheckFileExists = true;
            //opendialog.AddExtension 
            opendialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opendialog.Multiselect = false;
            opendialog.Filter = "PS4 Project File (*.gp4) | *.gp4";
            if (opendialog.ShowDialog() == DialogResult.OK)
            {
                project = PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(opendialog.FileName);
                TreeNode mainnode = new TreeNode("root");
                for (int i = 0; i < project.Rootdir.Dir.Count; i++)
                {

                    //List<PS4_Tools.PKG.GP4.File> listoffiles = new List<PS4_Tools.PKG.GP4.File>();
                    List<TreeNode> treenodes = new List<TreeNode>();
                    for (int x = 0; x < project.Files.File.Count; x++)
                    {
                        if (project.Files.File[x].Targ_path.Contains(project.Rootdir.Dir[i].Targ_name.ToString()))
                        {
                            //listoffiles.Add(project.Files.File[i]);
                            TreeNode node = new TreeNode(project.Files.File[x].Targ_path);
                            node.ImageIndex = 1;
                            node.SelectedImageIndex = 1;
                            treenodes.Add(node);

                        }
                    }
                    TreeNode[] projectfiles = treenodes.ToArray();
                    TreeNode treeNode = new TreeNode(project.Rootdir.Dir[i].Targ_name.ToString(), projectfiles);
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 0;
                    treeView1.Nodes.Add(treeNode);

                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This tool has not been completed and was suppose to replace the sce pub tools");
        }
    }
}