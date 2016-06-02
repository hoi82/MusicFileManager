using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Ionic.Zip;

namespace MusicFileManager
{
    public class AudioFileCheckerEndEventArgs
    {
        bool cancel = false;
        List<SimilarAudioFiles> similarFiles = null;

        public AudioFileCheckerEndEventArgs(bool cancel, List<SimilarAudioFiles> similarFiles)
        {
            this.cancel = cancel;
            this.similarFiles = similarFiles;
        }

        public bool Cancel { get { return this.cancel; } }
        public List<SimilarAudioFiles> SimilarFiles { get { return this.similarFiles; } }
    }

    public delegate void AudioFileCheckerStartEventHandler(object sender);
    public delegate void AudioFilecheckerEndEventHandler(object sender, AudioFileCheckerEndEventArgs e);

    public class AudioFileChecker
    {
        ProgressControl progressControl = null;
        public event AudioFileCheckerStartEventHandler OnStart = null;
        public event AudioFilecheckerEndEventHandler OnEnd = null;

        List<string> archivedFileWithAudio = null;
        List<string> audioFiles = null;        
        int current = 0;
        int total = 0;

        List<SimilarAudioFiles> similarFiles = null;

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
            similarFiles = new List<SimilarAudioFiles>();
        }

        public void RunAsync(List<string> archivedFileWithAudio, List<string> audioFiles)
        {
            this.archivedFileWithAudio = archivedFileWithAudio;
            this.audioFiles = audioFiles;            

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

            this.OnEnd(this, new AudioFileCheckerEndEventArgs(progressControl.Cancelled(), similarFiles));
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            CheckSimilarFileInArchive();
        }

        private List<string> GetaudioFilesInArchivedFile(string archivedFile)
        {
            List<string> audioFile = new List<string>();            

            return audioFiles;
        }

        private void CheckSimilarFileInArchive()
        {
            total = archivedFileWithAudio.Count() * audioFiles.Count();

            for (int i = 0; i < archivedFileWithAudio.Count; i++)
            {                
                for (int k = 0; i < audioFiles.Count; k++)
                {

                    double sim = 0;

                    if (sim >= dataSimilarity)
                    {
                        similarFiles.Add(new SimilarAudioFiles(archivedFileWithAudio[i], audioFiles[k]));
                    }

                    current++;
                    int perc = (int)((float)current / (float)total * 100);

                    if (progressControl != null)
                        progressControl.FireProgress(perc);
                }
            }
        }

        public double GetFileSimilarityWithByte(string file1, string file2)
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
            while (pos < len)
            {
                sf.Seek(-pos, SeekOrigin.End);
                tf.Seek(-pos, SeekOrigin.End);

                int sb = sf.ReadByte();
                int tb = tf.ReadByte();

                if (sb != tb)
                    dist++;

                pos++;
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
