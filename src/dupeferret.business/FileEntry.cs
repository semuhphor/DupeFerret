using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dupeferret.business {
    public class FileEntry {

        public int BaseDirectoryKey { get; private set; }
        public FileInfo Info { get; set; }
        public string FQFN { get; set; }

        public string First512Hash{ get; set;}

        public FileEntry(int baseDirectoryKey, string fqfn) {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.FQFN = fqfn;
            this.Info = new FileInfo(fqfn); 
        }

        public string FirstHash()
        {
            int readLength = Info.Length < 512L ? (int) Info.Length : 512;
            byte[] buffer = new byte[readLength];
            using (var fi = File.Open(FQFN, FileMode.Open))
            {
                fi.Read(buffer, 0, readLength);
                fi.Close();
            }
            First512Hash = GetHash(SHA512.Create(), buffer);
            return First512Hash;
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