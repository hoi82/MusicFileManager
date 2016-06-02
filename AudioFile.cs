using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TagLib;

namespace MusicFileManager
{
    public class AudioFile
    {
        string name = null;
        string filePath = null;
        byte[] data = null;
        Tag tag = null;

        public AudioFile(string name, string filePath, Tag tag) : this(name, filePath, null, tag)
        {

        }

        public AudioFile(string name, string filePath, byte[] data, Tag tag)
        {
            this.name = name;
            this.filePath = filePath;
            this.data = data;
            this.tag = tag;
        }

        public string Name { get { return this.name; } }
        public string FilePath { get { return this.filePath; } }
        public byte[] Data
        {
            get
            {
                //if (data == null)
                //{
                //    return System.IO.File.ReadAllBytes(filePath);
                //}
                //else
                //{
                //    return data;                    
                //}
                return data;
            }
        }
        public Tag Tag { get { return this.tag; } }
    }
}
