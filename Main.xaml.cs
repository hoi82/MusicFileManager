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
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Animation;
using MusicFileManager.CustomControls;
using System.Globalization;

namespace MusicFileManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {        
        enum ExtendMode { File, Option };

        MainController controller = null;

        Storyboard sb = null;
        DoubleAnimation aniWidth = null;
        DoubleAnimation aniHeight = null;

        bool extended = false;

        public MFMOption option = null;
        public MFMFileControl fileControl = null;

        ExtendMode mode = ExtendMode.File;
        Button prevPressedButton = null;
        public Button currentMouserOverButton = null;

        public MainWindow()
        {
            InitializeComponent();

            option = new MFMOption();            
            
            fileControl = new MFMFileControl();
            fileControl.SelectedItemBackground = Brushes.Orange;
            fileControl.SelectedItemForeground = Brushes.White;
            fileControl.UnSelectedItemBackground = Brushes.Aquamarine;
            fileControl.UnSelectedItemForeground = Brushes.White;
            fileControl.ProcessingFailItemBackground = Brushes.Red;
            fileControl.ProcessingFailItemForeground = Brushes.White;
            fileControl.ProcessingReadyItemBackground = Brushes.Orange;
            fileControl.ProcessingReadyItemForeground = Brushes.White;
            fileControl.ProcessingSuccessItemBackground = Brushes.Green;
            fileControl.ProcessingSuccessItemForeground = Brushes.White;            
            fileControl.ItemSize = 20;

            controller = new MainController(this);                                          
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
                //fileControl.Height = 388;
				fileControl.Height = double.NaN;
                fileControl.Margin = new Thickness(10, 150, 10, 30);
                fileControl.Background = new SolidColorBrush(Color.FromArgb(100, 50, 50, 50));
                grdMain.Children.Add(fileControl);                
            }
            else if (mode == ExtendMode.Option)
            {                
                option.Height = double.NaN;
                option.Margin = new Thickness(10, 150, 10, 30);
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

        Size MeasureString(string candidate, Label label)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public void SetPopUpDisplay(bool progressBarVisible, params string[] contents)
        {
            const double PADDING_BETWEEN_CONTROL = 10;
            double width = 0;
            double height = 0;
            //grdOuterPop.Width = width;
            //grdOuterPop.Height = height;

            if (progressBarVisible)
                prgPop.Visibility = Visibility.Visible;
            else
                prgPop.Visibility = Visibility.Hidden;

            if (contents.Length > 2)
            {
                if (!string.IsNullOrEmpty(contents[0]))
                    lblBackgroundPop.Content = contents[0];

                if (!string.IsNullOrEmpty(contents[1]))
                    lblUpperPop.Content = contents[1];

                if (!string.IsNullOrEmpty(contents[2]))
                    lblLowerPop.Content = contents[2];
            }

            Size upperSize = MeasureString(lblUpperPop.Content.ToString(), lblUpperPop);
            Size lowerSize = MeasureString(lblLowerPop.Content.ToString(), lblLowerPop);
            
            if (progressBarVisible)
            {
                height += prgPop.Height;
                prgPop.Margin = new Thickness(PADDING_BETWEEN_CONTROL, lblUpperPop.Margin.Top + upperSize.Height + PADDING_BETWEEN_CONTROL, PADDING_BETWEEN_CONTROL, 0);
                lblLowerPop.Margin = new Thickness(PADDING_BETWEEN_CONTROL, prgPop.Margin.Top + PADDING_BETWEEN_CONTROL, 0, 0);
            }
            else
            {
                lblLowerPop.Margin = new Thickness(PADDING_BETWEEN_CONTROL, lblUpperPop.Margin.Top + upperSize.Height + PADDING_BETWEEN_CONTROL, 0, 0);
            }

            width = Math.Max(upperSize.Width, lowerSize.Width) + PADDING_BETWEEN_CONTROL + ((lblUpperPop.Margin.Left + bdInnerPop.Margin.Left + bdOuterPop.Margin.Left) * 2);            
            height = lblLowerPop.Margin.Top + lowerSize.Height + PADDING_BETWEEN_CONTROL + ((lblUpperPop.Margin.Top + bdInnerPop.Margin.Top + bdOuterPop.Margin.Top) * 2);
            //글자로 인한 진동 방지            
            if (Math.Abs(width - grdOuterPop.Width) > lblUpperPop.FontSize)
                grdOuterPop.Width = width;
            else
                grdOuterPop.Width = Math.Max(grdOuterPop.Width, width);
            grdOuterPop.Height = height;
        }

        public void DisplayPopUp()
        {
            if (currentMouserOverButton == btnBrowse)
            {
                SetPopUpDisplay(false, "Searching Location", controller.SearchLocation, "Click this button for select directory for clean");                
            }
            else if (currentMouserOverButton == btnOption)
            {
                string upperStr = null;
                string lowerStr = null;                

                if (option.DeleteAudioWithOutBitRate)
                {
                    upperStr = string.Format("Include archive file with multiple musics : {0} \r\nInclude similar music files unconditionally : {1}", option.DeleteArchiveWithMulipleAudio, option.DeleteAudioWithOutBitRate);                    
                }
                else
                {
                    upperStr = string.Format("Include archive file with multiple musics : {0} \r\nInclude similar music files unconditionally : {1} \r\nAudio BitRate : {2} \r\nDuration : {3}", option.DeleteArchiveWithMulipleAudio, option.DeleteAudioWithOutBitRate, option.AudioBitRate, option.AudioDuration);                    
                }

                if (extended)
                    lowerStr = "Click for hide detail options";
                else
                    lowerStr = "Click for show detail options";

                SetPopUpDisplay(false, "Filtering Options", upperStr, lowerStr);                
            }
            else if (currentMouserOverButton == btnProc)
            {
                bool prgVisible = false;
                string upperStr = null;
                string lowerStr = null;

                lblBackgroundPop.Content = "Processing";
                if (controller.processingMode == ProcessingMode.ReadyFind)
                {                    
                    prgVisible = false;
                    upperStr = "Ready for Check Files";
                    lowerStr = "Click for Check Files";
                }
                else if (controller.processingMode == ProcessingMode.CollectFile)
                {                    
                    prgVisible = true;                    
                }
                else if (controller.processingMode == ProcessingMode.CheckDuplication)
                {
                    prgVisible = true;
                    upperStr = "Ready for Check Files";
                    lowerStr = "Click for Check Files";
                }
                else if (controller.processingMode == ProcessingMode.ReadyClean)
                {                    
                    prgVisible = false;
                    upperStr = "Ready for Clean files";
                    lowerStr = "Click for Clean files\r\nRight Click for Show Details";                    
                }
                else if (controller.processingMode == ProcessingMode.Clean)
                {                    
                    prgVisible = true;                    
                }

                SetPopUpDisplay(prgVisible, "Processing", upperStr, lowerStr);
            }
            else if (currentMouserOverButton == btnExit)
            {
                SetPopUpDisplay(false, "Exit", "Click for exit application", "Right click for tray icon");                
            }
        }

        void OpenPopUp(Button btn)
        {
            //if ((!popMain.IsOpen) && ((!extended) || (prevPressedButton != btn)))
            if (!popMain.IsOpen)
            {
                popMain.IsOpen = true;

                DisplayPopUp();
            }                
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
            controller.BrowseDirectory();
        }

        private void btnProc_Click(object sender, RoutedEventArgs e)
        {
            controller.Process();            
        }

        private void bdInnerBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnProc_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;

            if ((controller.processingMode == ProcessingMode.ReadyClean) || (controller.processingMode == ProcessingMode.Clean))
            {
                DoExtendAnimation(ExtendMode.File, sender as Button);
                ClosePopUp();
            }            
        }

        private void btnOption_Click(object sender, RoutedEventArgs e)
        {
            DoExtendAnimation(ExtendMode.Option, sender as Button);
            ClosePopUp();
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            currentMouserOverButton = sender as Button;
            OpenPopUp(currentMouserOverButton);
        }

        private void btn_MouseMove(object sender, MouseEventArgs e)
        {
            MovePopUp(sender as Button, e);
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            currentMouserOverButton = null;
            ClosePopUp();
        }
    }
}
