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
using System.ComponentModel;

namespace MusicFileManager
{
    /// <summary>
    /// ProgressControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressControl : UserControl
    {
        BackgroundWorker worker = null;   
     
        DoWorkEventHandler doWork = null; 
        ProgressChangedEventHandler progressChanged = null;
        RunWorkerCompletedEventHandler completed = null;

        bool supportCancellation = true;
        bool reportsProgress = true;
        bool cancel = false;

        public ProgressControl()
        {
            InitializeComponent();            

            //프로그레스바 초기화
            prgProgress.Minimum = 0;
            prgProgress.Maximum = 100;
        }

        private void InitializeWorker()
        {
            if (worker != null)
            {
                worker.Dispose();
            }

            cancel = false;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = this.supportCancellation;
            worker.WorkerReportsProgress = this.reportsProgress;

            worker.DoWork += this.doWork;
            worker.ProgressChanged += this.progressChanged;
            worker.RunWorkerCompleted += this.completed;
        }

        public void SetEvents(DoWorkEventHandler doWork, ProgressChangedEventHandler progressChanged,
            RunWorkerCompletedEventHandler completed)
        {
            this.doWork = doWork;
            this.progressChanged = progressChanged;
            this.completed = completed;
        }

        public void SetReportsProgress(bool reportProgress)
        {
            this.reportsProgress = reportProgress;
        }

        public void SetSupportsCancellation(bool supportsCancellation)
        {
            this.supportCancellation = supportsCancellation;
        }

        public void ProgressDisplay(int percentage, string text)
        {
            prgProgress.Value = percentage;
            txbProgress.Text = text;
        }

        public void InitializeDisplay()
        {
            ProgressDisplay(0, "Ready");
        }

        public void Run()
        {
            InitializeWorker();
            worker.RunWorkerAsync();
        }

        public void Run(object arg)
        {
            InitializeWorker();
            worker.RunWorkerAsync(arg);
        }

        public void Cancel()
        {            
            if (worker.WorkerSupportsCancellation)
            {
                cancel = true;
                worker.CancelAsync();
            }
        }

        public void FireProgress(int percentage)
        {            
            worker.ReportProgress(percentage);
        }

        public bool Cancelled()
        {
            return cancel;
        }

        private void ProgressController_Initialized(object sender, EventArgs e)
        {
            InitializeDisplay();
        }
    }
}
