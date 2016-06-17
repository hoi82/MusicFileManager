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
        const string regKeyMultiFileInArchive = "DeleteArchiveHasMultiAudio";
        const string regKeyDupAudioWithoutBitAndDur = "DeleteAudioWithoutBitRateAndDuration";
        const string regKeyBitRate = "BitRate";
        const string regKeyDuration = "Duration";

        RegistryKey regKey = null;

        MainController controller = null;

        FileToCleanControl ctrlClean = null;

        public MainWindow()
        {
            InitializeComponent();

            InitializeRegistryKey();                        

            controller = new MainController(prgControl, ctrlOption);
            controller.OnStart += controller_OnStart;
            controller.OnEnd += controller_OnEnd;

            ctrlClean = new FileToCleanControl(grdFileList);
            ctrlClean.OnOK += ctrlClean_OnOK;
            ctrlClean.OnCancel += ctrlClean_OnCancel;
        }

        void ctrlClean_OnCancel(object sender)
        {
            MessageBox.Show("Cancel");
        }

        void ctrlClean_OnOK(object sender, List<string> files)
        {
            MessageBox.Show("End");
        }

        void controller_OnEnd(object sender, List<DuplicatedFiles> fileToClean)
        {
            btnFind.IsEnabled = true;
            btnCancel.IsEnabled = false;
                        
            ctrlClean.Display(fileToClean);
        }

        void controller_OnStart(object sender)
        {
            btnFind.IsEnabled = false;
            btnCancel.IsEnabled = true;
        }

        void InitializeBinding()
        {
            Binding deleteArchiveBinding = new Binding("DeleteMultiAudioFileInArchive");
            deleteArchiveBinding.Source = controller;
            deleteArchiveBinding.Mode = BindingMode.TwoWay;
            ctrlOption.SetBinding(MFMOption.DeleteArchiveWithMulipleAudioProperty, deleteArchiveBinding);

            Binding deleteWithOutBitRateBinding = new Binding("DeleteAudioWithoutFiltering");
            deleteWithOutBitRateBinding.Source = controller;
            deleteWithOutBitRateBinding.Mode = BindingMode.OneWayToSource;
            ctrlOption.SetBinding(MFMOption.DeleteAudioWithOutBitRateProperty, deleteWithOutBitRateBinding);

            Binding bitrateBinding = new Binding("BitRate");
            bitrateBinding.Source = controller;
            bitrateBinding.Mode = BindingMode.OneWayToSource;
            ctrlOption.SetBinding(MFMOption.AudioBitRateProperty, bitrateBinding);

            Binding durationBinding = new Binding("Duration");
            durationBinding.Source = controller;
            durationBinding.Mode = BindingMode.OneWayToSource;
            ctrlOption.SetBinding(MFMOption.AudioDurationProperty, durationBinding);
        }

        private void InitializeRegistryKey()
        {                       
            using (regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true))
            {
                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(regKeyLocation, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    InitRegistryKeyValue(regKey);
                }
                OpenRegistryKeyValue(regKey);            
            }            
        }

        void InitRegistryKeyValue(RegistryKey key) 
        {
            try
            {
                regKey.SetValue(regKeySearch, AppDomain.CurrentDomain.BaseDirectory);
                regKey.SetValue(regKeyMultiFileInArchive, false);
                regKey.SetValue(regKeyDupAudioWithoutBitAndDur, false);
                regKey.SetValue(regKeyBitRate, 0);
                regKey.SetValue(regKeyDuration, 0);
            }
            catch (Exception)
            {
                
                throw;
            }            
        }

        void OpenRegistryKeyValue(RegistryKey key)
        {
            try
            {
                searchLocation = regKey.GetValue(regKeySearch) as string;                
                ctrlOption.DeleteArchiveWithMulipleAudio = Convert.ToBoolean(regKey.GetValue(regKeyMultiFileInArchive));
                ctrlOption.DeleteAudioWithOutBitRate = Convert.ToBoolean(regKey.GetValue(regKeyDupAudioWithoutBitAndDur));
                ctrlOption.AudioBitRate = Convert.ToInt32(regKey.GetValue(regKeyBitRate));
                ctrlOption.AudioDuration = TimeSpan.FromSeconds(Convert.ToDouble(regKey.GetValue(regKeyDuration)));
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        void SaveRegistryKeyValue(RegistryKey key)
        {
            try
            {
                regKey.SetValue(regKeySearch, searchLocation);
                regKey.SetValue(regKeyMultiFileInArchive, ctrlOption.DeleteArchiveWithMulipleAudio);
                regKey.SetValue(regKeyDupAudioWithoutBitAndDur, ctrlOption.DeleteAudioWithOutBitRate);
                regKey.SetValue(regKeyBitRate, ctrlOption.AudioBitRate);
                regKey.SetValue(regKeyDuration, ctrlOption.AudioDuration.TotalSeconds);
            }
            catch (Exception)
            {
                
                throw;
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

            using (regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true))
            {
                SaveRegistryKeyValue(regKey);
            }                        
        }
                     
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            controller.Cancel();            
        }
    }
}
