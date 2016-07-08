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
using System.Windows.Media.Animation;
using MusicFileManager.CustomControls;

namespace MusicFileManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        enum ExtendMode { File, Option };

        string searchLocation = null;
        string regKeyLocation = @"SOFTWARE\\Yong";
        const string regKeySearch = "SearchLocation";
        const string regKeyMultiFileInArchive = "DeleteArchiveHasMultiAudio";
        const string regKeyDupAudioWithoutBitAndDur = "DeleteAudioWithoutBitRateAndDuration";
        const string regKeyBitRate = "BitRate";
        const string regKeyDuration = "Duration";

        RegistryKey regKey = null;

        MainController controller = null;

        Storyboard sb = null;
        DoubleAnimation aniWidth = null;
        DoubleAnimation aniHeight = null;

        bool extended = false;

        MFMOption option = null;
        MFMFileControl fileControl = null;

        ExtendMode mode = ExtendMode.File;
        Button prevPressedButton = null;

        public MainWindow()
        {
            InitializeComponent();

            InitializeRegistryKey();                        

            controller = new MainController(this);
            controller.OnStart += controller_OnStart;
            controller.OnEnd += controller_OnEnd;

            option = new MFMOption();
            fileControl = new MFMFileControl();
        }

        void controller_OnEnd(object sender)
        {
            btnProc.Content = "Clean";
        }

        void controller_OnStart(object sender)
        {
            btnProc.Content = "Cancel";
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
                //ctrlOption.DeleteArchiveWithMulipleAudio = Convert.ToBoolean(regKey.GetValue(regKeyMultiFileInArchive));
                //ctrlOption.DeleteAudioWithOutBitRate = Convert.ToBoolean(regKey.GetValue(regKeyDupAudioWithoutBitAndDur));
                //ctrlOption.AudioBitRate = Convert.ToInt32(regKey.GetValue(regKeyBitRate));
                //ctrlOption.AudioDuration = TimeSpan.FromSeconds(Convert.ToDouble(regKey.GetValue(regKeyDuration)));
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
                //regKey.SetValue(regKeySearch, searchLocation);
                //regKey.SetValue(regKeyMultiFileInArchive, ctrlOption.DeleteArchiveWithMulipleAudio);
                //regKey.SetValue(regKeyDupAudioWithoutBitAndDur, ctrlOption.DeleteAudioWithOutBitRate);
                //regKey.SetValue(regKeyBitRate, ctrlOption.AudioBitRate);
                //regKey.SetValue(regKeyDuration, ctrlOption.AudioDuration.TotalSeconds);
            }
            catch (Exception)
            {
                
                throw;
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sb = new Storyboard();

            InitializeStoryBoard(sb);
            //txbLocation.Text = searchLocation;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            if (!extended)
            {
                this.Width -= 400;
                this.Height -= 400;
                CloseExtendedMode(mode);             
            }
            else
            {
                DisplayExtendedMode(mode);
            }
        }        

        void InitializeStoryBoard(Storyboard sb)
        {
            Duration dur = new Duration(TimeSpan.FromSeconds(0.5));
            aniWidth = new DoubleAnimation();
            aniHeight = new DoubleAnimation();

            aniWidth.Duration = dur;
            aniHeight.Duration = dur;

            aniWidth.EasingFunction = new ElasticEase() { Oscillations = 4, Springiness = 10, EasingMode = EasingMode.EaseInOut };
            aniHeight.EasingFunction = new ElasticEase() { Oscillations = 4, Springiness = 10, EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(aniWidth);
            sb.Children.Add(aniHeight);

            Storyboard.SetTarget(aniWidth, bdOuterBack);
            Storyboard.SetTarget(aniHeight, bdOuterBack);

            Storyboard.SetTargetProperty(aniWidth, new PropertyPath("(Border.Width)"));
            Storyboard.SetTargetProperty(aniHeight, new PropertyPath("(Border.Height)"));

            sb.Completed += sb_Completed;
        }

        void DoExtendAnimation(ExtendMode mode, Button pressedButton)
        {
            if ((prevPressedButton == pressedButton) && (extended))
            {
                this.mode = mode;
                SetUpAnimationInfo();
                sb.Begin();
                extended = false;
            }
            else if ((prevPressedButton != pressedButton) && (extended))
            {
                DisplayExtendedMode(mode);
            }
            else if ((prevPressedButton == pressedButton) && (!extended))
            {
                this.mode = mode;
                SetUpAnimationInfo();
                sb.Begin();
                extended = true;
            }
            else if ((prevPressedButton != pressedButton) && (!extended))
            {
                this.mode = mode;
                SetUpAnimationInfo();
                sb.Begin();
                extended = true;
            }
            prevPressedButton = pressedButton;
        }

        void SetUpAnimationInfo()
        {
            if (extended)
            {
                aniWidth.From = bdOuterBack.Width;
                aniWidth.To = bdOuterBack.Width - 400;

                aniHeight.From = bdOuterBack.Height;
                aniHeight.To = bdOuterBack.Height - 400;
            }
            else
            {
                this.Width += 400;
                this.Height += 400;

                aniWidth.From = bdOuterBack.Width;
                aniWidth.To = bdOuterBack.Width + 400;

                aniHeight.From = bdOuterBack.Height;
                aniHeight.To = bdOuterBack.Height + 400;
            }
        }

        void ResetExtendedMode()
        {
            grdMain.Children.Remove(fileControl);
            grdMain.Children.Remove(option);
        }

        void DisplayExtendedMode(ExtendMode mode)
        {
            ResetExtendedMode();
            if (mode == ExtendMode.File)
            {
                fileControl.Height = 388;
                fileControl.Margin = new Thickness(10, 150, 10, 10);
                fileControl.Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));
                grdMain.Children.Add(fileControl);
            }
            else if (mode == ExtendMode.Option)
            {                
                option.Height = 388;
                option.Margin = new Thickness(10, 150, 10, 10);
                grdMain.Children.Add(option);
            }
        }

        void CloseExtendedMode(ExtendMode mode)
        {
            if (mode == ExtendMode.File)
            {
                grdMain.Children.Remove(fileControl);
            }
            else if (mode == ExtendMode.Option)
            {
                grdMain.Children.Remove(option);  
            }
        }

        void OpenPopUp(Button btn)
        {
            if ((!popMain.IsOpen) && ((!extended) || (prevPressedButton != btn)))
                popMain.IsOpen = true;            
        }

        void ClosePopUp()
        {
            if (popMain.IsOpen)            
                popMain.IsOpen = false;            
        }

        void MovePopUp(Button btn, MouseEventArgs e)
        {
            OpenPopUp(btn);

            if (popMain.PlacementTarget != btn)
                popMain.PlacementTarget = btn;

            Point currentPos = e.GetPosition(btn);            
            popMain.HorizontalOffset = currentPos.X + 10;
            popMain.VerticalOffset = currentPos.Y + 10;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;
            System.Windows.Forms.DialogResult result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                searchLocation = fd.SelectedPath;
                //txbLocation.Text = fd.SelectedPath;
            }
            e.Handled = true;
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            controller.Run(true, searchLocation);

            using (regKey = Registry.CurrentUser.OpenSubKey(regKeyLocation, true))
            {
                SaveRegistryKeyValue(regKey);
            }
        }

        private void bdInnerBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnOption_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoExtendAnimation(ExtendMode.Option, sender as Button);
            ClosePopUp();
        }

        private void btnProc_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoExtendAnimation(ExtendMode.File, sender as Button);
            ClosePopUp();
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            OpenPopUp(sender as Button);
        }

        private void btn_MouseMove(object sender, MouseEventArgs e)
        {
            MovePopUp(sender as Button, e);
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            ClosePopUp();
        }
    }
}
