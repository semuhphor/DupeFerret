using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;


namespace dupeferret.business
{
    public class Traverser 
    {
        private readonly Dictionary<int, BaseDirectoryEntry> _baseDirectories = new Dictionary<int, BaseDirectoryEntry>();
        private readonly Dictionary<string, FileEntry> _uniqueFiles = new Dictionary<string, FileEntry>();
        private readonly Dictionary<long, List<FileEntry>> _filesByLength = new Dictionary<long, List<FileEntry>>();

        public Dictionary<string, FileEntry> UniqueFiles => _uniqueFiles;

        public Dictionary<long, List<FileEntry>> FilesByLength => _filesByLength;
        public int Count { get { return UniqueFiles.Values.Count; } }

        public Dictionary<int, BaseDirectoryEntry> BaseDirectories => _baseDirectories;

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

        public void GetAllFiles()
        {
            foreach(var baseDirectoryEntry in _baseDirectories.Values)
            {
                string dir = baseDirectoryEntry.Directory;
                Console.WriteLine("Processing {0}", dir);
                try
                {
                    var dirInfo = new DirectoryInfo(dir);
                    foreach(var fullyQualifiedFileName in BuildFileList(dirInfo))
                    {
                        bool entryIsNotDirectory = !Directory.Exists(fullyQualifiedFileName);
                        if (entryIsNotDirectory)
                        {
                            AddFileEntries(baseDirectoryEntry, fullyQualifiedFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void CleanSingles()
        {
            foreach(var key in FilesByLength.Keys)
            {
                if (FilesByLength[key].Count < 2)
                {
                    FilesByLength.Remove(key);
                }
            }
        }

        public Dictionary<string, List<FileEntry>> FindPossibleDupes(List<FileEntry> filesWithTheSameLength)
        {
            var dupeSets = new Dictionary<string, List<FileEntry>>();

            foreach(var entry in filesWithTheSameLength)
            {
                try
                {
                    var hash = entry.FirstHash();
                    if (!dupeSets.ContainsKey(hash))
                    {
                        dupeSets.Add(hash, new List<FileEntry>());
                    }
                    dupeSets[hash].Add(entry);
                }
                catch {}
            }
            foreach(var key in dupeSets.Keys)
            {
                if (dupeSets[key].Count < 2)
                {
                    dupeSets.Remove(key);
                }
            }
            return dupeSets;
        }

        private List<string> BuildFileList(DirectoryInfo dirInfo, List<string> fileList = null)
        {
            fileList ??= new List<string>();
            if (dirInfo.Name.StartsWith("."))
            {
                return fileList;
            }
            try
            {
                foreach(var file in dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    string fqfn = Path.Combine(file.DirectoryName, file.Name);
                    fileList.Add(fqfn);
                }
            }
            catch {}
            try
            {
                foreach(var dir in dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    BuildFileList(dir, fileList);
                }
            }
            catch {}
            return fileList;
        }

        private void AddFileEntries(BaseDirectoryEntry baseDirectoryEntry, string fullyQualifiedFileName)
        {
            if (!UniqueFiles.ContainsKey(fullyQualifiedFileName))
            {
                var fileEntry = new FileEntry(baseDirectoryEntry.Number, fullyQualifiedFileName);
                FileInfo info = fileEntry.Info;
                long length = info.Length;
                if (length == 0L || fileEntry.Info.Name.StartsWith("."))
                {
                    return;
                }
                UniqueFiles.Add(fullyQualifiedFileName, fileEntry);
                var lengthList = FilesByLength.ContainsKey(length) ? FilesByLength[length] : null;
                if (lengthList == null)
                {
                    lengthList = new List<FileEntry>();
                    FilesByLength.Add(length, lengthList);
                }
                FilesByLength[length].Add(fileEntry);
            }
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
