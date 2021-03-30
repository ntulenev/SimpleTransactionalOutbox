using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Infracructure.DB;

namespace DB
{
    public abstract class UnitOfWork<TContext> : IUnitOfWork, IDisposable, IAsyncDisposable
        where TContext : DbContext
    {

        public UnitOfWork(
            TContext context,
            IsolationLevel isolationLevel,
            ILogger<UnitOfWork<TContext>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _transaction =
                context.Database
                    .BeginTransaction(isolationLevel);

            _logger.LogInformation("UOF with transaction created.");
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Dispose(true);
            _isDisposed = true;

            _logger.LogInformation("Instance disposed");

            GC.SuppressFinalize(this);
        }

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

            GC.SuppressFinalize(this);
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            throw new NotImplementedException();
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
                _transaction?.Dispose();
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

        private readonly ILogger _logger;
        private IDbContextTransaction _transaction;
        private readonly TContext _context;
        private bool _isDisposed;
    }
}
