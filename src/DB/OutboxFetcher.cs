using Abstractions.DB;
using Abstractions.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DB;

/// <summary>
/// Class that pools Outbox messages for processing.
/// </summary>
public class OutboxFetcher : IOutboxFetcher
{
    /// <summary>
    /// Creates <see cref="OutboxFetcher"/>.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="options">Fetcher options.</param>
    public OutboxFetcher(
        OutboxContext context,
        IOptions<OutboxFetcherOptions> options)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _limit = (options ?? throw new ArgumentNullException(nameof(options))).Value.Limit;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        return await _context.OutboxMessages
                             .OrderBy(x => x.OccurredOn)
                             .Take(_limit)
                             .Select(x => new Models.OutboxMessage(x.MessageId, x.OccurredOn, x.MessageType, x.Body))
                             .ToListAsync(cancellationToken)
                             .ConfigureAwait(false);
    }

    private readonly OutboxContext _context;
    private readonly int _limit;

}
