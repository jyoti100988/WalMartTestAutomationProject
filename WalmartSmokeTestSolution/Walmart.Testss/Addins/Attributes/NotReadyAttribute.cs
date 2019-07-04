using System;

namespace Walmart.Testss.Addins.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NotReadyAttribute : Attribute
    {
    }
}
