using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicFileManager.Worker;

namespace MusicFileManager.Duplication
{
    public abstract class AbstractDuplicationEvaluator : DisplayableWorker, IDuplicationEvaluator
    {
        protected List<string> sourceFiles = null;
        protected List<string> targetFiles = null;
        protected List<DuplicatedFiles> duplicatedFiles = null;

        protected IDuplicationEvaluatorOption option = null;
        protected IDuplicationChcker duplicationChecker = null;

        protected int outerCurrent = 0;
        protected int outerTotal = 0;

        public AbstractDuplicationEvaluator(IDuplicationEvaluatorOption option, IDuplicationChcker duplicationChecker)
            : base()
        {
            this.option = option;
            this.duplicationChecker = duplicationChecker;
        }

        public event DuplicationEvaluatorStartEventHandler OnStartAsync;        
        public event DuplicationEvaluatorCompleteEventHandler OnCompleteAsync;
        public event DuplicationEvaluatorProgressEventHandler OnProgressAsync;
        public event DuplicationEvaluatorCancelEventHandler OnCancelAsync;

        public abstract List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync);

        public abstract List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync);        

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this, new EventArgs());
        }

        protected override void OnProcedure()
        {
            if (this.OnProgressAsync != null)
            {
                this.OnProgressAsync(this, new DuplicationEvaluatorProgressEventArgs(current, total, outerCurrent, outerTotal));
            }
        }

        protected override void OnCancelProcedure()
        {
            if (this.OnCancelAsync != null)
            {
                this.OnCancelAsync(this, new EventArgs());
            }
        }

        protected override void OnCompleteProcedure()
        {
            if (this.OnCompleteAsync != null)
                this.OnCompleteAsync(this, new DuplicationEvaluatorEndEventArgs(duplicatedFiles));
        }        
    }
}
