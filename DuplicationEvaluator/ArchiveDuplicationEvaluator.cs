using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public class ArchiveDuplicationEvaluator : DisplayableWorker.DisplayableWorker, IDuplicationEvaluator
    {
        List<string> archivedFiles = null;
        List<string> audioFiles = null;
        List<DuplicatedFiles> duplicatedFiles = null;
        
        IFileChecker fileChecker = null;
        Extractor.IFileExtractor fileExtractor = null;
        IDuplicationEvaluatorOption option = null;
        IDuplicationChcker<string> duplicationChecker = null;

        public ArchiveDuplicationEvaluator(IFileChecker fileChecker, Extractor.IFileExtractor fileExtractor)
        {
            this.fileChecker = fileChecker;
            this.fileExtractor = fileExtractor; 
        }

        public List<DuplicatedFiles> GetDuplications(List<string> list)
        {
            throw new NotImplementedException();
        }

        public List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2)
        {
            archivedFiles = list1;
            audioFiles = list2;
            Process();
            return duplicatedFiles;
        }

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            duplicatedFiles = new List<DuplicatedFiles>();
            ResetCount(audioFiles.Count());

            for (int i = 0; i < archivedFiles.Count(); i++)
            {
                DuplicatedFiles d = null;

                total = archivedFiles.Count();
                current = i + 1;

                if ((e != null) && bw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                List<string> extractedAudioFiles = fileExtractor.ExtractMathcedFiles(archivedFiles[i], fileChecker);

                //여러개의 음악파일이 존재하고 옵션에서 처리 안하게 되어있으면 건너뛴다.
                if (option != null)
                {
                    //if ((extractedAudioFiles.Count > 1) && !option.DeleteArchiveWithMulipleAudio)
                    //{
                    //    audioExtractor.CleanExtractedFiles();
                    //    continue;
                    //}
                    if (option.SatifyOption(null))
                    {
                        fileExtractor.CleanExtractedFiles();
                        continue;
                    }
                }

                bool isDuplicated = false;
                string audioFileName = null;

                for (int j = 0; j < extractedAudioFiles.Count(); j++)
                {
                    if ((e != null) && bw.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    total = extractedAudioFiles.Count();
                    current = j + 1;                    

                    for (int k = 0; k < audioFiles.Count(); k++)
                    {
                        if ((e != null) && bw.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        total = audioFiles.Count();
                        current = k + 1;

                        d = duplicationChecker.CheckDuplication(extractedAudioFiles[j], audioFiles[k]);

                        if (d != null)
                        {
                            isDuplicated = true;
                            audioFileName = audioFiles[k];
                            break;
                        }                        

                        //IncCount();
                        progressMessage = string.Format(MFMMessage.Message8, j + 1, extractedAudioFiles.Count(), i + 1, archivedFiles.Count, current, total);
                        bw.ReportProgress(CalcPercentage());
                    }

                    if (isDuplicated)
                        break;

                    bw.ReportProgress(CalcPercentage());
                }

                if (isDuplicated)
                {                    
                    if (!duplicatedFiles.Contains(d))
                        duplicatedFiles.Add(d);
                }

                fileExtractor.CleanExtractedFiles();

                bw.ReportProgress(CalcPercentage());
            }            
        }

        protected override void OnEndProcedure()
        {
            throw new NotImplementedException();
        }

        protected override void OnStartProcedure()
        {
            throw new NotImplementedException();
        }
    }
}
