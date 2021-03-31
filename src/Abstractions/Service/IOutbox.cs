using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.Service
{
    public interface IOutbox
    {
        public Task RunProcessingAsync(CancellationToken cancellationToken);
    }
}
