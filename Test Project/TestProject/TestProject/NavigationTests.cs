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
    public class NavigationTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;

        [TestMethod]
        public void NavigationTest()
        {
            //url = "http://localhost:57404/#/";
            url = "https://test-foundation.vpfin.vt.edu";
            
            DoFirefoxTest();
            DoBasicNavigation();
            takeScreenshot("Firefox");
            driver.Close();

            DoChromeTest();
            DoBasicNavigation();
            takeScreenshot("Chrome");
            driver.Close();
            
            /* Reaally slows down the test
            DoIETest();
            DoBasicNavigation();
            takeScreenshot("Internet_Explorer");
            driver.Close();
            */
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

        private void DoFirefoxTest() {
            driver = new FirefoxDriver();
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);           
            doLoginCAS();
            
        }

        private void DoChromeTest()
        {
            driver = new ChromeDriver(@"C:\selenium\");
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            doLoginCAS();
            
        }

        private void DoIETest()
        {
            driver = new InternetExplorerDriver(@"C:\selenium\");
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            js_executor.ExecuteScript("document.getElementById('overridelink').click()");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
            doLoginCAS();
           
        }

        private void DoBasicNavigation()
        {
           

            driver.FindElement(By.LinkText("Reports")).Click();
            driver.FindElement(By.LinkText("Funding Request")).Click();

            driver.FindElement(By.LinkText("Reports")).Click();
            driver.FindElement(By.LinkText("Narrative")).Click();

            IWebElement fundsLink = driver.FindElement(By.LinkText("Funds"));
            if(fundsLink.GetAttribute("href")!=null){
                fundsLink.Click();
            }
            else
            {
                fundsLink.Click();
                driver.FindElement(By.LinkText("Browse")).Click();
            }
           

            driver.Manage().Window.Maximize();
            //checkComputedLayout();
        }

        private void takeScreenshot(String testName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            //String date = String.Concat("__", DateTime.UtcNow.Month, "-", DateTime.UtcNow.Day, "-", DateTime.UtcNow.Year, "_", DateTime.UtcNow.Hour, "-", DateTime.UtcNow.Minute, "-", DateTime.UtcNow.Second, "__");
            String filename = String.Concat(testName, "__screenshot", ".png");
            ss.SaveAsFile(filename, ImageFormat.Png);
        }

        
        private void checkComputedLayout()
        {
            IWebElement table = driver.FindElement(By.CssSelector("table.tabular"));

            Assert.Inconclusive(String.Concat("Size: ",table.Size.Width));
            /*
            String content_width_str = (String)js_executor.ExecuteScript("var content = document.getElementsByClassName('large-12'); return window.getComputedStyle(content[0]).width;");
            String table_width_str = (String)js_executor.ExecuteScript("var table = document.getElementsByClassName('tabular'); return window.getDefaultComputedStyle(table[0],null).getPropertyValue('width');");
            double content_width = getValueFromPixels(content_width_str);
            double table_width = getValueFromPixels(table_width_str);

            if (content_width < table_width)
            {
              Assert.Inconclusive(String.Concat(content_width,",",table_width));
            }
            */
            
        }

        private double getValueFromPixels(String value){
            value=value.Replace("px", "");
            return Convert.ToDouble(value);
        }
    }


}
