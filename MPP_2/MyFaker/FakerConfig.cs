using MPP_2.MyFaker;
using MPP_2.MyGenerator;
using System.Reflection;

namespace MPP_2.Faker
{
    public class FakerConfig 
    {
        private Dictionary<Type, Dictionary<MemberInfo, ICustomGenerator>> classesConfigs;

        public FakerConfig() => this.classesConfigs = new Dictionary<Type, Dictionary<MemberInfo, ICustomGenerator>>();

        public void Add<TClass, TField, TGenerator>(Func<TClass, MemberInfo> fieldSelector) {
            Type generatorType = typeof(TGenerator);
            if (!(generatorType.GetInterfaces().Contains(typeof(ICustomGenerator)))) {
                throw new Exception("not generator");
            }
            Type memberType = typeof(TField);
            /*if(!(field.MemberType == MemberTypes.Field && (field as FieldInfo).FieldType == memberType))
            {

            }*/
            int akjjm = 12;
        }
    }
}
