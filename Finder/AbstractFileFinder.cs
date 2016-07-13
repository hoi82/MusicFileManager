using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MusicFileManager.Worker;
using MusicFileManager.CustomControls;

namespace MusicFileManager
{        
    public abstract class AbstractFileFinder : DisplayableWorker, IFileFinder
    {
        //Event
        public event FileFinderStartEventHandler OnStartAsync = null;

        public event FildFinderProgressEventHandler OnProgressAsync = null;

        public event FileFinderCompleteEventHandler OnCompleteAsync = null;

        public event FileFinderCancelEventHandler OnCancelAsync = null;               

        //Properties
        protected IFileChecker fileChecker = null;
        protected List<string> mathcedFiles = null;
        protected List<string> allFiles = null;
        protected MFMFileControl fileControl = null;

        //Serialization
        protected IFileFinder serializedFinder = null;  

        protected AbstractFileFinder() : base()
        {
            
        }

        protected AbstractFileFinder(IFileChecker fileChecker) : this()
        {
            this.fileChecker = fileChecker;
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
            allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            OnStartProcedure();            
            StartAsync();
        }

        public void GetMatchedFilesAsync(List<string> files)
        {
            working = true;
            allFiles = files;
            OnStartProcedure();
            StartAsync();
        }        

        public void SetSerializedFinder(IFileFinder finder, bool usePreMathcedFiles = false)
        {
            serializedFinder = finder;
            this.OnCompleteAsync += new FileFinderCompleteEventHandler(
                delegate 
                {
                    if (usePreMathcedFiles)
                        finder.GetMatchedFilesAsync(allFiles);
                    else
                        finder.GetMatchedFilesAsync(mathcedFiles);
                }
                );
        }        

        protected override void Process(DoWorkEventArgs e = null)
        {
            if (allFiles == null)
                return;

            ResetCount(allFiles.Count());            
            mathcedFiles = new List<string>();

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string sFile = allFiles[i];

                if ((e != null) && Canceled())
                {
                    e.Cancel = true;
                    break;
                }

                if (fileChecker.IsVaildFile(ref sFile, true))
                {
                    mathcedFiles.Add(sFile);
                }

                //IncCount();        
                current = i;
                OnProcedure();
            }
        }

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this, new FileFinderStartEventAtgs(allFiles));
        }       

        protected override void OnProcedure() 
        {
            if (this.OnProgressAsync != null)
            {
                this.OnProgressAsync(this, new FileFinderProgressEventArgs(current, total));
            }
        }

        protected override void OnCancelProcedure() 
        {
            if (this.OnCancelAsync != null)
            {
                this.OnCancelAsync(this, new EventArgs());
            }
        }

        protected override void OnCompleteProcedure()
        {
            working = false;

            if (this.OnCompleteAsync != null)
                this.OnCompleteAsync(this, new FileFinderEndEventArgs(mathcedFiles));
        }


        public void Cancel()
        {
            base.CancelAsync();
            if (serializedFinder != null)
            {
                serializedFinder.Cancel();
            }
        }
    }
}
