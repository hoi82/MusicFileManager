using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public class FileCleaner
    {
        List<string> files = null;        
        List<string> deletedFiles = new List<string>();
        List<string> undeletedFiles = new List<string>();
        BackgroundWorker bw = null;

        public FileCleaner(List<string> files)
        {
            this.files = files;

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;

            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (this.OnEnd != null)
            //    this.OnEnd(this, filetoClean);

            //if (e.Cancelled)
            //{
            //    progressControl.ProgressDisplay(0, "Cancelled");
            //}
            //else
            //{
            //    progressControl.ProgressDisplay(100, "Complete");
            //}
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressControl.ProgressDisplay(e.ProgressPercentage, progressMessage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            CleanFiles(e);
        }

        public void CleanFilesAsync()
        {
            bw.RunWorkerAsync();
        }

        public void CleanFilesAsync(List<string> files)
        {
            this.files = files;
            CancelCleanAsync();
        }

        public void CancelCleanAsync()
        {
            bw.CancelAsync();
        }

        public void CleanFiles(DoWorkEventArgs e = null)
        {
            if (files == null) return;

            for (int i = 0; i < files.Count; i++)
            {
                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (!File.Exists(files[i]))
                {
                    undeletedFiles.Add(files[i]);
                    continue;
                }

                try
                {
                    FileInfo fi = new FileInfo(files[i]);
                    fi.Delete();
                    if (File.Exists(files[i]))
                    {
                        undeletedFiles.Add(files[i]);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                                           
                }
                catch (IOException)
                {
                    
                }
            }
        }

        public void CleanFiles(List<string> files)
        {
            this.files = files;
            CleanFiles();
        }
    }
}
