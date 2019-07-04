using NUnit.Framework;
using Walmart.Framework;
using Walmart.Framework.Pages;
using System;


namespace Walmart.Testss.Addins.Extensions
{
    public static class LoginExtensions
    {
        public static HomePage LoginAs(this LoginPage page, WalmartUserRoles role)
        {
            dynamic homePage = page.GuestHomePage();
           //homePage.WaitLoading();
            switch (role)
            {
                case WalmartUserRoles.GuestUser:
                    Assert.AreEqual("Online Shopping Canada: Everyday Low Prices at Walmart.ca!",Convert.ToString(homePage.Title));
                    break;
                case WalmartUserRoles.AutoTest:
                    homePage = page.LoginClick();
                    Assert.AreEqual("Online Shopping Canada: Everyday Low Prices at Walmart.ca!", homePage.Title);
                    break;
                default:
                    break;
            }
                 
            return homePage;
        }
    }
}
