using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dupeferret.business {
    public class FileInfoHandler
    {
        private FileInfo _fileInfo;

        public long Length { get; private set; }
        public string Name { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public FileInfoHandler(string fqfn)
        {
            _fileInfo = new FileInfo(fqfn);
            Length = _fileInfo.Length;
            Name = _fileInfo.Name;
            CreationTime = _fileInfo.CreationTime;
            LastWriteTime = _fileInfo.LastWriteTime;
        }
    }

    public class FileEntry {

        public enum HashType { HashFirst512, HasFullFile }
        public int BaseDirectoryKey { get; private set; }
        public FileInfoHandler Info { get; set; }
        public string FQFN { get; set; }

        public string First512Hash{ get; set;}
        public string FullFileHash{ get; set; }

        public FileEntry(int baseDirectoryKey, string fqfn) {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.FQFN = fqfn;
            this.Info = new FileInfoHandler(fqfn); 
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
            using (var fi = File.Open(FQFN, FileMode.Open,FileAccess.Read, FileShare.Read))
            {
                fi.Read(buffer, 0, (int) readLength);
                fi.Close();
            }
            return GetHash(new SHA512Managed(), buffer);
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();
            var hash = hashAlgorithm.ComputeHash(data);

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}