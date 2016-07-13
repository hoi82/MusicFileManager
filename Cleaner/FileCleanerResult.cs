using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Cleaner
{
    public class FileCleanerResult
    {
        List<string> deletedFiles = null;
        List<string> unDeletedFiles = null;
        List<string> failedFiles = null;

        public FileCleanerResult(List<string> deletedFiles, List<string> unDeletedFiles, List<string> failedFiles)
        {
            this.deletedFiles = deletedFiles;
            this.unDeletedFiles = unDeletedFiles;
            this.failedFiles = failedFiles;
        }

        public List<string> DeletedFiles { get { return this.deletedFiles; } }
        public List<string> UnDeletedFiles { get { return this.unDeletedFiles; } }
        public List<string> FailedFiles { get { return this.failedFiles; } }
    }
}
