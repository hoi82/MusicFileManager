using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Ionic.Zip;

namespace MusicFileManager
{    
    //public delegate void ArchivedFileFinderStartEventHandler(object sender);
    //public delegate void ArchivedFileFinderCheckEventHandler(object sender, bool audioFile, string fileName, int currentCount, int totalCount);
    //public delegate void ArchivedfileFinderEndEventHandler(object sender);

    public class ArchivedFileFinder
    {        
        //List<string> allFiles = null;        
        //int current = 0;
        //int total = 0;

        //public event ArchivedFileFinderStartEventHandler OnStart;
        //public event ArchivedFileFinderCheckEventHandler OnCheck;
        //public event ArchivedfileFinderEndEventHandler OnEnd;

        public ArchivedFileFinder()
        {

        }

        public bool CheckArchivedFile(string file)
        {
            if (!System.IO.File.Exists(file))
                return false;
            return ZipFile.IsZipFile(file);
        }

        //public void FindArchivedFiles(List<string> files)
        //{
        //    if (files == null)
        //        return;            

        //    allFiles = files;
        //    total = files.Count;

        //    if (this.OnStart != null)
        //        this.OnStart(this);

        //    for (int i = 0; i < allFiles.Count(); i++)
        //    {
        //        bool IsArchivedFile = ZipFile.IsZipFile(allFiles[i]);
                
        //        current = i + 1;

        //        int perc = (int)((float)current / (float)total * 100);

        //        if (this.OnCheck != null)
        //        {
        //            this.OnCheck(this, IsArchivedFile, allFiles[i], current, total);
        //        }

        //    }

        //    if (this.OnEnd != null)
        //        this.OnEnd(this);
        //}        
    }
}
