﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace MusicFileManager
{
    public class MainController
    {
        BackgroundWorker bw = null;
        ProgressControl progressControl = null;

        AudioFileFinder audioFinder = null;
        ArchivedFileFinder archiveFinder = null;
        ArchivedAudioFileFinder archivedAudioFinder = null;
        AudioFileChecker checker = null;
        ArchivedFileManager archiveController = null;

        int total;
        int current;

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
            
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage, "test");
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker b = sender as BackgroundWorker;
            List<string> allFiles = GetFiles(e.Argument.ToString(), e);
            List<string> audioFiles = GetAudioFiles(allFiles, e);
            List<string> archivedFiles = GetArchivedFiles(allFiles, e);
            List<string> archivedAudioFiles = GetArchivedFileHasAudio(archivedFiles, e);
        }

        private void ResetCount(int total)
        {
            this.total = total;
            current = 1;
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

                bw.ReportProgress(CalcPercentage());
            }

            return archivedFiles;
        }

        private List<string> GetAudioFiles(List<string> allFiles, DoWorkEventArgs e)
        {
            List<string> audioFiles = new List<string>();
            ResetCount(audioFiles.Count());

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

                bw.ReportProgress(CalcPercentage());
            }

            return archivedAudioFiles;
        }

        private List<string> GetDuplicatedArchiveFiles(List<string> archivedAudioFile, List<string> audioFiles, DoWorkEventArgs e)
        {
            List<string> duplicatedArchives = new List<string>();
            ResetCount(archivedAudioFile.Count());

            for (int i = 0; i < archivedAudioFile.Count(); i++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                List<string> extractedAudioFiles = archiveController.ExtractAudioFilesArchivedFile(archivedAudioFile[i]);

                bool isDuplicated = false;
                for (int j = 0; j < extractedAudioFiles.Count(); j++)
                {
                    for (int k = 0; k < audioFiles.Count(); k++)
                    {
                        if (checker.CheckSimilarFilesByByte(extractedAudioFiles[j], audioFiles[k]))
                        {
                            isDuplicated = true;
                            break;
                        }
                    }

                    if (isDuplicated)
                        break;
                }

                if (isDuplicated)
                    duplicatedArchives.Add(archivedAudioFile[i]);

                archiveController.CleanExtractedFiles();

                IncCount();

                bw.ReportProgress(CalcPercentage());
            }
            return duplicatedArchives;
        }       

        public void Run(string directory)
        {            
            bw.RunWorkerAsync(directory);
        }

        public void Cancel()
        {
            bw.CancelAsync();
        }
    }
}
