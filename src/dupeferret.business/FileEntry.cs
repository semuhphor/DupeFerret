using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dupeferret.business {
    public class FileEntry {

        public enum HashType { HashFirst512, HasFullFile }
        public int BaseDirectoryKey { get; private set; }
        public FileInfo Info { get; set; }
        public string FQFN { get; set; }

        public string First512Hash{ get; set;}
        public string FullFileHash{ get; set; }

        public FileEntry(int baseDirectoryKey, string fqfn) {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.FQFN = fqfn;
            this.Info = new FileInfo(fqfn); 
        }

        public string FirstHash()
        {
            First512Hash =  HashIt(HashType.HashFirst512);
            return First512Hash;
        }

        public string FullHash()
        {
            FullFileHash =  HashIt(HashType.HasFullFile);
            return FullFileHash;
        }

        private string HashIt(HashType typeOfHash)
        {
            long readLength = Info.Length;
            if (typeOfHash == HashType.HashFirst512 && readLength > 512L)
            {
                readLength = 512L;
            }
            byte[] buffer = new byte[readLength];
            using (var fi = File.Open(FQFN, FileMode.Open))
            {
                fi.Read(buffer, 0, (int) readLength);
                fi.Close();
            }
            return GetHash(SHA512.Create(), buffer);
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
    }
    }
}