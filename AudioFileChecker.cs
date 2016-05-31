using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MusicFileManager
{
    public class AudioFileCheckerEndEventArgs
    {

    }

    public delegate void AudioFileCheckerStartEventHandler(object sender);
    public delegate void AudioFilecheckerEndEventHandler(object sender, AudioFileCheckerEndEventArgs e);

    public class AudioFileChecker
    {
        ProgressControl progressControl = null;
        AudioFileCheckerStartEventHandler OnStart = null;
        AudioFilecheckerEndEventHandler OnEnd = null;

        List<AudioFile> audioFilesInArchives = null;
        List<AudioFile> audioFiles = null;
        int current = 0;
        int total = 0;

        double nameSimilarity = 0;
        double dataSimilarity = 0;
        const double DEFALUT_NAME_SIMILARITY = 0.9;
        const double DEFAULT_DATA_SIMILARITY = 0.9;        

        public AudioFileChecker(ProgressControl progressControl) : 
            this(DEFALUT_NAME_SIMILARITY, DEFAULT_DATA_SIMILARITY, progressControl)
        {
            
        }

        public AudioFileChecker(double nameSimilarity, double dataSimilarity, ProgressControl progressControl)
        {
            this.nameSimilarity = nameSimilarity;
            this.dataSimilarity = dataSimilarity;
            this.progressControl = progressControl;
        }

        public void RunAsync(List<AudioFile> audioFilesInArchives, List<AudioFile> audioFiles)
        {
            this.audioFilesInArchives = audioFilesInArchives;
            this.audioFiles = audioFiles;
            total = audioFilesInArchives.Count() * audioFiles.Count();

            if (this.OnStart != null)
                this.OnStart(this);

            progressControl.InitializeDisplay();
            progressControl.SetEvents(DoWork, ProgressChanged, RunWorkerCompleted);
            progressControl.Run();
        }

        public void Cancel()
        {
            if (progressControl != null)
                progressControl.Cancel();
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressControl.ProgressDisplay(e.ProgressPercentage,
                string.Format("Comparing Audio Files in Archives with Actual Audio File ({0}/{1})...", current, total));
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (progressControl.Cancelled())
            {
                progressControl.ProgressDisplay(0, "Cancelled!");
            }
            else
            {
                progressControl.ProgressDisplay(100, "Completed!");
            }

            this.OnEnd(this, new AudioFileCheckerEndEventArgs());
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            Process();
        }

        private void Process()
        {
            for (int i = 0; i < audioFilesInArchives.Count; i++)
            {
                for (int j = 0; i < audioFiles.Count; j++)
                {
                    double sim = GetSimilarityFromEnd(audioFilesInArchives[i].Data, audioFiles[j].Data);

                    if (sim >= dataSimilarity)
                    {
                        //요기 해야됨.
                    }

                    current++;
                    int perc = (int)((float)current / (float)total * 100);

                    if (progressControl != null)
                        progressControl.FireProgress(perc);
                }
            }
        }

        private double GetSimilarityFromEnd(byte[] s, byte[] t)
        {
            int dist = 0;
            byte[] source = null;
            byte[] target = null;

            if (s.Length > t.Length)
            {
                source = t;
                target = s;
            }
            else
            {
                source = s;
                target = t;
            }

            int len = Math.Min(source.Length, target.Length);
            int lendiff = target.Length - source.Length;
            for (int i = len - 1; i > -1; i--)
            {
                if (target[i + lendiff] != source[i])
                    dist++;
            }

            return dist / source.Length;
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
