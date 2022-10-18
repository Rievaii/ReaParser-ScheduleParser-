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

        Dictionary<string, List<string>> WeekSchedule = new Dictionary<string, List<string>>();

        List<string> Classes = new List<string>();

        string URL = "https://rasp.rea.ru/";

        public void GetSchedule(string _GroupId)
        {
            var web = new HtmlWeb();

            string GroupId = _GroupId;

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

                Classes.Add("\n Расписание на : \n"+WeekDayLabel.Text +"\n"+ block.Text+"\n");
                //WeekSchedule.Add(WeekDayLabel.Text, Classes);
            }

            /*
            foreach (KeyValuePair<string, List<string>> kvp in WeekSchedule)
            {
                foreach (string value in kvp.Value)
                {
                    Console.WriteLine("Расписание на = {0}, \n {1} \n", kvp.Key, value);
                }
            }*/
            foreach(var element in Classes)
            {
                Console.Write(element); 
            }
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
            catch (OpenQA.Selenium.NoSuchElementException)
            { Console.WriteLine("Невозможно обнаружить элемент"); }
            return driver.FindElement(by);
        }
    }
}

