using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicFileManager.Worker;

namespace MusicFileManager.Cleaner
{
    public interface IFileCleaner
    {
        event FileCleanerStartEventHandler OnStartAsync;
        event FileCleanerEndEventHandler OnEndAsync;
        void CleanFiles(bool aSync, List<string> files = null);
        void CancelCleanAsync();
    }
}
