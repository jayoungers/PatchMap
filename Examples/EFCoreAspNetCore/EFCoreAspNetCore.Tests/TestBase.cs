using EFCoreAspNetCore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EFCoreAspNetCore.Tests
{
    public class TestBase : IDisposable
    {
        static TestBase()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var services = new ServiceCollection();

            Application.AddServices(services, configuration);
            Application.Configure(services.BuildServiceProvider());
        }

        protected readonly IServiceScope scope;
        protected readonly ExampleContext dbContext;

        public TestBase()
        {
            scope = Application.ServiceProvider.GetService<IServiceScopeFactory>().CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ExampleContext>();
        }

        public void DatesAreSimilar(DateTimeOffset expected, DateTimeOffset actual)
        {
            Assert.IsTrue(Math.Abs(expected.Subtract(actual).Seconds) < 5);
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
