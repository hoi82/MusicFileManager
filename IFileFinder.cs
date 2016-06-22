using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public interface IFileFinder<InType, OutType>
    {
        List<OutType> GetMatchedFiles(string directory);
        List<OutType> GetMatchedFiles(List<InType> files);
        void GetMatchedFilesAsync(string directory);
        void GetMatchedFilesAsync(List<InType> files);
    }
}
