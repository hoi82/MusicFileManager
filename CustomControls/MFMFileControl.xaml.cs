using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
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
        List<MFMFileItemControl> items = null;

        Panel panel = null;

        static MFMFileControl()
        {
            ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(MFMFileControl), new PropertyMetadata(10.0));            
            SelectedItemForegroundProperty = DependencyProperty.Register("SelectedItemForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedForegroundChanged)));
            UnSelectedItemForegroundProperty = DependencyProperty.Register("UnSelectedItemForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedForegroundChanged)));
            SelectedItemBackgroundProperty = DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedBackgroundChanged)));
            UnSelectedItemBackgroundProperty = DependencyProperty.Register("UnSelectedItemBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnUnSelectedBackgroundChanged)));
            ProcessingSuccessItemBackgroundProperty = DependencyProperty.Register("ProcessingSuccessItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingSuccessBackgroundChanged)));
            ProcessingSuccessItemForegroundProperty = DependencyProperty.Register("ProcessingSuccessItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingSuccessForegroundChanged)));
            ProcessingFailItemBackgroundProperty = DependencyProperty.Register("ProcessingFailItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingFailBackgroundChanged)));
            ProcessingFailItemForegroundProperty = DependencyProperty.Register("ProcessingFailItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingFailForegroundChanged)));
            ProcessingReadyItemBackgroundProperty = DependencyProperty.Register("ProcessingReadyItemBackground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingReadyBackgroundChanged)));
            ProcessingReadyItemForegroundProperty = DependencyProperty.Register("ProcessingReadyItemForeground", typeof(Brush), typeof(MFMFileItemControl), new PropertyMetadata(null, new PropertyChangedCallback(OnProcessingReadyForegroundChanged)));
            ItemContentBackgroundProperty = DependencyProperty.Register("ItemContentBackground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            ItemContentForegroundProperty = DependencyProperty.Register("ItemContentForeground", typeof(Brush), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MFMFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));
            ModeProperty = DependencyProperty.Register("Mode", typeof(MFMFileControlMode), typeof(MFMFileControl), new PropertyMetadata(MFMFileControlMode.Editing, new PropertyChangedCallback(OnModeChanged)));            
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnModechanged(e);
        }

        private void OnModechanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.Mode = (MFMFileControlMode)e.NewValue;
                }
            }
        }

        private static void OnProcessingReadyForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingReadyForegroundChanged(e);
        }

        private void OnProcessingReadyForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingReadyForeground = e.NewValue as Brush;
                }
            }
        }

        private static void OnProcessingReadyBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingReadyBackgroundChanged(e);
        }

        private void OnProcessingReadyBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingReadyBackground = e.NewValue as Brush;
                }
            }
        }

        private static void OnProcessingFailForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingFailForegroundChanged(e);
        }

        private void OnProcessingFailForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingFailForeground = e.NewValue as Brush;
                }
            }
        }

        private static void OnProcessingFailBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingFailBackgroundChanged(e);
        }

        private void OnProcessingFailBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingFailBackground = e.NewValue as Brush;
                }
            }
        }

        private static void OnProcessingSuccessForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingSuccessForegroundChanged(e);
        }

        private void OnProcessingSuccessForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingSuccessForeground = e.NewValue as Brush;
                }
            }
        }

        private static void OnProcessingSuccessBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnProcessingSuccessBackgroundChanged(e);
        }

        private void OnProcessingSuccessBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.ProcessingSuccessBackground = e.NewValue as Brush;
                }
            }
        }

        private static void OnUnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnUnSelectedBackgroundChanged(e);
        }

        private void OnUnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.UnSelectedBackground = e.NewValue as Brush;
                }
            }
        }

        private static void OnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnSelectedBackgroundChanged(e);
        }

        private void OnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.SelectedBackground = e.NewValue as Brush;
                }
            }
        }

        private static void OnUnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnUnSelectedForegroundChanged(e);
        }

        private void OnUnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.UnSelectedForeground = e.NewValue as Brush;
                }
            }
        }

        private static void OnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnSelectedForegroundChanged(e);
        }

        private void OnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in wpMain.Children)
            {
                if (item is MFMFileItemControl)
                {
                    MFMFileItemControl i = item as MFMFileItemControl;
                    i.SelectedForeground = e.NewValue as Brush;
                }
            }
        }

        public MFMFileControl()
        {
            InitializeComponent();

            datas = new List<object>();
            items = new List<MFMFileItemControl>();
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
            b.SelectedBackground = this.SelectedItemBackground;
            b.SelectedForeground = this.SelectedItemForeground;
            b.UnSelectedBackground = this.UnSelectedItemBackground;
            b.UnSelectedForeground = this.UnSelectedItemForeground;
            b.ProcessingSuccessBackground = this.ProcessingSuccessItemBackground;
            b.ProcessingSuccessForeground = this.ProcessingSuccessItemForeground;
            b.ProcessingReadyBackground = this.ProcessingReadyItemBackground;
            b.ProcessingReadyForeground = this.ProcessingReadyItemForeground;
            b.ProcessingFailBackground = this.ProcessingFailItemBackground;
            b.ProcessingFailForeground = this.ProcessingFailItemForeground;

            b.Mode = this.Mode;                        

            b.Click += b_Click;

            b.MouseMove += b_MouseMove;
            b.MouseLeave += b_MouseLeave;
            b.MouseEnter += b_MouseEnter;
        }

        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(this.tblItemContent.FontFamily, this.tblItemContent.FontStyle, this.tblItemContent.FontWeight, this.tblItemContent.FontStretch),
                this.tblItemContent.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
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
                grdItemContent.Width = MeasureString(message).Width + 20;
                grdItemContent.Height = MeasureString(message).Height + 20;
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
            AddItem(data, null);            
        }

        public void AddItem(object data, string name)
        {
            MFMFileItemControl item = new MFMFileItemControl() { Data = data, IconName = name };
            AddItem(item);   
        }

        public void AddItem(MFMFileItemControl item)
        {
            if (item != null)
            {
                SetItem(item);
                datas.Add(item.Data);
                wpMain.Children.Add(item);
                items.Add(item);
            }
        }

        public void DeleteItem(object data)
        {
            int index = datas.IndexOf(data);

            if (index > -1)
            {
                datas.Remove(data);
                wpMain.Children.RemoveAt(index);
                items.RemoveAt(index);
            }
        }

        public void AddItems(IList datas)
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
            items.Clear();
        }

        public List<MFMFileItemControl> SelectedItems()
        {
            List<MFMFileItemControl> items = new List<MFMFileItemControl>();
            foreach (var item in items)
            {                
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
            return items;
        }
    }
}
