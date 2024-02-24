using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.Exceptions
{
    public class CustomGeneratorException : Exception
    {
        public CustomGeneratorException(Exception e, Type generatorType) : base($"Exception {e} while using generator {generatorType.FullName}") { }

    }
}
