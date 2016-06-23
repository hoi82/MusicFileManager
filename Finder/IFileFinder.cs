using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public interface IFileFinder
    {
        List<string> GetMatchedFiles(string directory);
        List<string> GetMatchedFiles(List<string> files);
        void GetMatchedFilesAsync(string directory);
        void GetMatchedFilesAsync(List<string> files);
        void WaitAsync();

        event FileFinderStartEventHandler OnStartAsync;

        event FileFinderEndEventHandler OnEndAsync;
        void AddSerializedFinder(IFileFinder finder);
        
    }
}
