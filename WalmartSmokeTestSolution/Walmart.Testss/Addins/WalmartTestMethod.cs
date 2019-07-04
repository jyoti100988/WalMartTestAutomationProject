using NUnit.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Walmart.Testss.Addins.Attributes;

namespace Walmart.Testss.Addins
{
    public class WalmartTestMethod : NUnitTestMethod
    {
        private readonly object[] _args;

        ////private MethodInfo _methodinfo;

        /// <summary>
        /// Constructor to map method with this class.
        /// </summary>
        /// <param name="method">Method in test fixture without parameters.</param>
        public WalmartTestMethod(MethodInfo method)
            : base(method)
        {
            NUnitFramework.ApplyCommonAttributes(method, this);
            NUnitFramework.ApplyExpectedExceptionAttribute(method, this);

            var categories = new List<string>();
            var className = method.DeclaringType;
            
            categories.ForEach(c => Categories.Add(c));

            var notReadyAttr = method.GetCustomAttributes(typeof(NotReadyAttribute), true);
            if (notReadyAttr.Any())
            {
                Categories.Add("NotReady");
            }

            ////Add EUAT as Category
            var regressionAttr = method.GetCustomAttributes(typeof(RegressionAttribute), true);
            if (regressionAttr.Any())
            {
                Categories.Add("Regression");
            }

            ////Add ToFix as Category
            var tofixAttr = method.GetCustomAttributes(typeof(ToFixAttribute), true);
            if (tofixAttr.Any())
            {
                Categories.Add("ToFix");
            }
        }

        /// <summary>
        /// Costructor to map parametrized method with this class.
        /// </summary>
        /// <param name="method">Parametrized method.</param>
        /// <param name="args">Parameters.</param>
        public WalmartTestMethod(MethodInfo method, object[] args)
            : this(method)
        {
            var parameters = method.GetParameters().Select(p => p.Name).ToList();

            if (parameters.Count != args.Length)
            {
                throw new Exception("Parameters count mismatch in test method :" + method.Name);
            }

            object[] convertedParameters = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (typeof(IList).IsAssignableFrom(method.GetParameters().ToList()[i].ParameterType))
                    {
                        string[] splitParameterValue = ((string)args[i]).Trim().Split(',');

                        Type listType = method.GetParameters().ToList()[i].ParameterType.GetGenericArguments()[0];

                        Type genericListType = typeof(List<>).MakeGenericType(listType);

                        IList dynamicList = (IList)Activator.CreateInstance(genericListType);

                        foreach (var value in splitParameterValue)
                        {
                            dynamicList.Add(Convert.ChangeType(value.Trim(), listType));
                        }

                        convertedParameters[i] = dynamicList;
                    }
                    else
                    {
                        convertedParameters[i] = Convert.ChangeType(args[i], method.GetParameters().ToList()[i].ParameterType);
                    }
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Parameter {0} = \"{1}\" is Not Defined Correctly for test method {2}", parameters[i], args[i], method.Name));
                }
            }

            _args = convertedParameters;

            var parametersString = string.Empty;
            for (int i = 0; i < parameters.Count(); i++)
            {
                parametersString += string.Format("{0} = \"{1}\"", parameters[i], args[i]);
                if (i != parameters.Count - 1)
                {
                    parametersString += ", ";
                }
            }

            ////_methodinfo = method;
            TestName.Name = string.Format("{0} ({1})", TestName.Name, parametersString);
        }

        /// <summary>
        /// Execute method depends on parameters.
        /// </summary>
        /// <returns>Test results.</returns>
        protected override object RunTestMethod()
        {
            // Invoke test method
            if (_args == null || _args.Length == 0)
            {
                return base.RunTestMethod();
            }

            return Reflect.InvokeMethod(Method, Fixture, _args);
        }

    }
}
