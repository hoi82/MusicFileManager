using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public sealed class AudioFileFinder : AbstractFileFinder<string, AudioFile>
    {
        public AudioFileFinder(IFileChecker fileChecker) : base(fileChecker) { }

        public AudioFileFinder(IFileChecker fileChecker, ProgressControl progressControl)
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
                    TagLib.File f = TagLib.File.Create(sFile);
                    mathcedFiles.Add(new AudioFile(sFile, f.Properties.AudioBitrate, f.Properties.Duration));
                }

                IncCount();
                progressMessage = string.Format(MFMMessage.Message6, current, total);

                bw.ReportProgress(CalcPercentage());
            }            
        }       
    }
}
