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
    public sealed class ArchivedAudioFileFinder : AbstractFileFinder
    {     
        public ArchivedAudioFileFinder(IFileChecker fileChecker) : base(fileChecker) 
        {
            extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
        }

        public ArchivedAudioFileFinder(IFileChecker fileChecker, ProgressControl progressControl)
            : this(fileChecker) 
        {
            this.progressControl = progressControl;
        }

        string extractDir = null;
        const string EXTRACT_DIR = "Extracts";
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();          

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
                    
                    if (fileChecker.IsVaildFile(ref extractedPath))
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

        void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                fi.Delete();
            }
        }

        void DeleteDirectroy(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo di = new DirectoryInfo(dirPath);
                di.Delete(true);
            }
        }

        protected override void Process(DoWorkEventArgs e = null)
        {
            if (allFiles == null)
                return;

            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (CheckArchivedAudioFile(allFiles[i]))
                    mathcedFiles.Add(allFiles[i]);

                IncCount();
                progressMessage = string.Format(MFMMessage.Message7, current, total);

                bw.ReportProgress(CalcPercentage());
            }            
        }
    }
}
