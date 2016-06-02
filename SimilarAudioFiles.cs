using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public class SimilarAudioFiles
    {
        string originalFile = null;
        string duplicatedFile = null;

        public SimilarAudioFiles(string originalFile, string duplicatedFile)
        {
            this.originalFile = originalFile;
            this.duplicatedFile = duplicatedFile;
        }

        public string OriginalFile { get { return this.originalFile; } }
        public string DuplicatedFile { get { return this.duplicatedFile; } }
    }
}
