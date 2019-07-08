                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 using NUnit.Framework;

using System.Diagnostics;
using System.Text.RegularExpressions;
using Walmart.Framework;
using Walmart.Framework.Pages;
using Walmart.Framework.Pages.Item;
using Walmart.Testss.Addins.Attributes;
using Walmart.Testss.Addins.Extensions;

namespace Walmart.Testss.Scenarios.SearchItemPage
{
    /// <summary>
    /// the class is partial for other engineers to work simulteniously 
    /// </summary>
    [TestFixture]
    [Parallelizable]
    public partial class SearchItemPage
    {

        [Regression]
        [NotReady]
        //[Repeat(10)]
        [TestCase("Pencil", "jyoti.test@gmail.com", "Jyoti", "Singh", "Mississauga", "L4Z 3N7", "Ontario", "(123) 457-7890", "221 Barondale Dr Mississauga, ON, L4Z 3N7")]
        public void PlaceOrder(string searchItem, string guestEmail, string firstName, string lastName, string city, string postalCode, string province, string phoneNumber, string addressLine1)
        {
            Debug.WriteLine("Launching the Application ..\n");
            var homePage = Application.Open(WalmartEnvConfig.Level).LoginAs(WalmartUserRoles.GuestUser);
            var searchItemsPage = homePage.NavigateToSearchBox<SearchPage>(searchItem);

            Debug.Print(string.Format("searching for {0}", searchItem));
            var searchItemResultsPage = searchItemsPage.SearchItem(searchItem);

            ItemPage itemPage = searchItemResultsPage.ClickItem();
            itemPage.addToCart();

            Debug.Print("Clicking on Checkout button");
            var cartPage = itemPage.CheckoutDialog.ClickCheckout();

            Debug.Print("Clicking on Proceed to Checkout button");
            var checkoutPage = cartPage.ClickProceedCheckout();

            Debug.Print("Entering Email");
            checkoutPage.GuestEmail = guestEmail;

            Debug.Print("Clicking Next button");
            checkoutPage.Step1Next();

            Debug.Print("Clicking on Shipping Tab!");
            checkoutPage.ShippingTab();

            Debug.Print("Entering First Name");
            checkoutPage.FirstName = firstName;

            Debug.Print("Entering Last Name");
            checkoutPage.LastName = lastName;

            Debug.Print("Entering Address");
            checkoutPage.AddressLine = addressLine1;

            var regEx = @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$";

            Debug.Print("Validating Phone Number format");
            Assert.IsTrue(Regex.IsMatch(phoneNumber, regEx), "Phone Number is not in valid Format");

            checkoutPage.PhoneNumber = phoneNumber;

            Debug.Print("Asserting Postal code and city ");
            Assert.That(postalCode.Equals(checkoutPage.PostalCode) && city.Equals(checkoutPage.City1), "AddressAutofill did not fill expected values either for Postal code or city.");

            Debug.Print("Asserting Province");
            Assert.That(province.Equals(checkoutPage.Province), "Province is not autofilled correctly");

            Debug.WriteLine("Closing Application..");
            Application.Close();

        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            