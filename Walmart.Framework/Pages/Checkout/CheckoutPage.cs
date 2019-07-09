using Coypu;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Walmart.Framework.Control.Dropdown;

namespace Walmart.Framework.Pages.Checkout
{
    public class CheckoutPage : BasePage
    {
        public CheckoutPage(DriverScope browser) : base(browser)
        {
            Location = "/checkout";
        }

        public string GuestEmail
        {
            set { Browser.FindId("email").FillInWith(value); }
        }

        public void Step1Next()
        {
            Browser.FindId("step-1-next").Click();
        }

        public void ShippingTab()
        {
            Browser.FindId("shipping-to-home-label").Click();
        }

        public string FirstName
        {
            set
            {
                Browser.FindId("shipping-first-name").FillInWith(value);
            }
        }

        public string LastName
        {
            set
            {
                Browser.FindId("shipping-last-name").FillInWith(value);
            }
        }

        public string AddressLine
        {
            set
            {
                Browser.FindId("shipping-address1").FillInWith(value);

                WebDriverWait wait = new WebDriverWait(((IWebDriver)((BrowserSession)Browser).Native), TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(".//div[@id='shippingAddressAutocomplete']/label/ul[@class='typeahead dropdown-menu']/li")));

                var list = Browser.FindAllXPath(".//div[@id='shippingAddressAutocomplete']/label/ul[@class='typeahead dropdown-menu']/li");

                foreach (ElementScope ele in list)
                {
                    if (ele.Text.Equals(value))
                    {
                        Browser.TryUntil(() => { },() => ele.FindXPath("").Exists(),TimeSpan.FromSeconds(5));
                        ele.Click();
                        System.Threading.Thread.Sleep(2000);
                        break;
                    }
                }
            }
        }

        public string City1
        {
            get
            {
               return Browser.FindId("shipping-city").Value;
            }
        }
        public string Province
        {
            get
            {
               return new Dropdown(Browser, Browser.FindId("shipping-province")).SelectedOption;
            }

            set
            {
                Debug.Print(string.Format("Selecting {0} value in 'Province' dropdown...", value));
                var province = Application.Session.FindId("shipping-province").Click();
                new Dropdown(Browser, province.SelectOption(value));
              }
        }

        public string PostalCode
        {
            get
            {
                return Browser.FindId("shipping-postal-code").Value;
            }
            set
            {
                Browser.FindId("shipping-postal-code").FillInWith(value);
            }
        }

        public string PhoneNumber
        {
            set
            {
                Browser.FindId("shipping-phone").FillInWith(value);
            }
        }

        public void ClickContinue()
        {
            Browser.ClickButton(" Continue ");
        }
    }
}
