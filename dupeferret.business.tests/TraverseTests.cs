using System;
using System.IO;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class TraverseTests
    {
        public readonly Traverse _traverse;

        public TraverseTests() => _traverse = new Traverse();

        private string TrimOneDirectory(string path)
        {
            return Path.GetFullPath(path + Path.DirectorySeparatorChar + "..");
        }
        private string TestDataDirectory
        {
            get
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
        }

        [Fact]
        void SetCurrentDirectory() => Console.WriteLine(TestDataDirectory);
    }
}
