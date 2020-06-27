using System;
using System.IO;

namespace dupeferret.business
{
    public class FileInfoHandler
    {
        private FileInfo _fileInfo;

        public virtual long Length { get; private set; }
        public virtual string Name { get; private set; }
        public virtual DateTime CreationTime { get; private set; }
        public virtual DateTime LastWriteTime { get; private set; }
        public FileInfoHandler(string fqfn)
        {
            _fileInfo = new FileInfo(fqfn);
            Length = _fileInfo.Length;
            Name = _fileInfo.Name;
            CreationTime = _fileInfo.CreationTime;
            LastWriteTime = _fileInfo.LastWriteTime;
        }

        public FileInfoHandler() {}
    }
}