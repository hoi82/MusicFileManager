using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SevenZip;

namespace MusicFileManager.Checker
{
    public class ArchivedAudioFileChecker : IFileChecker
    {
        string extractDir = null;
        const string EXTRACT_DIR = "Extracts";
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();

        IFileChecker audioCheker = null;
        
        public ArchivedAudioFileChecker(IFileChecker audioChecker)
        {
            extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
            this.audioCheker = audioChecker;
        }

        public bool IsVaildFile(ref string fileName, bool fixExtensionIfVaild = false)
        {
            if (!System.IO.File.Exists(fileName))
                return false;

            bool IsAudio = false;

            SevenZipExtractor extractor = null;            
            try
            {                                              
                extractor = new SevenZipExtractor(fileName);
                foreach (ArchiveFileInfo afInfo in extractor.ArchiveFileData)
                {
                    string filePath = System.IO.Path.Combine(extractDir, afInfo.FileName);

                    if (afInfo.IsDirectory)
                    {
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        if (!filePath.Equals(extractDir))
                            extractedDir.Add(filePath);
                    }
                    else
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            if (!Path.GetDirectoryName(filePath).Equals(extractDir))                                
                                extractedDir.Add(Path.GetDirectoryName(filePath));
                        }                            

                        extractedFiles.Add(filePath);

                        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                        extractor.ExtractFile(afInfo.FileName, fs);
                        fs.Close();                        

                        if (audioCheker.IsVaildFile(ref filePath))
                        {
                            IsAudio = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {               
                CleanExtractedFiles();
                if (extractor != null)
                    extractor.Dispose();             
            }

            //try
            //{
            //    SevenZip.SevenZipExtractor z = new SevenZipExtractor(fileName);
            //    foreach (string entry in z.ArchiveFileNames)
            //    {
            //        string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");

            //        if (entry.IsDirectory)
            //        {
            //            extractedDir.Add(extractedPath);
            //            continue;
            //        }
            //        else
            //        {
            //            extractedFiles.Add(extractedPath);
            //        }

            //        entry.Extract(extractDir, ExtractExistingFileAction.OverwriteSilently);

            //        if (audioCheker.IsVaildFile(ref extractedPath))
            //        {
            //            IsAudio = true;
            //            break;
            //        }
            //    }
            //    z.Dispose();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //finally
            //{
            //    CleanExtractedFiles();
            //}

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
    }
}
