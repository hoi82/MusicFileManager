using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicFileManager.Checker
{
    public class DefaultFileChecker : IFileChecker
    {
        public bool IsVaildFile(ref string fileName, bool fixExtensionIfVaild = false)
        {
            if (!File.Exists(fileName))

                return false;
            FileAttributes attr = File.GetAttributes(fileName);
            return !attr.HasFlag(FileAttributes.Directory);
        }
    }
}
