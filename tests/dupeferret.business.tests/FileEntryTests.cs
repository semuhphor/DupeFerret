using System.Data;
using Xunit;
using System;
using System.IO;
using FakeItEasy;

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
        public void SimpleFileEntryhTest()
        {
            FileEntry fileEntry = new FileEntry(1, base.FQTestFileName);
            var simpleEntry = fileEntry.ToSimpleFileEntry();
            Assert.Equal(fileEntry.Info.Name, simpleEntry.Name);
            Assert.Equal(fileEntry.FQFN, simpleEntry.FQFN);
            Assert.Equal(fileEntry.Info.Length, simpleEntry.Length);
            Assert.Equal(fileEntry.Info.CreationTime, simpleEntry.CreationTime);
            Assert.Equal(fileEntry.Info.LastWriteTime, simpleEntry.LastWriteTime);
        } 

        [Fact]
        public void DupeFirstHashMatches()
        {
            var hash = new FileEntry(1, base.FQTestFileName).FirstHash();
            var dupHash = new FileEntry(2, base.FQDupFileName).FirstHash();
            Assert.Equal(dupHash, hash);
        } 

        // CompareTo checks

        private static DateTime now = DateTime.Now;
        private static DateTime earlier = now - TimeSpan.FromDays(1);

        [Fact]
        public void ReturnZeroIfEqualContent()
        {
            var nowInfo = A.Fake<FileInfoHandler>();
            A.CallTo(() => nowInfo.CreationTime).Returns(now);
            A.CallTo(() => nowInfo.LastWriteTime).Returns(now);
            var first = new FileEntry(1, "same", nowInfo);
            var second = new FileEntry(1, "same", nowInfo);
            Assert.Equal(0, first.CompareTo(second));
            Assert.Equal(0, second.CompareTo(first));
        }

        [Fact]
        public void SecondIsChosenIfEarlierCreateDate()
        {
            var nowInfo = A.Fake<FileInfoHandler>();
            var earlierInfo = A.Fake<FileInfoHandler>();

            A.CallTo(() => nowInfo.CreationTime).Returns(now);
            A.CallTo(() => earlierInfo.CreationTime).Returns(earlier);
            var first = new FileEntry(1, "first", nowInfo);
            var second = new FileEntry(1, "second", earlierInfo);
            Assert.Equal(1, first.CompareTo(second));
            Assert.Equal(-1, second.CompareTo(first));
        }

        [Fact]
        public void SecondChosenIfSameCreationButEarlierLastWrite()
        {
            var nowInfo = A.Fake<FileInfoHandler>();
            var earlierInfo = A.Fake<FileInfoHandler>();

            A.CallTo(() => nowInfo.CreationTime).Returns(now);
            A.CallTo(() => earlierInfo.CreationTime).Returns(now);
            A.CallTo(() => nowInfo.LastWriteTime).Returns(now);
            A.CallTo(() => earlierInfo.LastWriteTime).Returns(earlier);
            var first = new FileEntry(1, "first", nowInfo);
            var second = new FileEntry(1, "second", earlierInfo);
            Assert.Equal(1, first.CompareTo(second));
            Assert.Equal(-1, second.CompareTo(first));
        }

        [Fact]
        public void FirstChosenIfSameCreationAndLastWriteButLowerName()
        {
            var nowInfo = A.Fake<FileInfoHandler>();
            var earlierInfo = A.Fake<FileInfoHandler>();

            A.CallTo(() => nowInfo.CreationTime).Returns(now);
            A.CallTo(() => earlierInfo.CreationTime).Returns(now);
            A.CallTo(() => nowInfo.LastWriteTime).Returns(now);
            A.CallTo(() => earlierInfo.LastWriteTime).Returns(now);
            var first = new FileEntry(1, "A", nowInfo);
            var second = new FileEntry(1, "B", earlierInfo);
            Assert.Equal(-1, first.CompareTo(second));
            Assert.Equal(1, second.CompareTo(first));
        }

        [Fact]
        public void SameObjectReturnsZero()
        {
            var nowInfo = A.Fake<FileInfoHandler>();
            var first = new FileEntry(1, "first", null);
            Assert.Equal(0, first.CompareTo(first));
        }

        [Fact]
        public void MustBeFileEntry()
        {
            var fail = new FileEntry(1, "fails", null);
            Assert.Throws<ArgumentException>(() => fail.CompareTo(new object()));
        }

        [Fact]
        public void FollowsNull()
        {
            var only = new FileEntry(1, "followsNull", null);
            Assert.Equal(1, only.CompareTo(null));
        }
    }
}