using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public class AudioFile
    {
        string fileName;
        int bitRate;
        TimeSpan duration;

        public AudioFile(string fileName, int bitRate, TimeSpan duration)
        {
            this.fileName = fileName;
            this.bitRate = bitRate;
            this.duration = duration;
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        public int BitRate
        {
            get
            {
                return this.bitRate;
            }
            set
            {
                this.bitRate = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
            }
        }
    }
}
