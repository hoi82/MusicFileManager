using System;
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
using System.Threading;
using System.Windows.Forms;

namespace MusicFileManager
{
    public enum ProcessingMode { CollectFile, CheckDuplication, Clean }
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

        public ProcessingMode processingMode = ProcessingMode.CollectFile;
        
        public MainController()
        {                      
            filetoClean = new List<DuplicatedFiles>();
            audioExtractor = new FileExtractor();            
        }        

        public MainController(MainWindow window) : this()
        {
            this.window = window;

            fileFinder = new FileFinder(new DefaultFileChecker());            
            audioFinder = new FileFinder(new AudioFileChecker());
            archiveFinder = new FileFinder(new ArchiveFileChecker());
            archivedAudioFinder = new FileFinder(new ArchivedAudioFileChecker(new AudioFileChecker()));
            audioExtractor = new FileExtractor(new AudioFileChecker());

            archiveDuplicationOption = new ArchiveDuplicationEvaluatorOption(window.option);
            archiveDuplicationChecker = new ArchiveDuplicationChecker();
            archiveDuplicationEvaluator = new ArchiveDuplicationEvaluator(archiveDuplicationOption, archiveDuplicationChecker, audioExtractor);

            audioDuplcationOption = new AudioDuplicationEvaluatorOption(window.option);
            audioDuplicationChecker = new AudioDuplicationChecker();
            audioDuplicationEvaluator = new AudioDuplicationEvaluator(audioDuplcationOption, audioDuplicationChecker);

            fileFinder.SetSerializedFinder(audioFinder);
            audioFinder.SetSerializedFinder(archiveFinder, true);
            archiveFinder.SetSerializedFinder(archivedAudioFinder);

            fileFinder.OnStartAsync += fileFinder_OnStartAsync;
            fileFinder.OnProgressAsync += fileFinder_OnProgressAsync;
            fileFinder.OnCancelAsync += fileFinder_OnCancelAsync;
            fileFinder.OnCompleteAsync += fileFinder_OnCompleteAsync;

            audioFinder.OnStartAsync += audioFinder_OnStartAsync;
            audioFinder.OnProgressAsync += audioFinder_OnProgressAsync;
            audioFinder.OnCancelAsync += audioFinder_OnCancelAsync;
            audioFinder.OnCompleteAsync += audioFinder_OnCompleteAsync;

            archiveFinder.OnStartAsync += archiveFinder_OnStartAsync;
            archiveFinder.OnProgressAsync += archiveFinder_OnProgressAsync;
            archiveFinder.OnCancelAsync += archiveFinder_OnCancelAsync;
            archiveFinder.OnCompleteAsync += archiveFinder_OnCompleteAsync;

            archivedAudioFinder.OnStartAsync += archivedAudioFinder_OnStartAsync;
            archivedAudioFinder.OnProgressAsync += archivedAudioFinder_OnProgressAsync;
            archivedAudioFinder.OnCancelAsync += archivedAudioFinder_OnCancelAsync;
            archivedAudioFinder.OnCompleteAsync += archivedAudioFinder_OnCompleteAsync;

            archiveDuplicationEvaluator.OnStartAsync += archiveDuplicationEvaluator_OnStartAsync;
            archiveDuplicationEvaluator.OnProgressAsync += archiveDuplicationEvaluator_OnProgressAsync;
            archiveDuplicationEvaluator.OnCancelAsync += archiveDuplicationEvaluator_OnCancelAsync;
            archiveDuplicationEvaluator.OnCompleteAsync += archiveDuplicationEvaluator_OnCompleteAsync;

            audioDuplicationEvaluator.OnStartAsync += audioDuplicationEvaluator_OnStartAsync;
            audioDuplicationEvaluator.OnProgressAsync += audioDuplicationEvaluator_OnProgressAsync;
            audioDuplicationEvaluator.OnCancelAsync += audioDuplicationEvaluator_OnCancelAsync;
            audioDuplicationEvaluator.OnCompleteAsync += audioDuplicationEvaluator_OnCompleteAsync;

            //fileCleaner = new FileCleaner(window.prgControl);
            //fileCleaner.OnEndAsync += fileCleaner_OnEndAsync;

            //ctrlClean = new OldFileToCleanControl(window.grdPopUp);
            //ctrlClean.OnOK += ctrlClean_OnOK;
            //ctrlClean.OnCancel += ctrlClean_OnCancel;
        }

        void audioDuplicationEvaluator_OnCompleteAsync(object sender, DuplicationEvaluatorEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";
            processingMode = ProcessingMode.Clean;
            window.btnProc.Content = "Clean";
            filetoClean.AddRange(e.DuplicatedFiles);
        }

        void audioDuplicationEvaluator_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void audioDuplicationEvaluator_OnProgressAsync(object sender, DuplicationEvaluatorProgressEventArgs e)
        {
            if (window.currentMouserOverButton == window.btnProc)
            {
                window.Dispatcher.Invoke(() =>
                {
                    float cur = e.InnerCurrent + 1;
                    float total = e.InnerTotal;
                    window.lblUpperPop.Content = string.Format("Finding Duplicated Music Files {0}/{1} of {2}/{3}", cur, total, e.OuterCurrent + 1, e.OuterTotal);
                    window.prgPop.Value = cur / total * 100;
                });
            }            
        }

        void audioDuplicationEvaluator_OnStartAsync(object sender, EventArgs e)
        {
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
        }

        void archiveDuplicationEvaluator_OnCompleteAsync(object sender, DuplicationEvaluatorEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";  
            filetoClean.AddRange(e.DuplicatedFiles);
            audioDuplicationEvaluator.GetDuplications(audioFiles, true);
        }

        void archiveDuplicationEvaluator_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void archiveDuplicationEvaluator_OnProgressAsync(object sender, DuplicationEvaluatorProgressEventArgs e)
        {
            if (window.currentMouserOverButton == window.btnProc)
            {
                window.Dispatcher.Invoke(() =>
                {
                    float cur = e.InnerCurrent + 1;
                    float total = e.InnerTotal;
                    window.lblUpperPop.Content = string.Format("Finding Duplicated Archive Files {0}/{1} of {2}/{3}", cur, total, e.OuterCurrent + 1, e.OuterTotal);
                    window.prgPop.Value = cur / total * 100;
                }); 
            }            
        }

        void archiveDuplicationEvaluator_OnStartAsync(object sender, EventArgs e)
        {
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
        }

        void archivedAudioFinder_OnCompleteAsync(object sender, FileFinderEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";
            archivedAudioFiles = e.MatchedFiles;
            processingMode = ProcessingMode.CheckDuplication;
            archiveDuplicationEvaluator.GetDuplications(archivedAudioFiles, audioFiles, true);
        }

        void archivedAudioFinder_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void archivedAudioFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {            
            window.Dispatcher.Invoke(() =>
            {
                if (window.currentMouserOverButton == window.btnProc)
                {
                    float cur = e.Current + 1;
                    float total = e.Total;
                    window.lblUpperPop.Content = string.Format("Finding Archived Audio Files {0}/{1}", cur, total);
                    window.prgPop.Value = cur / total * 100;
                }                
            }); 
        }

        void archivedAudioFinder_OnStartAsync(object sender, FileFinderStartEventAtgs e)
        {
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
        }

        void archiveFinder_OnCompleteAsync(object sender, FileFinderEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";            
        }

        void archiveFinder_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void archiveFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {            
            window.Dispatcher.Invoke(() =>
            {
                if (window.currentMouserOverButton == window.btnProc)
                {
                    float cur = e.Current + 1;
                    float total = e.Total;
                    window.lblUpperPop.Content = string.Format("Finding Archived Files {0}/{1}", cur, total);
                    window.prgPop.Value = cur / total * 100;
                }                
            }); 
        }

        void archiveFinder_OnStartAsync(object sender, FileFinderStartEventAtgs e)
        {
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
        }

        void audioFinder_OnCompleteAsync(object sender, FileFinderEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";
            audioFiles = e.MatchedFiles;
        }

        void audioFinder_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void audioFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {            
            window.Dispatcher.Invoke(() =>
            {
                if (window.currentMouserOverButton == window.btnProc)
                {
                    float cur = e.Current + 1;
                    float total = e.Total;
                    window.lblUpperPop.Content = string.Format("Finding Music Files {0}/{1}", cur, total);
                    window.prgPop.Value = cur / total * 100;
                }                
            });            
        }

        void audioFinder_OnStartAsync(object sender, FileFinderStartEventAtgs e)
        {
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
        }

        void fileFinder_OnCompleteAsync(object sender, FileFinderEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";
        }

        void fileFinder_OnCancelAsync(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void fileFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {
            window.Dispatcher.Invoke(() =>
            {
                if (window.currentMouserOverButton == window.btnProc)
                {
                    float cur = e.Current + 1;
                    float total = e.Total;
                    window.lblUpperPop.Content = string.Format("Finding All Files {0}/{1}", cur, total);
                    window.prgPop.Value = cur / total * 100;
                }                
            });            
        }

        void fileFinder_OnStartAsync(object sender, FileFinderStartEventAtgs e)
        {
            processingMode = ProcessingMode.CollectFile;
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
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
