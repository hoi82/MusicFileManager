using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace MusicFileManager
{    
    //public delegate void AudioFileCheckerStartEventHandler(object sender);
    //public delegate void AudioFileCheckerCheckEventHandler(object sender, bool audioFile, string fileName, int currentCount, int totalCount);
    //public delegate void AudioFilecheckerEndEventHandler(object sender);

    public class AudioFileChecker
    {        
        //public event AudioFileCheckerStartEventHandler OnStart = null;
        //public event AudioFilecheckerEndEventHandler OnEnd = null;

        //List<string> archivedFileWithAudio = null;
        //List<string> audioFiles = null;        
        //int current = 0;
        //int total = 0;

        //List<SimilarAudioFiles> similarFiles = null;

        double nameSimilarity = 0;
        double dataSimilarity = 0;
        long minimumDataBuffer = 0;
        double maximumDataBufferRatio = 0;
        const double DEFALUT_NAME_SIMILARITY = 0.9;
        const double DEFAULT_DATA_SIMILARITY = 0.9;
        const long MINIMUM_DATA_BUFFER = 1024;
        const double MAXIMUM_DATA_BUFFER_RATIO = 0.1;

        public AudioFileChecker() : this(DEFALUT_NAME_SIMILARITY, DEFAULT_DATA_SIMILARITY, MINIMUM_DATA_BUFFER, MAXIMUM_DATA_BUFFER_RATIO) { }

        public AudioFileChecker(double nameSimilarity, double dataSimilarity, long minimumDataBuffer, double maximumDataBufferRatio)
        {
            this.nameSimilarity = nameSimilarity;
            this.dataSimilarity = dataSimilarity;
            this.minimumDataBuffer = minimumDataBuffer;
            this.maximumDataBufferRatio = maximumDataBufferRatio;
            //similarFiles = new List<SimilarAudioFiles>();
        }

        //public void RunAsync(List<string> archivedFileWithAudio, List<string> audioFiles)
        //{
        //    this.archivedFileWithAudio = archivedFileWithAudio;
        //    this.audioFiles = audioFiles;            

        //    if (this.OnStart != null)
        //        this.OnStart(this);
        //}                

        //private void CheckSimilarFileInArchive()
        //{
        //    total = archivedFileWithAudio.Count() * audioFiles.Count();

        //    for (int i = 0; i < archivedFileWithAudio.Count; i++)
        //    {                
        //        for (int k = 0; i < audioFiles.Count; k++)
        //        {

        //            double sim = 0;

        //            if (sim >= dataSimilarity)
        //            {
        //                similarFiles.Add(new SimilarAudioFiles(archivedFileWithAudio[i], audioFiles[k]));
        //            }

        //            current++;
        //            int perc = (int)((float)current / (float)total * 100);

        //            //if (progressControl != null)
        //            //    progressControl.FireProgress(perc);
        //        }
        //    }
        //}

        public bool CheckSimilarFilesByName(string file1, string file2)
        {
            string filteredFileName1 = RemoveUselessString(file1);
            string filteredFileName2 = RemoveUselessString(file2);
            return GetFileSimilarityWithName(filteredFileName1, filteredFileName2) >= nameSimilarity;
        }

        private double GetFileSimilarityWithName(string file1, string file2)
        {
            int len = Math.Max(file1.Length, file2.Length);
            return (len - GetDamerauLevDistance(file1, file2, len)) / len;
        }

        private string RemoveUselessString(string input)
        {
            string result = input;
            result = Regex.Replace(result, "^[0-9]+", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "^\\[.*\\]", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "^[^a-z0-9]+", "", RegexOptions.IgnoreCase);
            return result;
        }

        public bool CheckSimilarFilesByByte(string file1, string file2)
        {
            return GetFileSimilarityWithByte(file1, file2) >= dataSimilarity;
        }

        private double GetFileSimilarityWithByte(string file1, string file2)
        {
            FileStream sf = File.OpenRead(file1);
            FileStream tf = File.OpenRead(file2);
            FileStream temp = null;

            if (sf.Length > tf.Length)
            {
                temp = tf;
                tf = sf;
                sf = temp;
            }

            long pos = 0;
            long len = sf.Length;
            long dist = 0;

            bool loop = true;

            while (loop)
            {
                sf.Seek(-pos, SeekOrigin.End);
                tf.Seek(-pos, SeekOrigin.End);

                int sb = sf.ReadByte();
                int tb = tf.ReadByte();

                if (sb != tb)
                    dist++;

                pos++;

                //최소 범위 이상에서 정확도가 떨어지는 경우 루프를 탈출시킨다
                loop = pos < (len * maximumDataBufferRatio);
                
                double sim = (double)((pos - dist) / pos);
                if ((pos > minimumDataBuffer) && sim <= dataSimilarity)
                    loop = false;
            }        

            sf.Dispose();
            tf.Dispose();

            return (double)((len - dist) / len);
        }

        private int GetDamerauLevDistance(String s, String t, int max)
        {            
            int sLen = s.Length;
            int tLen = t.Length;

            if (sLen == 0)
                return tLen;

            if (tLen == 0)
                return sLen;

            if (sLen > tLen)
            {
                String temp = s;
                s = t;
                t = temp;

                sLen = tLen;
                tLen = t.Length;
            }

            if (max < 0)
                max = tLen;

            if (tLen - sLen > max)
                return max + 1;


            int[] _curRow = new int[sLen + 1];
            int[] _prevRow = new int[sLen + 1];
            int[] _transitRow = new int[sLen + 1];

            for (int i = 0; i <= sLen; i++)
                _prevRow[i] = i;

            char lastTargetChar = (char)0;

            for (int i = 1; i <= tLen; i++)
            {
                char targetChar = t[i - 1];
                _curRow[0] = i;

                int from = Math.Max(i - max - 1, 1);
                int to = Math.Min(i + max + 1, sLen);

                char lastSourceChar = (char)0;

                for (int j = from; j <= to; j++)
                {
                    char sourceChar = s[j - 1];

                    int cost = sourceChar == targetChar ? 0 : 1;
                    int val = Math.Min(Math.Min(_curRow[j - 1] + 1, _prevRow[j] + 1), _prevRow[j - 1] + cost);

                    if (sourceChar == lastTargetChar && targetChar == lastSourceChar)
                        val = Math.Min(val, _transitRow[j - 2] + cost);

                    _curRow[j] = val;
                    lastSourceChar = sourceChar;
                }

                lastTargetChar = targetChar;

                int[] tempRow = _transitRow;
                _transitRow = _prevRow;
                _prevRow = _curRow;
                _curRow = tempRow;
            }

            return _prevRow[sLen];                       
        }        
    }
}
