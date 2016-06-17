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
        bool havetoClean = true;
        DuplicateType duplicateType = DuplicateType.AlreadyExtractedArchive;

        public FileItem(DuplicatedFiles file)
        {
            this.orginalFile = file.OriginalFile;
            this.filetoClean = file.DuplicatedFile;
            this.duplicateType = file.SimilarType;
        }

        public string OriginalFile { get { return this.orginalFile; } }
        public string FileToClean { get { return this.filetoClean; } }
        public bool HaveToClean { get { return this.havetoClean; } set { this.havetoClean = value; } }
        public DuplicateType DuplicateType { get { return this.duplicateType; } }
    }

    public delegate void FileToCleanControlOnOKEventHandler(object sender, List<string> files);
    public delegate void FileToCleanControlOnCancelEventHandler(object sender);
    
    public partial class FileToCleanControl : UserControl
    {
        List<FileItem> items = new List<FileItem>();
        Panel panel = null;

        public event FileToCleanControlOnOKEventHandler OnOK = null;
        public event FileToCleanControlOnCancelEventHandler OnCancel = null;

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

            //if (files == null)
            //    return;

            //foreach (DuplicatedFiles file in files)
            //{
            //    items.Add(new FileItem(file));
            //}
            TestItemGenerate();
            dgFileList.ItemsSource = items;
        }

        private void TestItemGenerate()
        {
            for (int i = 0; i < 100; i++)
            {
                items.Add(new FileItem(new DuplicatedFiles("testorigin" + i, "testdup" + i, DuplicateType.DuplicateAudioFileName)));
            }
        }

        public void Close()
        {
            panel.Children.Remove(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (this.OnCancel != null)
            {
                this.OnCancel(this);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (this.OnOK != null)
            {
                List<string> files = new List<string>();
                foreach (FileItem item in items)
                {
                    if (item.HaveToClean)
                        files.Add(item.FileToClean);
                }
                this.OnOK(this, files);
            }
        }

        private void dgFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
