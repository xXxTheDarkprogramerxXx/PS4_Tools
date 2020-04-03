using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public enum ConnectionStatus
        {
            Connected,
            NotConnected,
            Unknown
        }
        public enum SKU_Power_State
        {
            On,
            Off,
            RestMode,
            DEAD
        }

        public enum ReleaseCheck
        {
            Release,
            Debug,
            Unknown
        }

        public class SKUS
        {
            public string Default { get; set; }
            public string Target { get; set; }
            public string SDK { get; set; }
            public ConnectionStatus Status { get; set; }
            public string Address { get; set; }

            public SKU_Power_State Power { get; set; }

            public string FileSerDir { get; set; }

            public DateTime? Expire { get; set; }

            public ReleaseCheck Release { get; set; }
        }

        public List<SKUS> SKU_List = new List<SKUS>();
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //on load load ps4 list however you want i added a simple added here
            SKUS newsku = new SKUS();

            newsku.Default = "★";
            newsku.Status = ConnectionStatus.Connected;
            newsku.FileSerDir = "C://somelongasslocationfortheusertobrowserto";
            newsku.Address = "192.168.8.101";
            newsku.Target = "XDPX Test Box";
            newsku.Power = SKU_Power_State.On;
            newsku.SDK = "5.050.031 (HEN)";
            newsku.Release = ReleaseCheck.Unknown;
            newsku.Expire = null;
            SKU_List.Add(newsku);

            lstSKU.ItemsSource = SKU_List;
           
        }

        private void lstSKU_SizeChanged(object sender, SizeChangedEventArgs e)
        {
       
        }

        private void lstSKU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //on item selection changed
                //if item is connected 
                //enable all functions
                var selecteditem = (SKUS)lstSKU.SelectedItem;//try casting it to skus
                if(selecteditem.Status == ConnectionStatus.Connected)
                {
                    //enable all items that are disabled
                    btnDisconnect.IsEnabled = true;
                    btnConnect.IsEnabled = true;
                    btnReboot.IsEnabled = true;

                }

            }
            catch(Exception ex)
            {

            }
        }
    }
}
