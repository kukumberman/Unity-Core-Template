using System;

namespace Injection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Injectable : Attribute
    {
        public readonly Type Type;

        public Injectable(Type type)
        {
            Type = type;
        }
    }
}
