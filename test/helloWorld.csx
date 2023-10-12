public class Foo : System.IDisposable
{
	public void Dispose() => Console.WriteLine($"{this.GetHashCode()} is dispose.");
}

using (new Foo())
{
	Console.WriteLine("space");
}
