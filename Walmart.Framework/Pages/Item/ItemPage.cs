using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walmart.Framework.Controls;
using Walmart.Framework.Pages.Checkout;

namespace Walmart.Framework.Pages.Item
{
    public class ItemPage: BasePage
    {
        public ItemPage(DriverScope browser): base(browser)
        {
        }

        public void addToCart(string quantity)
        {

        }

        /// <summary>
        /// Working with default qty
        /// </summary>
        public void addToCart()
        {

            Browser.TryUntil(() => { }, () => Browser.FindButton("Add to cart").Exists(), TimeSpan.FromSeconds(3));
            Browser.ClickButton("Add to cart");
        }

        public CheckoutDialog CheckoutDialog
        {
            get
            {
                return new CheckoutDialog(Browser, Browser.FindXPath(".//div[@class='schedule-items-grid']"));
            }
        }
    }

    public class CheckoutDialog : BaseControl
    {
        public CheckoutDialog(DriverScope driver, ElementScope root) : base(driver, root)
        {

        }

        public CartPage ClickCheckout()
        {
            Browser.ClickButton("Checkout");
            return new  CartPage(Browser);
        }

    }

}
