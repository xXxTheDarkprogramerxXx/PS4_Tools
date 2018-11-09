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
    public partial class GridWithDisplay : Form
    {
        List<PS4_Tools.PKG.Official.StoreItems> Items = new List<PS4_Tools.PKG.Official.StoreItems>();
        public GridWithDisplay(List<PS4_Tools.PKG.Official.StoreItems> items)
        {
            InitializeComponent();
            Items = items;
        }

        private void GridWithDisplay_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Items;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void downloadDLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Download DLC*/
        }
    }
}
