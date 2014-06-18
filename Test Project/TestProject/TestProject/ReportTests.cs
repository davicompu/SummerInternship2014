using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class ReportTests
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js_executor;
        private String url;
        private ArrayList data;
        [TestMethod]
        public void ReportTest()
        {
            url = url = "http://localhost:57404/#/";
            setupFirefoxDriver();
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            doNavigationToReport();
            DoAreaReportCheck();
            driver.Close();
        }

        private void doNavigationToReport()
        {
            driver.FindElement(By.LinkText("Reports")).Click();
            driver.FindElement(By.LinkText("Funding Request")).Click();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        }
        private void setupFirefoxDriver()
        {
            driver = new FirefoxDriver();
            js_executor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private void DoAreaReportCheck(){
            data=getTable();
            checkCalculations();
        }

        private ArrayList getTable()
        {
           ArrayList sections= new ArrayList();
           ArrayList members = new ArrayList();
           ArrayList subtotal = new ArrayList();
           String active_section = "";
           ArrayList r,tmp;
           IWebElement table=driver.FindElement(By.CssSelector("table.tabular"));
           ReadOnlyCollection<IWebElement> rows = table.FindElements(By.TagName("tr"));
           foreach (IWebElement row in rows)
           {
               tmp=getRowArrayList(row);

               if ((tmp.Count == 1)&&(members.Count==0))
               {
                   active_section=row.Text;
                   members = new ArrayList();
                   subtotal = new ArrayList();
               }
               else if (tmp.Count == 1)
               {
                   r = new ArrayList();
                   r.Add(active_section);
                   r.Add(members);
                   r.Add(subtotal);
                   sections.Add(r);
                   active_section = row.Text;
                   members = new ArrayList();
                   subtotal = new ArrayList();

               }
               else if(tmp.Count==5){
                   
                   subtotal.Add(tmp);
               }

               else if(tmp.Count>5)
               {
               
                   members.Add(tmp);
               }
              
           }

           r = new ArrayList();
           r.Add(active_section);
           r.Add(members);
           r.Add(subtotal);
           sections.Add(r);
            //dwvug var
           //Assert.Inconclusive(String.Concat("Sections: ", sections.Count));
           return sections;
        }
        private void checkCalculations()
        {
            String active_section = "";
            ArrayList members,subtotal;
            int approved_budget, projected_expenditures, requested_budget, variance;
            foreach (ArrayList section in data)
            {
                active_section = section[0].ToString();
                members = (ArrayList)section[1];
                approved_budget = getSumOfColumn(members, 3);
                projected_expenditures = getSumOfColumn(members, 4);
                requested_budget = getSumOfColumn(members, 5);
                variance = getSumOfColumn(members, 6);
                subtotal = (ArrayList)section[2];
                subtotal = (ArrayList)subtotal[0];
                Assert.AreEqual(approved_budget,getNumberFromText(subtotal[1].ToString()));
                Assert.AreEqual(projected_expenditures, getNumberFromText(subtotal[2].ToString()));
                Assert.AreEqual(requested_budget, getNumberFromText(subtotal[3].ToString()));
                Assert.AreEqual(variance, getNumberFromText(subtotal[4].ToString()));
            }
        }

        private int getSumOfColumn(ArrayList rows, int column_index)
        {
            int total = 0;
            String text;
            int number;
            foreach (ArrayList row in rows)
            {
                text = row[column_index].ToString();
                number = getNumberFromText(text);
                total += number;
            }

            return total;
        }

        private ArrayList getRowArrayList(IWebElement incoming_row)
        {
            ArrayList row = new ArrayList();
            ReadOnlyCollection<IWebElement> cells = incoming_row.FindElements(By.TagName("td"));
            foreach (IWebElement cell in cells)
            {
                row.Add(cell.Text);
            }
            return row;
        }
        private int getNumberFromText(String number)
        {
            if (number.Length != 0)
            {
                number = number.Replace("$", "");
                number = number.Replace("(", "-");
                number = number.Replace(")", "");
                number = number.Replace(",", "");
                return Convert.ToInt32(number);
            }
            else
            {
                return 0;
            }
        }
    }
}
