// See https://aka.ms/new-console-template for more information

using MPP_2;

Faker f = new Faker();
f.Create<TestClass>();
public class TestClass {

    public int a;
    public int b;
    public int c { get; private set; }
    public int d { private get; set; }

    public List<List<int>> l;

    public TestClass() { }
}