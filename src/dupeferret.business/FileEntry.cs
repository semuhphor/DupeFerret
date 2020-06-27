using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dupeferret.business
{

    public class FileEntry : IComparable {

        public enum HashType { HashFirst512, HasFullFile }
        public int BaseDirectoryKey { get; private set; }
        public FileInfoHandler Info { get; set; }
        public string FQFN { get; set; }

        public string First512Hash{ get; set;}
        public string FullFileHash{ get; set; }

        public FileEntry(int baseDirectoryKey, string fqfn, FileInfoHandler handler)
        {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.FQFN = fqfn;
            this.Info = handler;
        }
        public FileEntry(int baseDirectoryKey, string fqfn) : this(baseDirectoryKey, fqfn, new FileInfoHandler(fqfn)) {}
        

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

        // IComparabale
        public int CompareTo(object other)
        {
            if (other == null)                                  // q. other null?
                return 1;                                       // a. yes ... this comes afer null.

            if (this == other)                                  // q. comparing to self?
                return 0;                                       // a. yes .. they are the same.

            FileEntry otherEntry = other as FileEntry;                  

            if (otherEntry == null)                                         // q. other a filentry?
                throw new ArgumentException("Object is not a FileEntry");   // a. no .. can't compare it.

            int rc = this.Info.CreationTime.CompareTo(otherEntry.Info.CreationTime);
            
            if (rc != 0)
                return rc;
            
            rc = this.Info.LastWriteTime.CompareTo(otherEntry.Info.LastWriteTime);

            if (rc != 0)
                return rc;
            
            return this.FQFN.CompareTo(otherEntry.FQFN);
        }
    }
}