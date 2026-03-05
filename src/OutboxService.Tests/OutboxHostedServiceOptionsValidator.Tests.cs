using FluentAssertions;

using OutboxService.Config;
using OutboxService.Validation;

using Xunit;

namespace OutboxService.Tests;

public class OutboxHostedServiceOptionsValidatorTests
{
    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator can be created.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();

        // Act
        var exception = Record.Exception(() => _ = validator);

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator fails when options are null.")]
    [Trait("Category", "Unit")]
    public void FailsIfOptionsIsNull()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        OutboxHostedServiceOptions options = null!;

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator fails when min delay is not positive.")]
    [Trait("Category", "Unit")]
    public void FailsIfMinDelayIsNotPositive()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        var options = new OutboxHostedServiceOptions
        {
            MinDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.FromMilliseconds(100),
            StepsCount = 5
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator fails when max delay is not positive.")]
    [Trait("Category", "Unit")]
    public void FailsIfMaxDelayIsNotPositive()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        var options = new OutboxHostedServiceOptions
        {
            MinDelay = TimeSpan.FromMilliseconds(1),
            MaxDelay = TimeSpan.Zero,
            StepsCount = 5
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator fails when max delay is lower than min delay.")]
    [Trait("Category", "Unit")]
    public void FailsIfMaxDelayLowerThanMinDelay()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        var options = new OutboxHostedServiceOptions
        {
            MinDelay = TimeSpan.FromMilliseconds(20),
            MaxDelay = TimeSpan.FromMilliseconds(10),
            StepsCount = 5
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator fails when steps count is not positive.")]
    [Trait("Category", "Unit")]
    public void FailsIfStepsCountIsNotPositive()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        var options = new OutboxHostedServiceOptions
        {
            MinDelay = TimeSpan.FromMilliseconds(1),
            MaxDelay = TimeSpan.FromMilliseconds(10),
            StepsCount = 0
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxHostedServiceOptionsValidator passes for valid options.")]
    [Trait("Category", "Unit")]
    public void NotFailsIfOptionsIsCorrect()
    {
        // Arrange
        var validator = new OutboxHostedServiceOptionsValidator();
        var options = new OutboxHostedServiceOptions
        {
            MinDelay = TimeSpan.FromMilliseconds(1),
            MaxDelay = TimeSpan.FromMilliseconds(10),
            StepsCount = 5
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }
}
