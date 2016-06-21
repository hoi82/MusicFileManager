using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public delegate void FileCleanerEndEventHandler(object sender, List<string> deletedFiles, List<string> unDeletedFiles);
    public class FileCleaner
    {
        List<string> files = null;        
        List<string> deletedFiles = new List<string>();
        List<string> undeletedFiles = new List<string>();
        BackgroundWorker bw = null;
        ProgressControl progressControl = null;

        public event FileCleanerEndEventHandler OnEnd = null;

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
                this.OnEnd(this, deletedFiles, undeletedFiles);

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
                progressControl.ProgressDisplay(e.ProgressPercentage, "Deleting Files...{0}/{1}");
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

        private void CleanFiles(DoWorkEventArgs e = null)
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
                    this.OnEnd(this, deletedFiles, undeletedFiles);
                }
            }                
        }
    }
}
