using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicFileManager
{
    /// <summary>
    /// MFMOption.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MFMOption : UserControl
    {
        public static DependencyProperty DeleteArchiveWithMulipleAudioProperty;
        public static DependencyProperty DeleteAudioWithOutFreqAndBitRateProperty;
        public static DependencyProperty AudioBitRateProperty;
        public static DependencyProperty AudioDurationProperty;

        public MFMOption()
        {
            InitializeComponent();
        }

        static MFMOption()
        {
            DeleteArchiveWithMulipleAudioProperty = DependencyProperty.Register("DeleteArchiveWithMulipleAudio", typeof(bool), typeof(MFMOption), new PropertyMetadata(false));
            DeleteAudioWithOutFreqAndBitRateProperty = DependencyProperty.Register("DeleteAudioWithOutFreqAndBitRate", typeof(bool), typeof(MFMOption), new PropertyMetadata(false));
            AudioBitRateProperty = DependencyProperty.Register("AudioBitRate", typeof(int), typeof(MFMOption), new PropertyMetadata(0));
            AudioDurationProperty = DependencyProperty.Register("AudioDuration", typeof(TimeSpan), typeof(MFMOption), new PropertyMetadata(new TimeSpan(0,0,0)));
        }

        void InitializeBinding()
        {
            Binding deleteAudioMultiArchive = new Binding("DeleteArchiveWithMulipleAudio");
            deleteAudioMultiArchive.Source = DeleteArchiveWithMulipleAudioProperty;
            deleteAudioMultiArchive.Mode = BindingMode.TwoWay;
        }

        public bool DeleteArchiveWithMulipleAudio
        {
            get
            {
                return (bool)this.GetValue(DeleteArchiveWithMulipleAudioProperty);
            }
            set
            {
                this.SetValue(DeleteArchiveWithMulipleAudioProperty, value);
            }
        }

        public bool DeleteAudioWithOutFreqAndBitRate
        {
            get
            {
                return (bool)this.GetValue(DeleteAudioWithOutFreqAndBitRateProperty);
            }
            set
            {
                this.SetValue(DeleteAudioWithOutFreqAndBitRateProperty, value);
            }
        }

        public int AudioBitRate
        {
            get
            {
                return (int)this.GetValue(AudioBitRateProperty);
            }
            set
            {
                this.SetValue(AudioBitRateProperty, value);
            }
        }

        public TimeSpan AudioDuration
        {
            get
            {
                return (TimeSpan)this.GetValue(AudioDurationProperty);
            }
            set
            {
                this.SetValue(AudioDurationProperty, value);
            }
        }
    }
}
