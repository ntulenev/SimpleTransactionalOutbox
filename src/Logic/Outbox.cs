using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Service;

namespace Logic;

/// <summary>
/// Outbox processing logic.
/// </summary>
public class Outbox : IOutbox
{
    /// <summary>
    /// Creates <see cref="Outbox"/>.
    /// </summary>
    /// <param name="fetcher">DB data fetcher.</param>
    /// <param name="scopedFactory">Factory for creating DI Scopes.</param>
    /// <param name="logger">Logger.</param>
    public Outbox(IOutboxFetcher fetcher,
                  IServiceScopeFactory scopedFactory,
                  ILogger<Outbox> logger
                 )
    {
        _fetcher = fetcher ?? throw new ArgumentNullException(nameof(fetcher));
        _scopedFactory = scopedFactory ?? throw new ArgumentNullException(nameof(scopedFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task RunProcessingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Run fetching messages from outbox.");
        var messages = await _fetcher.ReadOutboxMessagesAsync(cancellationToken);

        _logger.LogInformation("Messages count {count}.", messages.Count);
        foreach (var item in messages)
        {
            using var scope = _scopedFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IOutboxMessageProcessor>();

            if (!await processor.TryProcessAsync(item, cancellationToken))
            {
                _logger.LogWarning("Unable to process {@item}. Stopping processing and retry later.", item);
                break;
            }
        }

        _logger.LogInformation("Finish processing messages.");
    }

    private readonly IOutboxFetcher _fetcher;
    private readonly IServiceScopeFactory _scopedFactory;
    private readonly ILogger _logger;

}
