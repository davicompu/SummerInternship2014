using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support;

namespace TestProject
{
    [TestClass]
    public class MainUnit
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;

        [TestMethod]
        public void NavigationTest()
        {
            url = url = "http://localhost:57404/#/";
            DoFirefoxTest();
            driver.Close();
        }

        private void DoFirefoxTest() {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(url);
        }

        private void DoChromeTest()
        {
            driver = new ChromeDriver(@"C:\selenium\");
        }

        private void DoIETest()
        {
            driver = new InternetExplorerDriver(@"C:\selenium\");
        }

        private void DoBasicNavigation()
        {
            js_executor = (IJavaScriptExecutor)driver;
        }
    }
}
