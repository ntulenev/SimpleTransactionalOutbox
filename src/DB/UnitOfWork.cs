using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Abstractions.DB;

namespace DB;

/// <summary>
/// Base UnitOfWork class.
/// </summary>
/// <typeparam name="TContext"></typeparam>
public abstract class UnitOfWork<TContext> :
    IUnitOfWork,
    IDisposable,
    IAsyncDisposable
    where TContext : DbContext
{
    /// <summary>
    /// Creates <see cref="UnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="isolationLevel">Transaction isolation level.</param>
    /// <param name="logger">Logger.</param>
    public UnitOfWork(
        TContext context,
        IsolationLevel? isolationLevel,
        ILogger<UnitOfWork<TContext>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (isolationLevel is not null)
        {
            _transaction =
                context.Database
                    .BeginTransaction(isolationLevel.Value);
        }
        _logger.LogInformation("UOF with transaction created.");
    }

    /// <summary>
    /// Disposes unit of work.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Dispose(true);
        _isDisposed = true;

        _logger.LogInformation("Instance disposed.");

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes unit of work is async way.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        await DisposeCoreAsync().ConfigureAwait(false);

        Dispose(false);

        _isDisposed = true;

        _logger.LogInformation("Instance disposed");

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        cancellationToken.ThrowIfCancellationRequested();

        using var loggingScope = _logger.BeginScope("Starting save changes.");

        try
        {
            _ = await _context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("Changes has been saved.");

            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken)
                                  .ConfigureAwait(false);

                _logger.LogInformation("Transaction committed successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on saving data in database.");
            throw;
        }
    }

    protected void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_transaction is not null)
            {
                _transaction?.Dispose();
            }
        }

        _transaction = null!;
    }

    protected async virtual ValueTask DisposeCoreAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync()
                              .ConfigureAwait(false);
        }

        _transaction = null!;
    }

    protected readonly ILogger _logger;
    protected readonly TContext _context;
    private IDbContextTransaction? _transaction;
    private bool _isDisposed;
}
