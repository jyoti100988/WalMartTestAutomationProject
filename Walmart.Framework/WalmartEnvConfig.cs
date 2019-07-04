using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Walmart.Framework
{
    public class WalmartEnvConfig
    {
       
        public static string Level
        {
            get
            {
                Debug.Print("Opening Level "+ ConfigurationManager.AppSettings["Environment"]);
                return GetEnvFromAppConfig(ConfigurationManager.AppSettings["Environment"]);
            }
        }

        public static string GetEnvFromAppConfig(string value)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var debugPath = Path.GetDirectoryName(executingAssembly.Location);
            var envConfigFile = debugPath + @"\Walmart.Testss.dll.config";
            
            XElement root = XElement.Load(envConfigFile);
            
            XElement environment = root.Elements("WalmartTestEnvironments").Elements("WalmartTestEnvironment").First(x => x.Attribute("name").Value == value);
            
            return environment.Attribute("url").Value;            
        }
    }
    public enum WalmartUserRoles
    {
        GuestUser,

        AutoTest,

        JyotiTest,
    }

}
