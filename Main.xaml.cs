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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using Ionic.Zip;

namespace MusicFileManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string searchLocation = null;
        string regKeyLocation = @"SOFTWARE\\Yong";
        const string regKeySearch = "SearchLocation";

        RegistryKey regKey = null;

        MainController controller = null;

        public MainWindow()
        {
            InitializeComponent();

            InitializeRegistryKey();            

            searchLocation = regKey.GetValue(regKeySearch) as string;

            controller = new MainController(prgControl);
            controller.OnStart += controller_OnStart;
            controller.OnEnd += controller_OnEnd;

            InitializeBinding();
        }

        void controller_OnEnd(object sender)
        {
            btnFind.IsEnabled = true;
            btnCancel.IsEnabled = false;
        }

        void controller_OnStart(object sender)
        {
            btnFind.IsEnabled = false;
            btnCancel.IsEnabled = true;
        }

        void InitializeBinding()
        {
            Binding checkboxBinding = new Binding("DeleteMultiAudioFileInArchive");
            checkboxBinding.Source = controller;
            checkboxBinding.Mode = BindingMode.TwoWay;
            //cbMultiAudio.SetBinding(CheckBox.IsCheckedProperty, checkboxBinding);
        }

        private void InitializeRegistryKey()
        {
            regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true);

            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(regKeyLocation, RegistryKeyPermissionCheck.ReadWriteSubTree);
                regKey.SetValue(regKeySearch, AppDomain.CurrentDomain.BaseDirectory);
            }
        }                

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txbLocation.Text = searchLocation;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.DragMove();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                searchLocation = fd.SelectedPath;
                txbLocation.Text = fd.SelectedPath;
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            controller.Run(searchLocation);            
        }
                     
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            controller.Cancel();            
        }
    }
}
