using System;

namespace Echo.Core.DependencyInjection
{
    public interface IServiceLocator
    {
        void Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TImplementation : class, TInterface;

        void RegisterInstance<TInterface>(TInterface instance)
            where TInterface : class;

        TInterface Resolve<TInterface>() where TInterface : class;
        object Resolve(Type serviceType);

        bool TryResolve<TInterface>(out TInterface service) where TInterface : class;

        void InjectDependencies(object target);
        void Unregister<TInterface>();
        void Clear();
    }
}
