using Abstractions.Models;

namespace Abstractions.DB
{
    /// <summary>
    /// <see cref="IUnitOfWork"/> for processing logic.
    /// </summary>
    public interface IProcessingDataUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Addes new <paramref name="data"/> to the stotage.
        /// </summary>
        /// <param name="data">Processing data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
