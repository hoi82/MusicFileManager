using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace MusicFileManager.Worker
{
    public abstract class DisplayableWorker : IDisposable
    {
        protected BackgroundWorker bw = null;

        protected ProgressControl progressControl = null;
        protected string progressMessage = null;
        protected string progressMessageOnStep = null;

        protected int total = 0;
        protected int current = 0;

        protected bool working = false;        

        protected DisplayableWorker()
        {
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            total = 0;
            current = 0;            
        }

        public DisplayableWorker(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
        }        

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {                   
            if (e.Cancelled)
            {
                if ((progressControl != null) && working)
                    progressControl.ProgressDisplay(0, "Cancelled");
            }
            else
            {
                if ((progressControl != null) && working)
                    progressControl.ProgressDisplay(100, "Complete");
            }            

            OnEndProcedure();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {               
            if ((progressControl != null) && working)            
                progressControl.ProgressDisplay(e.ProgressPercentage, progressMessage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Process(e);
        }

        protected void ResetCount(int total)
        {
            this.total = total;
            current = 0;
        }

        protected void IncCount()
        {
            current++;
        }

        protected int CalcPercentage()
        {
            int perc = (int)((float)current / (float)total * 100);

            return perc;
        }

        public void CancelAsync()
        {
            bw.CancelAsync();
        }

        public void Dispose()
        {
            if (bw != null)
            {
                bw.CancelAsync();
                bw.Dispose();
            }                        
        }

        protected abstract void Process(DoWorkEventArgs e = null);

        protected abstract void OnEndProcedure();

        protected abstract void OnStartProcedure();
    }
}
