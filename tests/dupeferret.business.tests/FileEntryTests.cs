using Xunit;
using System;

namespace dupeferret.business.tests
{
    public class FileEntryTests : TestBase
    {
        [Fact]
        public void NewFileEntryCreatesFileInfo()
        {
            int random = new Random().Next();

            var entry = new FileEntry(random, base.FQTestFileName);
            Assert.Equal(random, entry.BaseDirectoryKey);
            Assert.NotNull(entry.Info);
        }      

        [Fact]
        public void HashTest()
        {
            var entry = new FileEntry(1, base.FQTestFileName);
            var hash = entry.FirstHash();
            Assert.Equal("ffaf844d445ca6193a2a1bf5ae96c372f2a04d09c1b46493adfec2309d8e8e2233e6113a8306f28e7dc236781b73fad56f33a49bbb2fcb43af5f686cfaade102", hash);
        } 

        [Fact]
        public void DupeFirstHashMatches()
        {
            var hash = new FileEntry(1, base.FQTestFileName).FirstHash();
            var dupHash = new FileEntry(2, base.FQDupFileName).FirstHash();
            Assert.Equal(dupHash, hash);
        } 
    }
}