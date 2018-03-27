using System.Collections.Generic;
using Xunit;

namespace Common.Infra.Tests
{
    public class ExtensiosTests
    {
        [Fact]
        public void EmptyListIfEmpty_ListString_To_ListString_Success()
        {
            List<string> emptyList = null;
            var result = emptyList.EmptyListIfEmpty();

            Assert.NotNull(result);
        }

        [Fact]
        public void EmptyListIfEmpty_IEnumerableString_To_ListString_Success()
        {
            IEnumerable<string> emptyList = null;
            var result = emptyList.EmptyListIfEmpty();

            Assert.NotNull(result);
        }

        [Fact]
        public void EmptyListIfEmpty_NotEmpty_Success()
        {
            //Arrange
            string testItem = "1";
            IEnumerable<string> emptyList = new List<string> { testItem };

            //Act
            var result = emptyList.EmptyListIfEmpty<string>();

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Collection(result, itm => Assert.Equal(testItem, itm));
            //Assert.Equal(testItem, result[0]);
        }
    }
}
