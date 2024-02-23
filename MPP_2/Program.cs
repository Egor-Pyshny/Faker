using MPP_2.Faker;
using MPP_2.MyGenerator;


FakerConfig config = new FakerConfig();
config.Add<TestClass, int, Gen>(testc => testc.a);
Faker f = new Faker(config);
f.Create<TestClass>();

public class Gen: ICustomGenerator<int>
{

    public int Generate()
    {
        return (int)(object)5;
    }
}

public class TestClass {

    public int a;
    public int b;
    private int t;
    private int y;
    private A cA;
    public int c { get; private set; }
    public int d { private get; set; }
    private int dg {  get; set; }

    public List<A> l;
    public List<int> aasds;
    public int aasd() { return 5; }
    public TestClass(A a) { this.cA = a; }
    public TestClass() { }
}

public class A
{

    public int a;
    public int b;
    public A() { }
}