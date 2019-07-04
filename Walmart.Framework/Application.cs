using Coypu.Drivers.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using Coypu.Drivers;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using Coypu;
using Walmart.Framework.Pages;

namespace Walmart.Framework
{
    public static class Application
    {
        private static BrowserSession _session;

        private static SessionConfiguration _sessinConfiguration;

        public static BrowserSession Session
        {
            get
            {
                return _session;
            }
        }

        public static bool HasDialog
        {
            get
            {
                var hasDialog = false;
                if (_session != null)
                {
                    hasDialog = _session.HasDialog(string.Empty, new Options { Timeout = TimeSpan.FromSeconds(0) });
                }

                return hasDialog;
            }
        }

        public static string Uri
        {
            get
            {
                var uri = string.Empty;
                if (_session != null)
                {
                    uri = _session.Location.ToString();
                }

                return uri;
            }
        }

        public static TimeSpan Timeout
        {
            get { return _sessinConfiguration.Timeout; }
            set { _sessinConfiguration.Timeout = value; }
        }

        public static LoginPage Open(string appUrl)
        {
            if (_session == null)
            {
                Debug.WriteLine("Initializing new browser instance...");
                var sessionConfig = new SessionConfiguration
                {
                    Timeout = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["Timeout"])),
                    Browser = Browser.Parse(ConfigurationManager.AppSettings["Browser"]) ,
                    Driver=typeof(CustomChromeProfileSeleniumWebDriver)  
                    
                };

                if (sessionConfig.Browser == Browser.Chrome)
                {
                   _session = new BrowserSession(sessionConfig);
                    
                }

                if (sessionConfig.Browser == Browser.Firefox)
                {
                    _session = new BrowserSession(sessionConfig);
                } 

                _sessinConfiguration = sessionConfig;

                _session.MaximiseWindow();
            }
            
            Debug.WriteLine(string.Format("Visit {0}", appUrl));
            Session.Visit(appUrl);
            var loginPage = new LoginPage(Session);

            try
            {
                loginPage.WaitLoading();
            }
            catch (Exception exp)
            {
                loginPage.WaitLoading();
            }

            return loginPage;
        }


        public static void Close()
        {
            if (_session != null)
            {
                Debug.Print("Closing browser....");

                try
                {
                    Debug.Print(string.Format("Before session disposing, found {0} Chrome Driver processes",
                        Process.GetProcesses().Where(p => (p.ProcessName == "chromedriver")).ToList().Count
                        //Process.GetProcesses().Where(p => (p.ProcessName == "iexplore")).ToList().Count,
                        //Process.GetProcesses().Where(p => (p.ProcessName == "WerFault")).ToList().Count
                        ));

                    Debug.Print("Disposing browser session...");
                    _session.Dispose();

                    Debug.Print(string.Format("After session disposing, found {0} Chrome Driver processes",
                    Process.GetProcesses().Where(p => (p.ProcessName == "chromedriver")).ToList().Count
                    //Process.GetProcesses().Where(p => (p.ProcessName == "iexplore")).ToList().Count,
                    //Process.GetProcesses().Where(p => (p.ProcessName == "WerFault")).ToList().Count)
                    ));

                    try
                    {
                        Debug.Print("Killing web driver by processes...");
                        Process.GetProcesses()
                            .Where(p => ((p.ProcessName == "IEDriverServer") || (p.ProcessName == "chromedriver")) && !p.HasExited)
                            .ToList()
                            .ForEach(p => p.Kill());

                        Debug.Print("Killing 'Report Problem' windows processess..");
                        Process.GetProcesses()
                            .Where(p => (p.ProcessName == "WerFault"))
                            .ToList()
                            .ForEach(p => p.Kill());
                    }
                    catch (Exception exp)
                    {
                        Debug.Print(string.Format("Exception was thrown while killing processes. {0}", exp));
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(string.Format("Obtained exception when tried to Dispose driver. {0}", e));
                }
                finally
                {
                    _session = null;
                }
            }
        }

        public static void NavigateToPageOnDemand(string hyperLink)
        {
            Session.Visit(hyperLink);
        }
    }

    public class CustomChromeProfileSeleniumWebDriver : SeleniumWebDriver
    {
        public CustomChromeProfileSeleniumWebDriver(Browser browser)
            : base(CustomProfile(), browser)
        {
        }

        private static RemoteWebDriver CustomProfile()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("test-type");
            chromeOptions.AddArguments("start-maximized");
            chromeOptions.AddArguments("--disable-extensions");
            chromeOptions.AddArguments("no-sandbox");
            var path = Environment.CurrentDirectory + @"\tools";
            return new ChromeDriver(path, chromeOptions);
        }
    }


}
