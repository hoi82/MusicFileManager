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

namespace MusicFileManager
{
    /// <summary>
    /// FileToCleanControl.xaml에 대한 상호 작용 논리
    /// </summary>

    public class FileItem
    {
        string orginalFile = null;
        string filetoClean = null;
        bool haveToClean = false;

        public FileItem(DuplicatedFiles file)
        {
            this.orginalFile = file.OriginalFile;
            this.filetoClean = file.DuplicatedFile;
        }

        public string OriginalFile { get; set; }
        public string FileToClean { get; set; }
        bool HaveToClean { get; set; }
    }
    
    public partial class FileToCleanControl : UserControl
    {
        List<FileItem> items = new List<FileItem>();
        Panel panel = null;

        public FileToCleanControl(Panel panel)
        {
            InitializeComponent();            

            this.panel = panel;
        }

        private void SetDisplayPosition()
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
                items.Add(new FileItem(file));
                dgFileList.Items.Add(new FileItem(file));
            }
            //dgFileList.ItemsSource = items;
        }

        public void Close()
        {
            panel.Children.Remove(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
