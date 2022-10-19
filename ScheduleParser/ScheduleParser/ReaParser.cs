using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();
        private Dictionary<string, List<string>> WeekSchedule = new Dictionary<string, List<string>>();
        private List<string> Classes = new List<string>();
        private string URL = "https://rasp.rea.ru/";

        public List<string> RunParser(string _GroupId)
        {
            string GroupId = _GroupId;
            string currentwindow = driver.CurrentWindowHandle;
            
            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();

                input.SendKeys(GroupId);
                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();
            }
            catch (Exception) { Console.WriteLine("\n Unable to get to the website \n"); }

            Thread.Sleep(2000);

            for (int i = 1; i < 6; i++)
            {
                var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/thead/tr/th/h5"), 10);
                var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody"), 50);

                Classes.Add("\n Расписание на : \n" + WeekDayLabel.Text + "\n" + block.Text + "\n");
            }
            return Classes;
        }

        public List<string> GetWeekSchedule(string UserGroup)
        {
            return RunParser(UserGroup);
        }
    }
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            try
            {
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    return wait.Until(drv => drv.FindElement(by));
                }
            }
            catch (NoSuchElementException)
            { Console.WriteLine("Невозможно обнаружить элемент"); }
            return driver.FindElement(by);
        }
    }

}

