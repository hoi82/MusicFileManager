using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicFileManager.Duplication
{
    public class ArchiveDuplicationChecker : IDuplicationChcker
    {
        double dataSimilarity = 0.99;
        double maximumDataBufferRatio = 0.1;
        long minimumDataBuffer = 1024;

        public DuplicatedFiles CheckDuplication(string source, string target)
        {
            double sim = GetFileSimilarityWithByte(source, target);

            if (sim >= dataSimilarity)
                return new DuplicatedFiles(target, source, DuplicateType.AlreadyExtractedArchive);
            else
                return null;
        }

        double GetFileSimilarityWithByte(string file1, string file2)
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
    }
}
