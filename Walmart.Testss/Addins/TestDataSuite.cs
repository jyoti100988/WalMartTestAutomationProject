using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;
using System.Reflection;
using System.Xml;
using Walmart.Framework;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Walmart.Testss.Addins
{
    public delegate void TestDataBuildEventHandler(object sender, TestDataBuildEventArgs e);

    public class TestDataSuite : TestSuite
    {
        public TestDataSuite(MethodInfo method, string xmlPath)
            : base(method.Name)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("tr", string.Empty);

            var xmlInputs = xmlDoc.SelectNodes(string.Format("/tr:data/tr:test[@name='{0}']", method.Name));

            foreach (XmlNode xmlInput in xmlInputs)
            {

                var arguments = new List<string>();

               // var exeConfigPath = new Uri(typeof(TestDataSuite).Assembly.Location).LocalPath;
                var exeConfigPath = new Uri(@"E:\Wallmart\Walmart.Testss\Scenarios\SearchItems\SearchItems.xml").LocalPath;

                foreach (var param in method.GetParameters())
                {
                    var xmlParam = xmlInput.SelectSingleNode(string.Format("/tr:param[@name='{0}']", param.Name), namespaceManager);
                    if (xmlParam != null)
                    {
                        arguments.Add(xmlParam.InnerText);
                    }
                    else
                    {
                        if (bool.Parse(ConfigurationManager.OpenExeConfiguration(exeConfigPath).AppSettings.Settings["IncludeTestDataCheck"].Value))
                        {
                            throw new ArgumentException(string.Format("Input data for '{0}' parameter not found in '{1}' test.", param.Name, method.Name));
                        }
                    }
                }

                for (var i = 0; i < arguments.Count; i++)
                { 
                    if (OnTestDataBuild != null)
                    {
                        OnTestDataBuild(typeof(TestDataSuite), new TestDataBuildEventArgs(method.Name, method.GetParameters()[i].Name, arguments[i]));
                    }

                    var test = new WalmartTestMethod(method, arguments.ToArray());

                    Add(test);
                }
            }
        }
             public static event TestDataBuildEventHandler OnTestDataBuild;
    
    }

    public class TestDataBuildEventArgs
    {
        public TestDataBuildEventArgs(string testName, string parameterName, string testValue)
        {
            TestName = testName;
            ParameterName = parameterName;
            TestValue = testValue;
        }

        public string TestName { get; private set; }

        public string ParameterName { get; private set; }

        public string TestValue { get; private set; }
        
    }
}
