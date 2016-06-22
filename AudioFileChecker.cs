using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TagLib;
using System.IO;

namespace MusicFileManager
{    
    //public delegate void AudioFileFinderStartEventHandler(object sender);
    //public delegate void AudioFileFinderOnCheckEventHandler(object sender, bool audioFile, string fileName, int currentCount, int totalCount);
    //public delegate void AudioFileFinderEndEventHandler(object sender);

    public sealed class AudioFileChecker : AbstractFileChecker
    {        
        //List<string> allFiles = null;        

        string[] audioExtensions = { ".mp3", ".wav", ".flac", ".ogg" };

        //mp3 - ID3가 넘어온다.(3바이트)
        //flac - fLaC가 넘어온다. (4바이트)
        //wav - RIFF가 넘어온다. (4바이트) WAVE가 넘어온다(8바이트째부터 4바이트)
        //ogg - OggS가 넘어온다. (4바이트)
        //m4a - 중간에 ftypM4A가 들어있다.
        //wma - 앞쪽 16바이트가 항상 일정하다.
        const int HEADER_BUFFER = 16;

        const string MP3_HEADER = "ID3";
        const string FLAC_HEADER = "fLaC";
        const string WAV_HEADER = "WAVE";        
        const string OGG_HEADER = "OggS";
        const string M4A_HEADER = "ftypM4A";
        readonly byte[] WMA_HEADER = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };          

        public AudioFileChecker() : base()
        {
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 73, 68, 51 }, ".mp3"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 102, 76, 97, 67, 0, 0, 0, 34 }, ".flac"));
            //웨이브 파일은 중간 부분이 파일마다 다르므로 이 부분 처리해야함.
            //signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 73, 68, 51 }, ".wav"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 79, 103, 103, 83, 0, 2, 0, 0 }, ".ogg"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 0, 0, 0, 102, 116, 121, 112, 77, 52, 65 }, ".m4a"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 }, ".wma"));
        }                                 
    }
}
