using System;
using System.Collections.Generic;
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
    /// Interaction logic for CheckError.xaml
    /// </summary>
    public partial class CheckError : Window
    {
        public CheckError()
        {
            InitializeComponent();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            Fakekit_API.API.CheckError(txtError.Text);
        }
    }
}
