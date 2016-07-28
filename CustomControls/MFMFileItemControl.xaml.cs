using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MusicFileManager.CustomControls
{
    /// <summary>
    /// MFMCleanFileItemControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MFMFileItemControl : UserControl
    {        
        Brush unSelectedBackground = null;
        Brush unSelectedForeground = null;
        Brush selectedBackground = null;
        Brush selectedForeground = null;
        bool selected = false;
        Brush processingSuccessBackground = null;
        Brush processingSuccessForeground = null;
        Brush processingFailBackground = null;
        Brush processingFailForeground = null;
        Brush processingReadyBackground = null;
        Brush processingReadyForeground = null;
        MFMFileProcessing processing = MFMFileProcessing.Ready;
        string iconName = null;
        object data = null;
        MFMFileControlMode mode = MFMFileControlMode.Editing;

        public static readonly RoutedEvent ClickEvent;

        bool clicked = false;

        static MFMFileItemControl()
        {            
            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MFMFileItemControl));
        }        
        
        public MFMFileItemControl()
        {
            InitializeComponent();
        }

        public MFMFileItemControl(object data)
        {
            this.Data = data;
        }                       

        public Brush UnSelectedBackground
        {
            get
            {                
                return this.unSelectedBackground;
            }
            set
            {                
                this.unSelectedBackground = value;
                SetDispalyColor();
            }
        }

        public Brush UnSelectedForeground
        {
            get
            {                
                return this.unSelectedForeground;
            }
            set
            {                
                this.unSelectedForeground = value;
                SetDispalyColor();
            }
        }

        public Brush SelectedBackground
        {
            get
            {                
                return this.selectedBackground;
            }
            set
            {                
                this.selectedBackground = value;
                SetDispalyColor();
            }
        }

        public Brush SelectedForeground
        {
            get
            {                
                return this.selectedForeground;
            }
            set
            {                
                this.selectedForeground = value;
                SetDispalyColor();
            }
        }

        public bool Selected
        {
            get
            {                
                return this.selected;
            }
            set
            {                
                this.selected = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingSuccessBackground
        {
            get
            {                
                return this.processingSuccessBackground;
            }
            set
            {                
                this.processingSuccessBackground = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingSuccessForeground
        {
            get
            {                
                return this.processingSuccessForeground;
            }
            set
            {                
                this.processingSuccessForeground = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingFailBackground
        {
            get
            {                
                return this.processingFailBackground;
            }
            set
            {                
                this.processingFailBackground = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingFailForeground
        {
            get
            {                
                return this.processingFailForeground;
            }
            set
            {                
                this.processingFailForeground = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingReadyBackground
        {
            get
            {                
                return this.processingReadyBackground;
            }
            set
            {                
                this.processingReadyBackground = value;
                SetDispalyColor();
            }
        }

        public Brush ProcessingReadyForeground
        {
            get
            {                
                return this.processingReadyForeground;
            }
            set
            {                
                this.processingReadyForeground = value;
                SetDispalyColor();
            }
        }

        public MFMFileProcessing Processing
        {
            get
            {                
                return this.processing;
            }
            set
            {                
                this.processing = value;
                SetDispalyColor();
            }
        }

        public MFMFileControlMode Mode
        {
            get
            {                
                return this.mode;
            }
            set
            {                
                this.mode = value;
                SetDispalyColor();
            }
        }

        public string IconName
        {
            get
            {                
                return this.iconName;
            }
            set
            {                
                this.iconName = value;
                if (this.lblText != null)
                {
                    this.lblText.Content = value.Substring(0,2);
                }                    
            }
        }

        public object Data
        {
            get
            {                
                return this.data;
            }
            set
            {                
                this.data = value;
            }
        }

        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        void SetDispalyColor()
        {
            switch (this.mode)
            {
                case MFMFileControlMode.Editing:
                    {
                        if (selected)
                        {
                            this.elBack.Fill = selectedBackground;
                            this.lblText.Foreground = selectedForeground;
                        }
                        else
                        {
                            this.elBack.Fill = unSelectedBackground;
                            this.lblText.Foreground = unSelectedForeground;
                        }
                    }
                    break;
                case MFMFileControlMode.Processing:
                    {
                        switch (processing)
                        {
                            case MFMFileProcessing.Ready:
                                {
                                    this.elBack.Fill = processingReadyBackground;
                                    this.lblText.Foreground = processingReadyForeground;
                                }
                                break;
                            case MFMFileProcessing.Success:
                                {
                                    this.elBack.Fill = processingSuccessBackground;
                                    this.lblText.Foreground = processingSuccessForeground;
                                }
                                break;
                            case MFMFileProcessing.Fail:
                                {
                                    this.elBack.Fill = processingFailBackground;
                                    this.lblText.Foreground = processingFailForeground;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (clicked)
                clicked = false;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clicked = true;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (clicked)
            {
                clicked = false;
                RoutedEventArgs args = new RoutedEventArgs(MFMFileItemControl.ClickEvent);
                RaiseEvent(args);
            }
        }       
    }
}
