using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MusicFileManager
{
    public class FileFinderEndEventArgs<T> : EventArgs
    {
        List<T> matchedFiles = null;

        public FileFinderEndEventArgs(List<T> matchedFiles)
        {
            this.matchedFiles = matchedFiles;
        }

        public List<T> MatchedFiles { get { return this.matchedFiles; } }
    }

    public delegate void FileFinderStartEventHandler(object sender, EventArgs e);
    public delegate void FileFinderEndEventHandler<T>(object sender, FileFinderEndEventArgs<T> e);
    public abstract class AbstractFileFinder<InType, OutType> : IFileFinder<InType, OutType>, IDisposable
    {
        public event FileFinderStartEventHandler OnStart = null;
        public event FileFinderEndEventHandler<OutType> OnEnd = null;

        protected BackgroundWorker bw = null;

        protected ProgressControl progressControl = null;
        protected string progressMessage = null;

        protected int total = 0;
        protected int current = 0;

        protected IFileChecker fileChecker = null;
        protected List<OutType> mathcedFiles = new List<OutType>();
        protected List<InType> allFiles = null;

        protected AbstractFileFinder()
        {

        }

        protected AbstractFileFinder(IFileChecker fileChecker) : this()
        {
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            total = 0;
            current = 0;

            this.fileChecker = fileChecker;
        }        

        protected AbstractFileFinder(IFileChecker fileChecker, ProgressControl progressControl) 
            : this(fileChecker)
        {
            this.progressControl = progressControl;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.OnEnd != null)
                this.OnEnd(this, new FileFinderEndEventArgs<OutType>(mathcedFiles));

            if (e.Cancelled)
            {
                if (progressControl != null)
                    progressControl.ProgressDisplay(0, "Cancelled");
            }
            else
            {
                if (progressControl != null)
                    progressControl.ProgressDisplay(100, "Complete");
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (progressControl != null)
                progressControl.ProgressDisplay(e.ProgressPercentage, progressMessage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Process(e);
        }

        protected abstract void Process(DoWorkEventArgs e = null);        

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

        public List<OutType> GetMatchedFiles(string directory)
        {
            Process();
            return mathcedFiles;
        }

        public List<OutType> GetMatchedFiles(List<InType> files)
        {
            allFiles = files;
            Process();
            return mathcedFiles;
        }

        public void GetMatchedFilesAsync(string directory)
        {
            if (this.OnStart != null)
                this.OnStart(this, new EventArgs());
            bw.RunWorkerAsync();
        }

        public void GetMatchedFilesAsync(List<InType> files)
        {
            allFiles = files;
            if (this.OnStart != null)
                this.OnStart(this, new EventArgs());
            bw.RunWorkerAsync();
        }
    }
}
