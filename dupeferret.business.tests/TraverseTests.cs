using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using dupeferret.business;

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

        #region ResetTraverse
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
        #endregion
    }
}
