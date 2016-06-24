using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public interface IDuplicationEvaluator
    {
        List<DuplicatedFiles> GetDuplications(List<string> list);
        List<DuplicatedFiles> GetDuplications(List<string> list1, List<string> list2);
    }
}
