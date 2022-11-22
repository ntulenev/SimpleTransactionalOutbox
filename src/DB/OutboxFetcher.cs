using Microsoft.EntityFrameworkCore;

using Abstractions.DB;
using Abstractions.Models;

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
    public OutboxFetcher(
        OutboxContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
                             .OrderBy(x => x.OccurredOn)
                             .Take(LIMIT)
                             .Select(x => new Models.OutboxMessage(x.MessageId, x.OccurredOn, x.MessageType, x.Body))
                             .ToListAsync(cancellationToken)
                             .ConfigureAwait(false);
    }

    private readonly OutboxContext _context;

    private const int LIMIT = 10;

}
