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

        ArchivedFileExtractor archiveExtractor = null;
        List<string> archivedFiles = null;

        public MainWindow()
        {
            InitializeComponent();            

            InitializeRegistryKey();
            InitializeArchiveExtractor();

            searchLocation = regKey.GetValue(regKeySearch) as string;
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

        private void InitializeArchiveExtractor()
        {
            archiveExtractor = new ArchivedFileExtractor(prgControl);
            archiveExtractor.OnStart += archiveExtractor_OnStart;
            archiveExtractor.OnEnd += archiveExtractor_OnEnd;
        }

        void archiveExtractor_OnEnd(object sender, ArchivedFileExtractorEndEventArgs e)
        {
            btnClean.IsEnabled = true ;
            btnCancel.IsEnabled = false;

            if (!e.Cancel)
            {
                archivedFiles = e.ArchivedFiles;
                regKey.SetValue(regKeySearch, searchLocation);
            }                
        }

        void archiveExtractor_OnStart(object sender)
        {
            btnClean.IsEnabled = false;
            btnCancel.IsEnabled = true;
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
            cleanUp();            
        }

        private List<string> getFiles(string location)
        {
            List<string> allFiles = new List<string>();
            string[] allPaths = Directory.GetFiles(searchLocation, "*.*", SearchOption.AllDirectories);
            foreach (string s in allPaths)
            {
                FileAttributes attr = File.GetAttributes(s);
                if (!attr.HasFlag(FileAttributes.Directory))
                {
                    allFiles.Add(s);
                }
            }
            return allFiles.ToList();
        }

        private void cleanUp()
        {
            archiveExtractor.Run(getFiles(searchLocation));
        }        

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            prgControl.Cancel();
        }
    }
}
