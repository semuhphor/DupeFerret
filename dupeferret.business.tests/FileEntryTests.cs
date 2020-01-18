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
    }
}