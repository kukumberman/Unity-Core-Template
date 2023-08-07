using System;

namespace Injection
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
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
