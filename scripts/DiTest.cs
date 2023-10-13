using Godot;
using Autofac;

public partial class DiTest : Node
{
	public interface IServiceA
	{
		void MethodA();
	}

	public interface IServiceB
	{
		void MethodB();
	}

	public class ServiceA : IServiceA
	{
		public void MethodA()
		{
			GD.Print("ServiceA.MethodA called");
		}
	}

	public class ServiceB : IServiceB
	{
		private IServiceA _serviceA;

		public ServiceB Di(IServiceA d)
		{
			_serviceA = d;
			return this;
		}

		public void MethodB()
		{
			GD.Print("ServiceB.MethodB called");
			_serviceA.MethodA();
		}
	}

	public override void _Ready()
	{
		var builder = new ContainerBuilder();

		builder.RegisterType<ServiceA>().As<IServiceA>();
		// builder.RegisterType<ServiceB>();

		builder.Register(c => new ServiceB().Di(c.Resolve<ServiceA>()));

		var container = builder.Build();

		using (var scope = container.BeginLifetimeScope())
		{ 
			var s = scope.Resolve<ServiceB>();
			s.MethodB();
		}
	}
}

