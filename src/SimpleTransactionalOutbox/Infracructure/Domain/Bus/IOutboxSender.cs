using System.Threading;
using System.Threading.Tasks;

using Infracructure.Domain.Models;

namespace Infracructure.Domain.Bus
{
    public interface IOutboxSender
    {
        public Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
