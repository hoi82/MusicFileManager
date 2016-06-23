using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public interface IDuplicationEvaluator<Type1, Type2>
    {
        List<DuplicatedFiles> GetDuplications(List<Type1> list1, List<Type2> list2);
    }
}
