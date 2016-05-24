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
using Ionic.Zip;
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

        public MainWindow()
        {
            InitializeComponent();
            regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true);

            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(regKeyLocation, RegistryKeyPermissionCheck.ReadWriteSubTree);
                regKey.SetValue(regKeySearch, AppDomain.CurrentDomain.BaseDirectory);
            }

            searchLocation = regKey.GetValue(regKeySearch) as string;
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
            regKey.SetValue(regKeySearch, searchLocation);
        }

        private string[] getFiles(string location)
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
            return allFiles.ToArray();
        }

        private void cleanUp()
        {
            //프로그레스바 초기화
            prgCleanUp.Minimum = 0;
            prgCleanUp.Maximum = 100;

            //백그라운드 작업자 초기화
            BackgroundWorker worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

            worker.RunWorkerAsync();
            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgCleanUp.Value = e.ProgressPercentage;            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Cancelled");
            }
            else
            {
                MessageBox.Show("Completed");
            }

            if (e.Error != null)
            {
                
            }            
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            BackgroundWorker worker = sender as BackgroundWorker;
            string[] files = getFiles(searchLocation);
            
            for (int i = 0; i < files.Count(); i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                if (ZipFile.IsZipFile(files[i]))
                {
                    MessageBox.Show(files[i] + " is zip file");
                }
                int perc = (int)((float)(i+1) / (float)files.Count() * 100);
                worker.ReportProgress(perc);
            }
            
        }
    }
}
