using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SevenZip;

namespace MusicFileManager.Extractor
{
    public class FileExtractor : IFileExtractor
    {
        const string EXTRACT_DIR = @"Extracts";
        string extractPath = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();
        IFileChecker fileChecker = null;

        public FileExtractor()
        {

        }

        public FileExtractor(IFileChecker fileChecker)
        {
            this.fileChecker = fileChecker;
        }

        public void CleanExtractedFiles()
        {
            foreach (string s in extractedFiles)
            {
                FileInfo fi = new FileInfo(s);

                if (!IsFileLocked(fi))
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

        bool IsFileLocked(FileInfo fi)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = fi.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
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
                di.Delete();
            }
        }

        public List<string> ExtractMathcedFiles(string archiveFile, IFileChecker checker = null)
        {
            List<string> matchedFiles = new List<string>();

            SevenZipExtractor extractor = null;            
            try
            {                
                extractor = new SevenZipExtractor(archiveFile);

                foreach (ArchiveFileInfo afInfo in extractor.ArchiveFileData)
                {
                    string filePath = System.IO.Path.Combine(extractPath, afInfo.FileName);

                    if (afInfo.IsDirectory)
                    {
                        Directory.CreateDirectory(filePath);
                        if (!filePath.Equals(extractPath))
                            extractedDir.Add(filePath);
                    }
                    else
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            if (!Path.GetDirectoryName(filePath).Equals(extractPath))
                                extractedDir.Add(Path.GetDirectoryName(filePath));
                        }

                        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                        extractor.ExtractFile(afInfo.FileName, fs);
                        fs.Close();
                        fs.Dispose();
                        fs = null;

                        if (checker != null)
                            fileChecker = checker;

                        if (fileChecker.IsVaildFile(ref filePath))
                        {
                            matchedFiles.Add(filePath);
                            extractedFiles.Add(filePath);
                        }
                        else
                        {
                            DeleteFile(filePath);
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
                if (extractor != null)
                    extractor.Dispose();
            }
            
            return matchedFiles;
        }


        public void SetRootDirectory(string directory)
        {
            this.extractPath = directory;
        }


        public void SetFileChecker(IFileChecker checker)
        {
            this.fileChecker = checker;
        }
    }
}
