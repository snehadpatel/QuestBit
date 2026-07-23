using System;

namespace Echo.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
    }
}
