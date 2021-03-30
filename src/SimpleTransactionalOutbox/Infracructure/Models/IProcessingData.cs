using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infracructure.Models
{
    public interface IProcessingData
    {
        public long Id { get; }

        public int Value { get; }
    }
}
