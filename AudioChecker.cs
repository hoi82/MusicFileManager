using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{
    public class AudioChecker
    {
        private List<string> sourceList = null;
        private List<string> targetList = null;
        double nameSimilarity = 0;

        public AudioChecker()
        {
            
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

        public List<string> CheckSimilarName(List<string> l1, List<string> l2)
        {
            List<string> sList = new List<string>();

            if (l1.Count() > l2.Count)
            {
                sourceList = l2;
                targetList = l1;
            }
            else
            {
                sourceList = l1;
                targetList = l2;
            }

            foreach (string s in sourceList)
            {
                foreach (string t in targetList)
                {
                    double sim = GetDamerauLevDistance(s, t, -1) / Math.Min(s.Count(), t.Count());

                    if (sim >= nameSimilarity)
                        sList.Add(t);
                }
            }
            return sList;
        }
    }
}
