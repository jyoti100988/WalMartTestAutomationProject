using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walmart.Framework.Controls;
using Walmart.Framework.Pages.Item;

namespace Walmart.Framework.Pages.SearchResults
{
    public class SearchItemsResultPage: BasePage
    {
        public SearchItemsResultPage(DriverScope browser): base(browser)
        {
            Location = "/search";
        }

        public ItemPage ClickItem()
        {
            var allArticles=Browser.FindAllXPath(".//div[@id='shelf-thumbs']/div[@class='shelf-thumbs pnoDone']/article");
            allArticles.First().Click();
            return new ItemPage (Browser);
        }
    }
    
}
