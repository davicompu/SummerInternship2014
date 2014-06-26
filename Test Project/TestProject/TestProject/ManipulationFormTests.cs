using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Drawing.Imaging;

namespace TestProject
{
    [TestClass]
    public class ManipulationFormTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;
        [TestMethod]
        public void ManipulationFormTest()
        {
            //url = "http://localhost:57404/#/";
            url = "https://test-foundation.vpfin.vt.edu";
            setupFirefoxDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            doNavigationToForm();
            DoCalculationTest();
            DoSelectionTest();
            driver.Close();
        }

        private void doLoginCAS()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            Credential credentials = new Credentials();
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
        private void doNavigationToForm()
        {
            String link_text = "Create a new fund";

            //Navigate to the create new fund
            IWebElement link = driver.FindElement(By.LinkText(link_text));
            link.Click();
        }
        private void DoSelectionTest(){
           
            SelectElement select = new SelectElement(driver.FindElement(By.Name("Status")));
            try {
                Assert.IsFalse(select.IsMultiple);
            }
            catch (Exception e)
            {
                takeScreenshot("ManipulationTest-selection-");
                Assert.Fail("Selection Fails - Selection box can enable multiple");
            }
            
            
        }
        private void DoCalculationTest()
        {
            int variance_a, variance_b;
            IWebElement body = driver.FindElement(By.TagName("body"));
            IWebElement CurrentBudget = driver.FindElement(By.Name("CurrentBudget"));
            IWebElement ProjectedExpenditures = driver.FindElement(By.Name("ProjectedExpenditures"));
            IWebElement BudgetAdjustment = driver.FindElement(By.Name("BudgetAdjustment"));
            Random random = new Random();
            int number = random.Next(900000);
            variance_a = number;
            variance_b = number;
            CurrentBudget.SendKeys(number.ToString());
            number = random.Next(900000);
            variance_a -= number;
            ProjectedExpenditures.SendKeys(number.ToString());
            number = random.Next(900000);
            variance_b += number;
            BudgetAdjustment.SendKeys(number.ToString());
            body.Click();
            //Make a pause to let the input to reformat
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(600));
            IWebElement variance_a_client = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/section/section/div/div/section/fieldset[1]/div[7]/div/div/span/strong"));
            IWebElement variance_b_client = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/section/section/div/div/section/fieldset[2]/div[2]/div/div/span/strong"));

            try
            {
                Assert.AreEqual(variance_a, getNumberFromText(variance_a_client.Text));
            }
            catch (Exception e)
            {
                takeScreenshot("CalculationFormTest_first");
                Assert.Fail(String.Concat(variance_a, " client ", getNumberFromText(variance_a_client.Text)));
            }

            try
            {
                Assert.AreEqual(variance_b, getNumberFromText(variance_b_client.Text));

            }
            catch (Exception e)
            {
                takeScreenshot("CalculationFormTest_second");
                Assert.Fail(String.Concat(variance_b, " client ", getNumberFromText(variance_b_client.Text)));
            }

        }

        private int getNumberFromText(String number)
        {
            number = number.Replace("$", "");
            number = number.Replace("(", "-");
            number = number.Replace(")", "");
            number = number.Replace(",", "");
            return Convert.ToInt32(number);
        }
        private void takeScreenshot(String testName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            String date = String.Concat("__", DateTime.UtcNow.Month, "-", DateTime.UtcNow.Day, "-", DateTime.UtcNow.Year, "_", DateTime.UtcNow.Hour, "-", DateTime.UtcNow.Minute, "-", DateTime.UtcNow.Second, "__");
            String filename = String.Concat(testName, "__screenshot", date, ".png");
            ss.SaveAsFile(filename, ImageFormat.Png);
        }
    }
}
