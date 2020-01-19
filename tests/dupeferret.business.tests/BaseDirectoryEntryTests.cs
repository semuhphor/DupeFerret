using Xunit;
using System;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class BaseDirectoryEntryTests
    {
        [Fact]
        public void NewBaseDirectoryEntryTest()
        {
            int random = new Random().Next();
            var baseDirectoryEntry = new BaseDirectoryEntry(random, random.ToString());
            Assert.Equal(baseDirectoryEntry.Number, random);
            Assert.Equal(baseDirectoryEntry.Directory, random.ToString());
        }
    }
}