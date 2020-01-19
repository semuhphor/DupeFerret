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
            Assert.Equal("4475706c696361746531", hash);
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