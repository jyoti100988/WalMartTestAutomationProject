using Coypu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walmart.Framework.Pages.SearchResults;

namespace Walmart.Framework.Pages
{
    public class SearchPage : BasePage
    {
        public SearchPage(DriverScope browser): base(browser)
        {
            Location = "/search";
        }
        public SearchItemsResultPage SearchItem(string itemName)
        {
            Browser.FillIn("global-search").With(itemName);
            Browser.FindXPath("//button[@type='submit']").Click();
            SearchItemsResultPage page = new SearchItemsResultPage(Browser);

            Debug.Print("Clicked on search button");
            WaitUntil();

            return page;
        }
    }
}
