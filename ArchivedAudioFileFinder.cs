using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Ionic.Zip;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace MusicFileManager
{       
    //public delegate void ArchivedAudioFileFinderStartEventHandler(object sender);
    //public delegate void ArchivedAudioFileFinderCheckEventHandler(object sender, bool archivedAudioFile, string fileName, int currentCount, int totalCount);
    //public delegate void ArchivedAudioFileFinderEndEventHandler(object sender);

    public class ArchivedAudioFileFinder
    {        
        //List<string> archivedFiles = null;        
        //int current = 0;
        //int total = 0;
        string extractDir = null;
        const string EXTRACT_DIR = "Extracts";
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();

        //public event ArchivedAudioFileFinderStartEventHandler OnStart;
        //public event ArchivedAudioFileFinderCheckEventHandler OnCheck;
        //public event ArchivedAudioFileFinderEndEventHandler OnEnd;

        AudioFileChecker audioFinder = null;        

        public ArchivedAudioFileFinder(AudioFileChecker audioFinder)
        {
            extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
            this.audioFinder = audioFinder;            
        }

        public bool CheckArchivedAudioFile(string archivedFile)
        {
            if (!System.IO.File.Exists(archivedFile))
                return false;

            bool IsAudio = false;            
            try
            {
                ZipFile z = ZipFile.Read(archivedFile);
                foreach (ZipEntry entry in z.Entries)
                {
                    string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");

                    if (entry.IsDirectory)
                    {
                        extractedDir.Add(extractedPath);
                        continue;
                    }
                    else
                    {
                        extractedFiles.Add(extractedPath);
                    }

                    entry.Extract(extractDir, ExtractExistingFileAction.OverwriteSilently);
                    
                    if (audioFinder.IsVaildFile(ref extractedPath))
                    {
                        IsAudio = true;
                        break;
                    }                    
                }
                z.Dispose();                
            }
            catch (Exception)
            {                                
                throw;
            }
            finally
            {
                CleanExtractedFiles();
            }

            return IsAudio;
        }

        public void CleanExtractedFiles()
        {
            foreach (string s in extractedFiles)
            {
                DeleteFile(s);
            }

            extractedDir.Sort(delegate(string x, string y)
            {
                Regex regex = new Regex(@"[\\/]+");
                string[] xArr = regex.Split(x);
                string[] yArr = regex.Split(y);

                if (xArr.Length == yArr.Length) return 0;
                else if (xArr.Length < yArr.Length) return -1;
                else return 1;
            });

            for (int i = extractedDir.Count() - 1; i >= 0; i--)
            {
                DeleteDirectroy(extractedDir[i]);
            }
        }

        private void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                fi.Delete();
            }
        }

        private void DeleteDirectroy(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo di = new DirectoryInfo(dirPath);
                di.Delete(true);
            }
        }

        //private void GetArchivedFilesHasAudioFile(List<string> files)
        //{
        //    if (files == null)
        //        return;

        //    this.archivedFiles = files;
        //    total = this.archivedFiles.Count;

        //    if (this.OnStart != null)
        //        this.OnStart(this);

        //    for (int i = 0; i < archivedFiles.Count(); i++)
        //    {
        //        //압축전부 풀고 오디오 파일인지 체크하고 파일 삭제하는 부분
        //        ///////////////////////////////////                
        //        DirectoryInfo di = new DirectoryInfo(extractDir);
        //        ZipFile z = ZipFile.Read(archivedFiles[i]);
        //        foreach (ZipEntry entry in z.Entries)
        //        {
        //            if (!entry.IsDirectory)
        //            {
        //                entry.Extract(extractDir);
        //                string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");
        //                try
        //                {
        //                    TagLib.File f = TagLib.File.Create(extractedPath);
        //                    byte[] data = System.IO.File.ReadAllBytes(extractedPath);
        //                    string fileName = Path.GetFileName(extractedPath);
        //                    audioFiles.Add(new AudioFile(fileName, null, data, f.Tag));
        //                }
        //                catch (UnsupportedFormatException e)
        //                {
        //                    //System.IO.File.Delete(extractedPath);                            
        //                }
        //            }
        //        }
        //        z.Dispose();
        //        di.Delete(true);
        //        ///////////////////////////////////
        //        current = i + 1;

        //        int perc = (int)((float)current / (float)total * 100);

        //        if (progressControl != null)
        //            progressControl.FireProgress(perc);
        //    }            
        //}                
    }
}
