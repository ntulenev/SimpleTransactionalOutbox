using FluentAssertions;

using Xunit;

namespace Logic.Tests;

public class OutboxBackoffDelayProviderTests
{
    [Fact(DisplayName = "Backoff provider fails when min delay is not positive.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNonPositiveMinDelay()
    {
        // Arrange
        var minDelay = TimeSpan.Zero;
        var maxDelay = TimeSpan.FromMilliseconds(100);
        var stepsCount = 5;

        // Act
        var exception = Record.Exception(() => _ = new OutboxBackoffDelayProvider(minDelay, maxDelay, stepsCount));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Backoff provider fails when max delay is lower than min delay.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithMaxDelayLowerThanMinDelay()
    {
        // Arrange
        var minDelay = TimeSpan.FromMilliseconds(20);
        var maxDelay = TimeSpan.FromMilliseconds(10);
        var stepsCount = 5;

        // Act
        var exception = Record.Exception(() => _ = new OutboxBackoffDelayProvider(minDelay, maxDelay, stepsCount));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Backoff provider fails when steps count is not positive.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNonPositiveStepsCount()
    {
        // Arrange
        var minDelay = TimeSpan.FromMilliseconds(10);
        var maxDelay = TimeSpan.FromMilliseconds(100);
        var stepsCount = 0;

        // Act
        var exception = Record.Exception(() => _ = new OutboxBackoffDelayProvider(minDelay, maxDelay, stepsCount));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Backoff delay rises until max value and then stays there.")]
    [Trait("Category", "Unit")]
    public void GetNextDelayRiseUntilMaxValue()
    {
        // Arrange
        var provider = new OutboxBackoffDelayProvider(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(110),
            5);

        // Act
        var result = new[]
        {
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay(),
        };

        // Assert
        result.Should().Equal(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(30),
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(70),
            TimeSpan.FromMilliseconds(90),
            TimeSpan.FromMilliseconds(110),
            TimeSpan.FromMilliseconds(110));
    }

    [Fact(DisplayName = "Backoff delay resets to min value after success.")]
    [Trait("Category", "Unit")]
    public void ResetReturnDelayToMinValue()
    {
        // Arrange
        var provider = new OutboxBackoffDelayProvider(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(110),
            5);

        _ = provider.GetNextDelay();
        _ = provider.GetNextDelay();
        _ = provider.GetNextDelay();

        // Act
        provider.Reset();
        var result = provider.GetNextDelay();

        // Assert
        result.Should().Be(TimeSpan.FromMilliseconds(10));
    }

    [Fact(DisplayName = "Backoff delay stays constant when min and max are equal.")]
    [Trait("Category", "Unit")]
    public void GetNextDelayReturnsSameValueWhenMinEqualsMax()
    {
        // Arrange
        var provider = new OutboxBackoffDelayProvider(
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(50),
            5);

        // Act
        var result = new[]
        {
            provider.GetNextDelay(),
            provider.GetNextDelay(),
            provider.GetNextDelay()
        };

        // Assert
        result.Should().Equal(
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(50));
    }
}
