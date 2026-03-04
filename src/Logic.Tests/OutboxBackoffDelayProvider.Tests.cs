using FluentAssertions;

using Xunit;

namespace Logic.Tests;

public class OutboxBackoffDelayProviderTests
{
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
}
