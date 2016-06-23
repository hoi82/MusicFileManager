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
    /// <summary>
    /// 
    /// </summary>
    public class AudioFileComparer
    {        
        double nameSimilarity = 0;
        double dataSimilarity = 0;
        long minimumDataBuffer = 0;
        double maximumDataBufferRatio = 0;
        const double DEFALUT_NAME_SIMILARITY = 0.9;
        const double DEFAULT_DATA_SIMILARITY = 0.99;
        const long MINIMUM_DATA_BUFFER = 1024;
        const double MAXIMUM_DATA_BUFFER_RATIO = 0.1;

        public AudioFileComparer() : this(DEFALUT_NAME_SIMILARITY, DEFAULT_DATA_SIMILARITY, MINIMUM_DATA_BUFFER, MAXIMUM_DATA_BUFFER_RATIO) { }

        public AudioFileComparer(double nameSimilarity, double dataSimilarity, long minimumDataBuffer, double maximumDataBufferRatio)
        {
            this.nameSimilarity = nameSimilarity;
            this.dataSimilarity = dataSimilarity;
            this.minimumDataBuffer = minimumDataBuffer;
            this.maximumDataBufferRatio = maximumDataBufferRatio;            
        }        

        public bool CheckSimilarFilesByNameAndTag(string file1, string file2)
        {
            //하나만 틀려도 바로 넘어가게 연산자 | 로 설정
            if ((!File.Exists(file1)) | (!File.Exists(file2)))
                return false;

            string filteredFileName1 = RemoveUselessString(Path.GetFileNameWithoutExtension(file1));
            string filteredFileName2 = RemoveUselessString(Path.GetFileNameWithoutExtension(file2));
            //return GetFileSimilarityWithName(filteredFileName1, filteredFileName2) >= nameSimilarity;
            double nameDist = GetFileSimilarityWithName(filteredFileName1, filteredFileName2);
            double tagDist = GetFileSimilarityWithTag(file1, file2);
            bool isTrackSeries = IsTrackSeries(filteredFileName1, filteredFileName2, nameDist);
            return (((nameDist + tagDist) / 2) >= nameSimilarity) && (!isTrackSeries);
        }

        private double GetFileSimilarityWithName(string name1, string name2)
        {
            if ((name1 == name2) && (!string.IsNullOrEmpty(name1)))
                return 1.0;

            if ((name1 != name2) && (GetFileNameSimilarityFromStart(name1, name2) > 0.4))
            {
                return 0;
            }

            int len = Math.Max(name1.Length, name2.Length);
            return (double)(len - GetDamerauLevDistance(name1, name2, len)) / len;
        }

        private double GetFileNameSimilarityFromStart(string name1, string name2)
        {
            if ((name1 == name2) && (!string.IsNullOrEmpty(name1)))
                return 1.0;

            int maxLen = Math.Max(name1.Length, name2.Length);
            int minLen = Math.Min(name1.Length, name2.Length);
            int sCount = 0;

            for (int i = 0; i < minLen; i++)
            {
                if (name1[i] == name2[i])
                {
                    sCount++;
                }
                else
                {
                    break;
                }
            }

            return (double)sCount / maxLen;
        }

        private string RemoveUselessString(string input)
        {
            string result = input;
            result = Regex.Replace(result, "^[0-9]+", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, "^[\\[\\(].*[\\]\\)]", "", RegexOptions.IgnoreCase);            
            result = Regex.Replace(result, "[^a-z0-9]+", "", RegexOptions.IgnoreCase);
            return result;
        }

        private bool IsTrackSeries(string name1, string name2, double nameSimilarity)
        {
            Regex regex = new Regex(@"[0-9]+$", RegexOptions.IgnoreCase);
            string track1 = regex.Match(name1).Value;
            string track2 = regex.Match(name2).Value;

            if (string.IsNullOrEmpty(track1) || string.IsNullOrEmpty(track2))
            {
                return false;
            }            

            return nameSimilarity >= this.nameSimilarity;
        }

        public bool CheckSimilarFilesByByte(string file1, string file2)
        {
            double sim = GetFileSimilarityWithByte(file1, file2);

            if (sim >= dataSimilarity)
                return true;
            else
                return false;
            //return GetFileSimilarityWithByte(file1, file2) >= dataSimilarity;
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
                loop = (pos < (len * maximumDataBufferRatio));
                
                double sim = (double)(pos - dist) / (double)pos;
                if ((pos > minimumDataBuffer) && (sim <= dataSimilarity))
                    loop = false;
            }        

            sf.Dispose();
            tf.Dispose();

            return (double)(pos - dist) / pos;
        }

        private double GetFileSimilarityWithTag(string file1, string file2)
        {
            try
            {
                TagLib.Tag t1 = TagLib.File.Create(file1).Tag;
                TagLib.Tag t2 = TagLib.File.Create(file2).Tag;
                
                int sPoint = 0;
                int tPoint = 0;

                if ((!string.IsNullOrEmpty(t1.Title)) && (!string.IsNullOrEmpty(t2.Title)))
                {
                    if (t1.Title == t2.Title)
                    {
                        sPoint++;
                    }

                    tPoint++;
                }

                if ((!string.IsNullOrEmpty(t1.FirstPerformer)) && (!string.IsNullOrEmpty(t2.FirstPerformer)))
                {
                    if (t1.FirstPerformer == t2.FirstPerformer)
                    {
                        sPoint++;
                    }

                    tPoint++;
                }

                if ((!string.IsNullOrEmpty(t1.Album)) && (!string.IsNullOrEmpty(t2.Album)))
                {
                    if (t1.Album == t2.Album)
                    {
                        sPoint++;
                    }

                    tPoint++;
                }                

                if (tPoint == 0)
                    return 0;

                return (double)sPoint / tPoint;
            }
            catch (Exception)
            {
                return 0;
            }
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
