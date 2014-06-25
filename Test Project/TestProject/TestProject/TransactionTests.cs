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
    public class TransactionTests
    {
        private IWebDriver driver;
        private String edit_number;
        private IJavaScriptExecutor js_executor;
        private String url;
        [TestMethod]
        public void TransactionTest()
        {
            //url = "http://localhost:57404/#/";
            url = "https://test-foundation.vpfin.vt.edu";
            setupFirefoxDriver();
            DoTestTransaction();
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
            driver.Manage().Window.Maximize();
            doLoginCAS();
        }

        private void DoTestTransaction()
        {
            doCreateTest();
            doEditTest();
            doDeleteTest();
        }

        private void doDeleteTest()
        {
            Boolean success = false;

            driver.FindElement(By.PartialLinkText("Funds")).Click();
            driver.FindElement(By.PartialLinkText("Manage")).Click();
            driver.FindElement(By.PartialLinkText(edit_number)).Click();
            driver.FindElement(By.XPath("/html/body/div[2]/div/div[2]/div/section/div/div/div[2]/div/table/thead/tr/th[3]")).Click();
            driver.SwitchTo().Alert().Accept();
            driver.FindElement(By.PartialLinkText("Funds")).Click();
            driver.FindElement(By.PartialLinkText("Browse")).Click();
            js_executor.ExecuteScript("location.reload(); ");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

            try
            {
                driver.FindElement(By.LinkText(edit_number));
            }
            catch (Exception e)
            {
                success = true;
            }

            if (!success)
            {
                takeScreenshot("TransactionTest-Deletion");
                Assert.Fail("Transaction test in deletion failed");
            }
            
        }
            private void doCreateTest()
        {
            driver.FindElement(By.PartialLinkText("new")).Click();
            fillForm();

        }

        private void doEditTest()
        {
            driver.FindElement(By.PartialLinkText(edit_number)).Click();
            fillForm();

        }
        private void fillForm()
        {
            int[] values = new int[4];
            String status;

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            IWebElement found_name = driver.FindElement(By.Name("Number"));
            edit_number = found_name.GetAttribute("value");
            ReadOnlyCollection<IWebElement> inputs = driver.FindElements(By.TagName("input"));
            ReadOnlyCollection<IWebElement> textareas = driver.FindElements(By.TagName("textarea"));

            foreach (IWebElement input in inputs)
            {
                if ((input.GetAttribute("type") != "file") && (input.GetAttribute("name") != "Number"))
                {
                    Random random = new Random();

                    int number = random.Next(900000);
                    input.Clear();
                    input.SendKeys(number.ToString());


                }
            }

            foreach (IWebElement textarea in textareas)
            {
                driver.FindElement(By.TagName("body")).Click();
                Random random = new Random();
                int number = random.Next(900000);
                textarea.SendKeys(number.ToString());

            }

            values[0] = getNumberFromText(driver.FindElement(By.Name("CurrentBudget")).GetAttribute("value"));
            values[1] = getNumberFromText(driver.FindElement(By.Name("ProjectedExpenditures")).GetAttribute("value"));
            values[3] = getNumberFromText(driver.FindElement(By.Name("BudgetAdjustment")).GetAttribute("value"));
            values[2] = values[3] + values[0];

            SelectElement select = new SelectElement(driver.FindElement(By.Name("Status")));
            status = select.SelectedOption.Text;

            String submit_class = "span.text-button";
            IWebElement submit_button = driver.FindElement(By.CssSelector(submit_class));
            submit_button.Click();

            try
            {
                checkIfSaved(edit_number, values, status);
            }
            catch (Exception e)
            {
                takeScreenshot(String.Concat("TransactionTest-ListWhenSaved"));
                Assert.Fail("Trasaction Test failed on finding record "+edit_number);
            }
            
        }

        private Boolean checkIfSaved(String number, int[] values, String status)
        {

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            IWebElement table = driver.FindElement(By.CssSelector("table.tabular"));
            ReadOnlyCollection<IWebElement> rows = table.FindElements(By.TagName("tr"));
            ReadOnlyCollection<IWebElement> columns;
            foreach (IWebElement row in rows)
            {
                columns = row.FindElements(By.TagName("td"));

                if ((columns.Count > 5) && (columns[0].Text == number))
                {
                    try
                    {
                        Assert.AreEqual(values[0], getNumberFromText(columns[3].Text));
                        Assert.AreEqual(values[1], getNumberFromText(columns[4].Text));
                        Assert.AreEqual(values[2], getNumberFromText(columns[5].Text));
                        Assert.AreEqual(-(values[3]), getNumberFromText(columns[6].Text));

                        Assert.AreEqual(status, columns[7].Text);
                    }
                    catch (Exception e)
                    {
                        takeScreenshot(String.Concat("TransactionTest-Save"));
                        Assert.Fail(String.Concat("Transaction Test failed checking integrity information saved"));
                    }
                }

            }
            return false;

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
            String date = String.Concat(DateTime.UtcNow.Month, "-", DateTime.UtcNow.Day, "-", DateTime.UtcNow.Year, "_", DateTime.UtcNow.Hour, "-", DateTime.UtcNow.Minute, "-", DateTime.UtcNow.Second, "__");
            String filename = String.Concat(testName, "_screenshot_", date, ".png");
            ss.SaveAsFile(filename, ImageFormat.Png);
        }

    }
}
