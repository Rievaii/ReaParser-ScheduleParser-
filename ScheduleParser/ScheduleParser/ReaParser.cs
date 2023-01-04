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

                //--------------------------------only for testing-------------------------------------------
                /*
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                IWebElement ChooseWeek = driver.FindElement(By.CssSelector("#navigation-cont"));
                ChooseWeek.Click();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                IWebElement WeekNumber = driver.FindElement(By.XPath("//*[@id='carouselExampleIndicators']/div/div[5]/a[1]"));
                WeekNumber.Click();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);
                */
                //-----------------------------------stop testing------------------------------------------------
                Thread.Sleep(2000);

                for (int Day = 1; Day < 6; Day++)
                {
                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/thead/tr/th/h5"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody"));
                    int AmountOfClasses = Int32.Parse(block.GetAttribute("childElementCount"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                    //test direct approach to rasp with available rasp
                    
                    if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody")).GetAttribute("childElementCount")) > 1)
                    {
                        for (int subject = 1; subject < AmountOfClasses; subject++)
                        {

                            //Subject number (х Пара)
                            var SubjectNumber = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]/span")).GetAttribute("innerText");
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            //Start Time
                            var StartTime = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]/text()[1]")).GetAttribute("innerText");
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            //End Time
                            var EndTime = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]/text()[2]")).GetAttribute("innerText");
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            //Subject Info
                            var SubjectInfo = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]/a")).GetAttribute("innerText");
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


                            WeekClasses.Add("\n" + WeekDayLabel.Text + "\n" + SubjectNumber + "    " + StartTime + " " + EndTime + "\n" + SubjectInfo);

                        }
                    }
                    else
                    {
                        WeekClasses.Add("\n" + WeekDayLabel.Text + "\n" + "Нет занятий");
                    }
                    
                    //WeekClasses.Add("\n" + WeekDayLabel.Text + "\n" + block.GetAttribute("innerText"));
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

