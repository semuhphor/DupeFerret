using System;
using System.IO;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class TraverseTests
    {
        public Traverse _traverser;
        private readonly string _testDataDirectory;

        private string TrimOneDirectory(string path)
        {
            return Path.GetFullPath(path + Path.DirectorySeparatorChar + "..");
        }

        private string DetermineTestDataDirectory()
        {
            var codeBaseUrl = new Uri(typeof(TraverseTests).Assembly.CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath); 
            var path = Path.GetDirectoryName(codeBasePath);
            while (!path.EndsWith(Path.DirectorySeparatorChar + "bin"))
            {
                path = TrimOneDirectory(path);
            }
            path = TrimOneDirectory(path) + Path.DirectorySeparatorChar + "TestData";
            return path;
        }

        private void ResetTraverse()
        {
            _traverser = new Traverse();
        }

        public TraverseTests()
        {
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
    }
}
