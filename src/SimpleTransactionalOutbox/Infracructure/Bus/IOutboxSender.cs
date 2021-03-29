using System.Threading;
using System.Threading.Tasks;

using Infracructure.Models;

namespace Infracructure.Bus
{
    public interface IOutboxSender
    {
        public Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
