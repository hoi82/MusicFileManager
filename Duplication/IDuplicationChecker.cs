using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Duplication
{
    public interface IDuplicationChcker
    {
        DuplicatedFiles CheckDuplication(string source, string target);
    }
}
