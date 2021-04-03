using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.Bus
{
    /// <summary>
    /// Message publisher.
    /// </summary>
    public interface IOutboxSender
    {
        /// <summary>
        /// Publishes <paramref name="message"/> to external system.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Canellation token.</param>
        public Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
