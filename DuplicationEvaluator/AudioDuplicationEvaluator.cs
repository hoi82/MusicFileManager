using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public class AudioDuplicationEvaluator : DisplayableWorker.DisplayableWorker, IDuplicationEvaluator
    {
        List<string> audioFiles = null;
        List<DuplicatedFiles> duplicatedFiles = null;
        IDuplicationEvaluatorOption option = null;
        IDuplicationChcker<string> checker = null;        

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            duplicatedFiles = new List<DuplicatedFiles>();
            ResetCount(audioFiles.Count());

            for (int i = 0; i < audioFiles.Count(); i++)
            {
                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                for (int j = 0; j < audioFiles.Count(); j++)
                {
                    if ((e != null) && bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    //옵션에서 처리 안하게 되어있거나 같은 파일일 경우 건너뛴다.
                    if (option != null)
                    {
                        //if ((i == j) | ((!option.DeleteAudioWithOutBitRate) && ((audioFiles[j].BitRate < option.AudioBitRate) | (audioFiles[j].Duration < audioFiles[j].Duration)))) continue;
                        if ((i == j) | (option.SatifyOption(null))) continue;
                    }                    

                    DuplicatedFiles d = checker.CheckDuplication(audioFiles[i], audioFiles[j]);

                    if ((d != null) && (!duplicatedFiles.Contains(d)))
                    {
                        duplicatedFiles.Add(d);
                    }

                    current = j + 1;

                    progressMessage = string.Format(MFMMessage.Message9, i + 1, total, current, total);

                    bw.ReportProgress(CalcPercentage());
                }

                current = i + 1;

                progressMessage = string.Format(MFMMessage.Message10, current, total);

                bw.ReportProgress(CalcPercentage());
            }            
        }

        protected override void OnEndProcedure()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnStartProcedure()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public List<DuplicatedFiles> GetDuplications(List<string> list)
        {
            audioFiles = list;
            Process();
            return duplicatedFiles;
        }

        public List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2)
        {
            throw new NotImplementedException();
        }
    }
}
