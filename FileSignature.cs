using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{    
    public class FileSignature
    {
        FileSignatureType signatureType = FileSignatureType.None;
        byte[] signature = null;
        string extension = null;

        public FileSignature(FileSignatureType signatureType, byte[] signature, string extension)
        {
            this.signatureType = signatureType;
            this.signature = signature;
            this.extension = extension;
        }

        public FileSignatureType SignatureType { get { return this.signatureType; } }
        public byte[] Signature { get { return this.signature; } }
        public int SignatureLength
        {
            get
            {
                if (signature != null)
                {
                    return signature.Length;
                }
                return -1;
            }
        }
        public string Extension { get { return extension; } }
    }
}
