using System;

namespace Walmart.Testss.Addins.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RegressionAttribute: Attribute
    {
    }
}
