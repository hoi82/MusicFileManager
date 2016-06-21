﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace MusicFileManager
{
    public class ArchivedFileManager
    {
        const string EXTRACT_DIR = "Extracts";
        string extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
        List<string> extractedFiles = new List<string>();
        List<string> extractedDir = new List<string>();
        AudioFileFinder audioFinder = null;        

        public ArchivedFileManager(AudioFileFinder audioFinder)
        {                        
            this.audioFinder = audioFinder;
        }

        

        public List<string> ExtractAudioFilesArchivedFile(string fileName)
        {
            List<string> extractedAudioFiles = new List<string>();
            try
            {
                ZipFile z = ZipFile.Read(fileName);
                foreach (ZipEntry entry in z.Entries)
                {
                    string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");

                    if (entry.IsDirectory)
                    {
                        extractedDir.Add(extractedPath);
                        continue;
                    }                        

                    entry.Extract(extractDir);                    

                    if (audioFinder.CheckAudioFile(ref extractedPath))
                    {
                        extractedAudioFiles.Add(extractedPath);
                        extractedFiles.Add(extractedPath);
                    }
                    else
                    {
                        DeleteFile(extractedPath);
                    }                    
                }

                z.Dispose();
            }
            catch (Exception)
            {
                                
            }            
            return extractedAudioFiles;
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
                di.Delete();
            }
        }
    }
}
