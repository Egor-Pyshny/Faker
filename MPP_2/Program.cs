using MPP_2.Faker;
using MPP_2.MyGenerator;

FakerConfig config = new FakerConfig();
Faker f = new Faker();
var s = f.Create<TestClass>();
int a = 7;

public class Gen: ICustomGenerator<int>
{

    public int Generate()
    {
        return 5;
    }
}

public class TestClass {

    public int a;
    public int b;
    private int t;
    private int y;
    public A cA;
    public A casA { get; set; }
    public int c { get; private set; }
    public int d { private get; set; }
    private int dg {  get; set; }

    public List<List<A>> l;
    public List<int> aasds;
    public int aasd() { return 5; }
    public TestClass(int a) { this.a = a; }
    public TestClass() { }
}

public class A
{

    public int a;
    public int b;

    public List<A> l;

    public A() { }
}

public class B
{

    public int a;
    public int b;
    public B() { }
}