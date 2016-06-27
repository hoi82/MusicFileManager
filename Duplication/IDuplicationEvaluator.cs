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

    public delegate void DuplicationEvaluatorStartEventHandler(object sender);
    public delegate void DuplicationEvaluatorEndEventHandler(object sender, DuplicationEvaluatorEndEventArgs e);

    public interface IDuplicationEvaluator
    {
        event DuplicationEvaluatorStartEventHandler OnStartAsync;
        event DuplicationEvaluatorEndEventHandler OnEndAsync;
        List<DuplicatedFiles> GetDuplications(List<string> list, bool aSync);        
        List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2, bool aSync);                
    }
}
