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
    public class ArchivedAudioFileEventArgs : EventArgs
    {
        bool cancel = false;
        List<string> archivedFileWithAudio = null;

        public ArchivedAudioFileEventArgs(bool cancel, List<string> audioFiles)
        {
            this.cancel = cancel;
            this.archivedFileWithAudio = audioFiles;
        }

        public bool Cancel { get { return this.cancel; } }
        public List<string> ArchivedFileWithAudio { get { return this.archivedFileWithAudio; } }
    }

    public delegate void ArchivedAudioFileFinderStartEventHandler(object sender);
    public delegate void ArchivedAudioFileFinderEndEventHandler(object sender, ArchivedAudioFileEventArgs e);

    public class ArchivedAudioFileFinder
    {
        ProgressControl progressControl = null;
        List<string> archivedFiles = null;
        List<string> archivedAudiFile = null;
        int current = 0;
        int total = 0;
        string extractDir = null;
        const string EXTRACT_DIR = "Extracts";

        public event ArchivedAudioFileFinderStartEventHandler OnStart;
        public event ArchivedAudioFileFinderEndEventHandler OnEnd;

        public ArchivedAudioFileFinder()
        {
            extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
        }

        public ArchivedAudioFileFinder(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
        }

        private void Process()
        {
            
        }

        private void GetActualAudioFileFromArchivedfile()
        {            
            //audioFiles = new List<AudioFile>();

            //for (int i = 0; i < archivedFiles.Count(); i++)
            //{
            //    if (progressControl != null)
            //    {
            //        if (progressControl.Cancelled())
            //        {
            //            break;
            //        }
            //    }

            //    //압축전부 풀고 오디오 파일인지 체크하고 파일 삭제하는 부분
            //    ///////////////////////////////////                
            //    DirectoryInfo di = new DirectoryInfo(extractDir);
            //    ZipFile z = ZipFile.Read(archivedFiles[i]);
            //    foreach (ZipEntry entry in z.Entries)
            //    {
            //        if (!entry.IsDirectory)
            //        {
            //            entry.Extract(extractDir);
            //            string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");
            //            try
            //            {
            //                TagLib.File f = TagLib.File.Create(extractedPath);
            //                byte[] data = System.IO.File.ReadAllBytes(extractedPath);
            //                string fileName = Path.GetFileName(extractedPath);
            //                audioFiles.Add(new AudioFile(fileName, null, data, f.Tag));
            //            }
            //            catch (UnsupportedFormatException e)
            //            {
            //                //System.IO.File.Delete(extractedPath);                            
            //            }
            //        }
            //    }
            //    z.Dispose();
            //    di.Delete(true);
            //    ///////////////////////////////////
            //    current = i + 1;

            //    int perc = (int)((float)current / (float)total * 100);

            //    if (progressControl != null)
            //        progressControl.FireProgress(perc);
            //}
        }

        public void RunAsync(List<string> files)
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

            this.OnEnd(this, new ArchivedAudioFileEventArgs(progressControl.Cancelled(), archivedAudiFile));
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            GetActualAudioFileFromArchivedfile();
        }
    }
}
