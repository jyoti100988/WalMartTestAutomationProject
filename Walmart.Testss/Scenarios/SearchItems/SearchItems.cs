using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;
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

        private static ExtentReports _extent;
        private static ExtentTest _test;


        public void CreateReportFolders()
        {
            try
            {
                //To create report directory and add HTML report into it

                _extent = new ExtentReports();
                var dir = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\\bin\\Debug", string.Empty);
                DirectoryInfo di = Directory.CreateDirectory(dir + "\\Test_Execution_Reports");
                var htmlReporter = new ExtentHtmlReporter(dir + "\\Test_Execution_Reports" + "\\Automation_Report" + ".html");

                _extent.AddSystemInfo("User Name", "Jyoti");
                _extent.AttachReporter(htmlReporter);

                _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

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

        [TearDown]
        public void CreateReport()
        {

            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var stacktrace = "" + TestContext.CurrentContext.Result.StackTrace + "";
                var errorMessage = TestContext.CurrentContext.Result.Message;
                Status logstatus;
                switch (status)
                {
                    case TestStatus.Failed:
                        logstatus = Status.Fail;
                        string screenShotPath = Capture((IWebDriver)Application.Session.Browser, TestContext.CurrentContext.Test.Name);
                        _test.Log(logstatus, "Test ended with " + logstatus + " – " + errorMessage);
                        _test.Log(logstatus, "Snapshot below: " + _test.AddScreenCaptureFromPath(screenShotPath));
                        break;
                    case TestStatus.Skipped:
                        logstatus = Status.Skip;
                        _test.Log(logstatus, "Test ended with " + logstatus);
                        break;
                    default:
                        logstatus = Status.Pass;
                        _test.Log(logstatus, "Test ended with " + logstatus);
                        break;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            _extent.Flush();
        }

        public static string Capture(IWebDriver driver, string screenShotName)
        {
            string localpath = string.Empty;
            try
            {
                screenShotName = screenShotName.Replace("\"", "");
                System.Threading.Thread.Sleep(4000);
                ITakesScreenshot ts = (ITakesScreenshot)driver;
                Screenshot screenshot = ts.GetScreenshot();
                string pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
                var dir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", string.Empty);
                DirectoryInfo di = Directory.CreateDirectory(dir + "\\Defect_Screenshots\\");
                string finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "\\Defect_Screenshots\\" + screenShotName + ".png";
                localpath = new Uri(finalpth).LocalPath;
                screenshot.SaveAsFile(localpath);
            }
            catch (Exception e)
            {
                throw (e);
            }
            return localpath;
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            