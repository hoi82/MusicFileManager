using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public enum DuplicateType { AlreadyExtractedArchive, DuplicateAudioTag, DuplicateAudioFileName }

    public class DuplicatedAudioFiles
    {
        string originalFile = null;
        string duplicatedFile = null;
        DuplicateType similarType;

        public DuplicatedAudioFiles(string originalFile, string duplicatedFile, DuplicateType similarType)
        {
            this.originalFile = originalFile;
            this.duplicatedFile = duplicatedFile;
            this.similarType = similarType;
        }

        public override bool Equals(object obj)
        {
            if (obj is DuplicatedAudioFiles)
            {
                DuplicatedAudioFiles d = obj as DuplicatedAudioFiles;

                if ((this.originalFile == d.OriginalFile) | (this.originalFile == d.DuplicatedFile))
                    return true;
            }
            return false;
        }
        public string OriginalFile { get { return this.originalFile; } }
        public string DuplicatedFile { get { return this.duplicatedFile; } }
        public DuplicateType SimilarType { get { return this.similarType; } }        
    }
}
