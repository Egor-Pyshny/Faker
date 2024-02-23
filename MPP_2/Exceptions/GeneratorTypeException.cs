using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.Exceptions
{
    public class GeneratorTypeException : Exception
    {
        public GeneratorTypeException(Type generatorType, Type memberType) : base($"Generator actual returned type: {generatorType.FullName}, expected: {memberType.FullName}") { }
    }
}
