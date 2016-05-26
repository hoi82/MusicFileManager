using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Ionic.Zip;

namespace MusicFileManager
{
    public class ArchivedFileExtractorEndEventArgs : EventArgs
    {
        bool cancel = false;
        List<string> archivedFiles = null;        

        public ArchivedFileExtractorEndEventArgs(bool cancel, List<string> archivedFiles)
        {
            this.cancel = cancel;
            this.archivedFiles = archivedFiles;
        }

        public bool Cancel { get { return this.cancel; } }
        public List<string> ArchivedFiles { get { return this.archivedFiles; } }
    }

    public delegate void ArchivedFileExtractorStartEventHandler(object sender);
    public delegate void ArchivedfileExtractorEndEventHandler(object sender, ArchivedFileExtractorEndEventArgs e);

    public class ArchivedFileExtractor
    {
        ProgressControl progressControl = null;
        List<string> allFiles = null;
        List<string> archivedFiles = null;
        int current = 0;
        int total = 0;

        public event ArchivedFileExtractorStartEventHandler OnStart;
        public event ArchivedfileExtractorEndEventHandler OnEnd;

        public ArchivedFileExtractor()
        {

        }

        public ArchivedFileExtractor(ProgressControl progressControl)
        {
            this.progressControl = progressControl;
        }

        private void Process()
        {
            archivedFiles = new List<string>();

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if (progressControl != null)
                {
                    if (progressControl.Cancelled())
                    {                        
                        break;
                    }
                }

                if (ZipFile.IsZipFile(allFiles[i]))
                {
                    archivedFiles.Add(allFiles[i]);
                }
                current = i + 1;

                int perc = (int)((float)current / (float)total * 100);

                if (progressControl != null)
                    progressControl.FireProgress(perc);
            }
        }

        public void Run(List<string> files)
        {
            allFiles = files;
            total = allFiles.Count();

            if (this.OnStart != null)
                this.OnStart(this);

            progressControl.InitializeDisplay();
            progressControl.SetEvents(worker_DoWork, worker_ProgressChanged, worker_RunWorkerCompleted);
            progressControl.Run();            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage, 
                string.Format("Get Archived Files ({0}/{1})...", current, total));
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {  
            if (progressControl.Cancelled())
            {
                progressControl.ProgressDisplay(0, "Cancelled!");
            }
            else
            {
                progressControl.ProgressDisplay(100, "Completed!");
            }

            this.OnEnd(this, new ArchivedFileExtractorEndEventArgs(e.Cancelled, archivedFiles));            
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            Process();            
        }
    }
}
