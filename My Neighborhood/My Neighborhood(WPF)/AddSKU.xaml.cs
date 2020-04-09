using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for AddSKU.xaml
    /// </summary>
    public partial class AddSKU : Window
    {
        public bool SaveSKU = false;

        public AddSKU()
        {
            InitializeComponent();
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SKUS skuitem = new MainWindow.SKUS();
            skuitem.Address = txtipaddress.Text;
            skuitem.Target = txtTargetname.Text;
            skuitem.FileSerDir = txtfileserdir.Text;
            skuitem.Release = MainWindow.ReleaseCheck.Unknown;
            skuitem.Power = MainWindow.SKU_Power_State.Off;
            skuitem.Status = MainWindow.ConnectionStatus.NotConnected;
            MainWindow.SKU_List.Add(skuitem);
            SaveSKU = true;
            this.Close();
        }

        private void btnLoadIPs_Click(object sender, RoutedEventArgs e)
        {
            frmLoadIPS ips = new frmLoadIPS();
            ips.ShowDialog();
            if(ips.IPSelected != "")
            {
                txtipaddress.Text = ips.IPSelected;
            }
        }
    }
}
