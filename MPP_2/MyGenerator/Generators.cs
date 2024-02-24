using System.Collections;
using System.Reflection;
using System.Reflection.Metadata;
using MPP_2.Exceptions;
using MPP_2.Faker;
using MPP_2.MyFaker;

namespace MPP_2.MyGenerator
{
    public static class Generators
    {
        private static Random random = new Random();

        private delegate object Generator(Type type);

        private static readonly Dictionary<Type, Generator> valueGenerators = new Dictionary<Type, Generator>()
        {
            { typeof(int), generateInt},
            { typeof(float), generateFloat},
            { typeof(double), generateDouble},
            { typeof(long), generateLong},
            { typeof(byte), generateByte},
            { typeof(sbyte), generateSByte},
            { typeof(bool), generateBool},
            { typeof(uint), generateUInt},
            { typeof(ulong), generateULong},
            { typeof(decimal), generateDecimal},
            { typeof(char), generateChar},
            { typeof(object), generateObject},
            { typeof(string), generateString},
            { typeof(DateTime), generateDateTime},
            { typeof(IList), generateList},
        };


        private static object Generate(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IList)))
            {
                var f = type.GetInterfaces();
                foreach (var temp in f)
                {
                    if (temp.Name.Contains("IList`1") && temp.GenericTypeArguments.Length > 0)
                    {
                        return valueGenerators[typeof(IList)](temp);
                    }
                }
            }
            return valueGenerators[type](type);
        }

        public static object GenerateDTO(Type type, Dictionary<MemberInfo, Type>? config = null)
        {
            HashSet<Type> usedtypes = [];
            if (config == null) config = new Dictionary<MemberInfo, Type>();
            object InnerGenerator(Type type, bool considerType = true)
            {
                if (considerType && !usedtypes.Add(type) ) throw new CyclicDependenceException(usedtypes, type);
                ConstructorInfo? constructor = null;
                var asdasd = type.GetMembers();
                var privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => !field.Name.Contains(">k__BackingField")).ToList();
                var privateProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Concat(type.GetProperties().Where(prop => prop.SetMethod == null || prop.SetMethod != null && !prop.SetMethod.IsPublic).ToList()).ToList();
                var privateMembers = privateFields.Select(member => member.Name.ToLower()).ToList().Union(privateProperties.Select(member => member.Name.ToLower()).ToList()).ToList();
                int privateMembersMaxAmount = -1;
                foreach (var constr in type.GetConstructors())
                {
                    int privateMembersAmount = 0;
                    foreach (var param in constr.GetParameters())
                    {
                        if (privateMembers.Contains(param.Name!.ToLower())) privateMembersAmount++;
                    }
                    if (constr.IsPublic && privateMembersAmount > privateMembersMaxAmount)
                    {
                        privateMembersMaxAmount = privateMembersAmount;
                        constructor = constr;
                    }
                }
                if (constructor == null) throw new NoPublicConstructorException(type);
                List<object> parameters = [];
                foreach (var parameter in constructor.GetParameters())
                {
                    try
                    {
                        var mList = config!.Keys.Where(member => member.Name == parameter.Name).ToList();
                        if (mList.Count == 1)
                        {
                            var m = mList[0];
                            var gen = Activator.CreateInstance(config[m]);
                            Type typeOfT = m.MemberType == MemberTypes.Field ? (m as FieldInfo)!.FieldType : (m as PropertyInfo)!.PropertyType;
                            MethodInfo generateTypedMethod = config[m].GetMethod("Generate")!;
                            parameters.Add(generateTypedMethod.Invoke(gen, null)!);
                        }
                        else
                            parameters.Add(Generate(parameter.ParameterType));
                    }
                    catch (KeyNotFoundException)
                    {
                        parameters.Add(InnerGenerator(parameter.ParameterType));
                    }
                }
                var newDTO = constructor.Invoke(parameters.ToArray());
                /*List<FieldInfo> fields = new List<FieldInfo>();
                foreach (var field in type.GetFields())
                {
                    if (field.IsPublic) fields.Add(field);
                }
                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (var propertie in type.GetProperties())
                {
                    if (propertie.SetMethod!= null && propertie.SetMethod.IsPublic) properties.Add(propertie);
                }*/

                List<MemberInfo> errors = new List<MemberInfo>();
                var publicMembers = type.GetMembers().Where(_member => 
                (_member.MemberType == MemberTypes.Field && (_member as FieldInfo).IsPublic) || 
                (_member.MemberType == MemberTypes.Property && (_member as PropertyInfo).SetMethod != null && (_member as PropertyInfo).SetMethod.IsPublic))
                    .ToList();
                foreach (var member in publicMembers) {
                    try
                    {
                        var mList = config!.Keys.Where(_member => _member.Name == member.Name).ToList();
                        object value;
                        if (mList.Count == 1)
                        {
                            var m = mList[0];
                            var gen = Activator.CreateInstance(config[m]);
                            Type typeOfT = m.MemberType == MemberTypes.Field ? (m as FieldInfo)!.FieldType : (m as PropertyInfo)!.PropertyType;
                            MethodInfo generateTypedMethod = config[m].GetMethod("Generate")!;
                            try
                            {
                                if (member.MemberType == MemberTypes.Field)
                                    (member as FieldInfo).SetValue(newDTO, generateTypedMethod.Invoke(gen, null));
                                else
                                    (member as PropertyInfo).SetValue(newDTO, generateTypedMethod.Invoke(gen, null));
                            }catch (Exception ex)
                            {
                                throw new CustomGeneratorException(ex, config[m]);
                            }
                        }
                        else
                        {
                            if (member.MemberType == MemberTypes.Field)
                                (member as FieldInfo).SetValue(newDTO, Generate((member as FieldInfo).FieldType));
                            else
                                (member as PropertyInfo).SetValue(newDTO, Generate((member as PropertyInfo).PropertyType));
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        errors.Add(member);
                    }
                }
                /*                foreach (var field in fields)
                                {
                                    try
                                    {
                                        var mList = config!.Keys.Where(member => member.Name == field.Name).ToList();
                                        if (mList.Count == 1) {
                                            var m = mList[0];
                                            var gen = Activator.CreateInstance(config[m]);
                                            Type typeOfT = m.MemberType == MemberTypes.Field ? (m as FieldInfo)!.FieldType : (m as PropertyInfo)!.PropertyType;
                                            MethodInfo generateTypedMethod = config[m].GetMethod("Generate")!;
                                            field.SetValue(a, generateTypedMethod.Invoke(gen, null));
                                        }
                                        else
                                            field.SetValue(a, Generate(field.FieldType));
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        errors.Add(field);
                                    }
                                }
                                foreach (var prop in properties)
                                {
                                    try
                                    {
                                        var mList = config!.Keys.Where(member => member.Name == prop.Name).ToList();
                                        if (mList.Count == 1)
                                        {
                                            var m = mList[0];
                                            var gen = Activator.CreateInstance(config[m]);
                                            Type typeOfT = m.MemberType == MemberTypes.Field ? (m as FieldInfo)!.FieldType : (m as PropertyInfo)!.PropertyType;
                                            MethodInfo generateTypedMethod = config[m].GetMethod("Generate")!;
                                            prop.SetValue(a, generateTypedMethod.Invoke(gen, null));
                                        }
                                        else
                                            prop.SetValue(a, Generate(prop.PropertyType));
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        errors.Add(prop);
                                        object obj;
                                        if (prop.PropertyType.FullName!.Contains("System."))
                                            throw new NotImplementedException($"Generator for type {prop.PropertyType} has not been implemented yet");
                                        else
                                            obj = GenerateDTO(prop.PropertyType);
                                        prop.SetValue(a, obj);
                                    }
                                }
                */

                foreach (MemberInfo member in errors)
                {
                    Type typeOfMember = member.MemberType == MemberTypes.Field ? (member as FieldInfo)!.FieldType : (member as PropertyInfo)!.PropertyType;
                    Type intf = typeOfMember.GetInterface("IList`1");
                    if (intf != null)
                    {
                        Type genericType = intf.GenericTypeArguments[0];
                        while ((intf = intf.GenericTypeArguments[0].GetInterface("IList`1")) != null) { genericType = intf.GenericTypeArguments[0]; }
                        usedtypes.Add(genericType);
                        object ListGenerator(Type type) {
                            object? obj = null;
                            int length = random.Next(3, 6);
                            Type listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);
                            var res = (IList)Convert.ChangeType(Activator.CreateInstance(listType), listType)!;
                            for (int i = 0; i < length; i++)
                            {
                                if (type.GenericTypeArguments[0].GetInterface("IList`1") != null)
                                {
                                    obj = ListGenerator(type.GenericTypeArguments[0]);
                                }
                                else
                                {
                                    obj = InnerGenerator(type.GenericTypeArguments[0], false);
                                }
                                Convert.ChangeType(obj, type.GenericTypeArguments[0]);
                                res.Add(obj);
                            }
                            return res;
                        }
                        if (member.MemberType == MemberTypes.Field)
                            (member as FieldInfo).SetValue(newDTO, ListGenerator((member as FieldInfo).FieldType));
                        else
                            (member as PropertyInfo).SetValue(newDTO, ListGenerator((member as PropertyInfo).PropertyType));
                    }
                    else {
                        if (member.MemberType == MemberTypes.Field)
                            (member as FieldInfo).SetValue(newDTO, InnerGenerator((member as FieldInfo).FieldType));
                        else
                            (member as PropertyInfo).SetValue(newDTO, InnerGenerator((member as PropertyInfo).PropertyType));
                    }
                }
                return newDTO;
            }
            if (type.Assembly.FullName!.Contains("System."))
                return Generate(type);
            else
                return InnerGenerator(type);
        }

        private static object generateInt(Type type) => random.Next();

        private static object generateFloat(Type type) => random.NextSingle();

        private static object generateDouble(Type type) => random.NextDouble();

        private static object generateLong(Type type) => random.NextInt64();

        private static object generateByte(Type type) => (byte)random.Next(0, 256);

        private static object generateSByte(Type type) => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue + 1);

        private static object generateBool(Type type) => random.Next(2) == 0;

        private static object generateUInt(Type type) => (uint)random.Next();

        private static object generateULong(Type type) => (ulong)random.NextInt64();

        private static object generateDecimal(Type type) => (decimal)random.NextDouble();

        private static object generateChar(Type type) => (char)random.Next(char.MinValue, char.MaxValue + 1);

        private static object generateObject(Type type) => random.Next();

        private static object generateString(Type type)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = random.Next(10, 20);
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static object generateDateTime(Type type) => new DateTime(
                                                            year: random.Next(0, DateTime.Now.Year + 1),
                                                            month: random.Next(0, 13),
                                                            day: random.Next(0, 28),
                                                            hour: random.Next(0, 25),
                                                            minute: random.Next(0, 61),
                                                            second: random.Next(0, 61)
                                                            );

        private static object generateURL(Type type)
        {
            string start = "http://herecanbeyouradd.com/";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/";
            int length = random.Next(10, 20);
            var str = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return start + str;
        }

        private static object generateList(Type type)
        {
            object? obj = null;
            int length = random.Next(3, 6);
            Type listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);
            var res = (IList)Convert.ChangeType(Activator.CreateInstance(listType), listType)!;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    obj = Generate(type.GenericTypeArguments[0]);
                }
                catch (KeyNotFoundException) {
                    var temp = type.GetInterfaces().Where(interf => interf.Name.Contains("ILisy`")).ToList();
                    if (type.GenericTypeArguments[0].FullName!.Contains("System.") && temp.Count == 1)
                        throw new NotImplementedException($"Generator for type {type} has not been implemented yet");
                    throw new KeyNotFoundException();
                }
                Convert.ChangeType(obj, type.GenericTypeArguments[0]);
                res.Add(obj);
            }
            return res;
        }
    }
}
