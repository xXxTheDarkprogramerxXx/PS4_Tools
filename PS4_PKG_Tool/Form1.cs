using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS4_PKG_Tool
{
    public partial class Form1 : Form
    {

        public static PS4_Tools.PKG.GP4.Psproject project = new PS4_Tools.PKG.GP4.Psproject();

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            project = new PS4_Tools.PKG.GP4.Psproject();//start a new project
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.CheckFileExists = true;
            //opendialog.AddExtension 
            opendialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opendialog.Multiselect = false;
            opendialog.Filter = "PS4 Project File (*.gp4) | *.gp4";
            if(opendialog.ShowDialog() == DialogResult.OK)
            {
                project = PS4_Tools.PKG.GP4.ReadGP4(opendialog.FileName);
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
                    TreeNode treeNode = new TreeNode(project.Rootdir.Dir[i].Targ_name.ToString(),projectfiles);
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 0;
                    treeView1.Nodes.Add(treeNode);
                    
                }
            }

        }
    }
}
