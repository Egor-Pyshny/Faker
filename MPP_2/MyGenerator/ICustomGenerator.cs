using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.MyGenerator
{
    public interface ICustomGenerator
    {
        T Generate<T>();
    }
}
