using System;
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
        const int HEADER_BUFFER = 16;

        const string MP3_HEADER = "ID3";
        const string FLAC_HEADER = "fLaC";
        const string WAV_HEADER = "WAVE";        
        const string OGG_HEADER = "OggS";        
 
        //int current = 0;
        //int total = 0;

        //public event AudioFileFinderStartEventHandler OnStart;
        //public event AudioFileFinderOnCheckEventHandler OnCheck;
        //public event AudioFileFinderEndEventHandler OnEnd;

        public AudioFileFinder()
        {

        }

        public bool CheckAudioFile(string file)
        {
            return CheckByFileHeader(file);
        }

        private bool CheckByExtension(string file)
        {
            string ext = System.IO.Path.GetExtension(file);
            return audioExtensions.Contains(ext);
        }

        private bool CheckByFileHeader(string file)
        {
            if (!System.IO.File.Exists(file))
                return false;

            FileStream fs = new FileStream(file, FileMode.Open);
            byte[] header = new byte[HEADER_BUFFER];
            fs.Read(header, 0, header.Length);
            fs.Close();
            return IsMP3Header(header) || IsFLACHeader(header) || IsWAVEHeader(header) || IsOGGHeader(header);             
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
