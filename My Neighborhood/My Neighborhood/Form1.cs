using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_Neighborhood
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public enum ConnectionStatus
        {
            Connected,
            Disconnected,
            Unknown
        }
        public enum SKU_Power_State
        {
            ON,
            OFF,
            SLEEP,
            DEAD
        }

        public class SKUS
        {
            public string Name { get; set; }
            public string SDK { get; set; }         
            public ConnectionStatus ConnectionStat { get; set; }
            public string IP_Of_console { get; set; }

            public SKU_Power_State Power_State { get; set; }

            public string File_Ser_Dir { get; set; }
        }

        public List<SKUS> SKU_List = new List<SKUS>();

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This project was killed because the only ribbon we could add was the old shitty windows 7 one so we moved to wpf");
            //return;

            //on load load ps4 list however you want i added a simple added here
            SKUS newsku = new SKUS();

            newsku.ConnectionStat = ConnectionStatus.Connected;
            newsku.File_Ser_Dir = "TODO";
            newsku.IP_Of_console = "192.168.8.101";
            newsku.Name = "XDPX Test Box";
            newsku.Power_State = SKU_Power_State.ON;
            newsku.SDK = "5.050.031 (HEN)";
            SKU_List.Add(newsku);

            
            for (int i = 0; i < SKU_List.Count; i++)
            {
                var lvi = new ListViewItem(new string[] { "★", SKU_List[i].Name, SKU_List[i].SDK , SKU_List[i].ConnectionStat.ToString(), SKU_List[i].Power_State.ToString(), SKU_List[i].IP_Of_console , SKU_List[i].File_Ser_Dir });
                lstSkus.Items.Add(lvi);
                //add each item
                //var sku_control = new PS4Control();
                //sku_control.lblFileSerDir.Text = SKU_List[i].File_Ser_Dir;
                //sku_control.lblIpAddress.Text = SKU_List[i].IP_Of_console;
                //sku_control.lblName.Text = SKU_List[i].Name;
                //sku_control.lblSDKVersion.Text = SKU_List[i].SDK;
                //switch(SKU_List[i].ConnectionStat)
                //{
                //    case ConnectionStatus.Connected:
                //        sku_control.pbConState.Image = Properties.Resources.icons8_online_100;
                //        break;
                //    default:
                //        sku_control.pbConState.Image = Properties.Resources.icons8_offline_100;
                //        break;
                //}
                //switch(SKU_List[i].Power_State)
                //{
                //    case SKU_Power_State.ON:
                //        sku_control.pbPowerStatus.Image = Properties.Resources.icons8_shutdown_128__1_;
                //        break;
                //    case SKU_Power_State.SLEEP:
                //        sku_control.pbPowerStatus.Image = Properties.Resources.icons8_shutdown_128__2_;
                //        break;
                //    default:
                //        sku_control.pbPowerStatus.Image = Properties.Resources.icons8_shutdown_128;
                //        break;
                //}
                //sku_control.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
                //splitContainer1.Panel1.Controls.Add(sku_control);
            }
         
        }

        private void ribbon1_Click(object sender, EventArgs e)
        {

        }
    }
}
