using System;
using System.Linq;
using Walmart.Framework.Controls;
using Coypu;
using OpenQA.Selenium;

namespace Walmart.Framework.Control.Dropdown
{
    public class Dropdown : BaseControl
    {
        public Dropdown(DriverScope browser, ElementScope root)
            : base(browser, root)
        {
        }

        public bool IsReadOnly { get; set; }

        public string SelectedOption
        {
            get { return Root.SelectedOption; }
        }

        private ElementScope InputElement
        {
            get
            {
                return IsReadOnly ? Root.FindXPath(".//option[contains(@value,':')]")
                                  : Root.FindXPath(".//option[contains(@value,':')]");
            }
        }

        public void SelectOption(string value)
        {
            var option = Root.FindAllXPath("./option").FirstOrDefault(o => o.Text == value);
            if (option != null)
            {
                option.Click();
            }
            else
            {
                throw new Exception(string.Format("There is no '{0}' option in dropdown.", value));
            }
        }
    }
}
