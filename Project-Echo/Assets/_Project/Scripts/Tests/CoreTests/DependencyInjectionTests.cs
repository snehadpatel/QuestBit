using Echo.Core.DependencyInjection;
using NUnit.Framework;

namespace Echo.Tests.CoreTests
{
    public class DependencyInjectionTests
    {
        private interface ITestService
        {
            string GetName();
        }

        private class TestService : ITestService
        {
            public string GetName() => "EchoService";
        }

        private class InjectTarget
        {
            [Inject] public ITestService Service;
        }

        [Test]
        public void ServiceContainer_RegisterAndResolve_ReturnsSingletonInstance()
        {
            var container = new ServiceContainer();
            container.Register<ITestService, TestService>(ServiceLifetime.Singleton);

            var instance1 = container.Resolve<ITestService>();
            var instance2 = container.Resolve<ITestService>();

            Assert.IsNotNull(instance1);
            Assert.AreEqual("EchoService", instance1.GetName());
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void ServiceContainer_InjectDependencies_PopulatesField()
        {
            var container = new ServiceContainer();
            container.Register<ITestService, TestService>(ServiceLifetime.Singleton);

            var target = new InjectTarget();
            container.InjectDependencies(target);

            Assert.IsNotNull(target.Service);
            Assert.AreEqual("EchoService", target.Service.GetName());
        }
    }
}
