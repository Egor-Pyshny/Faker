using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.Exceptions
{
    public class NotGeneratorException : Exception
    {
        public NotGeneratorException(Type generatorType) : base($"{generatorType.FullName} do not implement ICustomGenerator interface") { }
    }
}
