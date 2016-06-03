using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TagLib;

namespace MusicFileManager
{    
    //public delegate void AudioFileFinderStartEventHandler(object sender);
    //public delegate void AudioFileFinderOnCheckEventHandler(object sender, bool audioFile, string fileName, int currentCount, int totalCount);
    //public delegate void AudioFileFinderEndEventHandler(object sender);

    public class AudioFileFinder
    {        
        //List<string> allFiles = null;        

        string[] audioExtensions = { ".mp3", ".wav", ".flac", ".ogg" };
 
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
            string test = System.IO.Path.GetExtension(file);
            return audioExtensions.Contains(System.IO.Path.GetExtension(file));
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
