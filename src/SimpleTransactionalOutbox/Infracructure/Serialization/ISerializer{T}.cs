using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infracructure.Serialization
{
    public interface ISerializer<T>
    {
        public string Serialize(T obj);
    }
}
