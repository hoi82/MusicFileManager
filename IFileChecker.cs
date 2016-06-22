using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public interface IFileChecker
    {
        bool IsVaildFile(ref string fileName, bool fixExtensionIfVaild = false);        
    }
}
