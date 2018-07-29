﻿using System;
using System.Collections.Generic;
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
                throw new DirectoryNotFoundException(string.Format(ErrorMessages.InvalidDirectory, directory));
            }
            var newEntry = new BaseDirectoryEntry(_baseDirectories.Count + 1, directory);
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
    }
}
