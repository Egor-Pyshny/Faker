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
        private void CreateInstance<T>(ref T o, Type type) { 
            
        }

        public T Create<T>() {
            Type type = typeof(T);

            Type t = typeof(List<>);
            var aasdasd = t.Assembly.FullName;
            var s = t.GetInterfaces();
            var dasd = type.GetMembers();
            Type t1 = typeof(List<int>);
            var s1 = t.GetInterfaces();

            ConstructorInfo constructor = null;
            int constrParams = 0;
            foreach (var constr in type.GetConstructors()) {
                if(constr.IsPublic && constr.GetParameters().Length >= constrParams) constructor = constr;
            }
            if (constructor == null) throw new Exception("asd");

            List<FieldInfo> fields = new List<FieldInfo>();
            foreach (var field in type.GetFields())
            {
                if(field.IsPublic) fields.Add(field);
            }

            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach (var propertie in type.GetProperties())
            {
                if(propertie.GetMethod!.IsPublic) properties.Add(propertie);
            }

            List<FieldInfo> fieldErrors = new List<FieldInfo>();
            T a = (T)constructor.Invoke(null);
            foreach (var field in fields) {
                try
                {
                    field.SetValue(a, Generators.Generate(field.FieldType));
                }
                catch (KeyNotFoundException) {
                    fieldErrors.Add(field);
                }
            }

            List<PropertyInfo> propErrors = new List<PropertyInfo>();
            foreach (var prop in properties) {
                try
                {
                    prop.SetValue(a, Generators.Generate(prop.PropertyType));
                }
                catch (KeyNotFoundException) {
                    propErrors.Add(prop);
                }
            }

            foreach (var member in fieldErrors) {
                if (member.FieldType.Assembly.FullName.Contains("System."))
                {
                    throw new NotImplementedException($"type {member.FieldType} not implemented");
                }
                else {
                    CreateInstance<T>(ref a, member.FieldType);
                }
            }

            return (T)new object();
        }
    }
}
