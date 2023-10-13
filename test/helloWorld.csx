using Microsoft.Extensions.DependencyInjection;
using System;

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
        Console.WriteLine("ServiceA.MethodA called");
    }
}

public class ServiceB : IServiceB
{
    private readonly IServiceA _serviceA;

    public ServiceB(IServiceA serviceA)
    {
        _serviceA = serviceA;
    }

    public void MethodB()
    {
        Console.WriteLine("ServiceB.MethodB called");
        _serviceA.MethodA(); // 使用注入的服务
    }
}

var services = new ServiceCollection();

// 注册服务
services.AddTransient<IServiceA, ServiceA>();
services.AddTransient<IServiceB, ServiceB>();

var serviceProvider = services.BuildServiceProvider();

// 通过IServiceProvider获取和解析服务实例
var serviceA = serviceProvider.GetService<IServiceA>();
var serviceB = serviceProvider.GetService<IServiceB>();

// 使用服务实例
serviceA.MethodA();
serviceB.MethodB();