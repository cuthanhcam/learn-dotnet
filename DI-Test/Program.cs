using DI_Test.Interfaces;
using DI_Test.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DI_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IMySingletonService>(services => new MySingletonService());
            serviceCollection.AddScoped<IMyScopedService>(services => new MyScopedService());
            serviceCollection.AddTransient<IMyTransientService>(services => new MyTransientService());

            var service = serviceCollection.BuildServiceProvider();

            object? obj;

            Console.WriteLine("Get singleton service");
            obj = service.GetService<IMySingletonService>();
            obj = service.GetService<IMySingletonService>();
            obj = service.GetService<IMySingletonService>();

            Console.WriteLine("Get scoped service");
            obj = service.GetService<IMyScopedService>();
            obj = service.GetService<IMyScopedService>();
            obj = service.GetService<IMyScopedService>();

            Console.WriteLine("Get transient service");
            obj = service.GetService<IMyTransientService>();
            obj = service.GetService<IMyTransientService>();
            obj = service.GetService<IMyTransientService>();

            Console.WriteLine();
            Console.WriteLine("--- Create new scope ---");
            Console.WriteLine();

            var scope = service.CreateScope();

            Console.WriteLine("Get singleton service");
            obj = scope.ServiceProvider.GetService<IMySingletonService>();
            obj = scope.ServiceProvider.GetService<IMySingletonService>();
            obj = scope.ServiceProvider.GetService<IMySingletonService>();

            Console.WriteLine("Get scoped service");
            obj = scope.ServiceProvider.GetService<IMyScopedService>();
            obj = scope.ServiceProvider.GetService<IMyScopedService>();
            obj = scope.ServiceProvider.GetService<IMyScopedService>();

            Console.WriteLine("Get transient service");
            obj = scope.ServiceProvider.GetService<IMyTransientService>();
            obj = scope.ServiceProvider.GetService<IMyTransientService>();
            obj = scope.ServiceProvider.GetService<IMyTransientService>();
        }
    }
}