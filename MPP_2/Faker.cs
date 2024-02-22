using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2
{
    public class Faker
    {
        public T Create<T>() {
            Type type = typeof(T);

            var obj = Generators.GenerateDTO(type);

            return (T)obj;
        }
    }
}
