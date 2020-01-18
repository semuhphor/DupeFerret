using System.IO;

namespace dupeferret.business {
    public class FileEntry {

        public int BaseDirectoryKey { get; private set; }
        public FileInfo Info { get; set; }
        public string FQFN { get; set; }

        public FileEntry(int baseDirectoryKey, string fqfn) {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.FQFN = fqfn;
            this.Info = new FileInfo(fqfn); 
        }
    }
}