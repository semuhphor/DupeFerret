using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace dupeferret.business
{
    public class Traverser
    {
        private readonly Dictionary<int, BaseDirectoryEntry> _baseDirectories = new Dictionary<int, BaseDirectoryEntry>();

        public Dictionary<int, BaseDirectoryEntry> GetBaseDirectories()
        {
            return _baseDirectories;
        }

        public Traverser()
        {
        }

        public void AddBaseDirectory(string directory)
        {
            if (!DirectoryExists(directory))
            {
                throw new DirectoryNotFoundException(ErrorMessages.InvalidDirectory.Format(directory));
            }
            if (AlreadyHasDirectory(directory))
            {
                throw new Exception(ErrorMessages.DuplicateBaseDirectory.Format(directory));
            }
            var info = new DirectoryInfo(directory);
            var newEntry = new BaseDirectoryEntry(_baseDirectories.Count + 1, info.FullName);
            _baseDirectories.Add(newEntry.Number, newEntry);
        }

        public List<FileEntry> GetAllFiles()
        {
            var fileList = new List<FileEntry>();
            foreach(var value in _baseDirectories.Values)
            {
                foreach(var fullyQualifiedFileName in Directory.GetFileSystemEntries(value.Directory, "*", SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(fullyQualifiedFileName))
                    {
                        fileList.Add(BuildFileEntry(value, fullyQualifiedFileName));
                    }
                }
            }
            return fileList;
        }

        private FileEntry BuildFileEntry(BaseDirectoryEntry baseDirectoryEntry, string fullyQualifiedFileName)
        {
            if (!fullyQualifiedFileName.StartsWith(baseDirectoryEntry.Directory))
            {
                throw new InvalidDataException(ErrorMessages.DirectoryNotInFQFN.Format(baseDirectoryEntry.Directory) + fullyQualifiedFileName);
            }
            var fqfn = fullyQualifiedFileName.Substring(baseDirectoryEntry.Directory.Length);
            if (fqfn.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fqfn = fqfn.Substring(1);
            }
            return new FileEntry(baseDirectoryEntry.Number, fqfn);
        }
        private bool DirectoryExists(string dir)
        {
            try
            {
                return Directory.Exists(dir);
            }
            catch
            {
                return false;
            }
        }

        private bool AlreadyHasDirectory(string dir)
        {
            foreach (var value in _baseDirectories.Values)
            {
                if (value.Directory == dir)
                {
                    return true; 
                }
            }
            return false;
        }
    }
}
