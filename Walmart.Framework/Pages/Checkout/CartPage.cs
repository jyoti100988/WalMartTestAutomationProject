using Coypu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Framework.Pages.Checkout
{
    public class CartPage: BasePage
    {
        public CartPage(DriverScope browser) : base(browser)
        {
            Location = "/cart";
        }

        public CheckoutPage ClickProceedCheckout()
        {
            WaitLoading();
            Browser.ClickLink("Proceed to checkout");
            return new CheckoutPage(Browser);
        }
    }
}
