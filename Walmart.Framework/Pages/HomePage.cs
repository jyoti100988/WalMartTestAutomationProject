using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Framework.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(DriverScope browser): base(browser)
        {
            Location = "/walmart.ca";
        }

        public virtual T NavigateTo <T>(string pathId) where T : IPage
        {
            //WaitAngular();
            Browser.FindId(pathId).Click();
            var page = PageFactory.Create<T>(Browser);
            page.WaitLoading();
            return page;
        }
        public virtual SearchPage NavigateToSearchBox<SearchPage>(string itemName) where SearchPage : IPage
        {
            //WaitAngular();
            Browser.FindId("global-search").Click();
            var page = PageFactory.Create<SearchPage>(Browser);            
           // page.WaitLoading();
            //page.Location = string.Format(@"/search/{0}", itemName);
            return page;
        }
    }
}
