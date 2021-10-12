using System;
using Xunit;
using LingUnion;

namespace TestProject1
{
    public class CheckIfExistsTests
    {
        [Theory]
        [InlineData(".html")]
        [InlineData(".jpg")]
        [InlineData(".txt")]
        public void DefaultProtocolCheckTest(string protocol)
        {
            Assert.True(ProtocolRegistry.CheckIfExists(protocol), $"default protocol {protocol} exist");
        }

        //[Theory]
        //[InlineData("*")] // Window specific
        //[InlineData("\\")] // 
        //public void SpecialCharactorCheckTest(string protocol)
        //{

        //}

        [Theory]
        [InlineData("oims;tkgjsa;oerijgs;dfkgsarg")]
        [InlineData("0000")]
        [InlineData("-")]
        public void MeaningLessProtocolCheckTest(string protocol)
        {
            Assert.False(ProtocolRegistry.CheckIfExists(protocol), $"meaningless protocol {protocol} exist");
        }
    }
}
