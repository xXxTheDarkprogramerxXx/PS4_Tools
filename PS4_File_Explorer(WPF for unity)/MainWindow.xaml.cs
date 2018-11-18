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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PS4_File_Explorer_WPF_for_unity_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> MainRootDirectories = new List<string>();
        string MainRootPath = "";

        /*All Code here will be ported to unity lets hope it works out the way i plan*/
        public void GetAllDirectories()
        {
            /*Get All Directories*/
            MainRootPath = System.IO.Path.GetPathRoot(Directory.GetCurrentDirectory());
            List<string> AllSub_Directories = new List<string>();
            MainRootDirectories = Directory.GetDirectories(MainRootPath).ToList();
            /*Now if a user clicks one something outside of the root we just need to add that up browser button*/
            /*Fuck i hope Sony's shit allows this else this is not going to go well*/
            /*If Unity Doesn't allow this we will have to create a c/cpp callable dll for unity just to get the File/Directory Listings*/
            /*Mono is lovable sometimes we can call syscalls trough it which does make life a lot easier*/
        }

        public MainWindow()
        {
            InitializeComponent();
            GetAllDirectories();
        }
    }

    

}
