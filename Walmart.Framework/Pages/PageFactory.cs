using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Framework.Pages
{
    public static class PageFactory
    {
        public static T Create<T>(DriverScope browser) where T : IPage
        {
            return (T)Activator.CreateInstance(typeof(T), browser);
        }
    }
}
