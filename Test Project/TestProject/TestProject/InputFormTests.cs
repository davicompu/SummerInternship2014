using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;
using OpenQA.Selenium;
using System.Drawing.Imaging;

namespace TestProject
{
    [TestClass]
    public class InputFormTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;

        [TestMethod]
        public void InputFormTest()
        {
            url = url = "http://localhost:57404/#/";
            setupFirefoxDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            doNavigationToCreateForm();
            DoEmptyFormTest();

            doNavigationToEditForm();
            DoEditFormTest();

            driver.Close();
        }

        private void setupFirefoxDriver()
        {
            driver = new FirefoxDriver();
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
        }


        private void doNavigationToCreateForm()
        {
            String link_text = "Create a new fund";

            //Navigate to the create new fund
            IWebElement link = driver.FindElement(By.LinkText(link_text));
            link.Click();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private void doNavigationToEditForm()
        {
            String link_text = "Funds";
            IWebElement link = driver.FindElement(By.LinkText(link_text));
            link.Click();
            //Navigate to any fund
            IWebElement table = driver.FindElement(By.CssSelector("table.tabular"));
            ReadOnlyCollection<IWebElement> links = table.FindElements(By.TagName("a"));
            Random random = new Random();
            int number = random.Next(0, links.Count);
            links[number].Click();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2)); //Forced wait to avoid testing from stopping because of loading issues
            String submit_class = "span.text-button";
            IWebElement submit_button = driver.FindElement(By.CssSelector(submit_class));
            
        }

        private void DoEmptyFormTest()
        {
            //In the Create Form
            String submit_class = "span.text-button";
            IWebElement submit_button = driver.FindElement(By.CssSelector(submit_class));
            testDefaultForm(submit_button);
            testEmptyFormWithBudgetAjustment(submit_button);
            testClearedForm(submit_button);

        }

        private void DoEditFormTest()
        {
            String submit_class = "span.text-button";
            IWebElement submit_button = driver.FindElement(By.CssSelector(submit_class));
            int textareas_valid = 0;
            int fields_to_validate = getInputFields();
            int validation_messages = getValidationMessages();
            IWebElement budgetAdjustment_input = driver.FindElement(By.Name("BudgetAdjustment"));
            int budgetAdjustment_input_val=getNumberFromText(budgetAdjustment_input.GetAttribute("value"));
            IWebElement desciption_area = driver.FindElement(By.Name("Description"));
            IWebElement budgetAdjustmentNote_area = driver.FindElement(By.Name("BudgetAdjustmentNote"));

            if (desciption_area.GetAttribute("value").Length > 0)
            {
                textareas_valid++;
            }

            if ((budgetAdjustment_input_val==0)&&(budgetAdjustmentNote_area.GetAttribute("value").Length==0))
            {
                textareas_valid++;
            }

            else if (budgetAdjustmentNote_area.GetAttribute("value").Length > 0)
            {
                textareas_valid++;
            }
         

            fields_to_validate-=textareas_valid;
            //debug vars
            //String message = String.Concat(budgetAdjustmentNote_area.GetAttribute("value").Length, ">>>", fields_to_validate, " == ", validation_messages);
            //Assert.Inconclusive(message);
            try {

                Assert.AreEqual(fields_to_validate, validation_messages);
            }
            catch (Exception e)
            {
                String m = String.Concat("Edit Form Test - There is no correct validation with input fields ",fields_to_validate,"/",validation_messages);
                takeScreenshot("EditFormTest");
                Assert.Fail(m);
            }
            submit_button.Click();

        }

        private void testDefaultForm(IWebElement button)
        {

            int important_fields;
            int validation_messages;

            button.Click();
            important_fields = getInputFields();
            validation_messages = getValidationMessages();

            try
            {
                Assert.AreEqual(important_fields - 1, validation_messages);

            }
            catch (Exception e)
            {
                String e_message = String.Concat("Default Form Test - There are not enough validation messages for empty fields (", important_fields - 1, "/", validation_messages, ")");
                takeScreenshot("DefaultFormTest");
                Assert.Fail(e_message);
            }
        }

        private void testClearedForm(IWebElement button)
        {
            int important_fields;
            int validation_messages;
            clearInputFields();
            button.Click();
            important_fields = getInputFields();
            validation_messages = getValidationMessages();

            try
            {
                Assert.AreEqual(important_fields - 1, validation_messages);
            }
            catch (Exception e)
            {
                String e_message = String.Concat("Cleared Form Test - There are not enough validation messages for empty fields (", important_fields - 1, "/", validation_messages, ")");
                takeScreenshot("ClearedFormTest");
                Assert.Fail(e_message);
            }
        }

        private void testEmptyFormWithBudgetAjustment(IWebElement button)
        {
            int important_fields;
            int validation_messages;
            driver.FindElement(By.Name("BudgetAdjustment")).SendKeys("1");
            button.Click();
            important_fields = getInputFields();
            validation_messages = getValidationMessages();
            //Produce validation
            try
            {
                Assert.AreEqual(important_fields, validation_messages);
            }
            catch (Exception e)
            {
                String e_message = String.Concat("Empty with budget adjustment Form Test - There are not enough validation messages for empty fields (", important_fields, "/", validation_messages, ")");
                takeScreenshot("EmptyBudgetAdjustmentTest");
                Assert.Fail(e_message);
            }
            driver.FindElement(By.Name("BudgetAdjustment")).Clear();
        }

        private int getInputFields()
        {
            ReadOnlyCollection<IWebElement> inputs = driver.FindElements(By.TagName("input"));
            ReadOnlyCollection<IWebElement> text_areas = driver.FindElements(By.TagName("textarea"));
            int input_count = 0;
            foreach (IWebElement input in inputs)
            {

                if ((!(input.GetAttribute("data-bind").Contains("negative-value"))) && (input.GetAttribute("type") == "text"))
                {
                    if (input.GetAttribute("value") == "")
                    {
                        input_count++;
                    }
                }


            }

            return input_count + text_areas.Count;
        }


        private void clearInputFields()
        {
            ReadOnlyCollection<IWebElement> inputs = driver.FindElements(By.TagName("input"));
            ReadOnlyCollection<IWebElement> text_areas = driver.FindElements(By.TagName("textarea"));
            foreach (IWebElement input in inputs)
            {

                if (input.GetAttribute("type") == "text")
                {
                    input.Clear();
                }
            }

            foreach (IWebElement textarea in text_areas)
            {
                textarea.Clear();
            }
        }

        private int getValidationMessages()
        {
            String error_class = "label.error-message";
            int error_count = 0;
            ReadOnlyCollection<IWebElement> error_messages = driver.FindElements(By.CssSelector(error_class));
            foreach (IWebElement em in error_messages)
            {

                if (em.GetAttribute("style") != "display: none;")
                {
                    error_count++;
                }

            }

            return error_count;
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
        private int getNumberFromText(String number)
        {
            number = number.Replace("$", "");
            number = number.Replace("(", "-");
            number = number.Replace(")", "");
            number = number.Replace(",", "");
            return Convert.ToInt32(number);
        }

    }


}
