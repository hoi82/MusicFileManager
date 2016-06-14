using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public delegate void MainControllerStartEvent(object sender);
    public delegate void MainContollerEndEvent(object sender);

    public class MainController
    {
        public event MainControllerStartEvent OnStart = null;
        public event MainContollerEndEvent OnEnd = null;

        BackgroundWorker bw = null;

        ProgressControl progressControl = null;
        string progressMessage = null;

        AudioFileFinder audioFinder = null;
        ArchivedFileFinder archiveFinder = null;
        ArchivedAudioFileFinder archivedAudioFinder = null;
        AudioFileChecker checker = null;
        ArchivedFileManager archiveController = null;

        int total;
        int current;

        MFMOption option = null;
        
        public MainController()
        {
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            total = 0;
            current = 0;

            audioFinder = new AudioFileFinder();
            archiveFinder = new ArchivedFileFinder();
            archivedAudioFinder = new ArchivedAudioFileFinder(audioFinder);
            checker = new AudioFileChecker();
            archiveController = new ArchivedFileManager(audioFinder);
        }        

        public MainController(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
        }

        public MainController(ProgressControl progressControl, MFMOption option) : this(progressControl)            
        {
            this.option = option;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.OnEnd != null)
                this.OnEnd(this);

            if (e.Cancelled)
            {
                progressControl.ProgressDisplay(0, "Cancelled");
            }
            else
            {
                progressControl.ProgressDisplay(100, "Complete");
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage, progressMessage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker b = sender as BackgroundWorker;
            List<string> allFiles = GetFiles(e.Argument.ToString(), e);
            List<AudioFile> audioFiles = GetAudioFiles(allFiles, e);
            List<string> archivedFiles = GetArchivedFiles(allFiles, e);
            List<string> archivedAudioFiles = GetArchivedFileHasAudio(archivedFiles, e);
            List<DuplicatedFiles> extractedArchiveAudioFiles = GetDuplicatedArchiveFiles(archivedFiles, audioFiles, e);
            List<DuplicatedFiles> duplicatedAudioFiles = GetDuplicatedAudioFiles(audioFiles, e);        
        }

        private void ResetCount(int total)
        {
            this.total = total;
            current = 0;
        }

        private void IncCount()
        {
            current++;
        }

        private int CalcPercentage()
        {
            int perc = (int)((float)current / (float)total * 100);

            return perc;
        }

        private List<string> GetFiles(string location, DoWorkEventArgs e)
        {
            List<string> allFiles = new List<string>();            
            string[] allPaths = Directory.GetFiles(location, "*.*", SearchOption.AllDirectories);            
            ResetCount(allPaths.Count());

            for (int i = 0; i < allPaths.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                FileAttributes attr = File.GetAttributes(allPaths[i]);
                if (!attr.HasFlag(FileAttributes.Directory))
                {
                    allFiles.Add(allPaths[i]);
                }

                IncCount();
                progressMessage = "Getting All Files from Directroy..";

                bw.ReportProgress(CalcPercentage());
            }

            return allFiles;
        }

        private List<string> GetArchivedFiles(List<string> allFiles, DoWorkEventArgs e)
        {            
            List<string> archivedFiles = new List<string>();
            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }         

                if (archiveFinder.CheckArchivedFile(allFiles[i]))
                    archivedFiles.Add(allFiles[i]);

                IncCount();
                progressMessage = string.Format("Finding Archive Files.....{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }

            return archivedFiles;
        }

        private List<AudioFile> GetAudioFiles(List<string> allFiles, DoWorkEventArgs e)
        {
            List<AudioFile> audioFiles = new List<AudioFile>();
            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string sFile = allFiles[i];

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }                        

                if (audioFinder.CheckAudioFile(ref sFile, true))
                {
                    TagLib.File f = TagLib.File.Create(sFile);
                    audioFiles.Add(new AudioFile(sFile, f.Properties.AudioBitrate, f.Properties.Duration));
                }                    

                IncCount();
                progressMessage = string.Format("Finding Audio Files.....{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }

            return audioFiles;
        }

        private List<string> GetArchivedFileHasAudio(List<string> archivedFiles, DoWorkEventArgs e)
        {
            List<string> archivedAudioFiles = new List<string>();
            ResetCount(archivedFiles.Count());

            for (int i = 0; i < archivedFiles.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                } 

                if (archivedAudioFinder.CheckArchivedAudioFile(archivedFiles[i]))
                    archivedAudioFiles.Add(archivedFiles[i]);

                IncCount();
                progressMessage = string.Format("Finding Archive Files has Audio File.....{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }

            return archivedAudioFiles;
        }

        private List<DuplicatedFiles> GetDuplicatedArchiveFiles(List<string> archivedAudioFile, List<AudioFile> audioFiles, DoWorkEventArgs e)
        {
            List<DuplicatedFiles> duplicatedArchives = new List<DuplicatedFiles>();
            ResetCount(audioFiles.Count());

            for (int i = 0; i < archivedAudioFile.Count(); i++)
            {
                total = archivedAudioFile.Count();
                current = i + 1;

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                List<string> extractedAudioFiles = archiveController.ExtractAudioFilesArchivedFile(archivedAudioFile[i]);

                //여러개의 음악파일이 존재하고 옵션에서 처리 안하게 되어있으면 건너뛴다.
                if ((extractedAudioFiles.Count > 1) && !option.DeleteArchiveWithMulipleAudio)
                {
                    archiveController.CleanExtractedFiles();
                    continue;
                }

                bool isDuplicated = false;
                string audioFileName = null;

                for (int j = 0; j < extractedAudioFiles.Count(); j++)
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    total = extractedAudioFiles.Count();
                    current = j + 1;

                    for (int k = 0; k < audioFiles.Count(); k++)
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        total = audioFiles.Count();
                        current = k + 1;

                        if (checker.CheckSimilarFilesByByte(extractedAudioFiles[j], audioFiles[k].FileName))
                        {
                            isDuplicated = true;
                            audioFileName = audioFiles[k].FileName;
                            break;
                        }

                        //IncCount();
                        progressMessage = string.Format("Check {0}/{1} Audio File in {2}/{3} Archive File with {4}/{5} AudioFile", j + 1, extractedAudioFiles.Count(), i + 1, archivedAudioFile.Count, current, total);
                        bw.ReportProgress(CalcPercentage());
                    }

                    if (isDuplicated)
                        break;

                    bw.ReportProgress(CalcPercentage());
                }

                if (isDuplicated)
                {
                    DuplicatedFiles d = new DuplicatedFiles(audioFileName, archivedAudioFile[i], DuplicateType.AlreadyExtractedArchive);

                    if (!duplicatedArchives.Contains(d))
                        duplicatedArchives.Add(d);  
                }                    

                archiveController.CleanExtractedFiles();

                bw.ReportProgress(CalcPercentage());
            }
            return duplicatedArchives;
        }

        private List<DuplicatedFiles> GetDuplicatedAudioFiles(List<AudioFile> audioFiles, DoWorkEventArgs e)
        {
            List<DuplicatedFiles> duplicatedAudioFiles = new List<DuplicatedFiles>();
            ResetCount(audioFiles.Count());

            for (int i = 0; i < audioFiles.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                for (int j = 0; j < audioFiles.Count(); j++)
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }                    

                    //옵션에서 처리 안하게 되어있거나 같은 파일일 경우 건너뛴다.
                    if ((i == j) | ((!option.DeleteAudioWithOutFreqAndBitRate) && ((audioFiles[j].BitRate < option.AudioBitRate) | (audioFiles[j].Duration < audioFiles[j].Duration)))) continue;

                    if (checker.CheckSimilarFilesByNameAndTag(audioFiles[i].FileName, audioFiles[j].FileName))
                    {                        
                        DuplicatedFiles d = new DuplicatedFiles(audioFiles[i].FileName, audioFiles[j].FileName, DuplicateType.DuplicateAudioTag);

                        if (!duplicatedAudioFiles.Contains(d))
                            duplicatedAudioFiles.Add(d);
                    }                        

                    current = j + 1;

                    progressMessage = string.Format("Checking {0}/{1} Audio File with Other Audio Files.....{2}/{3}", i+1, total, current, total);

                    bw.ReportProgress(CalcPercentage());
                }

                current = i + 1;

                progressMessage = string.Format("Finding Archive Files has Audio File.....{0}/{1}", current, total);

                bw.ReportProgress(CalcPercentage());
            }

            return duplicatedAudioFiles;
        }        

        public void Run(string directory)
        {
            if (this.OnStart != null)
                this.OnStart(this);

            bw.RunWorkerAsync(directory);
        }

        public void Cancel()
        {
            bw.CancelAsync();
        }
    }
}
