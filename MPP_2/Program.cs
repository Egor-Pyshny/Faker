// See https://aka.ms/new-console-template for more information

using MPP_2;

Faker f = new Faker();
f.Create<TestClass>();
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

    public TestClass(A a) { this.cA = a; }
    public TestClass() { }
}

public class A
{

    public int a;
    public int b;

    public A() { }
}