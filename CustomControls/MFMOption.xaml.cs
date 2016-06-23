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
        public static DependencyProperty DeleteAudioWithOutBitRateProperty;
        public static DependencyProperty AudioBitRateProperty;
        public static DependencyProperty AudioDurationProperty;

        public MFMOption()
        {
            InitializeComponent();
        }

        static MFMOption()
        {
            DeleteArchiveWithMulipleAudioProperty = DependencyProperty.Register("DeleteArchiveWithMulipleAudio", typeof(bool), typeof(MFMOption), new PropertyMetadata(false));
            DeleteAudioWithOutBitRateProperty = DependencyProperty.Register("DeleteAudioWithOutBitRate", typeof(bool), typeof(MFMOption), new PropertyMetadata(false));
            AudioBitRateProperty = DependencyProperty.Register("AudioBitRate", typeof(int), typeof(MFMOption), new PropertyMetadata(0));
            AudioDurationProperty = DependencyProperty.Register("AudioDuration", typeof(TimeSpan), typeof(MFMOption), new PropertyMetadata(new TimeSpan(0,0,0)));
        }

        //get 부분 - return Dispatcher.Invoke((()=>));
        //set 부분 - Dispatcher.BeginInvoke((Action)(()=>));
        public bool DeleteArchiveWithMulipleAudio
        {
            get
            {
                return Dispatcher.Invoke((()=> (bool)this.GetValue(DeleteArchiveWithMulipleAudioProperty)));                
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(DeleteArchiveWithMulipleAudioProperty, value)));                
            }
        }

        public bool DeleteAudioWithOutBitRate
        {
            get
            {
                return Dispatcher.Invoke((() => (bool)this.GetValue(DeleteAudioWithOutBitRateProperty)));                
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(DeleteAudioWithOutBitRateProperty, value)));                
            }
        }

        public int AudioBitRate
        {
            get
            {
                return Dispatcher.Invoke((() => (int)this.GetValue(AudioBitRateProperty)));                
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(AudioBitRateProperty, value)));                
            }
        }

        public TimeSpan AudioDuration
        {
            get
            {
                return Dispatcher.Invoke((() => (TimeSpan)this.GetValue(AudioDurationProperty)));                
            }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => this.SetValue(AudioDurationProperty, value)));                
            }
        }
    }
}
