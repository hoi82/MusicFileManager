using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.DuplicationEvaluator
{
    public interface IDuplicationChcker<DataType>
    {
        DuplicatedFiles CheckDuplication(DataType source, DataType target);
    }
}
