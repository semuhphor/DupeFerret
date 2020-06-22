using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;


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

        public enum HashType{ Small, Full }
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

        public List<List<FileEntry>> GetDupeSets()
        {
            var sameLengthList = GetAllFiles();
            var sameSmallHashList = GetDupesByHash(sameLengthList, HashType.Small);
            var sameFullHashList = GetDupesByHash(sameSmallHashList, HashType.Full);

            return sameFullHashList;
        }

        public List<List<FileEntry>> GetDupesByHash(List<List<FileEntry>> similarFiles, HashType hashType)
        {
            var returnList = new List<List<FileEntry>>();

            foreach(var list in similarFiles)
            {
                var sameByHash = FindDupes(list, hashType);
                returnList.AddRange(sameByHash);
            }
            return returnList;
        }

        public List<List<FileEntry>> GetAllFiles()
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
            return CleanSingles(FilesByLength);
        }

        public List<List<FileEntry>> CleanSingles<T>(IDictionary<T, List<FileEntry>> dict)
        {
            var returnList = new List<List<FileEntry>>();

            foreach(var key in dict.Keys)
            {
                if (dict[key].Count > 1)
                {
                    returnList.Add(dict[key]);
                }
                dict.Remove(key);
            }
            return returnList;
        }

        public List<List<FileEntry>> FindDupes(List<FileEntry> similarFiles, HashType hashType)
        {
            var dupeSets = new Dictionary<string, List<FileEntry>>();

            foreach(var entry in similarFiles)
            {
                try
                {
                    var hash = (hashType == HashType.Small) ?  entry.FirstHash() : entry.FullHash();
                    if (!dupeSets.ContainsKey(hash))
                    {
                        dupeSets.Add(hash, new List<FileEntry>());
                    }
                    dupeSets[hash].Add(entry);
                }
                catch {}
            }
            return CleanSingles(dupeSets);
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
                if (length > int.MaxValue || length == 0L || fileEntry.Info.Name.StartsWith("."))
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
