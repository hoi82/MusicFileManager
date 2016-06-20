﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TagLib;
using System.IO;

namespace MusicFileManager
{    
    //public delegate void AudioFileFinderStartEventHandler(object sender);
    //public delegate void AudioFileFinderOnCheckEventHandler(object sender, bool audioFile, string fileName, int currentCount, int totalCount);
    //public delegate void AudioFileFinderEndEventHandler(object sender);

    public class AudioFileFinder
    {        
        //List<string> allFiles = null;        

        string[] audioExtensions = { ".mp3", ".wav", ".flac", ".ogg" };

        //mp3 - ID3가 넘어온다.(3바이트)
        //flac - fLaC가 넘어온다. (4바이트)
        //wav - RIFF가 넘어온다. (4바이트) WAVE가 넘어온다(8바이트째부터 4바이트)
        //ogg - OggS가 넘어온다. (4바이트)
        //m4a - 중간에 ftypM4A가 들어있다.
        //wma - 앞쪽 16바이트가 항상 일정하다.
        const int HEADER_BUFFER = 16;

        const string MP3_HEADER = "ID3";
        const string FLAC_HEADER = "fLaC";
        const string WAV_HEADER = "WAVE";        
        const string OGG_HEADER = "OggS";
        const string M4A_HEADER = "ftypM4A";
        readonly byte[] WMA_HEADER = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };          
 
        //int current = 0;
        //int total = 0;

        //public event AudioFileFinderStartEventHandler OnStart;
        //public event AudioFileFinderOnCheckEventHandler OnCheck;
        //public event AudioFileFinderEndEventHandler OnEnd;

        public AudioFileFinder()
        {

        }

        public bool CheckAudioFile(ref string file, bool fixExtensionIfInvalid = false)
        {
            return CheckByFileHeader(ref file, fixExtensionIfInvalid);
        }

        private string ChangeFileNameAndExtension(string originalFilePath, string extension)
        {
            if (!System.IO.File.Exists(originalFilePath))
            {
                throw new FileNotFoundException();
            }

            string origianlext = Path.GetExtension(originalFilePath);

            if (origianlext.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                return originalFilePath;

            string newFilePath = Path.ChangeExtension(originalFilePath, extension);
            int count = 2;

            while (System.IO.File.Exists(newFilePath))
            {
                string oldFileName = Path.GetFileNameWithoutExtension(newFilePath);
                string directory = Path.GetDirectoryName(newFilePath);
                string ext = Path.GetExtension(newFilePath);
                string newFileName = oldFileName + string.Format(" ({0})", count);
                
                newFilePath = directory + "\\" + newFileName + ext;
                count++;
            }

            try
            {
                System.IO.File.Move(originalFilePath, newFilePath);
            }
            catch (Exception)
            {
                
                throw;
            } 
            return newFilePath;
        }

        private bool CheckByExtension(string file)
        {
            string ext = System.IO.Path.GetExtension(file);
            return audioExtensions.Contains(ext);
        }

        private bool CheckByFileHeader(ref string file, bool fixExtensionIfInvalid = false)
        {
            if (!System.IO.File.Exists(file))
                return false;

            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                byte[] header = new byte[HEADER_BUFFER];
                fs.Read(header, 0, header.Length);
                fs.Close();

                if (IsMP3Header(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".mp3");
                    return true;

                }

                if (IsFLACHeader(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".flac");
                    return true;
                }

                if (IsWAVEHeader(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".wav");
                    return true;
                }

                if (IsOGGHeader(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".ogg");
                    return true;
                }

                if (IsWAVEHeader(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".wav");
                    return true;
                }

                if (IsM4AHeader(header))
                {
                    if (fixExtensionIfInvalid)
                        file = ChangeFileNameAndExtension(file, ".m4a");
                    return true;
                }                
            }
            catch (Exception)
            {
                
            }
            return false;
            
        }

        private bool IsMP3Header(byte[] header)
        {
            string hstr = Encoding.ASCII.GetString(header);
            return hstr.StartsWith(MP3_HEADER);
        }

        private bool IsFLACHeader(byte[] header)
        {
            string hstr = Encoding.ASCII.GetString(header);
            return hstr.StartsWith(FLAC_HEADER);
        }

        private bool IsWAVEHeader(byte[] header)
        {
            string hstr = Encoding.ASCII.GetString(header);
            hstr = hstr.Substring(8, 4);
            return hstr.StartsWith(WAV_HEADER);
        }

        private bool IsOGGHeader(byte[] header)
        {
            string hstr = Encoding.ASCII.GetString(header);
            return hstr.StartsWith(OGG_HEADER);
        }

        private bool IsM4AHeader(byte[] header)
        {
            string hstr = Encoding.ASCII.GetString(header);
            return hstr.Contains(M4A_HEADER);
        }

        private bool IsWMAHeader(byte[] header)
        {
            byte[] frontheader = new byte[16];
            Array.Copy(header, frontheader, 16);
            return frontheader.SequenceEqual(WMA_HEADER);
        }

        //public void FindAudioFiles(List<string> files)
        //{
        //    if (files == null)
        //        return;            
            
        //    allFiles = files;
        //    total = files.Count;

        //    if (this.OnStart != null)
        //        this.OnStart(this);

        //    for (int i = 0; i < allFiles.Count(); i++)
        //    {                                
        //        bool isAudioFile = audioExtensions.Contains(System.IO.Path.GetExtension(allFiles[i]));

        //        current = i + 1;

        //        int perc = (int)((float)current / (float)total * 100);

        //        if (this.OnCheck != null)
        //        {
        //            this.OnCheck(this, isAudioFile, allFiles[i], current, total);
        //        }                
        //    }

        //    if (this.OnEnd != null)
        //        this.OnEnd(this);
        //}                       
    }
}
