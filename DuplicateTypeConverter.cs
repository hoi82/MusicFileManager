using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicFileManager
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class DuplicateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DuplicateType dtype = (DuplicateType)value;

            switch (dtype)
            {
                case DuplicateType.AlreadyExtractedArchive:
                    return MFMMessage.Message1;
                case DuplicateType.DuplicateAudioTag:
                    return MFMMessage.Message2;
                case DuplicateType.DuplicateAudioFileName:
                    return MFMMessage.Message3;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string typeMessage = (string)value;

            switch (typeMessage)
            {
                case MFMMessage.Message1:
                    return DuplicateType.AlreadyExtractedArchive;
                case MFMMessage.Message2:
                    return DuplicateType.DuplicateAudioTag;
                case MFMMessage.Message3:
                    return DuplicateType.DuplicateAudioFileName;
                default:
                    return DuplicateType.AlreadyExtractedArchive;
            }
        }
    }
}
