using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MusicFileManager.Worker;

namespace MusicFileManager.Cleaner
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

    public delegate void FileCleanerStartEventHandler(object sender);
    public delegate void FileCleanerEndEventHandler(object sender, FileCleanerEndEventArgs e);
    public class FileCleaner : DisplayableWorker, IFileCleaner
    {
        List<string> files = null;        
        List<string> deletedFiles = new List<string>();
        List<string> undeletedFiles = new List<string>();

        public event FileCleanerStartEventHandler OnStartAsync = null;

        public event FileCleanerEndEventHandler OnEndAsync = null;

        public FileCleaner() : base()
        {

        }

        public FileCleaner(ProgressControl progressControl)
            : base(progressControl)
        {
            
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

        public void CleanFiles(bool aSync, List<string> files)
        {
            this.files = files;
            if (aSync)
            {
                bw.RunWorkerAsync();
            }
            else
            {
                Process();

                if (this.OnEndAsync != null)
                {
                    this.OnEndAsync(this, new FileCleanerEndEventArgs(deletedFiles, undeletedFiles));
                }
            }                
        }

        protected override void Process(DoWorkEventArgs e = null)
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
                        undeletedFiles.Add(files[i]);                        
                    }
                    else
                    {
                        //fi.Delete();
                        deletedFiles.Add(files[i]);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    undeletedFiles.Add(files[i]);
                }
                catch (IOException)
                {
                    undeletedFiles.Add(files[i]);
                }

                IncCount();
                progressMessage = string.Format("Deleting Files...{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }
        }

        protected override void OnEndProcedure()
        {
            if (this.OnEndAsync != null)
                this.OnEndAsync(this, new FileCleanerEndEventArgs(deletedFiles, undeletedFiles));
        }

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this);
        }        
    }
}
