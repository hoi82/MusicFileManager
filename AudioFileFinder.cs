using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TagLib;

namespace MusicFileManager
{
    public class AudioFileFinderEndEventArgs : EventArgs
    {
        bool cancel = false;
        List<string> audioFiles = null;

        public AudioFileFinderEndEventArgs(bool cancel, List<string> audioFiles)
        {
            this.cancel = cancel;
            this.audioFiles = audioFiles;
        }

        public bool Cancel { get { return cancel; } }
        public List<string> AudioFiles { get { return audioFiles; } }
    }

    public delegate void AudioFileFinderStartEventHandler(object sender);
    public delegate void AudioFileFinderEndEventHandler(object sender, AudioFileFinderEndEventArgs e);

    public class AudioFileFinder
    {
        ProgressControl progressControl = null;
        List<string> allFiles = null;
        List<string> audioFiles = null;
        int current = 0;
        int total = 0;

        public event AudioFileFinderStartEventHandler OnStart;
        public event AudioFileFinderEndEventHandler OnEnd;

        public AudioFileFinder()
        {

        }

        public AudioFileFinder(ProgressControl progressControl)
        {
            this.progressControl = progressControl;
        }

        private void Process()
        {
            audioFiles = new List<string>();

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if (progressControl != null)
                {
                    if (progressControl.Cancelled())
                    {
                        break;
                    }
                }
                try
                {
                    TagLib.File f = TagLib.File.Create(allFiles[i]);
                    audioFiles.Add(allFiles[i]);
                }
                catch (TagLib.UnsupportedFormatException e)
                {                    
                    //throw;
                }                              

                current = i + 1;

                int perc = (int)((float)current / (float)total * 100);

                if (progressControl != null)
                    progressControl.FireProgress(perc);
            }
        }

        public void Run(List<string> files)
        {
            allFiles = files;
            total = allFiles.Count();

            if (this.OnStart != null)
                this.OnStart(this);

            progressControl.InitializeDisplay();
            progressControl.SetEvents(DoWork, ProgressChanged, RunWorkerCompleted);
            progressControl.Run();
        }

        public void Cancel()
        {
            if (progressControl != null)
                progressControl.Cancel();
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage,
                string.Format("Finding Audio Files ({0}/{1})...", current, total));
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (progressControl.Cancelled())
            {
                progressControl.ProgressDisplay(0, "Cancelled!");
            }
            else
            {
                progressControl.ProgressDisplay(100, "Completed!");
            }

            this.OnEnd(this, new AudioFileFinderEndEventArgs(progressControl.Cancelled(), audioFiles));
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            Process();
        }
    }
}
