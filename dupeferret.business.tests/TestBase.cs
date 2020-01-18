using System;
using System.IO;

namespace dupeferret.business.tests
{
    public abstract class TestBase
    {
        internal const string TestFileName = "Dup_1_A";

        internal string FQTestFileName;

        internal FileInfo TestFileInfo;

        internal string TestDataDirectory{get; set;}

        internal string _path; // for testing more than one directory.

        public TestBase()
        {
            TestDataDirectory = DetermineTestDataDirectory();
            FQTestFileName = Path.Combine(TestDataDirectory, TestFileName);
            TestFileInfo = new FileInfo(FQTestFileName);
        }

        #region directory helpers
        private string TrimOneDirectory(string path)
        {
            return Path.GetFullPath(path + Path.DirectorySeparatorChar + "..");
        }

        internal string DetermineTestDataDirectory()
        {
            var codeBaseUrl = new Uri(typeof(TraverserTests).Assembly.CodeBase);
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

        internal string TestDiretorySet1{ get {return Path.Combine(TestDataDirectory, "Set1");}}
        internal string TestDiretorySet2{ get {return Path.Combine(TestDataDirectory, "Set2");}}
        
        #endregion

    }
}