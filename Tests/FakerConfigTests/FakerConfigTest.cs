using FluentAssertions;
using MPP_2.Exceptions;
using MPP_2.Faker;

namespace Tests.FakerConfigTests
{
    [TestClass]
    public class FakerConfigTest
    {
        [TestMethod]
        public void ClassMemberExceptionTest()
        {
            Action action = () => { 
                FakerConfig fakerConfig = new FakerConfig();
                TestClass1 t = new TestClass1();
                fakerConfig.Add<TestClass, int, GenInt>(test => t.f1);
            };
            action.Should().Throw<ClassMemberException>();
        }

        [TestMethod]
        public void NotPropertyOrFieldExceptionTest()
        {
            Action action = () => {
                FakerConfig fakerConfig = new FakerConfig();
                fakerConfig.Add<TestClass, int, GenInt>(test => test.proc());
            };
            action.Should().Throw<NotPropertyOrFieldException>();
        }

        [TestMethod]
        public void NotGeneratorExceptionTest()
        {
            Action action = () => {
                FakerConfig fakerConfig = new FakerConfig();
                fakerConfig.Add<TestClass, int, TestClass1>(test => test.f1);
            };
            action.Should().Throw<NotGeneratorException>();
        }

        [TestMethod]
        public void GeneratorTypeExceptionTest()
        {
            Action action = () => {
                FakerConfig fakerConfig = new FakerConfig();
                fakerConfig.Add<TestClass, int, GenString>(test => test.f1);
            };
            action.Should().Throw<GeneratorTypeException>();
        }


        [TestMethod]
        public void CommonUseTest()
        {
            FakerConfig fakerConfig = new FakerConfig();
            fakerConfig.Add<TestClass, int, GenInt>(test => test.f1);
            fakerConfig.Add<TestClass, int, GenInt>(test => test.f2);
            fakerConfig.Add<TestClass, string, GenString>(test => test.f3);
            fakerConfig.Add<TestClass, TestClass1, GenTestClass1>(test => test.f4);
            Faker faker = new Faker(fakerConfig);
            TestClass obj = faker.Create<TestClass>();
            obj.f1.Should().Be(42);
            obj.f2.Should().Be(42);
            obj.f3.Should().Be("42");
            obj.f4.f1.Should().Be(42);
            obj.f4.f2.Should().Be(null);
        }
    }
}
