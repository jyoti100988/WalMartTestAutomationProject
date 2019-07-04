using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Coypu;
//using log4net;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Walmart.Framework.Pages
{
    public delegate void LoadedHandler(object sender);

    public class BasePage : IPage
    {
        //private static int indexOfLogs;
        private string location = string.Empty;

        private ElementScope root;

        public BasePage(DriverScope browser)
        {
            Browser = browser;
        }

        public BasePage(DriverScope browser, ElementScope root) : this(browser)
        {
            this.root = root;
        }

        //public static event LoadedHandler Loaded;

        protected DriverScope Browser { get; set; }

        public string Title
        {
            get { return ((BrowserWindow)Browser).Title; }
        }

        public string LandingPageTitle(string title)
        {
            WaitAngular();
            return Browser.FindXPath(string.Format("//h3[text()='{0}']",title)).Text;
        }

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public void BrowserRefresh()
        {
            ((BrowserSession)Browser).Refresh(); ////browser.ExecuteScript("window.location.reload(true);");
           // WaitProcessing(Browser);
        }

        public virtual T NavigateTo<T>(string path) where T : IPage
        {
            IWebDriver driver = (IWebDriver)((BrowserSession)Browser).Native;

            if (ConfigurationManager.AppSettings["Browser"] == "Chrome")
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("$('#ctl00_Messages').remove()");
            }

            var page = PageFactory.Create<T>(Browser);
            page.WaitLoading();
       
            return page;
        }

        public void WaitProcessing(DriverScope scope)
        {
            // Wait until progress bar disappears during timeout
            var timeout = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"]));

            try
            {
                if (scope is BrowserWindow)
                {
                    scope.TryUntil(() => { Thread.Sleep(1000); }, () => (bool)((BrowserWindow)scope).ExecuteScript("if ($ === undefined) { return true } else { return $('body.cursorWait').length === 0 && $('#callbackFeedback.callbackFeedback').length === 0}"), timeout);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(string.Format("JS processing is not finished for '{0}'.", timeout), exp);
            }

            // Wait 1 seconds to allow UI rendered
        }

        public void CancelDialog()
        {
            ((BrowserWindow)Browser).CancelModalDialog();
        }

        public void AcceptDialog()
        {
            ((BrowserWindow)Browser).AcceptModalDialog();
            WaitProcessing((BrowserWindow)Browser);

            Console.WriteLine("Accepted the pop up");
        }

        public virtual void WaitLoading()
        {
            try
            {
                Browser.TryUntil(
                () => { },
                () =>
                {
                    bool returnVal = false;
                    string location = null;

                    try
                    {
                        location = Browser.Location.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    if (location != null)
                    {
                        returnVal = location.ToString().Contains(Location);
                    }

                    return returnVal;
                },
                TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"])));
               
                Browser.TryUntil(
                    () => { },
                    () => (bool)((BrowserWindow)Browser).ExecuteScript("return document.readyState == 'complete'"),
                    TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"])));
                Browser.FindXPath("//body");
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Expected page by the '{0}' is not loaded for '{1}' seconds.", Location, ConfigurationManager.AppSettings["Timeout"]), exp);
            }
        }
        public void WaitAngular()
        {
            Browser.RetryUntilTimeout(
                    () =>
                    {
                        var executor = (IJavaScriptExecutor)((BrowserSession)Browser).Native;
                        executor.ExecuteAsyncScript(@"var el = document.querySelector(arguments[0]); var callback = arguments[1]; angular.element(el).injector().get('$browser').notifyWhenNoOutstandingRequests(callback);", "body");
                    });
        }
        public virtual void WaitUntil()
        {
            try
            {
                Browser.TryUntil(
                () => { },
                () =>
                {
                    bool returnVal = false;
                    string location = null;

                    try
                    {
                        location = Browser.Location.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    if (location != null)
                    {
                        returnVal = location.ToString().Contains(Location);
                    }

                    return returnVal;
                },
                TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"])));

                Browser.TryUntil(
                    () => { },
                    () => (bool)((BrowserWindow)Browser).ExecuteScript("return document.readyState == 'complete'"),
                    TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"])));
                Browser.FindXPath("//body");
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Expected page by the '{0}' is not loaded for '{1}' seconds.", Location, ConfigurationManager.AppSettings["Timeout"]), exp);
            }
        }
               
        public Uri Url
        {
            get { return Browser.Location; }
        }

        public string TopMenu
        {
            get { return string.Empty; }
        }

        protected string JsTemplate
        {
            get
            {
                return "(function (path){ var tokens = path.split(\" > \"), parent = $find('ctl00_topMenu1'); while(tokens.length > 0) { parent = getItemByText(parent, tokens.shift()); } if (parent)  parent.click(); function getItemByText(parent, text) { if (!parent) return null; var item, items = parent.get_items(), length = items.get_count(); for(var i = 0; i < length; i++) { item = items.getItem(i); if (item.get_text() == text) { return item; }} return null; }})('%menuPath%');";
            }
        }
        public string CurrentWindowHandle
        {
            get { return ((IWebDriver)((BrowserSession)Browser).Native).CurrentWindowHandle; }
        }

        [TearDown]
        public void Quit()
        {
            if (Application.Session != null)
            {
                ((BrowserWindow)Browser).ExecuteScript("self.close()");               
            }
            Application.Close();
        }
               
        public bool HasDialog(string text)
        {
            return ((BrowserWindow)Browser).HasDialog(text, Options.NoWait);
        }

    }
}