using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public class FileFinderEndEventArgs : EventArgs
    {
        List<string> matchedFiles = null;

        public FileFinderEndEventArgs(List<string> matchedFiles)
        {
            this.matchedFiles = matchedFiles;
        }

        public List<string> MatchedFiles { get { return this.matchedFiles; } }
    }

    public delegate void FileFinderStartEventHandler(object sender, EventArgs e);
    public delegate void FileFinderEndEventHandler(object sender, FileFinderEndEventArgs e);
    public abstract class AbstractFileFinder
        : DisplayableWorker.DisplayableWorker, IFileFinder
    {
        //Event
        public event FileFinderStartEventHandler OnStartAsync = null;

        public event FileFinderEndEventHandler OnEndAsync = null;        

        //Properties
        protected IFileChecker fileChecker = null;
        protected List<string> mathcedFiles = new List<string>();
        protected List<string> allFiles = null;

        //Serialization
        protected List<IFileFinder> serializedFinder = null;        

        protected AbstractFileFinder() : base()
        {
            serializedFinder = new List<IFileFinder>();
        }

        protected AbstractFileFinder(IFileChecker fileChecker) : this()
        {
            this.fileChecker = fileChecker;
        }        

        protected AbstractFileFinder(IFileChecker fileChecker, ProgressControl progressControl) 
            : this(fileChecker)
        {
            this.progressControl = progressControl;
        }             

        public List<string> GetMatchedFiles(string directory)
        {
            working = false;
            allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            Process();
            return mathcedFiles;
        }

        public List<string> GetMatchedFiles(List<string> files)
        {
            working = false;
            allFiles = files;
            Process();
            return mathcedFiles;
        }

        public void GetMatchedFilesAsync(string directory)
        {
            working = true;            
            OnStartProcedure();
            allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            bw.RunWorkerAsync();
        }

        public void GetMatchedFilesAsync(List<string> files)
        {
            working = true;
            allFiles = files;
            OnStartProcedure();
            bw.RunWorkerAsync();
        }

        protected override void OnEndProcedure()
        {
            if (this.OnEndAsync != null)
                this.OnEndAsync(this, new FileFinderEndEventArgs(mathcedFiles));

            for (int i = 0; i < serializedFinder.Count(); i++)
            {
                serializedFinder[i].GetMatchedFilesAsync(mathcedFiles);
                serializedFinder[i].WaitAsync();
            }
        }

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this, new EventArgs());
        }


        public void AddSerializedFinder(IFileFinder finder)
        {
            this.serializedFinder.Add(finder);
        }


        public void WaitAsync()
        {
            this.WaitDone();
        }        
    }
}
