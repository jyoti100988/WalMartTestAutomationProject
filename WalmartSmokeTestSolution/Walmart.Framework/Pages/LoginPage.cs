using Coypu;
using System.Diagnostics;

namespace Walmart.Framework.Pages
{
    public class LoginPage: BasePage
    {

        public LoginPage(DriverScope browser)
            : base(browser)
        {
            Location = ""; // supposed to provide the whole page identification
        }

        public string Username
        {
            get
            {
                return Browser.FindField("").Value;
            }

            set
            {
                Browser.FillIn("").With(value);
            }
        }
        
        public string Password
        {
            get
            {
                return Browser.FindField("").Value;
            }

            set
            {
                Browser.FillIn("").With(value);
            }
        }

        public string LoginErrorMessage
        {
            get { return Browser.FindXPath("").Text; }
        }

        public HomePage LoginClick()
        {
            Debug.Print("Clicking on 'Login' button...");
            Browser.ClickButton("");
            var page = PageFactory.Create<HomePage>(Browser);
           
            return page;
        }
        public HomePage GuestHomePage()
        {
            var page = PageFactory.Create<HomePage>(Browser);
            return page;
        }

    }
}
