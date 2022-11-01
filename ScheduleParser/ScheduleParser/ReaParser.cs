using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ScheduleParser
{
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();
        private List<string> WeekClasses = new List<string>();
        private List<string> DayClasses = new List<string>();
        private string URL = "https://rasp.rea.ru/";
        public bool UnableToGetToWebSite { get; set; }

        private List<string> RunParser(string _GroupId)
        {
            string GroupId = _GroupId;

            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();

                input.SendKeys(GroupId);
                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();


                Thread.Sleep(2000);

                for (int i = 1; i < 6; i++)
                {
                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/thead/tr/th/h5"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                    WeekClasses.Add("\n" + WeekDayLabel.Text + "\n" + block.GetAttribute("innerText"));
                }
            }
            catch (Exception)
            { 
                Console.WriteLine("\n Unable to get to the website \n");
                UnableToGetToWebSite = true;
            }
            return WeekClasses;
        }
        //:Override for a day
        public List<string> RunParser(string _GroupId, int ExactDay)
        {
            string GroupId = _GroupId;

            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();

                input.SendKeys(GroupId);
                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();


                Thread.Sleep(2000);

                if (ExactDay < 6)
                {
                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{ExactDay}]/div/table/thead/tr/th/h5"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{ExactDay}]/div/table/tbody"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                    DayClasses.Add("\n" + WeekDayLabel.Text + "\n" + block.GetAttribute("innerText"));
                }
                else
                {
                    DayClasses.Add("Нет занятий \n");
                }
            }
            catch (Exception) 
            {
                Console.WriteLine("\n Unable to get to the website \n");
                UnableToGetToWebSite = true;
            }
            return DayClasses;
        }
        public List<string> GetWeekSchedule(string UserGroup)
        {
            return RunParser(UserGroup);
        }
        public void ClearWeekSchedule()
        {
            WeekClasses.Clear();
        }
        public List<string> GetDaySchedule(string UserGroup, int ExactDay)
        {
            return RunParser(UserGroup, ExactDay);
        }
        public void ClearDaySchedule()
        {
            DayClasses.Clear();
        }
    }
}

