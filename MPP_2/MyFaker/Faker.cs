using MPP_2.MyFaker;
using MPP_2.MyGenerator;

namespace MPP_2.Faker
{
    public class Faker : IFaker
    {
        private readonly FakerConfig? config;
        public T Create<T>() {
            Type type = typeof(T);

            var obj = Generators.GenerateDTO(type, config);

            return (T)obj;
        }

        public Faker() => this.config = null;

        public Faker(FakerConfig config) => this.config = config;
    }
}
