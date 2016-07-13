using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MusicFileManager.Worker;
using MusicFileManager.CustomControls;

namespace MusicFileManager.Cleaner
{
    /// <summary>
    /// 
    /// </summary>
    public class FileCleaner : DisplayableWorker, IFileCleaner
    {        
        public event FileCleanerStartEventHandler OnStartAsync = null;

        public event FileCleanerCompleteEventHandler OnCompleteAsync = null;

        public event FileCleanerProgressEventHander OnProgressAsync = null;

        public event FileCleanerCancelEventHandler OnCancelAsync = null;
        
        List<string> deletedFiles = new List<string>();
        List<string> failedFiles = new List<string>();
        List<string> undeletedFiles = new List<string>();

        MFMFileControl fileControl = null;

        public FileCleaner() : base()
        {

        }

        public FileCleaner(MFMFileControl fileControl) : this()
        {
            this.fileControl = fileControl;
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
            CancelAsync();
        }                

        protected override void Process(DoWorkEventArgs e = null)
        {
            if (fileControl == null) return;

            List<MFMFileItemControl> items = fileControl.Items();

            total = items.Count;

            for (int i = 0; i < items.Count; i++)
            {
                if ((e != null) && Canceled())
                {
                    e.Cancel = true;
                    break;
                }

                DuplicatedFiles d = items[i].Data as DuplicatedFiles;

                if (items[i].Selected)
                {
                    if (d == null)
                    {
                        failedFiles.Add("unknown");
                    }
                    else
                    {
                        if (File.Exists(d.DuplicatedFile))
                        {
                            FileInfo fi = new FileInfo(d.DuplicatedFile);

                            if (IsFileLocked(fi))
                            {
                                failedFiles.Add(d.DuplicatedFile);
                            }
                            else
                            {
                                fi.Delete();
                                deletedFiles.Add(d.DuplicatedFile);
                            }
                        }
                        else
                        {
                            failedFiles.Add(d.DuplicatedFile);
                        }
                    }
                }
                else
                {
                    if (d == null)
                        failedFiles.Add("unknown");
                    else
                        undeletedFiles.Add(d.DuplicatedFile);
                }

                current = i;

                OnProcedure();
            }
        }

        protected override void OnCompleteProcedure()
        {
            if (this.OnCompleteAsync != null)
                this.OnCompleteAsync(this, new FileCleanerCompleteEventArgs(deletedFiles, undeletedFiles));
        }

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this, new EventArgs());
        }

        protected override void OnProcedure()
        {
            if (this.OnProgressAsync != null)
                this.OnProgressAsync(this, new FileCleanerProgressEventArgs(current, total));
        }

        protected override void OnCancelProcedure()
        {
            if (this.OnCancelAsync != null)
                this.OnCancelAsync(this, new EventArgs());
        }


        public FileCleanerResult CleanFiles(List<string> files)
        {
            throw new NotImplementedException();
        }

        public void CleanFilesAsync(List<string> files)
        {
            throw new NotImplementedException();
        }

        public FileCleanerResult CleanFiles()
        {
            Process();
            return new FileCleanerResult(deletedFiles, undeletedFiles, failedFiles);
        }

        public void CleanFilesAsync()
        {
            OnStartProcedure();
            StartAsync();
        }
    }
}
