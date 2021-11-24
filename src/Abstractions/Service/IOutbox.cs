namespace Abstractions.Service
{
    /// <summary>
    /// Outbox service interface.
    /// </summary>
    public interface IOutbox
    {
        /// <summary>
        /// Processes outbox work phase.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task RunProcessingAsync(CancellationToken cancellationToken);
    }
}
