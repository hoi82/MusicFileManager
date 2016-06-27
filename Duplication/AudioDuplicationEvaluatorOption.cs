using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace MusicFileManager.Duplication
{
    public class AudioDuplicationEvaluatorOption : IDuplicationEvaluatorOption
    {
        MFMOption option = null;

        public AudioDuplicationEvaluatorOption(MFMOption option)
        {
            this.option = option;
        }

        public bool SatifyOption(object data)
        {
            TagLib.File tf = null;
            try
            {
                tf = TagLib.File.Create(data.ToString());
                if ((!option.DeleteAudioWithOutBitRate) && ((tf.Properties.AudioBitrate < option.AudioBitRate) | (tf.Properties.Duration < option.AudioDuration)))
                    return false;
            }
            catch (Exception)
            {
                return false;                
            }            

            return true;
        }
    }
}
