﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Duplication
{
    public class AudioDuplicationEvaluator : AbstractDuplicationEvaluator
    {       
        public AudioDuplicationEvaluator(IDuplicationEvaluatorOption option, IDuplicationChcker duplicationChecker)
            : base(option, duplicationChecker)
        {
            
        }

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            duplicatedFiles = new List<DuplicatedFiles>();
            ResetCount(sourceFiles.Count());

            for (int i = 0; i < sourceFiles.Count(); i++)
            {
                if ((e != null) && Canceled())
                {
                    e.Cancel = true;
                    break;
                }

                for (int j = 0; j < sourceFiles.Count(); j++)
                {
                    if ((e != null) && Canceled())
                    {
                        e.Cancel = true;
                        break;
                    }

                    //옵션에서 처리 안하게 되어있거나 같은 파일일 경우 건너뛴다.
                    if (option != null)
                    {                        
                        if ((i == j) | (!option.SatifyOption(sourceFiles[j]))) continue;
                    }                    

                    DuplicatedFiles d = duplicationChecker.CheckDuplication(sourceFiles[i], sourceFiles[j]);

                    if ((d != null) && (!duplicatedFiles.Contains(d)))
                    {
                        duplicatedFiles.Add(d);
                    }

                    current = j + 1;

                    OnProcedure();
                }

                current = i + 1;

                OnProcedure();
            }            
        }        

        public override List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync)
        {
            sourceFiles = list;            
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

        public override List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync)
        {
            throw new NotImplementedException();
        }
    }
}
