using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public class AudioDuplicationEvaluator : DisplayableWorker.DisplayableWorker, IDuplicationEvaluator<string, string>
    {
        List<string> audioFiles = null;
        List<string> archivedAudioFile = null;

        public List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2)
        {
            return null;            
        }

        protected override void Process(System.ComponentModel.DoWorkEventArgs e = null)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnEndProcedure()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnStartProcedure()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
