using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Duplication
{
    public class DuplicationEvaluatorEndEventArgs : EventArgs
    {
        List<DuplicatedFiles> duplicatedFiles = null;

        public DuplicationEvaluatorEndEventArgs(List<DuplicatedFiles> duplicatedFiles)
        {
            this.duplicatedFiles = duplicatedFiles;
        }

        public List<DuplicatedFiles> DuplicatedFiles { get { return this.duplicatedFiles; } }
    }

    public class DuplicationEvaluatorProgressEventArgs : EventArgs
    {
        int innerCurrent = 0;
        int innerTotal = 0;
        int outerCurrent = 0;
        int outerTotal = 0;

        public DuplicationEvaluatorProgressEventArgs(int innerCurrent, int innerTotal, int outerCurrent, int outerTotal)
        {
            this.innerCurrent = innerCurrent;
            this.innerTotal = innerTotal;
            this.outerCurrent = outerCurrent;
            this.outerTotal = outerTotal;
        }

        public int InnerCurrent { get { return this.innerCurrent; } }
        public int InnerTotal { get { return this.innerTotal; } }
        public int OuterCurrent { get { return this.outerCurrent; } }
        public int OuterTotal { get { return this.outerTotal; } }
    }

    public delegate void DuplicationEvaluatorStartEventHandler(object sender, EventArgs e);
    public delegate void DuplicationEvaluatorProgressEventHandler(object sender, DuplicationEvaluatorProgressEventArgs e);
    public delegate void DuplicationEvaluatorCancelEventHandler(object sender, EventArgs e);
    public delegate void DuplicationEvaluatorCompleteEventHandler(object sender, DuplicationEvaluatorEndEventArgs e);

    public interface IDuplicationEvaluator
    {
        event DuplicationEvaluatorStartEventHandler OnStartAsync;
        event DuplicationEvaluatorProgressEventHandler OnProgressAsync;
        event DuplicationEvaluatorCancelEventHandler OnCancelAsync;
        event DuplicationEvaluatorCompleteEventHandler OnCompleteAsync;        
        List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync);        
        List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync);
        void Cancel();
    }
}
