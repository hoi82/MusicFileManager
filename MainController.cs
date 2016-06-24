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

        ProgressControl progressControl = null;
        string progressMessage = null;       

        MFMOption option = null;

        List<DuplicatedFiles> filetoClean = null;


        //.........................................................//

        IFileFinder fileFinder = null;
        IFileFinder audioFinder = null;
        IFileFinder archiveFinder = null;
        IFileFinder archivedAudioFinder = null;
        IFileExtractor audioExtractor = null;
        
        List<string> audioFiles = null;        
        List<string> archivedAudioFiles = null;
        
        public MainController()
        {                      
            filetoClean = new List<DuplicatedFiles>();
            audioExtractor = new FileExtractor();
        }        

        public MainController(ProgressControl progressControl) : this()
        {
            this.progressControl = progressControl;
            fileFinder = new FileFinder(new Checker.DefaultFileChecker(), progressControl, MFMMessage.Message4);
            audioFinder = new FileFinder(new AudioFileChecker(), progressControl, MFMMessage.Message6);
            archiveFinder = new FileFinder(new ArchiveFileChecker(), progressControl, MFMMessage.Message5);
            archivedAudioFinder = new FileFinder(new Checker.ArchivedAudioFileChecker(new AudioFileChecker()), progressControl, MFMMessage.Message7);

            fileFinder.SetSerializedFinder(audioFinder);
            audioFinder.SetSerializedFinder(archiveFinder, true);
            archiveFinder.SetSerializedFinder(archivedAudioFinder);
            
            audioFinder.OnEndAsync += audioFinder_OnEndAsync;            
            archivedAudioFinder.OnEndAsync += archivedAudioFinder_OnEndAsync;
        }

        void archivedAudioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            archivedAudioFiles = e.MatchedFiles;
        }

        void audioFinder_OnEndAsync(object sender, FileFinderEndEventArgs e)
        {
            audioFiles = e.MatchedFiles;
        }

        public MainController(ProgressControl progressControl, MFMOption option) : this(progressControl)            
        {
            this.option = option;
        }                                           

        public void Run(bool aSync, string directory)
        {
            fileFinder.GetMatchedFilesAsync(directory);                        
        }        
    }
}
