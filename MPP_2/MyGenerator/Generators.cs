using System.Collections;
using System.Reflection;
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

        public static object GenerateDTO(Type type, FakerConfig? config = null)
        {
            HashSet<Type> usedtypes = [];

            object InnerGenerator(Type type, bool considerType = true)
            {
                if (!usedtypes.Add(type) && considerType) throw new Exception("LoopDetected");
                ConstructorInfo constructor = null;
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
                if (constructor == null) throw new Exception("no public constructor");
                List<object> parameters = [];
                foreach (var parameter in constructor.GetParameters())
                {
                    try
                    {
                        parameters.Add(Generate(parameter.ParameterType));
                    }
                    catch (KeyNotFoundException)
                    {
                        parameters.Add(InnerGenerator(parameter.ParameterType));
                    }
                }
                var a = constructor.Invoke(parameters.ToArray());
                List<FieldInfo> fields = new List<FieldInfo>();
                foreach (var field in type.GetFields())
                {
                    if (field.IsPublic) fields.Add(field);
                }

                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (var propertie in type.GetProperties())
                {
                    if (propertie.GetMethod!.IsPublic) properties.Add(propertie);
                }

                List<FieldInfo> fieldErrors = new List<FieldInfo>();
                foreach (var field in fields)
                {
                    try
                    {
                        field.SetValue(a, Generate(field.FieldType));
                    }
                    catch (KeyNotFoundException)
                    {
                        fieldErrors.Add(field);
                    }
                }

                List<PropertyInfo> propErrors = new List<PropertyInfo>();
                foreach (var prop in properties)
                {
                    try
                    {
                        prop.SetValue(a, Generate(prop.PropertyType));
                    }
                    catch (KeyNotFoundException)
                    {
                        propErrors.Add(prop);
                    }
                }

                foreach (var member in fieldErrors)
                {
                    if (member.FieldType.GetInterfaces().Contains(typeof(IList)))
                    {
                        var f = member.FieldType.GetInterfaces();
                        foreach (var temp in f)
                        {
                            if (temp.Name.Contains("IList`1") && temp.GenericTypeArguments.Length > 0 && !temp.GenericTypeArguments[0].FullName.Contains("System."))
                            {
                                Random random = new Random();
                                object obj = new object();
                                int length = random.Next(3, 6);
                                Type listType = typeof(List<>).MakeGenericType(temp.GenericTypeArguments[0]);
                                var res = (IList)Convert.ChangeType(Activator.CreateInstance(listType), listType);
                                if (!usedtypes.Contains(temp.GenericTypeArguments[0])) res.Add(InnerGenerator(temp.GenericTypeArguments[0]));
                                for (int i = 0; i < length; i++)
                                {
                                    obj = InnerGenerator(temp.GenericTypeArguments[0], false);
                                    Convert.ChangeType(obj, temp.GenericTypeArguments[0]);
                                    res.Add(obj);
                                }
                                member.SetValue(a, res);
                            }
                        }
                    }
                    else if (member.FieldType.Assembly.FullName.Contains("System."))
                    {
                        throw new NotImplementedException($"type {member.FieldType} not implemented");
                    }
                    else
                    {
                        member.SetValue(a, InnerGenerator(member.FieldType));
                    }
                }
                return a;
            }
            if (type.Assembly.FullName.Contains("System."))
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
            int length = random.Next(10, 20);
            Type listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);
            var res = (IList)Convert.ChangeType(Activator.CreateInstance(listType), listType);
            for (int i = 0; i < length; i++)
            {
                obj = Generate(type.GenericTypeArguments[0]);
                Convert.ChangeType(obj, type.GenericTypeArguments[0]);
                res.Add(obj);
            }
            return res;
        }
    }
}
