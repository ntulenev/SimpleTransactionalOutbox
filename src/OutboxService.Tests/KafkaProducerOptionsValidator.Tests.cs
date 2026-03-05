using FluentAssertions;

using OutboxService.Config;
using OutboxService.Validation;

using Xunit;

namespace OutboxService.Tests;

public class KafkaProducerOptionsValidatorTests
{
    [Fact(DisplayName = "KafkaProducerOptionsValidator can be created.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {
        // Arrange
        var validator = new KafkaProducerOptionsValidator();

        // Act
        var exception = Record.Exception(() => _ = validator);

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "KafkaProducerOptionsValidator fails when options are null.")]
    [Trait("Category", "Unit")]
    public void FailsIfOptionsIsNull()
    {
        // Arrange
        var validator = new KafkaProducerOptionsValidator();
        KafkaProducerOptions options = null!;

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "KafkaProducerOptionsValidator fails when bootstrap servers are null.")]
    [Trait("Category", "Unit")]
    public void FailsIfBootstrapServersIsNull()
    {
        // Arrange
        var validator = new KafkaProducerOptionsValidator();
        var options = new KafkaProducerOptions
        {
            BootstrapServers = null!
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "KafkaProducerOptionsValidator fails when bootstrap servers are empty.")]
    [Trait("Category", "Unit")]
    public void FailsIfBootstrapServersIsEmpty()
    {
        // Arrange
        var validator = new KafkaProducerOptionsValidator();
        var options = new KafkaProducerOptions
        {
            BootstrapServers = []
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "KafkaProducerOptionsValidator passes for valid options.")]
    [Trait("Category", "Unit")]
    public void NotFailsIfOptionsIsCorrect()
    {
        // Arrange
        var validator = new KafkaProducerOptionsValidator();
        var options = new KafkaProducerOptions
        {
            BootstrapServers = ["localhost:9092"]
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }
}
