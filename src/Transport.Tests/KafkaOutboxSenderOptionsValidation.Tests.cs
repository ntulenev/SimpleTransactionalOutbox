using Xunit;

using FluentAssertions;

using Transport.Validation;

namespace Transport.Tests
{
    public class KafkaOutboxSenderOptionsValidationTests
    {
        [Fact(DisplayName = "KafkaOutboxSenderOptionsValidation can be created.")]
        [Trait("Category", "Unit")]
        public void CanBeCreated()
        {

            // Act
            var exception = Record.Exception(() => new KafkaOutboxSenderOptionsValidation());

            // Assert
            exception.Should().BeNull();
        }
    }
}
