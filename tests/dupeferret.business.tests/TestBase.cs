using System;
using System.IO;

namespace dupeferret.business.tests
{
    public abstract class TestBase
    {
        internal const string TestFileName = "Dup_1_A";
        internal const string DupeFileName = "Dup_1_D";

        internal string FQTestFileName;
        internal string FQDupFileName;

        internal FileInfo TestFileInfo;

        internal string TestDataDirectory{get; set;}

        internal string _path; // for testing more than one directory.

        public TestBase()
        {
            TestDataDirectory = DetermineTestDataDirectory();
            FQTestFileName = Path.Combine(TestDiretorySet1, TestFileName);
            FQDupFileName = Path.Combine(TestDiretorySet2, DupeFileName);
            TestFileInfo = new FileInfo(FQTestFileName);
        }

        #region directory helpers
        private string TrimOneDirectory(string path)
        {
            return Path.GetFullPath(path + Path.DirectorySeparatorChar + "..");
        }

        internal string DetermineTestDataDirectory()
        {
            var codeBasePath = typeof(TraverserTests).Assembly.Location;
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