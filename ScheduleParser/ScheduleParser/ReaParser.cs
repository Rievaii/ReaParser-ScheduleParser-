using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace ScheduleParser
{
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();

        public void GetSchedule(string _GroupId)
        {
            string URL = "https://rasp.rea.ru/";

            string GroupId = _GroupId;

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

                //get by each class
                for (int i = 1; i < 6; i++)
                {
                    var AmountOfClasses = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody"), 10).GetAttribute("childElementCount");
                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/thead/tr/th/h5"), 10);

                    Console.WriteLine(WeekDayLabel.Text);

                    for (int j = 1; j < Int32.Parse(AmountOfClasses) + 1; j++)
                    {
                        var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody/tr[{j}]"), 50);

                        Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");

                        if (block == null)
                        {
                            Console.WriteLine("Нет пары");
                        }
                        else
                        {
                            Console.WriteLine(block.Text);
                        }

                        Console.WriteLine("---------------------------------------------------------------");
                    }
                }
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                Console.WriteLine("Нет занятий");
            }

            Console.ReadLine();
            driver.Quit();
        }
        //:Override for current day
        public void GetSchedule(string _GroupId, int _WeekDay)
        {
            string URL = "https://rasp.rea.ru/";

            string GroupId = _GroupId;

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

                var AmountOfClasses = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/tbody"), 10).GetAttribute("childElementCount");
                var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/thead/tr/th/h5"), 10);

                Console.WriteLine(WeekDayLabel.Text);

                for (int j = 1; j < Int32.Parse(AmountOfClasses) + 1; j++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/tbody/tr[{j}]"), 50);

                    Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");

                    Console.WriteLine(block.Text);

                    Console.WriteLine("---------------------------------------------------------------");
                }

            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                Console.WriteLine("Нет занятий");
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

