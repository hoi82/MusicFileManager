using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MusicFileManager.Extractor;

namespace MusicFileManager
{
    public delegate void MainControllerStartEvent(object sender);
    public delegate void MainContollerEndEvent(object sender, List<DuplicatedFiles> fileToClean);

    public class MainController
    {
        public event MainControllerStartEvent OnStart = null;
        public event MainContollerEndEvent OnEnd = null;

        BackgroundWorker bw = null;

        ProgressControl progressControl = null;
        string progressMessage = null;

        //AudioFileChecker audioFinderToDelete = null;
        //ArchiveFileChecker archiveFinderToDelete = null;
        ArchivedAudioFileFinder archivedAudioFinderToDelete = null;
        AudioFileComparer checker = null;
        

        int total;
        int current;

        MFMOption option = null;

        List<DuplicatedFiles> filetoClean = null;


        //.........................................................//

        IFileFinder fileFinder = null;
        IFileFinder audioFinder = null;
        IFileFinder archiveFinder = null;
        IFileFinder archivedAudioFinder = null;
        IFileExtractor audioExtractor = null;
        
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

            checker = new AudioFileComparer();            

            filetoClean = new List<DuplicatedFiles>();
            audioExtractor = new FileExtractor();
        }        

        public MainController(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
            fileFinder = new FileFinder(new Checker.DefaultFileChecker(), progressControl);
            audioFinder = new FileFinder(new AudioFileChecker(), progressControl);
            archiveFinder = new FileFinder(new ArchiveFileChecker(), progressControl);
            archivedAudioFinder = new ArchivedAudioFileFinder(new AudioFileChecker(), progressControl);
            //fileFinder.AddSerializedFinder(audioFinder);
            //fileFinder.AddSerializedFinder(archiveFinder);
            //archiveFinder.AddSerializedFinder(archivedAudioFinder);

            fileFinder.OnEndAsync += fileFinder_OnEndAsync;
            audioFinder.OnEndAsync += audioFinder_OnEndAsync;
            archiveFinder.OnEndAsync += archiveFinder_OnEndAsync;
            archivedAudioFinder.OnEndAsync += archivedAudioFinder_OnEndAsync;
        }

        void archivedAudioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            int num = e.MatchedFiles.Count;
        }

        void archiveFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            int num = e.MatchedFiles.Count;
        }

        void audioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            int num = e.MatchedFiles.Count;
        }

        void fileFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            int num = e.MatchedFiles.Count;

            if (this.OnEnd != null)
                this.OnEnd(this, filetoClean);
        }

        public MainController(ProgressControl progressControl, MFMOption option) : this(progressControl)            
        {
            this.option = option;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.OnEnd != null)
                this.OnEnd(this, filetoClean);

            if (e.Cancelled)
            {
                if (progressControl != null)
                    progressControl.ProgressDisplay(0, "Cancelled");
            }
            else
            {
                if (progressControl != null)
                    progressControl.ProgressDisplay(100, "Complete");
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (progressControl != null)
                progressControl.ProgressDisplay(e.ProgressPercentage, progressMessage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Process(e);
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

        private List<DuplicatedFiles> GetDuplicatedArchiveFiles(List<string> archivedAudioFile, List<AudioFile> audioFiles, DoWorkEventArgs e)
        {
            List<DuplicatedFiles> duplicatedArchives = new List<DuplicatedFiles>();
            ResetCount(audioFiles.Count());

            for (int i = 0; i < archivedAudioFile.Count(); i++)
            {
                total = archivedAudioFile.Count();
                current = i + 1;

                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                List<string> extractedAudioFiles = audioExtractor.ExtractMathcedFiles(archivedAudioFile[i], new AudioFileChecker());

                //여러개의 음악파일이 존재하고 옵션에서 처리 안하게 되어있으면 건너뛴다.
                if (option != null)
                {
                    if ((extractedAudioFiles.Count > 1) && !option.DeleteArchiveWithMulipleAudio)
                    {
                        audioExtractor.CleanExtractedFiles();
                        continue;
                    }
                }                

                bool isDuplicated = false;
                string audioFileName = null;

                for (int j = 0; j < extractedAudioFiles.Count(); j++)
                {
                    if ((e != null) && bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    total = extractedAudioFiles.Count();
                    current = j + 1;

                    for (int k = 0; k < audioFiles.Count(); k++)
                    {
                        if ((e != null) && bw.CancellationPending)
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
                        progressMessage = string.Format(MFMMessage.Message8, j + 1, extractedAudioFiles.Count(), i + 1, archivedAudioFile.Count, current, total);
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

                audioExtractor.CleanExtractedFiles();

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
                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                for (int j = 0; j < audioFiles.Count(); j++)
                {
                    if ((e!= null) && bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }                    

                    //옵션에서 처리 안하게 되어있거나 같은 파일일 경우 건너뛴다.
                    if (option != null)
                    {
                        if ((i == j) | ((!option.DeleteAudioWithOutBitRate) && ((audioFiles[j].BitRate < option.AudioBitRate) | (audioFiles[j].Duration < audioFiles[j].Duration)))) continue;
                    }                    

                    if (checker.CheckSimilarFilesByNameAndTag(audioFiles[i].FileName, audioFiles[j].FileName))
                    {
                        string origin = null;
                        string duplicated = null;

                        if (IsParentOrLowTreedFolder(audioFiles[i].FileName, audioFiles[j].FileName))
                        {
                            origin = audioFiles[i].FileName;
                            duplicated = audioFiles[j].FileName;
                        }
                        else
                        {
                            origin = audioFiles[j].FileName;
                            duplicated = audioFiles[i].FileName;
                        }

                        DuplicatedFiles d = new DuplicatedFiles(origin, duplicated, DuplicateType.DuplicateAudioTag);

                        if (!duplicatedAudioFiles.Contains(d))
                            duplicatedAudioFiles.Add(d);
                    }                        

                    current = j + 1;

                    progressMessage = string.Format(MFMMessage.Message9, i+1, total, current, total);

                    bw.ReportProgress(CalcPercentage());
                }

                current = i + 1;

                progressMessage = string.Format(MFMMessage.Message10, current, total);

                bw.ReportProgress(CalcPercentage());
            }

            return duplicatedAudioFiles;
        }

        private bool IsParentOrLowTreedFolder(string source, string target)
        {
            if (string.IsNullOrEmpty(source) | string.IsNullOrEmpty(target))
                return false;

            string sourceDir = Path.GetDirectoryName(source);
            string targetDir = Path.GetDirectoryName(target);

            if (target.StartsWith(source))
                return true;

            if (sourceDir[0] == targetDir[0])
            {
                string[] sourceArr = sourceDir.Split(Path.DirectorySeparatorChar);
                string[] targetArr = targetDir.Split(Path.DirectorySeparatorChar);

                if (sourceArr.Length < targetArr.Length)
                    return true;
            }

            return false;
        }

        public void Run(bool aSync, string directory)
        {
            fileFinder.GetMatchedFilesAsync(directory);

            //if (this.OnStart != null)
            //    this.OnStart(this);            

            //if (aSync)
            //    bw.RunWorkerAsync(directory);
            //else
            //{
            //    Process();

            //    if (this.OnEnd != null)
            //        this.OnEnd(this, filetoClean);
            //}
        }

        private void Process(DoWorkEventArgs e = null)
        {           
            //List<string> allFiles = fileFinder.GetMatchedFiles(e.Argument.ToString());
            //List<string> audioFiles = audioFinder.GetMatchedFiles(allFiles);
            //List<string> archivedFiles = archiveFinder.GetMatchedFiles(allFiles);
            //List<string> archivedAudioFiles = archivedAudioFinder.GetMatchedFiles(archivedFiles);                     



            //List<DuplicatedFiles> extractedArchiveAudioFiles = GetDuplicatedArchiveFiles(archivedFiles, audioFiles, e);
            //List<DuplicatedFiles> duplicatedAudioFiles = GetDuplicatedAudioFiles(audioFiles, e);
            //filetoClean.AddRange(extractedArchiveAudioFiles);
            //filetoClean.AddRange(duplicatedAudioFiles);
        }

        public void Cancel()
        {
            bw.CancelAsync();
        }
    }
}
