using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Ionic.Zip;

namespace MusicFileManager
{
    public class ArchivedFileFinderEndEventArgs : EventArgs
    {
        bool cancel = false;
        List<string> archivedFiles = null;        

        public ArchivedFileFinderEndEventArgs(bool cancel, List<string> archivedFiles)
        {
            this.cancel = cancel;
            this.archivedFiles = archivedFiles;
        }

        public bool Cancel { get { return this.cancel; } }
        public List<string> ArchivedFiles { get { return this.archivedFiles; } }
    }

    public delegate void ArchivedFileFinderStartEventHandler(object sender);
    public delegate void ArchivedfileFinderEndEventHandler(object sender, ArchivedFileFinderEndEventArgs e);

    public class ArchivedFileFinder
    {
        ProgressControl progressControl = null;
        List<string> allFiles = null;
        List<string> archivedFiles = null;
        int current = 0;
        int total = 0;

        public event ArchivedFileFinderStartEventHandler OnStart;
        public event ArchivedfileFinderEndEventHandler OnEnd;

        public ArchivedFileFinder()
        {

        }

        public ArchivedFileFinder(ProgressControl progressControl)
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
            progressControl.SetEvents(DoWork, ProgressChanged, RunWorkerCompleted);
            progressControl.Run();            
        }

        public void Cancel()
        {
            if (progressControl != null)
                progressControl.Cancel();
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage, 
                string.Format("Finding Archived Files ({0}/{1})...", current, total));
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {  
            if (progressControl.Cancelled())
            {
                progressControl.ProgressDisplay(0, "Cancelled!");
            }
            else
            {
                progressControl.ProgressDisplay(100, "Completed!");
            }

            this.OnEnd(this, new ArchivedFileFinderEndEventArgs(progressControl.Cancelled(), archivedFiles));            
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {            
            Process();            
        }
    }
}
