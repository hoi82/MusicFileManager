using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public class FileFinderStartEventAtgs : EventArgs
    {
        List<string> preMatchedFiles = null;

        public FileFinderStartEventAtgs(List<string> preMatchedFiles)
        {
            this.preMatchedFiles = preMatchedFiles;
        }

        public List<string> PreMatchedFiles { get { return this.preMatchedFiles; } }
    }

    public class FileFinderEndEventArgs : EventArgs
    {
        List<string> matchedFiles = null;

        public FileFinderEndEventArgs(List<string> matchedFiles)
        {
            this.matchedFiles = matchedFiles;
        }

        public List<string> MatchedFiles { get { return this.matchedFiles; } }
    }

    public class FileFinderProgressEventArgs : EventArgs
    {
        int current = 0;
        int total = 0;

        public FileFinderProgressEventArgs(int current, int total)
        {
            this.current = current;
            this.total = total;
        }

        public int Current { get { return this.current; } }
        public int Total { get { return this.total; } }
    }

    public delegate void FileFinderStartEventHandler(object sender, FileFinderStartEventAtgs e);
    public delegate void FildFinderProgressEventHandler(object sender, FileFinderProgressEventArgs e);
    public delegate void FileFinderCompleteEventHandler(object sender, FileFinderEndEventArgs e);
    public delegate void FileFinderCancelEventHandler(object sender, EventArgs e);

    public interface IFileFinder
    {
        event FileFinderStartEventHandler OnStartAsync;
        event FildFinderProgressEventHandler OnProgressAsync;
        event FileFinderCompleteEventHandler OnCompleteAsync;
        event FileFinderCancelEventHandler OnCancelAsync;
       
        List<string> GetMatchedFiles(string directory);
        List<string> GetMatchedFiles(List<string> files);
        void GetMatchedFilesAsync(string directory);
        void GetMatchedFilesAsync(List<string> files);
        void SetSerializedFinder(IFileFinder finder, bool usePreMathcedFiles = false);
        void Cancel();
        
    }
}
