﻿using System;
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
    /// <summary>
    /// MFMCleanFileControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MFMCleanFileControl : UserControl
    {
        public static DependencyProperty ItemSizeProperty;
        public static DependencyProperty ItemsProperty;
        public static DependencyProperty SelectedItemForegroundProperty;
        public static DependencyProperty UnSelectedItemForegroundProperty;
        public static DependencyProperty SelectedItemBackgroundProperty;
        public static DependencyProperty UnSelectedItemBackgroundProperty;
        public static DependencyProperty ItemContentBackgroundProperty;
        public static DependencyProperty ItemContentForegroundProperty;
        public static DependencyProperty TitleProperty;
        public static DependencyProperty EditingProperty;

        Panel panel = null;

        static MFMCleanFileControl()
        {
            ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(MFMCleanFileControl), new PropertyMetadata(10.0));
            ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<MFMCleanFileItemControl>), typeof(MFMCleanFileControl), new PropertyMetadata(new ObservableCollection<MFMCleanFileItemControl>(), new PropertyChangedCallback(OnItemsChanged)));
            SelectedItemForegroundProperty = DependencyProperty.Register("SelectedItemForeground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null));
            UnSelectedItemForegroundProperty = DependencyProperty.Register("UnSelectedItemForeground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null));
            SelectedItemBackgroundProperty = DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null));
            UnSelectedItemBackgroundProperty = DependencyProperty.Register("UnSelectedItemBackground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null));
            ItemContentBackgroundProperty = DependencyProperty.Register("ItemContentBackground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            ItemContentForegroundProperty = DependencyProperty.Register("ItemContentForeground", typeof(Brush), typeof(MFMCleanFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContentChanged)));
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MFMCleanFileControl), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));
            EditingProperty = DependencyProperty.Register("Editing", typeof(bool), typeof(MFMCleanFileControl), new PropertyMetadata(false));
        }

        public MFMCleanFileControl()
        {
            InitializeComponent();
            SetValue(ItemsProperty, new ObservableCollection<MFMCleanFileItemControl>()); 
        }

        public MFMCleanFileControl(Panel panel) : this()
        {
            this.panel = panel;
        }

        public double ItemSize
        {
            get
            {
                return Dispatcher.Invoke(() => (double)this.GetValue(ItemSizeProperty));
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(ItemSizeProperty, value)));
            }
        }

        public ObservableCollection<MFMCleanFileItemControl> Items
        {
            get
            {
                return Dispatcher.Invoke(() => (ObservableCollection<MFMCleanFileItemControl>)this.GetValue(ItemsProperty));
                //return (ObservableCollection<object>)this.GetValue(ItemsProperty);
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(ItemsProperty, value)));
                //this.SetValue(ItemsProperty, value);
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

        public bool Editing
        {
            get
            {
                return (bool)GetValue(EditingProperty);
            }
            set
            {
                SetValue(EditingProperty, value);
            }
        }

        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileControl).OnTitleChanged(e);
        }

        static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileControl).OnItemsChanged();
        }

        static void OnItemContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MFMCleanFileControl).OnItemContentChanged(e);
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
                        AddItems(e.NewItems);
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

        void AddItems(System.Collections.IList values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                MFMCleanFileItemControl item = value as MFMCleanFileItemControl;
                if (item != null)
                {
                    SetItem(item);
                    wpMain.Children.Add(item);
                }
            }
        }

        void SetItem(MFMCleanFileItemControl b)
        {
            Binding selectedForegroundBinding = new Binding("SelectedItemForeground");
            selectedForegroundBinding.Source = this;
            selectedForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMCleanFileItemControl.SelectedForegroundProperty, selectedForegroundBinding);

            Binding selectedbackgroundBinding = new Binding("SelectedItemBackground");
            selectedbackgroundBinding.Source = this;
            selectedbackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMCleanFileItemControl.SelectedBackgroundProperty, selectedbackgroundBinding);

            Binding unselectedForegroundBinding = new Binding("UnSelectedItemForeground");
            unselectedForegroundBinding.Source = this;
            unselectedForegroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMCleanFileItemControl.UnSelectedForegroundProperty, unselectedForegroundBinding);

            Binding unselectedbackgroundBinding = new Binding("UnSelectedItemBackground");
            unselectedbackgroundBinding.Source = this;
            unselectedbackgroundBinding.Mode = BindingMode.OneWay;

            b.SetBinding(MFMCleanFileItemControl.UnSelectedBackgroundProperty, unselectedbackgroundBinding);

            b.Click += b_Click;

            b.MouseMove += b_MouseMove;
            b.MouseLeave += b_MouseLeave;
            b.MouseEnter += b_MouseEnter;
        }

        void b_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!popItem.IsOpen)
                popItem.IsOpen = true;

            MFMCleanFileItemControl bi = sender as MFMCleanFileItemControl;

            if (popItem.PlacementTarget != bi)
                popItem.PlacementTarget = bi;

            if (bi.Data != null)
            {
                string message = "Origianl file : " + bi.Data.OriginalFile + @"\r\nDuplicated file : " + bi.Data.DuplicatedFile + @"\r\n Type : " + bi.Data.SimilarType.ToString();
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
            MFMCleanFileItemControl bi = sender as MFMCleanFileItemControl;

            Point currentPos = e.GetPosition(bi);

            popItem.HorizontalOffset = currentPos.X + 10;
            popItem.VerticalOffset = currentPos.Y + 10;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            if (Editing)
            {
                MFMCleanFileItemControl bi = sender as MFMCleanFileItemControl;
                bi.Selected = !bi.Selected;
            }            
        }

        void DeleteItems(System.Collections.IList values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                MFMCleanFileItemControl b = value as MFMCleanFileItemControl;

                if (b != null)
                {
                    wpMain.Children.Remove(b);
                }
            }
        }

        void MoveItem(int oldIndex, int newIndex)
        {
            MFMCleanFileItemControl b = wpMain.Children[oldIndex] as MFMCleanFileItemControl;

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
                MFMCleanFileItemControl oldItem = oldvalues[i] as MFMCleanFileItemControl;

                if (oldItem != null)
                {
                    int index = wpMain.Children.IndexOf(oldItem);
                    if (index != -1)
                    {
                        MFMCleanFileItemControl newItem = newvalues[i] as MFMCleanFileItemControl;

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
            Items.Add(new MFMCleanFileItemControl(data));
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
                Items.Add(new MFMCleanFileItemControl(file));
            }
        }
    }
}
