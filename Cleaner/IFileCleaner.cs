using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicFileManager.Worker;

namespace MusicFileManager.Cleaner
{
    public class FileCleanerCompleteEventArgs : EventArgs
    {
        List<string> deletedFiles = null;
        List<string> unDeletedFiles = null;

        public FileCleanerCompleteEventArgs(List<string> deletedFiles, List<string> unDeletedFiles)
        {
            this.deletedFiles = deletedFiles;
            this.unDeletedFiles = unDeletedFiles;
        }

        public List<string> DeletedFiles { get { return this.deletedFiles; } }
        public List<string> UnDeletedFiles { get { return this.unDeletedFiles; } }
    }

    public class FileCleanerProgressEventArgs : EventArgs
    {
        int current = 0;
        int total = 0;

        public FileCleanerProgressEventArgs(int current, int total)
        {
            this.current = current;
            this.total = total;
        }

        public int Current { get { return this.current; } }
        public int Total { get { return this.total; } }
    }

    public delegate void FileCleanerStartEventHandler(object sender, EventArgs e);
    public delegate void FileCleanerProgressEventHander(object sender, FileCleanerProgressEventArgs e);
    public delegate void FileCleanerCancelEventHandler(object sender, EventArgs e);
    public delegate void FileCleanerCompleteEventHandler(object sender, FileCleanerCompleteEventArgs e);
    public interface IFileCleaner
    {
        event FileCleanerStartEventHandler OnStartAsync;
        event FileCleanerProgressEventHander OnProgressAsync;
        event FileCleanerCancelEventHandler OnCancelAsync;
        event FileCleanerCompleteEventHandler OnCompleteAsync;
        FileCleanerResult CleanFiles(List<string> files);
        void CleanFilesAsync(List<string> files);
        FileCleanerResult CleanFiles();
        void CleanFilesAsync();
        void CancelCleanAsync();
    }
}
