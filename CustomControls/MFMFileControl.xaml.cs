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
        public static DependencyProperty ItemsProperty;
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

        Panel panel = null;

        static MFMFileControl()
        {
            ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(MFMFileControl), new PropertyMetadata(10.0));
            ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<MFMFileItemControl>), typeof(MFMFileControl), new PropertyMetadata(new ObservableCollection<MFMFileItemControl>(), new PropertyChangedCallback(OnItemsChanged)));
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
            ModeProperty = DependencyProperty.Register("Mode", typeof(MFMFileControlMode), typeof(MFMFileControl), new PropertyMetadata(MFMFileControlMode.Editing, new PropertyChangedCallback(OnModeChanged)));            
        }

        public MFMFileControl()
        {
            InitializeComponent();
            SetValue(ItemsProperty, new ObservableCollection<MFMFileItemControl>()); 
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
                //return Dispatcher.Invoke(() => (double)this.GetValue(ItemSizeProperty));
            }
            set
            {
                this.SetValue(ItemSizeProperty, value);
                //Dispatcher.BeginInvoke((Action)(() => this.SetValue(ItemSizeProperty, value)));
            }
        }

        public ObservableCollection<MFMFileItemControl> Items
        {
            get
            {
                //return Dispatcher.Invoke(() => (ObservableCollection<MFMFileItemControl>)this.GetValue(ItemsProperty));
                return (ObservableCollection<MFMFileItemControl>)this.GetValue(ItemsProperty);
            }
            set
            {
                //Dispatcher.BeginInvoke((Action)(() => this.SetValue(ItemsProperty, value)));
                this.SetValue(ItemsProperty, value);
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

        static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnItemsChanged();
        }

        static void OnItemContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnItemContentChanged(e);
        }

        static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMFileControl).OnModeChanged(e);
        }

        void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            lbTitle.Content = e.NewValue;
        }

        void OnItemsChanged()
        {
            if (Items != null)
                Items.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsChanged);
        }

        void OnItemContentChanged(DependencyPropertyChangedEventArgs e)
        {
            grdItemContent.Background = ItemContentBackground;
            tblItemContent.Foreground = ItemContentForeground;
        }

        void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //추가되는 아이템은 NewItem에, 제거되는 아이템은 OldItem에 들어있다.
            //Move의 경우 움직이는 아이템이 NewItem과 Old아이템 모두에 들어있고,
            //이전 인덱스가 OldStartingIndex, 새 인덱스가 NewStartingIndex에 들어있다.
            //Replace의 경우 들어올 아이템이 NewItem, 이전 아이템이 OldItem
            //Reset일 경우 NewItem과 OldItem 둘다 null
            //사후 이벤트이기 때문에 실제 리스트는 이미 바껴있는 상태다.                        

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        AddItems(e.NewStartingIndex, e.NewItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        MoveItem(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        DeleteItems(e.OldItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        ReplaceItems(e.OldItems, e.NewItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        ClearItems();
                    }
                    break;
                default:
                    break;
            }
        }

        void AddItems(int startingIndex, System.Collections.IList values)
        {
            if (values == null)
                return;

            for (int i = 0; i < values.Count; i++)
            {
                MFMFileItemControl item = values[i] as MFMFileItemControl;
                if (item != null)
                {
                    SetItem(item);
                    wpMain.Children.Insert(startingIndex + i, item);
                }
            }  
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

        void DeleteItems(System.Collections.IList values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                MFMFileItemControl b = value as MFMFileItemControl;

                if (b != null)
                {
                    wpMain.Children.Remove(b);
                }
            }
        }

        void MoveItem(int oldIndex, int newIndex)
        {
            MFMFileItemControl b = wpMain.Children[oldIndex] as MFMFileItemControl;

            if (b != null)
            {
                wpMain.Children.RemoveAt(oldIndex);
                wpMain.Children.Insert(newIndex, b);
            }
        }

        void ReplaceItems(System.Collections.IList oldvalues, System.Collections.IList newvalues)
        {
            if ((oldvalues == null) | (newvalues == null))
                return;

            for (int i = 0; i < oldvalues.Count; i++)
            {
                MFMFileItemControl oldItem = oldvalues[i] as MFMFileItemControl;

                if (oldItem != null)
                {
                    int index = wpMain.Children.IndexOf(oldItem);
                    if (index != -1)
                    {
                        MFMFileItemControl newItem = newvalues[i] as MFMFileItemControl;

                        if (newItem != null)
                            wpMain.Children[index] = newItem;
                    }
                }
            }
        }

        void ClearItems()
        {
            wpMain.Children.Clear();
        }

        public void Add(DuplicatedFiles data)
        {
            Items.Add(new MFMFileItemControl(data));
        }

        void SetDisplayPosition()
        {
            this.Width = double.NaN;
            this.Height = double.NaN;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public void Display(List<DuplicatedFiles> files)
        {
            SetDisplayPosition();
            panel.Children.Add(this);

            if (files == null)
                return;

            foreach (DuplicatedFiles file in files)
            {
                Items.Add(new MFMFileItemControl(file));
            }
        }
    }
}
