using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Testss.Addins.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]   
    public class ToFixAttribute : Attribute
    {
    }
}
