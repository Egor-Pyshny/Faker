using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2.MyFaker
{
    public interface IFakerConfig
    {
        void Add<TClass, TField, TICustomGenerator>(Func<TClass, TField> fieldSelector);
    }
}
