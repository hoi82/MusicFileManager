using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public enum DuplicateType { AlreadyExtractedArchive, DuplicateAudioTag, DuplicateAudioFileName }

    public class DuplicatedFiles
    {
        string originalFile = null;
        string duplicatedFile = null;
        DuplicateType similarType;

        public DuplicatedFiles(string originalFile, string duplicatedFile, DuplicateType similarType)
        {
            this.originalFile = originalFile;
            this.duplicatedFile = duplicatedFile;
            this.similarType = similarType;            
        }

        public override bool Equals(object obj)
        {
            if (obj is DuplicatedFiles)
            {
                DuplicatedFiles d = obj as DuplicatedFiles;

                if ((this.originalFile == d.OriginalFile) | (this.originalFile == d.DuplicatedFile))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }        

        public string OriginalFile { get { return this.originalFile; } }
        public string DuplicatedFile { get { return this.duplicatedFile; } set { this.duplicatedFile = value; } }
        public DuplicateType SimilarType { get { return this.similarType; } }        
    }
}
