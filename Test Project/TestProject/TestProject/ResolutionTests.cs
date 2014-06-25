using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support;
using System.Drawing.Imaging;
namespace TestProject
{
    [TestClass]
    public class ResolutionTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;
        [TestMethod]
        public void ResolutionTest()
        {
            //url = "http://localhost:57404/#/";
            url = "https://test-foundation.vpfin.vt.edu";
            
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
         
            driver.Close();
        }

        private void doLoginCAS()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            Credentials credentials = new Credentials();
            IWebElement username_input = driver.FindElement(By.Id("username"));
            IWebElement password_input = driver.FindElement(By.Id("password"));
            IWebElement submit_button = driver.FindElement(By.Name("submit"));

            username_input.SendKeys(credentials.getPID());
            password_input.SendKeys(credentials.getPassword());
            submit_button.Click();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        }

    }
}
