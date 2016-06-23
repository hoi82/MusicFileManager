using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace MusicFileManager.Extractor
{
    public class FileExtractor : IFileExtractor
    {
        const string EXTRACT_DIR = "Extracts";
        string extractPath = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();                                   

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
                di.Delete();
            }
        }

        public List<string> ExtractMathcedFiles(string archiveFile, IFileChecker checker)
        {
            List<string> matchedFiles = new List<string>();
            ZipFile z = null;
            try
            {
                z = ZipFile.Read(archiveFile);
                foreach (ZipEntry entry in z.Entries)
                {
                    string extractedPath = extractPath + @"\" + entry.FileName;//.Replace("/",@"\");

                    if (entry.IsDirectory)
                    {
                        extractedDir.Add(extractedPath);
                        continue;
                    }

                    entry.Extract(extractPath);

                    if (checker.IsVaildFile(ref extractedPath))
                    {
                        matchedFiles.Add(extractedPath);
                        extractedFiles.Add(extractedPath);
                    }
                    else
                    {
                        DeleteFile(extractedPath);
                    }
                }                
            }
            catch (Exception)
            {
                
            }
            finally
            {
                if (z != null)
                    z.Dispose();
            }
            return matchedFiles;
        }


        public void SetRootDirectory(string directory)
        {
            this.extractPath = directory;
        }
    }
}
