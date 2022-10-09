using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ScheduleParser
{
    //class "row" -> "col-lg-6 col-12" -> div class = "container" -> class = "table table-light" -> slot n / no slots 
    //@https://rasp.rea.ru/?q=15.27%D0%B4-%D0%B8%D1%81%D1%8215%2F22%D0%B1
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();
        
        public void RunParse(string _URL)
        {
            string URL = "https://rasp.rea.ru/";

            string GroupId = "15.27Д-ИСТ15/22б";

            var web = new HtmlWeb();

            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();


                input.SendKeys(GroupId);
                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();

            }
            catch (Exception) { Console.WriteLine("Unable to open browser"); }

            Thread.Sleep(2000);

            Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");
            try
            {
                var slot= driver.FindElement(By.XPath("//*[@id='zoneTimetable']/div/div[1]/div"),10);
                Console.WriteLine(slot.GetAttribute("innerText"));
                Console.WriteLine(slot.Text);

                /*Console.WriteLine(slots.Count);

                for (int i = 0; i < slots.Count; i++)
                {
                    Console.WriteLine(slots[i].GetAttribute("innerText"));
                    Console.WriteLine(slots[i]);
                    Console.WriteLine(slots[i].Text);
                }
                */
            }
            catch (Exception)
            {
                Console.WriteLine("No such elements found");
            }
            Console.WriteLine("---------------------------------------------------------------");

            Console.ReadLine();
            driver.Quit();
        }
    }
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}

