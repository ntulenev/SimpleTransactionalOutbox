using FluentAssertions;

using Xunit;

namespace Models.Tests
{
    public class ProcessingDataTests
    {
        [Fact(DisplayName = " ProcessingData can be created.")]
        [Trait("Category", "Unit")]
        public void OutboxMessageShouldBeCreatedWithValidParams()
        {
            // Arrange
            int id = 42;
            int value = 5;

            // Act
            ProcessingData res = null!;
            var exception = Record.Exception(() => res = new ProcessingData(id, value));

            // Assert
            exception.Should().BeNull();

            res.Id.Should().Be(id);
            res.Value.Should().Be(value);

        }
    }
}
