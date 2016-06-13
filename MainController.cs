﻿using System;
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
        bool deleteMultiAudioFileInArchive = false;
        bool deleteAudioWithoutFrequencyFiltering = false;
        public bool DeleteMultiAudioFileInArchive { get; set; }
        public bool DeleteAudioWithoutFrequencyFiltering { get; set; }
        public MainController()
        {
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            audioFinder = new AudioFileFinder();
            archiveFinder = new ArchivedFileFinder();
            archivedAudioFinder = new ArchivedAudioFileFinder(audioFinder);
            checker = new AudioFileChecker();
            archiveController = new ArchivedFileManager(audioFinder);

            total = 0;
            current = 0;
        }

        public MainController(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
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
            List<string> audioFiles = GetAudioFiles(allFiles, e);
            List<string> archivedFiles = GetArchivedFiles(allFiles, e);
            List<string> archivedAudioFiles = GetArchivedFileHasAudio(archivedFiles, e);
            List<DuplicatedAudioFiles> extractedArchiveAudioFiles = GetDuplicatedArchiveFiles(archivedFiles, audioFiles, e);
            List<DuplicatedAudioFiles> duplicatedAudioFiles = GetDuplicatedAudioFiles(audioFiles, e);        
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

        private List<string> GetAudioFiles(List<string> allFiles, DoWorkEventArgs e)
        {
            List<string> audioFiles = new List<string>();
            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }         

                if (audioFinder.CheckAudioFile(allFiles[i]))
                    audioFiles.Add(allFiles[i]);

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

        private List<DuplicatedAudioFiles> GetDuplicatedArchiveFiles(List<string> archivedAudioFile, List<string> audioFiles, DoWorkEventArgs e)
        {
            List<DuplicatedAudioFiles> duplicatedArchives = new List<DuplicatedAudioFiles>();
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

                bool isDuplicated = false;
                string audioFileName = null;

                if ((extractedAudioFiles.Count > 1) && !this.deleteMultiAudioFileInArchive)
                    continue;

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

                        if (checker.CheckSimilarFilesByByte(extractedAudioFiles[j], audioFiles[k]))
                        {
                            isDuplicated = true;
                            audioFileName = audioFiles[k];
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
                    DuplicatedAudioFiles d = new DuplicatedAudioFiles(audioFileName, archivedAudioFile[i], DuplicateType.AlreadyExtractedArchive);

                    if (!duplicatedArchives.Contains(d))
                        duplicatedArchives.Add(d);  
                }                    

                archiveController.CleanExtractedFiles();

                bw.ReportProgress(CalcPercentage());
            }
            return duplicatedArchives;
        }

        private List<DuplicatedAudioFiles> GetDuplicatedAudioFiles(List<string> audioFiles, DoWorkEventArgs e)
        {
            List<DuplicatedAudioFiles> duplicatedAudioFiles = new List<DuplicatedAudioFiles>();
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

                    if (i == j) continue;

                    if (checker.CheckSimilarFilesByNameAndTag(audioFiles[i], audioFiles[j]))
                    {
                        DuplicatedAudioFiles d = new DuplicatedAudioFiles(audioFiles[i], audioFiles[j], DuplicateType.DuplicateAudioTag);

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