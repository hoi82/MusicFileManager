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
        protected List<DuplicatedFiles> duplicatedFiles = null;

        protected IDuplicationEvaluatorOption option = null;
        protected IDuplicationChcker duplicationChecker = null;

        public AbstractDuplicationEvaluator(ProgressControl progressControl, IDuplicationEvaluatorOption option, IDuplicationChcker duplicationChecker)
            : base(progressControl)
        {
            this.option = option;
            this.duplicationChecker = duplicationChecker;
        }

        public event DuplicationEvaluatorStartEventHandler OnStartAsync;

        public event DuplicationEvaluatorEndEventHandler OnEndAsync;

        protected override void OnEndProcedure()
        {
            if (this.OnEndAsync != null)
                this.OnEndAsync(this, new DuplicationEvaluatorEndEventArgs(duplicatedFiles));
        }

        protected override void OnStartProcedure()
        {
            if (this.OnStartAsync != null)
                this.OnStartAsync(this);
        }

        public abstract List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync);

        public abstract List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync);        
    }
}
