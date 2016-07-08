﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using MusicFileManager.Checker;
using MusicFileManager.Extractor;
using MusicFileManager.Duplication;
using MusicFileManager.Cleaner;

namespace MusicFileManager
{    
    public delegate void MainControllerStartEvent(object sender);
    public delegate void MainContollerEndEvent(object sender);

    public class MainController
    {
        public event MainControllerStartEvent OnStart = null;
        public event MainContollerEndEvent OnEnd = null;

        MainWindow window = null;        

        IFileFinder fileFinder = null;
        IFileFinder audioFinder = null;
        IFileFinder archiveFinder = null;
        IFileFinder archivedAudioFinder = null;
        IFileExtractor audioExtractor = null;

        IDuplicationEvaluatorOption archiveDuplicationOption = null;
        IDuplicationChcker archiveDuplicationChecker = null;
        IDuplicationEvaluator archiveDuplicationEvaluator = null;

        IDuplicationEvaluatorOption audioDuplcationOption = null;
        IDuplicationChcker audioDuplicationChecker = null;
        IDuplicationEvaluator audioDuplicationEvaluator = null;

        IFileCleaner fileCleaner = null;
        
        List<string> audioFiles = null;        
        List<string> archivedAudioFiles = null;
        List<DuplicatedFiles> filetoClean = null;

        OldFileToCleanControl ctrlClean = null;
        
        public MainController()
        {                      
            filetoClean = new List<DuplicatedFiles>();
            audioExtractor = new FileExtractor();            
        }        

        public MainController(MainWindow window) : this()
        {
            //this.window = window;

            //fileFinder = new FileFinder(new DefaultFileChecker(), window.prgControl, MFMMessage.Message4);
            //audioFinder = new FileFinder(new AudioFileChecker(), window.prgControl, MFMMessage.Message6);
            //archiveFinder = new FileFinder(new ArchiveFileChecker(), window.prgControl, MFMMessage.Message5);
            //archivedAudioFinder = new FileFinder(new ArchivedAudioFileChecker(new AudioFileChecker()), window.prgControl, MFMMessage.Message7);
            //audioExtractor = new FileExtractor(new AudioFileChecker());

            //archiveDuplicationOption = new ArchiveDuplicationEvaluatorOption(window.ctrlOption);
            //archiveDuplicationChecker = new ArchiveDuplicationChecker();
            //archiveDuplicationEvaluator = new ArchiveDuplicationEvaluator(window.prgControl, archiveDuplicationOption, archiveDuplicationChecker, audioExtractor);

            //audioDuplcationOption = new AudioDuplicationEvaluatorOption(window.ctrlOption);
            //audioDuplicationChecker = new AudioDuplicationChecker();
            //audioDuplicationEvaluator = new AudioDuplicationEvaluator(window.prgControl, audioDuplcationOption, audioDuplicationChecker);            

            //fileFinder.SetSerializedFinder(audioFinder);
            //audioFinder.SetSerializedFinder(archiveFinder, true);
            //archiveFinder.SetSerializedFinder(archivedAudioFinder);
            
            //audioFinder.OnEndAsync += audioFinder_OnEndAsync;            
            //archivedAudioFinder.OnEndAsync += archivedAudioFinder_OnEndAsync;

            //archiveDuplicationEvaluator.OnEndAsync += archiveDuplicationEvaluator_OnEndAsync;
            //audioDuplicationEvaluator.OnEndAsync += audioDuplicationEvaluator_OnEndAsync;

            //fileCleaner = new FileCleaner(window.prgControl);
            //fileCleaner.OnEndAsync += fileCleaner_OnEndAsync;

            //ctrlClean = new OldFileToCleanControl(window.grdPopUp);
            //ctrlClean.OnOK += ctrlClean_OnOK;
            //ctrlClean.OnCancel += ctrlClean_OnCancel;
        }

        void ctrlClean_OnCancel(object sender)
        {
            if (this.OnEnd != null)
                this.OnEnd(this);
        }

        void ctrlClean_OnOK(object sender, List<string> files)
        {
            fileCleaner.CleanFiles(true, files);
        }

        void fileCleaner_OnEndAsync(object sender, FileCleanerEndEventArgs e)
        {
            if (this.OnEnd != null)
                this.OnEnd(this);

            //ModalDialogControl m = new ModalDialogControl(window.grdPopUp, DialogButton.OK);

            string message = string.Format("Deleted Files : {0} \r\n Undeleted Files : {1}", e.DeletedFiles.Count(), e.UnDeletedFiles.Count());
            //m.ShowDialog(message);
        }

        void audioDuplicationEvaluator_OnEndAsync(object sender, DuplicationEvaluatorEndEventArgs e)
        {
            filetoClean.AddRange(e.DuplicatedFiles);
            ctrlClean.Display(filetoClean);
        }

        void archiveDuplicationEvaluator_OnEndAsync(object sender, DuplicationEvaluatorEndEventArgs e)
        {
            filetoClean.AddRange(e.DuplicatedFiles);
            audioDuplicationEvaluator.GetDuplications(audioFiles, true);
        }

        void archivedAudioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            archivedAudioFiles = e.MatchedFiles;
            archiveDuplicationEvaluator.GetDuplications(archivedAudioFiles, audioFiles, true);
        }

        void audioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            audioFiles = e.MatchedFiles;
        }                                          

        public void Run(bool aSync, string directory)
        {
            fileFinder.GetMatchedFilesAsync(directory); 
            //DuplicatedFiles d = audioDuplicationChecker.CheckDuplication(@"C:\내꺼\Music\アニメ\01. Singing！.mp3", @"C:\내꺼\Music\アニメ\2011 top 100\10. 01. Singing！.mp3");    
        }        
    }
}
