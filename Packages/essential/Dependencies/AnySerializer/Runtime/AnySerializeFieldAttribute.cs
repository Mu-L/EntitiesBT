using System;

namespace AnySerializer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AnySerializeFieldAttribute : Attribute
    {
    }
}