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
    /// <summary>
    /// 
    /// </summary>
    public class ArchivedAudioFileFinder
    {        
        string extractDir = null;
        const string EXTRACT_DIR = "Extracts";
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();

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
    }
}
