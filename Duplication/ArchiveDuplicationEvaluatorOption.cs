using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager.Duplication
{
    public class ArchiveDuplicationEvaluatorOption : IDuplicationEvaluatorOption
    {
        MFMOption option = null;
        public ArchiveDuplicationEvaluatorOption(MFMOption option)
        {
            this.option = option;
        }
        public bool SatifyOption(object data)
        {
            if (data == null)
                return false;            

            int audioFileCount = Convert.ToInt32(data);

            if ((!option.DeleteArchiveWithMulipleAudio) && (audioFileCount > 1))
                return false;

            return true;
        }
    }
}
