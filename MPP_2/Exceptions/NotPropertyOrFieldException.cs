using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.Exceptions
{
    public class NotPropertyOrFieldException : Exception
    {
        public NotPropertyOrFieldException(Type selectorType) : base($"{selectorType} is not MemberExpression") { }
    }
}
