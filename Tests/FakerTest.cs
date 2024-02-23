using FluentAssertions;
using MPP_2.Exceptions;
using MPP_2.Faker;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class FakerTest
    {
        /*
        тесты на цикл \ выбор конструктора \на обычную раюоту с вложенными полями
        */
        [TestMethod]
        public void CycleDetectionTest()
        {
            Action action = () => {
                Faker f = new Faker();
                f.Create<HelperClasses.CycleTestClass>();
            };
            action.Should().Throw<CyclicDependenceException>();
        }

        [TestMethod]
        public void ConstructorSelectionTest()
        {
            Faker f = new Faker();
            HelperClasses.ConstructorClass obj = f.Create<HelperClasses.ConstructorClass>();
            obj.Check();
        }

        [TestMethod]
        public void CommonClassTest()
        {
            Faker f = new Faker();
            HelperClasses.CommonClass obj = f.Create<HelperClasses.CommonClass>();
            void ReverseCheck(Type type) {
                var fieldsAndProps = type.GetMembers().Where(member => member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field).ToList();
            }
        }
    }
}