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
using AventStack.ExtentReports;
using System.IO;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Walmart.Framework
{
    public static class Application
    {
        private static BrowserSession _session;

        private static SessionConfiguration _sessinConfiguration;

        private static ExtentReports _extent;
        private static ExtentTest _test;

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
                        string screenShotPath = Capture((IWebDriver)_session.Native, TestContext.CurrentContext.Test.Name);
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
            Debug.Print("PAth:" + path);
            return new ChromeDriver(path, chromeOptions);
        }
    }


}
