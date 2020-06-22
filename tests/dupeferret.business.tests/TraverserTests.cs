using System.Linq;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace dupeferret.business.tests
{
    public class TraverserTests : TestBase
    {
        private readonly ITestOutputHelper _output;
        public Traverser _traverser;

        public TraverserTests(ITestOutputHelper output) : base()
        {
            _output = output;
            ResetTraverse();    
        }

        [Fact]
        public void BaseDirectorySetTest()
        {
            ResetTraverse();
            _traverser.AddBaseDirectory(TestDataDirectory);
            var baseDirectories = _traverser.BaseDirectories;
            Assert.Single(baseDirectories);
            Assert.Equal(1, baseDirectories[1].Number);
            Assert.Equal(DetermineTestDataDirectory(), baseDirectories[1].Directory);
            _traverser.AddBaseDirectory(_path);
            Assert.Equal(2,baseDirectories.Count);
            Assert.Equal(2, baseDirectories[2].Number);
            Assert.Equal(_path, baseDirectories[2].Directory);
            _output.WriteLine(TestDataDirectory);
            _output.WriteLine(baseDirectories[1].Directory);
            _output.WriteLine(_path);
            _output.WriteLine(baseDirectories[2].Directory);
        }

        [Fact]
        public void CannotAddSameDirectoryTest()
        {
            ResetTraverse();
            _traverser.AddBaseDirectory(TestDataDirectory);
            Exception ex = Assert.Throws<Exception>(() => _traverser.AddBaseDirectory(TestDataDirectory));
            Assert.Equal(ErrorMessages.DuplicateBaseDirectory.Format(TestDataDirectory), ex.Message);
        }

        [Fact]
        public void AddingBadDirectoryThrowsExceptionTest()
        {
            var _badDirectory = TestDataDirectory + Path.DirectorySeparatorChar + "DoesNoteExistDir";
            Exception ex = Assert.Throws<DirectoryNotFoundException>(() => _traverser.AddBaseDirectory(_badDirectory));
            Assert.Equal(ErrorMessages.InvalidDirectory.Format(_badDirectory), ex.Message);
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTest()
        {
            _traverser.AddBaseDirectory(TestDataDirectory);
            _traverser.GetAllFiles();
            Assert.Equal(12, _traverser.UniqueFiles.Count);
            Assert.Equal(8, CountFiles("dup"));
            Assert.Equal(4, CountFiles("notdup"));
        }

        [Fact]
        public void NoZeroLengthFiles()
        {
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.GetAllFiles();
            Assert.Equal(0, CountFiles("zero"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestSet1()
        {
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.GetAllFiles();
            Assert.Equal(9, _traverser.Count);
            Assert.Equal(6, CountFiles("dup"));
            Assert.Equal(3, CountFiles("notdup"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestSet2()
        {
            _traverser.AddBaseDirectory(TestDiretorySet2);
            _traverser.GetAllFiles();
            Assert.Equal(3, _traverser.Count);
            Assert.Equal(2, CountFiles("dup"));
            Assert.Equal(1, CountFiles("notdup"));
        }

        [Fact]
        public void EnumerateFilesGetsAllEntriesTestBothSets()
        {
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.AddBaseDirectory(TestDiretorySet2);
             _traverser.GetAllFiles();
            Assert.Equal(12, _traverser.Count);
            Assert.Equal(8, CountFiles("dup"));
            Assert.Equal(4, CountFiles("notdup"));
        }

        [Fact]
        public void FindPossibleDupesTest()
        {
            _traverser.AddBaseDirectory(TestDataDirectory);
            var list = _traverser.GetDupeSets();
            foreach(var dupeSet in list)
            {
                Console.WriteLine("--------");
                dupeSet.ForEach(entry => { Console.WriteLine($"{entry.FQFN}"); });
                Console.WriteLine("");
            }
        }

        #region ResetTraverse


        private int CountFiles(string startingWith)
        {
            int filesThatMatch = 0;
            foreach(var fileEntry in _traverser.UniqueFiles.Values)
            {
                if (Path.GetFileName(fileEntry.Info.Name).ToLower().StartsWith(startingWith))
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

    }
}
