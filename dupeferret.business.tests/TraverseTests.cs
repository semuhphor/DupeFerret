using System;
using System.IO;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class TraverseTests
    {
        public readonly Traverse _traverse;
        public readonly string _testDataDirectory = Directory.GetCurrentDirectory();

        public TraverseTests()
        {
            _traverse = new Traverse();
            
        }

        [Fact]
        public void SetCurrentDirectory()
        {
            Console.WriteLine(_testDataDirectory);
        }
    }
}
