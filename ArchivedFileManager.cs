using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;

namespace MusicFileManager
{
    public class ArchivedFileManager
    {
        const string EXTRACT_DIR = "Extracts";
        string extractDir = null;
        List<string> extractedFiles = null;
        AudioFileFinder audioFinder = null;

        public List<string> ExtractedFiles { get { return this.extractedFiles; } }

        public ArchivedFileManager(AudioFileFinder audioFinder)
        {
            extractedFiles = new List<string>();
            extractDir = System.AppDomain.CurrentDomain.BaseDirectory + EXTRACT_DIR;
            this.audioFinder = audioFinder;
        }

        public List<string> ExtractAudioFilesArchivedFile(string fileName)
        {
            List<string> extractedAudioFiles = new List<string>();

            ZipFile z = ZipFile.Read(fileName);
            foreach (ZipEntry entry in z.Entries)
            {
                if (!entry.IsDirectory)
                {
                    entry.Extract(extractDir);
                    string extractedPath = extractDir + @"\" + entry.FileName;//.Replace("/",@"\");

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
            }

            z.Dispose();
            return extractedAudioFiles;
        }

        public void CleanExtractedFiles()
        {
            foreach (string s in extractedFiles)
            {
                DeleteFile(s);
            }
        }

        private void DeleteFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }
    }
}
