using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace dupeferret.business
{
    public class Traverse
    {
        private readonly Dictionary<int, BaseDirectoryEntry> _baseDirectories = new Dictionary<int, BaseDirectoryEntry>();

        public Dictionary<int, BaseDirectoryEntry> GetBaseDirectories()
        {
            return _baseDirectories;
        }

        public Traverse()
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
