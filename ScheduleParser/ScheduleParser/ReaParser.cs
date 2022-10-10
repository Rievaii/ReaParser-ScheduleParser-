using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ScheduleParser
{
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();

        public void GetSchedule(string GroupId)
        {
            string URL = "https://rasp.rea.ru/";

            string _GroupId = GroupId;

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
            catch (Exception) { Console.WriteLine("Unable to get to website"); }

            Thread.Sleep(2000);

            try
            { 
                //get for the weekend 
                for (int i = 1; i < 6; i++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div"), 10);
                    
                    Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");

                    if (block == null)
                    {
                        Console.WriteLine("Нет занятий");
                    }
                    else
                    {
                        Console.WriteLine(block.Text);
                    }

                    Console.WriteLine("---------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No such elements found");
            }

            Console.ReadLine();
            driver.Quit();
        }
        public void GetSchedule(string GroupId, int weekday)
        {
            string URL = "https://rasp.rea.ru/";

            string _GroupId = GroupId;

            if(weekday < 1 || weekday > 6)
            {
                throw new Exception("Weekday does not exist");
            }

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
            catch (Exception) { Console.WriteLine("Unable to get to website"); }

            Thread.Sleep(2000);

            //Get one day schedule block
            try
            {
                var slot = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{weekday}]/div"), 10);

                Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");
                Console.WriteLine(slot.Text);
                Console.WriteLine("---------------------------------------------------------------");

            }
            catch (Exception)
            {
                Console.WriteLine("No such elements found");
            }

            Console.ReadLine();
            driver.Quit();
        }
        public void GetSchedule()
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
            catch (Exception) { Console.WriteLine("Unable to get to website"); }

            Thread.Sleep(2000);

            try
            {
                //test by each class 
                for (int i = 1; i < 4; i++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='today']/tbody/tr[{i}]"), 10);

                    Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");

                    if (block == null)
                    {
                        Console.WriteLine("Нет занятий");
                    }
                    else
                    {
                        Console.WriteLine(block.Text);
                    }

                    Console.WriteLine("---------------------------------------------------------------");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No such elements found");
            }

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

