﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Duplication
{
    public class ArchiveDuplicationEvaluator : AbstractDuplicationEvaluator
    {                                       
        Extractor.IFileExtractor fileExtractor = null;                

        public ArchiveDuplicationEvaluator(IDuplicationEvaluatorOption option, IDuplicationChcker duplicationChecker, Extractor.IFileExtractor fileExtractor)
            : base(option, duplicationChecker)
        {                        
            this.fileExtractor = fileExtractor;
        }             

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            duplicatedFiles = new List<DuplicatedFiles>();
            //ResetCount(targetFiles.Count());

            for (int i = 0; i < sourceFiles.Count(); i++)
            {
                DuplicatedFiles d = null;

                outerTotal = sourceFiles.Count();
                outerCurrent = i;

                if ((e != null) && Canceled())
                {
                    e.Cancel = true;
                    break;
                }

                List<string> extractedAudioFiles = fileExtractor.ExtractMathcedFiles(sourceFiles[i]);

                //여러개의 음악파일이 존재하고 옵션에서 처리 안하게 되어있으면 건너뛴다.
                if (option != null)
                {                    
                    if (!option.SatifyOption(extractedAudioFiles.Count()))
                    {
                        fileExtractor.CleanExtractedFiles();
                        continue;
                    }
                }

                bool isDuplicated = false;
                string audioFileName = null;

                for (int j = 0; j < extractedAudioFiles.Count(); j++)
                {
                    if ((e != null) && Canceled())
                    {
                        e.Cancel = true;
                        break;
                    }

                    //outerTotal = extractedAudioFiles.Count();
                    //outerCurrent = j;

                    for (int k = 0; k < targetFiles.Count(); k++)
                    {
                        if ((e != null) && Canceled())
                        {
                            e.Cancel = true;
                            break;
                        }

                        total = targetFiles.Count();
                        current = k + 1;

                        d = duplicationChecker.CheckDuplication(extractedAudioFiles[j], targetFiles[k]);

                        if (d != null)
                        {
                            d.DuplicatedFile = sourceFiles[i];
                            isDuplicated = true;
                            audioFileName = targetFiles[k];
                            break;
                        }

                        //IncCount();
                        OnProcedure();
                    }

                    if (isDuplicated)
                        break;

                    OnProcedure();
                }

                if (isDuplicated)
                {
                    if (!duplicatedFiles.Contains(d))
                        duplicatedFiles.Add(d);
                }

                fileExtractor.CleanExtractedFiles();

                OnProcedure();
            }
        }


        public override List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync)
        {
            throw new NotImplementedException();
        }

        public override List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync)
        {
            sourceFiles = list1;
            targetFiles = list2;
            if (aSync)
            {
                working = true;
                OnStartProcedure();
                StartAsync();
                return null;
            }
            else
            {
                working = false;
                Process();
                return duplicatedFiles;
            }   
        }
    }
}
