using NUnit.Core;
using NUnit.Core.Extensibility;
using Pranas;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Walmart.Framework;

namespace Walmart.Testss.Addins
{
    [NUnitAddin(Type = ExtensionType.Core, Name = "Walmart Listener")]
    public class NUnitListenerAddin : EventListener, IAddin
    {
        public bool Install(IExtensionHost host)
        {
            if (host == null)
            {
                return false;
            }

            var listeners = host.GetExtensionPoint("EventListeners");
            if (listeners == null)
            {
                return false;
            }

            listeners.Install(this);

            //XmlConfigurator.Configure(new FileInfo("Log.config"));

            var exeConfigPath = new Uri(typeof(NUnitListenerAddin).Assembly.Location).LocalPath;
            Debug.Print(string.Format("Assembly config path: {0}", exeConfigPath));

            WalmartEnvConfig.GetEnvFromAppConfig(ConfigurationManager.OpenExeConfiguration(exeConfigPath).AppSettings.Settings["Environment"].Value);

            return true;
        }

        public void RunFinished(Exception exception)
        {
            Debug.Print("Something wrong in RunFinished event.", exception);
            RunFinished();
        }

        public void RunFinished(TestResult result)
        {
            RunFinished();
        }

        public void RunStarted(string name, int testCount)
        {
           
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void TestStarted(TestName testName)
        {
        }

        public void TestFinished(TestResult result)
        {
            try
            {
                if (!result.IsSuccess && result.ResultState != ResultState.Ignored && result.ResultState != ResultState.Skipped && result.ResultState != ResultState.NotRunnable)
                {
                    Debug.Print(string.Format("{0}\n{1}", result.Message, result.StackTrace));

                    // Try to get screenshots of application
                    var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Screenshots";
                    Directory.CreateDirectory(dirPath);
                    var fileName = Path.GetInvalidFileNameChars().Aggregate(result.Test.TestName.Name, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty));

                    if (dirPath.Length > 249)
                    {
                        Debug.Print("Directory path is very long!");
                        throw new Exception("Directory path is very long!");
                    }

                    if (dirPath.Length + fileName.Length >= 249)
                    {
                        fileName = fileName.Substring(0, 249 - dirPath.Length);
                    }

                    Debug.Print(string.Format("Taking screenshot: {0}\\{1}", dirPath, fileName));
                    try
                    {
                        ScreenshotCapture.TakeScreenshot(true).Save(string.Format("{0}\\{1}_Full.png", dirPath, fileName), ImageFormat.Png);
                    }
                    catch (Exception exp)
                    {
                       
                    }
                }
                
                Debug.Print(string.Format("Test Result : {0} and Test Name : {1}", result.ResultState, result.Name));
                Application.Close();
            }
            catch (Exception exp)
            {
                Debug.Print(exp.Message, exp);
            }
        }

        public void TestOutput(TestOutput testOutput)
        {
        }

        public void UnhandledException(Exception exception)
        {
            Debug.Print("Unhandled exception in NUnit Addin layer", exception);
        }

        private void RunFinished()
        {
            try
            {
                
                Application.Close();
            }
            catch (Exception exp)
            {
                Debug.Print("Something wrong in RunFinished event.", exp);
            }
        }
    }
}
