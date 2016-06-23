using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFileManager
{    
    public sealed class ArchiveFileChecker : AbstractFileChecker
    {        
        public ArchiveFileChecker() : base()
        {
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 31, 139, 8 }, ".gz"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 55, 122, 188, 175, 39, 28 }, ".7z"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 65, 76, 90, 1 }, ".alz"));
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 66, 90, 104 }, ".bz2"));
            //동일한 헤더의 확장자가 너무 많아 문제소지있음.
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 80, 75, 3, 4 }, ".zip")); //PKZIP 
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 80, 75, 76, 73, 84, 69 }, ".zip")); //PKLite
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] { 80, 75, 83, 112, 88 }, ".zip")); //PKSFX
            signtures.Add(new FileSignature(FileSignatureType.Header, new byte[] {87, 105, 110, 90, 105, 112}, ".zip")); //Winzip

        }            
    }
}
