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
using System.Windows.Shapes;

namespace MusicFileManager
{
    /// <summary>
    /// CleanResultModal.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CleanResultModal : Window
    {
        public string message = null;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                this.tblMessage.Text = message;
            }
        }

        public CleanResultModal()
        {
            InitializeComponent();
        }

        public CleanResultModal(string message) : this()
        {
            this.Message = message;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
