using MPP_2.Exceptions;
using MPP_2.MyGenerator;
using System.Linq.Expressions;
using System.Reflection;

namespace MPP_2.MyFaker
{
    public class FakerConfig 
    {
        private Dictionary<Type, Dictionary<MemberInfo, Type>> classesConfigs;

        public FakerConfig() => this.classesConfigs = new Dictionary<Type, Dictionary<MemberInfo, Type>>();

        public Dictionary<MemberInfo, Type> GetClassConfig(Type type) => this.classesConfigs[type];

        public void Add<TClass, TField, TGenerator>(Expression<Func<TClass, TField>> fieldSelector) {
            Type classType = typeof(TClass);
            Type generatorType = typeof(TGenerator);
            Type fieldType = typeof(TField);
            
            //check if fieldSelector is field or prop
            MemberInfo? member = (fieldSelector.Body is MemberExpression) ? ((MemberExpression)fieldSelector.Body).Member : throw new NotPropertyOrFieldException(fieldSelector.Type);

            //check if field is TClass field and
            if (member.DeclaringType != classType)
            {
                throw new ClassMemberException(classType, member);
            }

            //check if TGenerator is generator and its type is TField type
            Type[] generatorReturnedTypes = null;
            var intf = generatorType.GetInterface(typeof(ICustomGenerator<>).FullName!);
            if (intf != null)
                generatorReturnedTypes = intf.GetGenericArguments();
            else
                throw new NotGeneratorException(generatorType);
            if (fieldType != generatorReturnedTypes[0]) 
            {
                throw new GeneratorTypeException(generatorReturnedTypes[0], fieldType);
            }

            if (!classesConfigs.ContainsKey(classType)) {
                classesConfigs.Add(classType, new Dictionary<MemberInfo, Type>());
            }
            Dictionary <MemberInfo, Type> classConfig = classesConfigs[classType];
            if (!classConfig.ContainsKey(member)) 
                classConfig.Add(member, generatorType);
            else
                classConfig[member] = generatorType;
        }
    }
}
