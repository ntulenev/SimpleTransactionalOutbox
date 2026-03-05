using DB;

using FluentAssertions;

using OutboxService.Validation;

using Xunit;

namespace OutboxService.Tests;

public class OutboxFetcherOptionsValidatorTests
{
    [Fact(DisplayName = "OutboxFetcherOptionsValidator can be created.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {
        // Arrange
        var validator = new OutboxFetcherOptionsValidator();

        // Act
        var exception = Record.Exception(() => _ = validator);

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxFetcherOptionsValidator fails when options are null.")]
    [Trait("Category", "Unit")]
    public void FailsIfOptionsIsNull()
    {
        // Arrange
        var validator = new OutboxFetcherOptionsValidator();
        OutboxFetcherOptions options = null!;

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Theory(DisplayName = "OutboxFetcherOptionsValidator fails when limit is not positive.")]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    public void FailsIfLimitIsNotPositive(int limit)
    {
        // Arrange
        var validator = new OutboxFetcherOptionsValidator();
        var options = new OutboxFetcherOptions
        {
            Limit = limit
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact(DisplayName = "OutboxFetcherOptionsValidator passes for valid options.")]
    [Trait("Category", "Unit")]
    public void NotFailsIfOptionsIsCorrect()
    {
        // Arrange
        var validator = new OutboxFetcherOptionsValidator();
        var options = new OutboxFetcherOptions
        {
            Limit = 5
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }
}
