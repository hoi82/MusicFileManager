using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Extractor
{
    public interface IFileExtractor
    {
        List<string> ExtractMathcedFiles(string archiveFile, IFileChecker checker = null);        
        void SetFileChecker(IFileChecker checker);
        void CleanExtractedFiles();
        void SetRootDirectory(string directory);
    }
}
