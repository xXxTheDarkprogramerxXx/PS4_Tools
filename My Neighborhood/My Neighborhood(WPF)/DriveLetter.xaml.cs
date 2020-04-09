using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for DriveLetter.xaml
    /// </summary>
    public partial class DriveLetter : Window
    {
        public string DriveSelected = "";

        public DriveLetter()
        {
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxOpenDrive.ItemsSource = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (Char)i + ":")
          .Except(DriveInfo.GetDrives().Select(s => s.Name.Replace("\\", "")));
        }

        private void btnDrive_Click(object sender, RoutedEventArgs e)
        {
            DriveSelected = cbxOpenDrive.SelectedValue.ToString();
            this.Close();
        }
    }
}
