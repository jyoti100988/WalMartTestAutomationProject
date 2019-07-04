using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Framework.Controls
{
    public class BaseControl
    {
        public BaseControl(DriverScope browser, ElementScope root)
        {
            Browser = browser;
            Root = root;
        }

        public bool Exists
        {
            get { return Root.Exists(new Options() { Timeout = TimeSpan.FromSeconds(15) }); }
        }

        public ElementScope Root { get; set; }

        protected DriverScope Browser { get; set; }
    }
}
