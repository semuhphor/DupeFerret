using System.IO;

namespace dupeferret.business {
    public class FileEntry {

        public int BaseDirectoryKey { get; private set; }
        public string RelativePath { get; private set; }
        public DirectoryInfo DirInfo { get; set; }

        public FileEntry (int baseDirectoryKey, string relativePath) {
            this.BaseDirectoryKey = baseDirectoryKey;
            this.RelativePath = relativePath;
        }
    }
}