using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MusicFileManager
{
    public sealed class FileFinder : AbstractFileFinder
    {
        public FileFinder(IFileChecker fileChecker) : base(fileChecker) { }

        public FileFinder(IFileChecker fileChecker, ProgressControl progressControl)
            : base(fileChecker, progressControl) { }

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            if (allFiles == null)
                return;

            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string sFile = allFiles[i];

                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (fileChecker.IsVaildFile(ref sFile, true))
                {
                    mathcedFiles.Add(sFile);
                }

                IncCount();
                progressMessage = string.Format(MFMMessage.Message6, current, total);

                bw.ReportProgress(CalcPercentage());                
            }
        } 
    }
}
