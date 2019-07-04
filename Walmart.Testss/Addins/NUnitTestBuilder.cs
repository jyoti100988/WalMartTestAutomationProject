using NUnit.Core;
using NUnit.Core.Extensibility;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Walmart.Testss.Addins.Attributes;

namespace Walmart.Testss.Addins
{
    public delegate void TestBuildEventHandler(object sender, TestBuildEventArgs e);

    [NUnitAddin]
    public class NUnitTestBuilder : IAddin, ITestCaseBuilder
    {
        public static event TestBuildEventHandler OnTestBuild;

        public bool Install(IExtensionHost host)
        {
            var testCaseBuilders = host.GetExtensionPoint("TestCaseBuilders");
            testCaseBuilders.Install(this);
            
            return true;
        }


        /// <summary>
        /// Build test for NUnit from test method.
        /// </summary>
        /// <param name="method">Test method in test fixture.</param>
        /// <returns>Test object for NUnit.</returns>
        public Test BuildFrom(MethodInfo method)
        {
            try
            {
                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                Test nunitMethod = null;
              
                if (method.GetParameters().Any())
                {
                    if (method.DeclaringType != null && method.DeclaringType.Namespace != null)
                    {
                        //var path =
                            //method.DeclaringType.Namespace.Replace("Walmart.Testss.", string.Empty).Replace(".", @"\") +
                            //@"\" + method.DeclaringType.Name;
                        var path = @"E:\Walmart Assignment\WalmartSmokeTestSolution\Walmart.Testss\Scenarios\SearchItems\SearchItems";
                        if (File.Exists(path + ".xml"))
                        {
                            nunitMethod = new TestDataSuite(method, path + ".xml");
                        }
                        else
                        {
                           throw new Exception(string.Format("Input file for suite not found on the following path : {0}", path));
                        }
                    }
                }
                else
                {
                    nunitMethod = new WalmartTestMethod(method);
                }

                //var exeConfigPath = new Uri(typeof(NUnitTestBuilder).Assembly.Location).LocalPath;
                //if (bool.Parse(ConfigurationManager.OpenExeConfiguration(exeConfigPath).AppSettings.Settings["IncludeSkipped"].Value))
                //{
                //    if (nunitMethod != null && !isCompatible)
                //    {
                //        nunitMethod.RunState = RunState.Ignored;
                //        nunitMethod.IgnoreReason = "Not compatible";
                //    }
                //}
                //else
                //{
                //    if (nunitMethod != null && !isCompatible)
                //    {
                //        nunitMethod = null;
                //    }
                //}

                if (OnTestBuild != null)
                {
                    OnTestBuild(typeof(NUnitTestBuilder), new TestBuildEventArgs(nunitMethod));
                }

                return nunitMethod;
            }
            catch (Exception e)
            {
                Debug.Print("Something wrong with tests building.", e);
                throw;
            }
        }

        /// <summary>
        /// Define whether test method in class can be built based on <see cref=""/>.
        /// </summary>
        /// <param name="method">Input method.</param>
        /// <returns>True if method can be built to test.</returns>
        public bool CanBuildFrom(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            
            var customTestAttrs = Reflect.GetAttributes(method, typeof(RegressionAttribute).FullName, false);

            if (customTestAttrs.Any())
            {
                return true;
            }

            return false;
        }
    }

}

    public class TestBuildEventArgs
    {
        public TestBuildEventArgs(Test test)
        {
            Test = test;
        }

        public Test Test { get; private set; }
    }

