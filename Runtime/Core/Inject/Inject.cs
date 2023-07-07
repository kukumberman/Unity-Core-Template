using System;

namespace Injection
{
    // csharpier-ignore
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class Inject : Attribute
    {
        public readonly string Name;

        public Inject()
        {
            Name = null;
        }

        public Inject(string name)
        {
            Name = name;
        }
    }
}
