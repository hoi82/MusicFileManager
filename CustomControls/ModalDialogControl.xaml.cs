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
using System.Windows.Threading;

namespace MusicFileManager
{
    public enum DialogButton { OK, OKCancel, YesNo, YesNoCancel, AbortRetryIgnore }
    public enum DialogResult { OK, Cancel, Yes, No, Abort, Retry, Ignore }
    /// <summary>
    /// ModalDialogControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModalDialogControl : UserControl
    {
        Panel panel = null;
        DispatcherFrame frame = null;
        DialogButton buttons = DialogButton.OK;
        DialogResult result = DialogResult.OK;

        public static DependencyProperty CaptionProperty;
        public static DependencyProperty MessageProperty;        

        public string Caption
        {
            get { return Dispatcher.Invoke(() => (string)this.GetValue(CaptionProperty)); }
            set { Dispatcher.BeginInvoke((Action)(() => this.SetValue(CaptionProperty, value))); }
        }

        public string Message
        {
            get { return Dispatcher.Invoke(()=>(string)this.GetValue(MessageProperty)); }
            set { Dispatcher.BeginInvoke((Action)(() => this.SetValue(MessageProperty, value))); }
        }

        static ModalDialogControl()
        {
            CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ModalDialogControl), new PropertyMetadata(null));
            MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ModalDialogControl), new PropertyMetadata(null));
        }

        public DialogButton Buttons 
        {
            get { return this.buttons; }
            set
            {
                this.buttons = value;
                SetButtonDisplay();
            } 
        }

        ModalDialogControl()
        {
            InitializeComponent();            
        }        

        public ModalDialogControl(Panel panel, DialogButton buttons = DialogButton.OK)
            : this()
        {
            this.panel = panel;
            this.Buttons = buttons;            
        }

        public ModalDialogControl(Panel panel, DialogButton buttons, string caption = null, string message = null)
            : this(panel, buttons)
        {
            this.Caption = caption;
            this.Message = message;
        }

        public DialogResult ShowDialog()
        {
            SetDisplayPosition();            

            panel.Children.Add(this);

            frame = new DispatcherFrame(true);            
            Dispatcher.PushFrame(frame);
            return result;
        }

        public DialogResult ShowDialog(string message)
        {
            this.Message = message;
            return ShowDialog();
        }

        public DialogResult ShowDialog(string caption, string message)
        {
            this.Caption = caption;
            this.Message = message;
            return ShowDialog();
        }

        private void CloseDisplay()
        {
            frame.Continue = false;
            panel.Children.Remove(this);
        }

        private void SetDisplayPosition()
        {
            this.Width = double.NaN;
            this.Height = double.NaN;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void SetButtonDisplay()
        {
            switch (buttons)
            {
                case DialogButton.OK:
                    {
                        btnLeft.Visibility = Visibility.Hidden;
                        btnCenter.Visibility = Visibility.Hidden;                        
                        btnRight.Visibility = Visibility.Visible;
                        btnRight.Content = "OK";
                    }
                    break;
                case DialogButton.OKCancel:
                    {
                        btnLeft.Visibility = Visibility.Hidden;
                        btnCenter.Visibility = Visibility.Visible;
                        btnCenter.Content = "OK";
                        btnRight.Visibility = Visibility.Visible;
                        btnRight.Content = "Cancel";
                    }
                    break;
                case DialogButton.YesNo:
                    {
                        btnLeft.Visibility = Visibility.Hidden;
                        btnCenter.Visibility = Visibility.Visible;
                        btnCenter.Content = "Yes";
                        btnRight.Visibility = Visibility.Visible;
                        btnRight.Content = "No";
                    }
                    break;
                case DialogButton.YesNoCancel:
                    {
                        btnLeft.Visibility = Visibility.Visible;
                        btnLeft.Content = "Yes";
                        btnCenter.Visibility = Visibility.Visible;
                        btnCenter.Content = "No";
                        btnRight.Visibility = Visibility.Visible;
                        btnRight.Content = "Cancel";
                    }
                    break;
                case DialogButton.AbortRetryIgnore:
                    {
                        btnLeft.Visibility = Visibility.Visible;
                        btnLeft.Content = "Abort";
                        btnCenter.Visibility = Visibility.Visible;
                        btnCenter.Content = "Retry";
                        btnRight.Visibility = Visibility.Visible;
                        btnRight.Content = "Ignore";
                    }
                    break;
                default:
                    break;
            }            
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            switch (buttons)
            {
                case DialogButton.OK:
                    break;
                case DialogButton.OKCancel:
                    break;
                case DialogButton.YesNo:
                    break;
                case DialogButton.YesNoCancel:
                    result = DialogResult.Yes;
                    break;
                case DialogButton.AbortRetryIgnore:
                    result = DialogResult.Abort;
                    break;
                default:
                    break;
            }
            CloseDisplay();
        }

        private void btnCenter_Click(object sender, RoutedEventArgs e)
        {
            switch (buttons)
            {
                case DialogButton.OK:
                    break;
                case DialogButton.OKCancel:
                    result = DialogResult.OK;
                    break;
                case DialogButton.YesNo:
                    result = DialogResult.Yes;
                    break;
                case DialogButton.YesNoCancel:
                    result = DialogResult.No;
                    break;
                case DialogButton.AbortRetryIgnore:
                    result = DialogResult.Retry;
                    break;
                default:
                    break;
            }
            CloseDisplay();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            switch (buttons)
            {
                case DialogButton.OK:
                    result = DialogResult.OK;
                    break;
                case DialogButton.OKCancel:
                    result = DialogResult.Cancel;
                    break;
                case DialogButton.YesNo:
                    result = DialogResult.No;
                    break;
                case DialogButton.YesNoCancel:
                    result = DialogResult.Cancel;
                    break;
                case DialogButton.AbortRetryIgnore:
                    result = DialogResult.Ignore;
                    break;
                default:
                    break;
            }
            CloseDisplay();
        }
    }
}
