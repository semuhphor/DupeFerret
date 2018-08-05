using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using dupeferret.business;
using System.Collections.Generic;

namespace dupeferret.business.tests
{
    public class TraverseTests
    {
        private readonly ITestOutputHelper _output;
        public Traverser _traverser;
        private readonly string _testDataDirectory;
        private string _path; // for testing more than one directory.

        public TraverseTests(ITestOutputHelper output)
        {
            _output = output;
            _testDataDirectory = DetermineTestDataDirectory();
            ResetTraverse();    
        }

        [Fact]
        public void BaseDirectorySetTest()
        {
            ResetTraverse();
            _traverser.AddBaseDirectory(_testDataDirectory);
            var baseDirectories = _traverser.GetBaseDirectories();
            Assert.Single(baseDirectories);
            Assert.Equal(1, baseDirectories[1].Number);
            Assert.Equal(DetermineTestDataDirectory(), baseDirectories[1].Directory);
            _traverser.AddBaseDirectory(_path);
            Assert.Equal(2,baseDirectories.Count);
            Assert.Equal(2, baseDirectories[2].Number);
            Assert.Equal(_path, baseDirectories[2].Directory);
            _output.WriteLine(_testDataDirectory);
            _output.WriteLine(baseDirectories[1].Directory);
            _output.WriteLine(_path);
            _output.WriteLine(baseDirectories[2].Directory);
        }

        [Fact]
        public void CannotAddSameDirectoryTest()
        {
            ResetTraverse();
            _traverser.AddBaseDirectory(_testDataDirectory);
            Exception ex = Assert.Throws<Exception>(() => _traverser.AddBaseDirectory(_testDataDirectory));
            Assert.Equal(ErrorMessages.DuplicateBaseDirectory.Format(_testDataDirectory), ex.Message);
        }

        [Fact]
        public void AddingBadDirectoryThrowsExceptionTest()
        {
            var _badDirectory = _testDataDirectory + Path.DirectorySeparatorChar + "DoesNoteExistDir";
            Exception ex = Assert.Throws<DirectoryNotFoundException>(() => _traverser.AddBaseDirectory(_badDirectory));
            Assert.Equal(ErrorMessages.InvalidDirectory.Format(_badDirectory), ex.Message);
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTest()
        {
            _traverser.AddBaseDirectory(_testDataDirectory);
            var fileList = _traverser.GetAllFiles();
            Assert.Equal(12, fileList.Count);
            Assert.Equal(8, CountFiles(fileList, "dup"));
            Assert.Equal(4, CountFiles(fileList, "notdup"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestSet1()
        {
            _traverser.AddBaseDirectory(TestDiretorySet1);
            var fileList = _traverser.GetAllFiles();
            Assert.Equal(9, fileList.Count);
            Assert.Equal(6, CountFiles(fileList, "dup"));
            Assert.Equal(3, CountFiles(fileList, "notdup"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestSet2()
        {
            _traverser.AddBaseDirectory(TestDiretorySet2);
            var fileList = _traverser.GetAllFiles();
            Assert.Equal(3, fileList.Count);
            Assert.Equal(2, CountFiles(fileList, "dup"));
            Assert.Equal(1, CountFiles(fileList, "notdup"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestBothSets()
        {
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.AddBaseDirectory(TestDiretorySet2);
            var fileList = _traverser.GetAllFiles();
            Assert.Equal(12, fileList.Count);
            Assert.Equal(8, CountFiles(fileList, "dup"));
            Assert.Equal(4, CountFiles(fileList, "notdup"));
        }

        #region ResetTraverse

        private int CountFiles(List<string> fileList, string startingWith)
        {
            int filesThatMatch = 0;
            foreach(var file in fileList)
            {
                if (Path.GetFileName(file).ToLower().StartsWith(startingWith))
                {
                    filesThatMatch++;
                }
            }
            return filesThatMatch;
        }

        private void ResetTraverse()
        {
            _traverser = new Traverser();
        }
        #endregion

        #region directory helpers
        private string TrimOneDirectory(string path)
        {
            return Path.GetFullPath(path + Path.DirectorySeparatorChar + "..");
        }

        private string DetermineTestDataDirectory()
        {
            var codeBaseUrl = new Uri(typeof(TraverseTests).Assembly.CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath); 
            var path = Path.GetDirectoryName(codeBasePath);
            _path = path; 
            while (!path.EndsWith(Path.DirectorySeparatorChar + "bin"))
            {
                path = TrimOneDirectory(path);
            }
            path = TrimOneDirectory(path) + Path.DirectorySeparatorChar + "TestData";
            return path;
        }

        private string TestDiretorySet1{ get {return Path.Combine(_testDataDirectory, "Set1");}}
        private string TestDiretorySet2{ get {return Path.Combine(_testDataDirectory, "Set2");}}
        
        #endregion
    }
}
