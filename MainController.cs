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
    public enum ProcessingMode { ReadyFind, CollectFile, CheckDuplication, ReadyClean, Clean }
    public delegate void MainControllerStartEvent(object sender);
    public delegate void MainContollerEndEvent(object sender);

    public class MainController
    {
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

        public ProcessingMode processingMode = ProcessingMode.ReadyFind;
        
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

            fileCleaner = new FileCleaner(window.fileControl);

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

            fileCleaner.OnStartAsync += fileCleaner_OnStartAsync;
            fileCleaner.OnProgressAsync += fileCleaner_OnProgressAsync;
            fileCleaner.OnCancelAsync += fileCleaner_OnCancelAsync;
            fileCleaner.OnCompleteAsync += fileCleaner_OnCompleteAsync;
        }

        void fileCleaner_OnCompleteAsync(object sender, FileCleanerCompleteEventArgs e)
        {
            throw new NotImplementedException();
        }

        void fileCleaner_OnCancelAsync(object sender, EventArgs e)
        {
            processingMode = ProcessingMode.ReadyClean;
            window.option.IsEnabled = true;
            window.btnProc.Content = "Clean";
        }

        void fileCleaner_OnProgressAsync(object sender, FileCleanerProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Deleting Files {0}/{1}", e.Current + 1, e.Total), e.Current + 1, e.Total);            
        }

        void fileCleaner_OnStartAsync(object sender, EventArgs e)
        {
            processingMode = ProcessingMode.Clean;
            window.option.IsEnabled = false;
            window.btnProc.Content = "Cancel";
        }

        void audioDuplicationEvaluator_OnCompleteAsync(object sender, DuplicationEvaluatorEndEventArgs e)
        {
            window.lblUpperPop.Content = "Complete";
            processingMode = ProcessingMode.ReadyClean;
            window.btnProc.Content = "Clean";
            filetoClean.AddRange(e.DuplicatedFiles);
            window.option.IsEnabled = true;
        }

        void audioDuplicationEvaluator_OnCancelAsync(object sender, EventArgs e)
        {
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void audioDuplicationEvaluator_OnProgressAsync(object sender, DuplicationEvaluatorProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Finding Duplicated Music Files {0}/{1} of {2}/{3}", e.InnerCurrent + 1, e.InnerTotal, e.OuterCurrent + 1, e.OuterTotal), e.InnerCurrent + 1, e.InnerTotal);            
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
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void archiveDuplicationEvaluator_OnProgressAsync(object sender, DuplicationEvaluatorProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Finding Duplicated Archive Files {0}/{1} of {2}/{3}", e.InnerCurrent + 1, e.InnerTotal, e.OuterCurrent + 1, e.OuterTotal), e.InnerCurrent + 1, e.InnerTotal);            
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
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void archivedAudioFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Finding Archived Audio Files {0}/{1}", e.Current + 1, e.Total), e.Current + 1, e.Total);            
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
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void archiveFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Finding Archived Files {0}/{1}", e.Current + 1, e.Total), e.Current + 1, e.Total);            
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
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void audioFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {
            DisplayProgressPopUp(string.Format("Finding Music Files {0}/{1}", e.Current + 1, e.Total), e.Current + 1, e.Total);            
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
            processingMode = ProcessingMode.ReadyFind;
            window.btnProc.Content = "Find";
            window.DisplayPopUp(window.currentMouserOverButton);
            window.option.IsEnabled = true;
        }

        void fileFinder_OnProgressAsync(object sender, FileFinderProgressEventArgs e)
        {            
            DisplayProgressPopUp(string.Format("Finding All Files {0}/{1}", e.Current + 1, e.Total), e.Current + 1, e.Total);                                     
        }

        void fileFinder_OnStartAsync(object sender, FileFinderStartEventAtgs e)
        {
            processingMode = ProcessingMode.CollectFile;
            window.btnProc.Content = "Cancel";
            window.prgPop.Value = 0;
            window.DisplayPopUp(window.currentMouserOverButton);
            window.lblLowerPop.Content = "Click for cancel";
            window.option.IsEnabled = false;
        }                                                

        public void Find(string directory)
        {
            fileFinder.GetMatchedFilesAsync(directory); 
            //DuplicatedFiles d = audioDuplicationChecker.CheckDuplication(@"C:\내꺼\Music\アニメ\01. Singing！.mp3", @"C:\내꺼\Music\アニメ\2011 top 100\10. 01. Singing！.mp3");    
        }

        public void CancelFind()
        {
            fileFinder.Cancel();
            audioDuplicationEvaluator.Cancel();
            archiveDuplicationEvaluator.Cancel();
        }        

        public void Clean()
        {
            fileCleaner.CleanFilesAsync();
        }

        public void CancelClean()
        {
            fileCleaner.CancelCleanAsync();
        }

        public void DisplayProgressPopUp(string message, int current, int total)
        {
            if (window.currentMouserOverButton == window.btnProc)
            {
                window.Dispatcher.Invoke(() => 
                {
                    window.lblUpperPop.Content = message;
                    window.prgPop.Value = (float)current / (float)total * 100;
                    window.lblLowerPop.Content = "Click for Cancel";
                });                               
            } 
        }
    }
}
