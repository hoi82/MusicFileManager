using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Ionic.Zip;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public class AudioFileExtractorEventArgs : EventArgs
    {
        bool cancel = false;
        List<string> audioFileNames = null;

        public AudioFileExtractorEventArgs(bool cancel, List<string> audioFileNames)
        {
            this.cancel = cancel;
            this.audioFileNames = audioFileNames;
        }

        public bool Cancel { get { return this.cancel; } }
        public List<string> AudioFileNames { get { return this.audioFileNames; } }
    }

    public delegate void AudioFileExtractorStartEventHandler(object sender);
    public delegate void AudioFileExtractorEndEventHandler(object sender, AudioFileExtractorEventArgs e);

    public class AudioFileExtractor
    {
        ProgressControl progressControl = null;
        List<string> archivedFiles = null;
        List<string> audioFiles = null;
        int current = 0;
        int total = 0;
        const string EXTRACT_DIR = "Extracts";

        public event AudioFileExtractorStartEventHandler OnStart;
        public event AudioFileExtractorEndEventHandler OnEnd;

        public AudioFileExtractor()
        {

        }

        public AudioFileExtractor(ProgressControl progressControl)
        {
            this.progressControl = progressControl;
        }

        private void Process()
        {
            audioFiles = new List<string>();

            for (int i = 0; i < archivedFiles.Count(); i++)
            {
                if (progressControl != null)
                {
                    if (progressControl.Cancelled())
                    {
                        break;
                    }
                }
                
                //압축전부 풀고 오디오 파일인지 체크하고 파일 삭제하는 부분
                ///////////////////////////////////
                string extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
                ZipFile z = ZipFile.Read(archivedFiles[i]);
                z.FlattenFoldersOnExtract = true;
                z.ExtractAll(extractDir);                
                DirectoryInfo di = new DirectoryInfo(extractDir);
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        TagLib.File f = TagLib.File.Create(fi.FullName);
                        audioFiles.Add(fi.FullName);
                    }
                    catch (UnsupportedFormatException e)
                    {
                                                
                    }                    
                }
                di.Delete(true);
                ///////////////////////////////////
                current = i + 1;

                int perc = (int)((float)current / (float)total * 100);

                if (progressControl != null)
                    progressControl.FireProgress(perc);
            }
        }

        public void Run(List<string> files)
        {
            archivedFiles = files;
            total = archivedFiles.Count();

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
                string.Format("Finding Audio Files from Archived Files ({0}/{1})...", current, total));
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

            this.OnEnd(this, new AudioFileExtractorEventArgs(progressControl.Cancelled(), audioFiles));
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            Process();
        }
    }
}
