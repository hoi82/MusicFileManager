using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public sealed class ArchiveFileFinder : AbstractFileFinder<string, string>
    {
        public ArchiveFileFinder(IFileChecker fileChecker) : base(fileChecker) { }

        public ArchiveFileFinder(IFileChecker fileChecker, ProgressControl progressControl)
            : base(fileChecker, progressControl) { }

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {            
            ResetCount(allFiles.Count());

            for (int i = 0; i < allFiles.Count(); i++)
            {
                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                string sFile = allFiles[i];

                if (fileChecker.IsVaildFile(ref sFile))
                    mathcedFiles.Add(allFiles[i]);                

                IncCount();
                progressMessage = string.Format(MFMMessage.Message5, current, total);

                bw.ReportProgress(CalcPercentage());
            }            
        }
    }
}
