using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
namespace TestProject
{
    [TestClass]
    public class ResolutionTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;
        private int[] desktop_resolution,tablet_resolution,mobile_resolution,large_mobile_resolution;

        [TestMethod]
        public void ResolutionTest()
        {
            desktop_resolution = new int[]{1280, 800};
            tablet_resolution = new int[] { 1024, 768 };
            mobile_resolution = new int[] { 320, 568 };
            large_mobile_resolution = new int[] { 480, 800 };

            //url = "http://localhost:57404/#/";
            url = "https://test-foundation.vpfin.vt.edu";
            setupFirefoxDriver();
            //setupChormeDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            DoResolutionTest();
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

        private void setupFirefoxDriver()
        {
            driver = new FirefoxDriver();
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            doLoginCAS();
        }

        private void setupChormeDriver()
        {
            driver = new ChromeDriver(@"C:\selenium\");
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            doLoginCAS();

        }
        private void DoResolutionTest()
        {
            doDesktopTest();
            doTabletTest();
            doLargeMobileTest();
            doMobileTest();
        }

        private void doDesktopTest()
        {
            chageResolution(desktop_resolution);
            //Reduce the zoom from 100% to 90%
            IWebElement html = driver.FindElement(By.TagName("html"));
            String keys = String.Concat(Keys.Control, Keys.Subtract);
            html.SendKeys(keys);
            takeScreenshot("Desktop",desktop_resolution);
            try
            {
                Assert.IsTrue(isMenuVisible());
            }
            catch (Exception e)
            {
                takeScreenshot(String.Concat("ResolutionTest-Desktop", desktop_resolution[0], "x", desktop_resolution[1]));
                String message = String.Concat("Menu test -desktop- fail because long menu is not visible in ", desktop_resolution[0], "x", desktop_resolution[1]);
                Assert.Fail(message);
            }
           
        }

        private void doMobileTest()
        {
            chageResolution(mobile_resolution);
            takeScreenshot("Mobile", mobile_resolution);
            try
            {
                Assert.IsFalse(isMenuVisible());
            }
            catch (Exception e)
            {
                takeScreenshot(String.Concat("ResolutionTest-Mobile", mobile_resolution[0], "x", mobile_resolution[1]));
                String message = String.Concat("Menu test -mobile- fail because long menu is visible in ", mobile_resolution[0], "x", mobile_resolution[1]);
                Assert.Fail(message);
            }
        }

        private void doTabletTest()
        {
            chageResolution(tablet_resolution);
            takeScreenshot("Tablet", tablet_resolution);
            try
            {
                Assert.IsTrue(isMenuVisible());
            }
            catch (Exception e)
            {
                takeScreenshot(String.Concat("ResolutionTest-Tablet", tablet_resolution[0], "x", tablet_resolution[1]));
                String message = String.Concat("Menu test -tablet- fail because long menu is visible in ", tablet_resolution[0], "x", tablet_resolution[1]);
                Assert.Fail(message);
            }

        }

        private void doLargeMobileTest()
        {
            chageResolution(large_mobile_resolution);
            takeScreenshot("LargeMobile", large_mobile_resolution);
            try
            {
                Assert.IsFalse(isMenuVisible());
            }
            catch (Exception e)
            {
                takeScreenshot(String.Concat("ResolutionTest-LargeMobile", large_mobile_resolution[0], "x", large_mobile_resolution[1]));
                String message = String.Concat("Menu test -large mobile- fail because long menu is visible in ", large_mobile_resolution[0], "x", large_mobile_resolution[1]);
                Assert.Fail(message);
            }
        }


        private Boolean isMenuVisible()
        {
            try
            {
                driver.FindElement(By.LinkText("Funds"));
                driver.FindElement(By.LinkText("Reports"));
                driver.FindElement(By.LinkText("Logout"));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private void chageResolution(int [] resolution)
        {
            
            driver.Manage().Window.Size = new System.Drawing.Size(resolution[0], resolution[1]);
        }
        private void takeScreenshot(String testName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            String date = String.Concat(DateTime.UtcNow.Month, "-", DateTime.UtcNow.Day, "-", DateTime.UtcNow.Year, "_", DateTime.UtcNow.Hour, "-", DateTime.UtcNow.Minute, "-", DateTime.UtcNow.Second, "__");
            String filename = String.Concat(testName, "_screenshot_", date, ".png");
            ss.SaveAsFile(filename, ImageFormat.Png);
        }

        private void takeScreenshot(String title, int [] resolution)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            String filename = String.Concat(title, resolution[0], "x", resolution[1], ".png");
            ss.SaveAsFile(filename, ImageFormat.Png);
        }
    }
}
