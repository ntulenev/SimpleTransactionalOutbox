namespace DB;

/// <summary>
/// Options for <see cref="OutboxFetcher"/>.
/// </summary>
public class OutboxFetcherOptions
{
    /// <summary>
    /// Maximum number of messages fetched in one query.
    /// </summary>
    public int Limit { get; init; } = 10;
}
