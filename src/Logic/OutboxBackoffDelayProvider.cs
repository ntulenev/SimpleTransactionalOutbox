using Abstractions.Service;

namespace Logic;

/// <summary>
/// Produces a linear backoff sequence for outbox polling.
/// </summary>
public class OutboxBackoffDelayProvider : IOutboxBackoffDelayProvider
{
    /// <summary>
    /// Creates <see cref="OutboxBackoffDelayProvider"/>.
    /// </summary>
    /// <param name="minDelay">Minimum delay value.</param>
    /// <param name="maxDelay">Maximum delay value.</param>
    /// <param name="stepsCount">Number of empty attempts required to reach the maximum delay.</param>
    public OutboxBackoffDelayProvider(TimeSpan minDelay, TimeSpan maxDelay, int stepsCount)
    {
        if (minDelay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(minDelay), "Minimum delay should be positive.");
        }

        if (maxDelay < minDelay)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDelay), "Maximum delay should be greater than or equal to minimum delay.");
        }

        if (stepsCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stepsCount), "Steps count should be positive.");
        }

        _minDelay = minDelay;
        _maxDelay = maxDelay;
        _stepsCount = stepsCount;
    }

    /// <inheritdoc/>
    public TimeSpan GetNextDelay()
    {
        lock (_sync)
        {
            var delay = CalculateDelay(_emptyAttempts);

            if (_emptyAttempts < _stepsCount)
            {
                _emptyAttempts++;
            }

            return delay;
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (_sync)
        {
            _emptyAttempts = 0;
        }
    }

    private TimeSpan CalculateDelay(int emptyAttempts)
    {
        if (_minDelay == _maxDelay || emptyAttempts >= _stepsCount)
        {
            return _maxDelay;
        }

        var ratio = (double)emptyAttempts / _stepsCount;
        var ticks = _minDelay.Ticks + (long)Math.Round((_maxDelay.Ticks - _minDelay.Ticks) * ratio);
        return TimeSpan.FromTicks(ticks);
    }

    private int _emptyAttempts;
    private readonly int _stepsCount;
    private readonly Lock _sync = new();
    private readonly TimeSpan _minDelay;
    private readonly TimeSpan _maxDelay;
}
