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
    public partial class MFMCleanFileItemControl : UserControl, ICustomTypeDescriptor
    {
        public static DependencyProperty UnSelectedBackgroundProperty;
        public static DependencyProperty UnSelectedForegroundProperty;
        public static DependencyProperty SelectedBackgroundProperty;
        public static DependencyProperty SelectedForegroundProperty;
        public static DependencyProperty SelectedProperty;
        public static DependencyProperty IconNameProperty;
        public static DependencyProperty DataProperty;

        public static readonly RoutedEvent ClickEvent;

        bool clicked = false;

        static MFMCleanFileItemControl()
        {
            UnSelectedBackgroundProperty = DependencyProperty.Register("UnSelectedBackground", typeof(Brush), typeof(MFMCleanFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedBackgroundChanged)));
            UnSelectedForegroundProperty = DependencyProperty.Register("UnSelectedForeground", typeof(Brush), typeof(MFMCleanFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedForegroundChanged)));
            SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(MFMCleanFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedBackgroundChanged)));
            SelectedForegroundProperty = DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(MFMCleanFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedForegroundChanged)));
            SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(MFMCleanFileItemControl), new PropertyMetadata(false, new PropertyChangedCallback(OnSelectedChanged)));
            IconNameProperty = DependencyProperty.Register("IconName", typeof(string), typeof(MFMCleanFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnIconNameChanged)));
            DataProperty = DependencyProperty.Register("Data", typeof(DuplicatedFiles), typeof(MFMCleanFileItemControl), new PropertyMetadata(null));

            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MFMCleanFileItemControl));
        }
        public MFMCleanFileItemControl()
        {
            InitializeComponent();
        }

        public MFMCleanFileItemControl(DuplicatedFiles data)
        {
            this.Data = data;
        }

        static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnSelectedChanged(e);
        }

        static void OnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnSelectedForegroundChanged(e);
        }

        static void OnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnSelectedBackgroundChanged(e);
        }

        static void OnUnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnUnSelectedForegroundChanged(e);
        }

        static void OnUnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnUnSelectedBackgroundChanged(e);
        }

        static void OnIconNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileItemControl).OnIconNameChanged(e);
        }

        void OnSelectedChanged(DependencyPropertyChangedEventArgs e)
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

        void OnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Selected)
                lblText.Foreground = e.NewValue as Brush;
        }

        void OnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Selected)
                elBack.Fill = e.NewValue as Brush;
        }

        void OnUnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!Selected)
                lblText.Foreground = e.NewValue as Brush;
        }

        void OnUnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!Selected)
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

        public DuplicatedFiles Data
        {
            get
            {
                return (DuplicatedFiles)GetValue(DataProperty);
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
                RoutedEventArgs args = new RoutedEventArgs(MFMCleanFileItemControl.ClickEvent);
                RaiseEvent(args);
            }
        }
    }
}
