using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public enum MFMFileControlMode { Editing, Processing }
    public enum MFMFileProcessing { Ready, Success, Fail }
    /// <summary>
    /// MFMCleanFileControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MFMFileControl : UserControl
    {
        public static DependencyProperty ItemSizeProperty;        
        public static DependencyProperty SelectedItemForegroundProperty;
        public static DependencyProperty UnSelectedItemForegroundProperty;
        public static DependencyProperty SelectedItemBackgroundProperty;
        public static DependencyProperty UnSelectedItemBackgroundProperty;
        public static DependencyProperty ProcessingSuccessItemBackgroundProperty;
        public static DependencyProperty ProcessingSuccessItemForegroundProperty;
        public static DependencyProperty ProcessingFailItemBackgroundProperty;
        public static DependencyProperty ProcessingFailItemForegroundProperty;
        public static DependencyProperty ProcessingReadyItemBackgroundProperty;
        public static DependencyProperty ProcessingReadyItemForegroundProperty;
        public static DependencyProperty ItemContentBackgroundProperty;
        public static DependencyProperty ItemContentForegroundProperty;
        public static DependencyProperty TitleProperty;
        public static DependencyProperty ModeProperty;

        List<object> datas = null;

        Panel panel = null;

        static MFMFileControl()
        {
            ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(MFMFileControl), new PropertyMetadata(10.0));            
            SelectedItemForegroundProperty = DependencyProperty.Register("SelectedItemForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null));
            UnSelectedItemForegroundProperty = DependencyProperty.Register("UnSelectedItemForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null));
            SelectedItemBackgroundProperty = DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null));
            UnSelectedItemBackgroundProperty = DependencyProperty.Register("UnSelectedItemBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null));
            ProcessingSuccessItemBackgroundProperty = DependencyProperty.Register("ProcessingSuccessItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ProcessingSuccessItemForegroundProperty = DependencyProperty.Register("ProcessingSuccessItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ProcessingFailItemBackgroundProperty = DependencyProperty.Register("ProcessingFailItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ProcessingFailItemForegroundProperty = DependencyProperty.Register("ProcessingFailItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ProcessingReadyItemBackgroundProperty = DependencyProperty.Register("ProcessingReadyItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ProcessingReadyItemForegroundProperty = DependencyProperty.Register("ProcessingReadyItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null));
            ItemContentBackgroundProperty = DependencyProperty.Register("ItemContentBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            ItemContentForegroundProperty = DependencyProperty.Register("ItemContentForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));
            ModeProperty = DependencyProperty.Register("Mode", typeof(MFMFileControlMode), typeof(MFMFileControl), new PropertyMetadata(MFMFileControlMode.Editing));            
        }

        public MFMFileControl()
        {
            InitializeComponent();

            datas = new List<object>();
        }

        public MFMFileControl(Panel panel) : this()
        {
            this.panel = panel;
        }

        public double ItemSize
        {
            get
            {
                return (double)this.GetValue(ItemSizeProperty);                
            }
            set
            {
                this.SetValue(ItemSizeProperty, value);                
            }
        }        

        public Brush SelectedItemForeground
        {
            get
            {
                return (Brush)this.GetValue(SelectedItemForegroundProperty);
            }
            set
            {
                this.SetValue(SelectedItemForegroundProperty, value);
            }
        }

        public Brush SelectedItemBackground
        {
            get
            {
                return (Brush)this.GetValue(SelectedItemBackgroundProperty);
            }
            set
            {
                this.SetValue(SelectedItemBackgroundProperty, value);
            }
        }

        public Brush UnSelectedItemForeground
        {
            get
            {
                return (Brush)GetValue(UnSelectedItemForegroundProperty);
            }
            set
            {
                SetValue(UnSelectedItemForegroundProperty, value);
            }
        }

        public Brush UnSelectedItemBackground
        {
            get
            {
                return (Brush)GetValue(UnSelectedItemBackgroundProperty);
            }
            set
            {
                SetValue(UnSelectedItemBackgroundProperty, value);
            }
        }

        public Brush ProcessingSuccessItemBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingSuccessItemBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingSuccessItemBackgroundProperty, value);
            }
        }

        public Brush ProcessingSuccessItemForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingSuccessItemForegroundProperty);
            }
            set
            {
                SetValue(ProcessingSuccessItemForegroundProperty, value);
            }
        }

        public Brush ProcessingFailItemBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingFailItemBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingFailItemBackgroundProperty, value);
            }
        }

        public Brush ProcessingFailItemForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingFailItemForegroundProperty);
            }
            set
            {
                SetValue(ProcessingFailItemForegroundProperty, value);
            }
        }

        public Brush ProcessingReadyItemBackground
        {
            get
            {
                return (Brush)GetValue(ProcessingReadyItemBackgroundProperty);
            }
            set
            {
                SetValue(ProcessingReadyItemBackgroundProperty, value);
            }
        }

        public Brush ProcessingReadyItemForeground
        {
            get
            {
                return (Brush)GetValue(ProcessingReadyItemForegroundProperty);
            }
            set
            {
                SetValue(ProcessingReadyItemForegroundProperty, value);
            }
        }

        public Brush ItemContentBackground
        {
            get
            {
                return (Brush)GetValue(ItemContentBackgroundProperty);
            }
            set
            {
                SetValue(ItemContentBackgroundProperty, value);
            }
        }

        public Brush ItemContentForeground
        {
            get
            {
                return (Brush)GetValue(ItemContentForegroundProperty);
            }
            set
            {
                SetValue(ItemContentForegroundProperty, value);
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
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

        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnTitleChanged(e);
        }

        static void OnItemContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnItemContentChanged(e);
        }

        void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            lbTitle.Content = e.NewValue;
        }

        void OnItemContentChanged(DependencyPropertyChangedEventArgs e)
        {
            grdItemContent.Background = ItemContentBackground;
            tblItemContent.Foreground = ItemContentForeground;
        }         

        void SetItem(MFMFileItemControl b)
        {
            Binding selectedForegroundBinding = new Binding("SelectedItemForeground");
            selectedForegroundBinding.Source = this;
            selectedForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.SelectedForegroundProperty, selectedForegroundBinding);

            Binding selectedbackgroundBinding = new Binding("SelectedItemBackground");
            selectedbackgroundBinding.Source = this;
            selectedbackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.SelectedBackgroundProperty, selectedbackgroundBinding);

            Binding unselectedForegroundBinding = new Binding("UnSelectedItemForeground");
            unselectedForegroundBinding.Source = this;
            unselectedForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.UnSelectedForegroundProperty, unselectedForegroundBinding);

            Binding unselectedbackgroundBinding = new Binding("UnSelectedItemBackground");
            unselectedbackgroundBinding.Source = this;
            unselectedbackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.UnSelectedBackgroundProperty, unselectedbackgroundBinding);

            Binding procSuccessBackgroundBinding = new Binding("ProcessingSuccessItemBackground");
            procSuccessBackgroundBinding.Source = this;
            procSuccessBackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingSuccessBackgroundProperty, procSuccessBackgroundBinding);

            Binding procSuccessForegroundBinding = new Binding("ProcessingSuccessItemForeground");
            procSuccessForegroundBinding.Source = this;
            procSuccessForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingSuccessForegroundProperty, procSuccessForegroundBinding);

            Binding procFailBackgroundBinding = new Binding("ProcessingFailItemBackground");
            procFailBackgroundBinding.Source = this;
            procFailBackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingFailBackgroundProperty, procFailBackgroundBinding);

            Binding procFailForegroundBinding = new Binding("ProcessingFailItemForeground");
            procFailForegroundBinding.Source = this;
            procFailForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingFailForegroundProperty, procFailForegroundBinding);

            Binding procReadyBackgroundBinding = new Binding("ProcessingReadyItemBackground");
            procReadyBackgroundBinding.Source = this;
            procReadyBackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingReadyBackgroundProperty, procReadyBackgroundBinding);

            Binding procReadyForegroundBinding = new Binding("ProcessingReadyItemForeground");
            procReadyForegroundBinding.Source = this;
            procReadyForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ProcessingReadyForegroundProperty, procReadyForegroundBinding);

            Binding modeBinding = new Binding("Mode");
            modeBinding.Source = this;
            modeBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMFileItemControl.ModeProperty, modeBinding);

            b.Click += b_Click;

            b.MouseMove += b_MouseMove;
            b.MouseLeave += b_MouseLeave;
            b.MouseEnter += b_MouseEnter;
        }

        void b_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!popItem.IsOpen)
                popItem.IsOpen = true;

            MFMFileItemControl bi = sender as MFMFileItemControl;

            if (popItem.PlacementTarget != bi)
                popItem.PlacementTarget = bi;

            if (bi.Data != null)
            {
                string message = bi.Data.ToString();
                tblItemContent.Text = message;
            }                
            else
                tblItemContent.Text = null;
        }

        void b_MouseLeave(object sender, MouseEventArgs e)
        {
            if (popItem.IsOpen)
                popItem.IsOpen = false;
        }

        void b_MouseMove(object sender, MouseEventArgs e)
        {
            MFMFileItemControl bi = sender as MFMFileItemControl;

            Point currentPos = e.GetPosition(bi);

            popItem.HorizontalOffset = currentPos.X + 10;
            popItem.VerticalOffset = currentPos.Y + 10;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == MFMFileControlMode.Editing)
            {
                MFMFileItemControl bi = sender as MFMFileItemControl;
                bi.Selected = !bi.Selected;
            }                        
        }

        public void AddItem(object data)
        {
            MFMFileItemControl item = new MFMFileItemControl() { Data = data };
            SetItem(item);
            datas.Add(data);
            wpMain.Children.Add(item);            
        }

        public void AddItem(object data, string name)
        {
            MFMFileItemControl item = new MFMFileItemControl() { Data = data, IconName = name };
            SetItem(item);
            datas.Add(data);
            wpMain.Children.Add(item);
        }

        public void DeleteItem(object data)
        {
            int index = datas.IndexOf(data);

            if (index > -1)
            {
                datas.Remove(data);
                wpMain.Children.RemoveAt(index);
            }
        }

        public void AddItems(IList<object> datas)
        {
            foreach (var data in datas)
            {
                AddItem(data);
            }
        }

        public void ClearItems()
        {
            datas.Clear();
            wpMain.Children.Clear();
        }

        public List<MFMFileItemControl> SelectedItems()
        {
            List<MFMFileItemControl> items = new List<MFMFileItemControl>();
            foreach (var child in wpMain.Children)
            {
                MFMFileItemControl item = child as MFMFileItemControl;

                if (item != null)
                {
                    if (item.Selected)
                        items.Add(item);
                }
            }
            return items;
        }

        public List<MFMFileItemControl> Items()
        {
            List<MFMFileItemControl> items = new List<MFMFileItemControl>();
            foreach (var child in wpMain.Children)
            {
                MFMFileItemControl item = child as MFMFileItemControl;

                if (item != null)
                {                    
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
