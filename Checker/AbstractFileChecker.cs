using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicFileManager
{
    public abstract class AbstractFileChecker : IFileChecker
    {
        protected List<FileSignature> signtures = new List<FileSignature>();
        public bool IsVaildFile(ref string file, bool fixExtensionIfVaild = false)
        {
            if (!System.IO.File.Exists(file))
                return false;

            FileStream fs = null;
            bool isValid = false;
            string ext = null;

            try
            {
                fs = new FileStream(file, FileMode.Open);

                for (int i = 0; i < signtures.Count(); i++)
                {
                    FileSignature f = signtures[i];
                    byte[] sig = new byte[f.SignatureLength];

                    if (f.SignatureType == FileSignatureType.Header)
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                    }
                    else if (f.SignatureType == FileSignatureType.Tail)
                    {
                        fs.Seek(-f.SignatureLength, SeekOrigin.End);
                    }

                    fs.Read(sig, 0, f.SignatureLength);

                    if (sig.SequenceEqual(f.Signature))
                    {
                        isValid = true;
                        ext = f.Extension;
                        break;                        
                    }                        
                }                    
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();

                if (isValid && fixExtensionIfVaild)
                    file = ChangeFileNameAndExtension(file, ext);
            }
            return isValid;
        }

        private string ChangeFileNameAndExtension(string originalFilePath, string extension)
        {
            if (!System.IO.File.Exists(originalFilePath))
            {
                throw new FileNotFoundException();
            }

            string origianlext = Path.GetExtension(originalFilePath);

            if (origianlext.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                return originalFilePath;

            string newFilePath = Path.ChangeExtension(originalFilePath, extension);
            int count = 2;

            while (System.IO.File.Exists(newFilePath))
            {
                string oldFileName = Path.GetFileNameWithoutExtension(newFilePath);
                string directory = Path.GetDirectoryName(newFilePath);
                string ext = Path.GetExtension(newFilePath);
                string newFileName = oldFileName + string.Format(" ({0})", count);

                newFilePath = directory + "\\" + newFileName + ext;
                count++;
            }

            try
            {
                System.IO.File.Move(originalFilePath, newFilePath);
            }
            catch (Exception)
            {

                throw;
            }
            return newFilePath;
        }
    }
}
