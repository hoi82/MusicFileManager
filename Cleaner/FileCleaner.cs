using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public class FileCleanerEndEventArgs : EventArgs
    {
        List<string> deletedFiles = null;
        List<string> unDeletedFiles = null;

        public FileCleanerEndEventArgs(List<string> deletedFiles, List<string> unDeletedFiles)
        {
            this.deletedFiles = deletedFiles;
            this.unDeletedFiles = unDeletedFiles;
        }

        public List<string> DeletedFiles { get { return this.deletedFiles; } }
        public List<string> UnDeletedFiles { get { return this.unDeletedFiles; } }
    }

    public delegate void FileCleanerEndEventHandler(object sender, FileCleanerEndEventArgs e);
    public class FileCleaner
    {
        List<string> files = null;        
        List<string> deletedFiles = new List<string>();
        List<string> undeletedFiles = new List<string>();
        BackgroundWorker bw = null;
        ProgressControl progressControl = null;

        public event FileCleanerEndEventHandler OnEnd = null;

        int current = 0;
        int total = 0;
        string progressMessage = null;

        public FileCleaner()
        {
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;

            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        public FileCleaner(List<string> files) : this()
        {
            this.files = files;            
        }

        public FileCleaner(List<string> files, ProgressControl progressControl)
            : this(files)
        {
            this.progressControl = progressControl;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.OnEnd != null)
                this.OnEnd(this, new FileCleanerEndEventArgs(deletedFiles, undeletedFiles));

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
            CleanFiles(e);
        }

        bool IsFileLocked(FileInfo fi)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = fi.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public void CancelCleanAsync()
        {
            bw.CancelAsync();
        }

        private void ResetCount(int total)
        {
            this.total = total;
            current = 0;
        }

        private void IncCount()
        {
            current++;
        }

        private int CalcPercentage()
        {
            int perc = (int)((float)current / (float)total * 100);

            return perc;
        }

        void CleanFiles(DoWorkEventArgs e = null)
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
                    if (IsFileLocked(fi))
                    {
                        fi.Delete();
                        deletedFiles.Add(files[i]);
                    }
                    else
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

                IncCount();
                progressMessage = string.Format("Deleting Files...{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }
        }

        public void CleanFiles(bool aSync, List<string> files = null)
        {
            if (files != null)
                this.files = files;

            if (aSync)
            {
                bw.RunWorkerAsync();
            }
            else
            {
                CleanFiles();

                if (this.OnEnd != null)
                {
                    this.OnEnd(this, new FileCleanerEndEventArgs(deletedFiles, undeletedFiles));
                }
            }                
        }
    }
}
