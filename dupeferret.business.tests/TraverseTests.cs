using System;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class TraverseTests
    {
        public readonly Traverse _traverse;

        public TraverseTests()
        {
            _traverse = new Traverse();
        }

        [Fact]
        public void GetValue()
        {
            Assert.Equal("TestValue", _traverse.GetValue());
        }
    }
}
