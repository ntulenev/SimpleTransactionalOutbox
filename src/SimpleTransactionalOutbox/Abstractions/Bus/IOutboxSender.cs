using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.Bus
{
    public interface IOutboxSender
    {
        public Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
