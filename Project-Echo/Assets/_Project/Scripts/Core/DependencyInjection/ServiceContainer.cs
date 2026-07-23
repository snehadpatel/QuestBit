using System;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.Core.DependencyInjection
{
    public class ServiceContainer : IServiceLocator
    {
        private class ServiceDescriptor
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
            public object Instance { get; set; }
        }

        private readonly Dictionary<Type, ServiceDescriptor> _descriptors = new Dictionary<Type, ServiceDescriptor>();
        private static ServiceContainer _globalInstance;

        public static ServiceContainer Global => _globalInstance ??= new ServiceContainer();

        public void Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TImplementation : class, TInterface
        {
            var serviceType = typeof(TInterface);
            _descriptors[serviceType] = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = typeof(TImplementation),
                Lifetime = lifetime,
                Instance = null
            };
        }

        public void RegisterInstance<TInterface>(TInterface instance) where TInterface : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            var serviceType = typeof(TInterface);
            _descriptors[serviceType] = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = instance.GetType(),
                Lifetime = ServiceLifetime.Singleton,
                Instance = instance
            };
        }

        public TInterface Resolve<TInterface>() where TInterface : class
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        public object Resolve(Type serviceType)
        {
            if (!_descriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service of type {serviceType.FullName} is not registered.");
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.Instance != null)
            {
                return descriptor.Instance;
            }

            var instance = CreateInstance(descriptor.ImplementationType);

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                descriptor.Instance = instance;
            }

            return instance;
        }

        public bool TryResolve<TInterface>(out TInterface service) where TInterface : class
        {
            if (_descriptors.ContainsKey(typeof(TInterface)))
            {
                service = Resolve<TInterface>();
                return true;
            }

            service = null;
            return false;
        }

        public void InjectDependencies(object target)
        {
            if (target == null) return;

            var targetType = target.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in targetType.GetFields(flags))
            {
                if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                {
                    var resolvedValue = Resolve(field.FieldType);
                    field.SetValue(target, resolvedValue);
                }
            }

            foreach (var prop in targetType.GetProperties(flags))
            {
                if (prop.CanWrite && Attribute.IsDefined(prop, typeof(InjectAttribute)))
                {
                    var resolvedValue = Resolve(prop.PropertyType);
                    prop.SetValue(target, resolvedValue);
                }
            }
        }

        public void Unregister<TInterface>()
        {
            _descriptors.Remove(typeof(TInterface));
        }

        public void Clear()
        {
            _descriptors.Clear();
        }

        private object CreateInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();
            if (constructors.Length == 0)
            {
                return Activator.CreateInstance(implementationType);
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = Resolve(parameters[i].ParameterType);
            }

            var instance = constructor.Invoke(args);
            InjectDependencies(instance);
            return instance;
        }
    }
}
