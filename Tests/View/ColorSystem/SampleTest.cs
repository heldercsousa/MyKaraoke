using Xunit;
using FluentAssertions;

namespace MyVocaList.Tests.View.ColorSystem
{
    /// <summary>
    /// Sample test to verify test project setup
    /// </summary>
    public class SampleTest
    {
        [Fact]
        public void TestProject_ShouldBeConfiguredCorrectly()
        {
            // Arrange
            var expected = 42;

            // Act
            var actual = 40 + 2;

            // Assert
            actual.Should().Be(expected);
        }
    }
}
