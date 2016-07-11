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
    public partial class MFMFileItemControl : UserControl, ICustomTypeDescriptor
    {
        public static DependencyProperty UnSelectedBackgroundProperty;
        public static DependencyProperty UnSelectedForegroundProperty;
        public static DependencyProperty SelectedBackgroundProperty;
        public static DependencyProperty SelectedForegroundProperty;
        public static DependencyProperty SelectedProperty;
        public static DependencyProperty ProcessingSuccessBackgroundProperty;
        public static DependencyProperty ProcessingSuccessForegroundProperty;
        public static DependencyProperty ProcessingFailBackgroundProperty;
        public static DependencyProperty ProcessingFailForegroundProperty;
        public static DependencyProperty ProcessingReadyBackgroundProperty;
        public static DependencyProperty ProcessingReadyForegroundProperty;
        public static DependencyProperty ProcessingProperty;
        public static DependencyProperty IconNameProperty;
        public static DependencyProperty DataProperty;
        public static DependencyProperty ModeProperty;

        public static readonly RoutedEvent ClickEvent;

        bool clicked = false;

        static MFMFileItemControl()
        {
            UnSelectedBackgroundProperty = DependencyProperty.Register("UnSelectedBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedBackgroundChanged)));
            UnSelectedForegroundProperty = DependencyProperty.Register("UnSelectedForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedForegroundChanged)));
            SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedBackgroundChanged)));
            SelectedForegroundProperty = DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedForegroundChanged)));
            SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(MFMFileItemControl), new PropertyMetadata(false, new PropertyChangedCallback(OnSelectedChanged)));
            ProcessingSuccessBackgroundProperty = DependencyProperty.Register("ProcessingSuccessBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingSuccessBackgroundChanged)));
            ProcessingSuccessForegroundProperty = DependencyProperty.Register("ProcessingSuccessForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingSuccessForegroundChanged)));
            ProcessingFailBackgroundProperty = DependencyProperty.Register("ProcessingFailBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingFailBackgroundChanged)));
            ProcessingFailForegroundProperty = DependencyProperty.Register("ProcessingFailForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingFailForegroundChanged)));
            ProcessingReadyBackgroundProperty = DependencyProperty.Register("ProcessingReadyBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingReadyBackgroundChanged)));
            ProcessingReadyForegroundProperty = DependencyProperty.Register("ProcessingReadyForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingReadyForegroundChanged)));
            ProcessingProperty = DependencyProperty.Register("Processing", typeof(MFMFileProcessing), typeof(MFMFileItemControl), new PropertyMetadata(MFMFileProcessing.Ready, new PropertyChangedCallback(OnProcessingChanged)));
            IconNameProperty = DependencyProperty.Register("IconName", typeof(string), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnIconNameChanged)));
            DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ModeProperty = DependencyProperty.Register("Mode", typeof(MFMFileControlMode), typeof(MFMFileItemControl), new PropertyMetadata(MFMFileControlMode.Editing, new PropertyChangedCallback(OnModeChanged)));

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

        static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnModeChanged(e);
        }        

        static void OnProcessingReadyForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingReadyForegroundChanged(e);
        }        

        static void OnProcessingReadyBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingReadyBackgroundChanged(e);
        }        

        static void OnProcessingFailForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingFailForegroundChanged(e);
        }

        static void OnProcessingFailBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingFailBackgroundChanged(e);
        }        

        static void OnProcessingSuccessForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingSuccessForegroundChanged(e);
        }        

        static void OnProcessingSuccessBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingSuccessBackgroundChanged(e);
        }        

        static void OnProcessingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnProcessingChanged(e);
        }

        static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnSelectedChanged(e);
        }

        static void OnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnSelectedForegroundChanged(e);
        }

        static void OnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnSelectedBackgroundChanged(e);
        }

        static void OnUnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnUnSelectedForegroundChanged(e);
        }

        static void OnUnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnUnSelectedBackgroundChanged(e);
        }

        static void OnIconNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileItemControl).OnIconNameChanged(e);
        }

        void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (Mode)
            {
                case MFMFileControlMode.Editing:
                    {
                        if (Selected)
                        {
                            elBack.Fill = SelectedBackground;
                            lblText.Foreground = SelectedForeground;
                        }
                        else
                        {
                            elBack.Fill = UnSelectedBackground;
                            lblText.Foreground = UnSelectedForeground;
                        }
                    }
                    break;
                case MFMFileControlMode.Processing:
                    {
                        switch (Processing)
                        {
                            case MFMFileProcessing.Ready:
                                {
                                    elBack.Fill = ProcessingReadyBackground;
                                    lblText.Foreground = ProcessingReadyForeground;
                                }
                                break;
                            case MFMFileProcessing.Success:
                                {
                                    elBack.Fill = ProcessingSuccessBackground;
                                    lblText.Foreground = ProcessingSuccessForeground;
                                }
                                break;
                            case MFMFileProcessing.Fail:
                                {
                                    elBack.Fill = ProcessingFailBackground;
                                    lblText.Foreground = ProcessingFailForeground;
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

        void OnProcessingReadyForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Processing) && (Processing == MFMFileProcessing.Ready))
                lblText.Foreground = ProcessingReadyForeground;
        }

        void OnProcessingReadyBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Processing) && (Processing == MFMFileProcessing.Ready))
                elBack.Fill = ProcessingReadyBackground;
        }

        void OnProcessingFailForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Processing) && (Processing == MFMFileProcessing.Fail))
                lblText.Foreground = ProcessingFailForeground;
        }

        void OnProcessingFailBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Mode == MFMFileControlMode.Processing && Processing == MFMFileProcessing.Fail)
                elBack.Fill = ProcessingFailBackground;
        }

        void OnProcessingSuccessForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Processing) && (Processing == MFMFileProcessing.Success))
                lblText.Foreground = ProcessingSuccessForeground;
        }

        void OnProcessingSuccessBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Mode == MFMFileControlMode.Processing && Processing == MFMFileProcessing.Success)
                elBack.Fill = ProcessingSuccessBackground;
        }

        void OnProcessingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Mode == MFMFileControlMode.Processing)
            {
                switch (Processing)
                {
                    case MFMFileProcessing.Ready:
                        {
                            elBack.Fill = ProcessingReadyBackground;
                            lblText.Foreground = ProcessingReadyForeground;
                        }
                        break;
                    case MFMFileProcessing.Success:
                        {
                            elBack.Fill = ProcessingSuccessBackground;
                            lblText.Foreground = ProcessingSuccessForeground;
                        }
                        break;
                    case MFMFileProcessing.Fail:
                        {
                            elBack.Fill = ProcessingFailBackground;
                            lblText.Foreground = ProcessingFailForeground;
                        }
                        break;
                    default:
                        break;
                }
            }            
        }

        void OnSelectedChanged(DependencyPropertyChangedEventArgs e)
        {            
            if ((Mode == MFMFileControlMode.Editing) && Selected)
            {
                elBack.Fill = SelectedBackground;
                lblText.Foreground = SelectedForeground;
            }
            else
            {
                elBack.Fill = UnSelectedBackground;
                lblText.Foreground = UnSelectedForeground;
            }
        }

        void OnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Editing) && Selected)
                lblText.Foreground = e.NewValue as Brush;
        }

        void OnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Editing) && Selected)
                elBack.Fill = e.NewValue as Brush;
        }

        void OnUnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Editing) && !Selected)
                lblText.Foreground = e.NewValue as Brush;
        }

        void OnUnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Mode == MFMFileControlMode.Editing) && !Selected)
                elBack.Fill = e.NewValue as Brush;
        }

        void OnIconNameChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e != null)
            {
                lblText.Content = e.NewValue.ToString().Substring(0, 2);
            }
            else
            {
                lblText.Content = "??";
            }
        }       

        public Brush UnSelectedBackground
        {
            get
            {
                return (Brush)GetValue(UnSelectedBackgroundProperty);
            }
            set
            {
                SetValue(UnSelectedBackgroundProperty, value);
            }
        }

        public Brush UnSelectedForeground
        {
            get
            {
                return (Brush)GetValue(UnSelectedForegroundProperty);
            }
            set
            {
                SetValue(UnSelectedForegroundProperty, value);
            }
        }

        public Brush SelectedBackground
        {
            get
            {
                return (Brush)GetValue(SelectedBackgroundProperty);
            }
            set
            {
                SetValue(SelectedBackgroundProperty, value);
            }
        }

        public Brush SelectedForeground
        {
            get
            {
                return (Brush)GetValue(SelectedForegroundProperty);
            }
            set
            {
                SetValue(SelectedForegroundProperty, value);
            }
        }

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        public Brush ProcessingSuccessBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingSuccessBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingSuccessBackgroundProperty, value);
            }
        }

        public Brush ProcessingSuccessForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingSuccessForegroundProperty);
            }
            set
            {
                SetValue(ProcessingSuccessForegroundProperty, value);
            }
        }

        public Brush ProcessingFailBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingFailBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingFailBackgroundProperty, value);
            }
        }

        public Brush ProcessingFailForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingFailForegroundProperty);
            }
            set
            {
                SetValue(ProcessingFailForegroundProperty, value);
            }
        }

        public Brush ProcessingReadyBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingReadyBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingReadyBackgroundProperty, value);
            }
        }

        public Brush ProcessingReadyForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingReadyForegroundProperty);
            }
            set
            {
                SetValue(ProcessingReadyForegroundProperty, value);
            }
        }

        public MFMFileProcessing Processing
        {
            get
            {
                return (MFMFileProcessing)GetValue(ProcessingProperty);
            }
            set
            {
                SetValue(ProcessingProperty, value);
            }
        }

        public MFMFileControlMode Mode
        {
            get
            {
                return (MFMFileControlMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        public string IconName
        {
            get
            {
                return (string)GetValue(IconNameProperty);
            }
            set
            {
                SetValue(IconNameProperty, value);
            }
        }

        public object Data
        {
            get
            {
                return GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
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

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
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
