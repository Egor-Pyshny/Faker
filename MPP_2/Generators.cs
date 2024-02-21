using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPP_2
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


        public static object Generate(Type type) {
            if (type.GetInterfaces().Contains(typeof(IList)))
            {
                var f = type.GetInterfaces();
                foreach (var temp in f)
                {
                    if (temp.Name.Contains("IList") && temp.GenericTypeArguments.Length > 0)
                    {
                        return valueGenerators[typeof(IList)](temp);
                    }
                }
            }
            return valueGenerators[type](type);
        }

        private static object generateInt(Type type) => random.Next();

        private static object generateFloat(Type type) => random.NextSingle();

        private static object generateDouble(Type type) => random.NextDouble();

        private static object generateLong(Type type) => random.NextInt64();

        private static object generateByte(Type type) => (byte)random.Next(0, 256);

        private static object generateSByte(Type type) => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue + 1);

        private static object generateBool(Type type) => (random.Next(2) == 0);

        private static object generateUInt(Type type) => (uint)random.Next();

        private static object generateULong(Type type) => (ulong)random.NextInt64();

        private static object generateDecimal(Type type) => (decimal)random.NextDouble();

        private static object generateChar(Type type) => (char)random.Next(char.MinValue, char.MaxValue + 1);

        private static object generateObject(Type type) => random.Next();

        private static object generateString(Type type) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = random.Next(10, 20);
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static object generateDateTime(Type type) => new DateTime(
                                                            year: random.Next(0,DateTime.Now.Year+1),
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
