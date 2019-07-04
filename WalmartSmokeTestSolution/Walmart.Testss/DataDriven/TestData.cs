using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Testss.DataDriven
{
    public static class TestData
    {
        public static IEnumerable Credentials
        {
            get { return DataDrivenHelper.ReadDataDriveFile(@"E:\Walmart Assignment\WalmartSmokeTestSolution\Walmart.Testss\Scenarios\SearchItems", "credential", new[] { "user", "password" }, "credential"); }
        }
    }
}
