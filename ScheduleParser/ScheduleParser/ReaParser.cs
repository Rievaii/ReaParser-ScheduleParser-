using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class ReaParser
    {
        private IWebDriver driver = new ChromeDriver();

        private string URL = "https://rasp.rea.ru/";
        public bool UnableToGetToWebSite { get; set; }

        private string WeekSchedule;
        private string DaySchedule;

        public async Task<string> RunParser(string _GroupId)
        {

            string GroupId = _GroupId;
            WeekSchedule = "";

            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();

                input.SendKeys(GroupId);

                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();

                Thread.Sleep(1000);

                await Task.Run(() => { 
                for (int Day = 1; Day <= 6; Day++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);
                    int AmountOfClasses = Int32.Parse(block.GetAttribute("childElementCount"));


                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/thead/tr/th/h5"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


                    if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody")).GetAttribute("childElementCount")) > 1)
                    {
                        for (int subject = 1; subject <= AmountOfClasses; subject++)
                        {
                            try
                            {
                                //x - Пара
                                var SubjectNumber = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]/span")).GetAttribute("innerText");
                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]")).GetAttribute("childElementCount")) != 0)
                                {
                                    //Subject Info
                                    var SubjectInfo = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]/a")).GetAttribute("innerText");
                                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                    //result string view
                                    WeekSchedule += "\n" + WeekDayLabel.Text + "\n" + SubjectNumber + ":    " + SubjectInfo + "\n";
                                    //Console.WriteLine(WeekSchedule);
                                }
                            }
                            catch (NoSuchElementException e)
                            {
                                //=> Class does not exist
                                //Console.WriteLine(e);
                            }
                        }
                    }
                    else
                    {
                        WeekSchedule += "\n" + WeekDayLabel.Text + "\n" + "Нет занятий" + "\n";
                    }
                }
                });
                return WeekSchedule;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n Unable to get to the website \n" + e.Message);
                UnableToGetToWebSite = true;
                return "";
            }
        }

        //:Override for the day 
        
        public string RunParser(string _GroupId, string Date)
        {
            string GroupId = _GroupId;
            
            DaySchedule = "";

            try
            {
                driver.Navigate().GoToUrl(URL);

                IWebElement input = driver.FindElement(By.CssSelector("#search"));
                input.Click();

                input.SendKeys(GroupId);
                IWebElement search = driver.FindElement(By.Id("manual-search-btn"));
                search.Click();

                Thread.Sleep(1000);

                
                for (int Day = 1; Day <= 6; Day++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);
                    int AmountOfClasses = Int32.Parse(block.GetAttribute("childElementCount"));


                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/thead/tr/th/h5"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    int SeparatorIndex = WeekDayLabel.Text.IndexOf(",") + 2;
                    string WeekDayLabelDate = WeekDayLabel.Text.Substring(SeparatorIndex);

                    //get only for an exact day - make an exeption if day does not exist
                    if (Date == WeekDayLabelDate)
                    {
                        if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody")).GetAttribute("childElementCount")) > 1)
                        {
                            for (int subject = 1; subject <= AmountOfClasses; subject++)
                            {
                                try
                                {
                                    //x - Пара
                                    var SubjectNumber = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]/span")).GetAttribute("innerText");
                                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                    if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]")).GetAttribute("childElementCount")) != 0)
                                    {
                                        //Subject Info
                                        var SubjectInfo = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]/a")).GetAttribute("innerText");
                                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                        //result string view
                                        //only if WeekDayLabel.Text == Date we send output 
                                        DaySchedule += "\n" + WeekDayLabel.Text + "\n" + SubjectNumber + ":    " + SubjectInfo + "\n";
                                    }
                                }
                                catch (NoSuchElementException e)
                                {
                                    //=> Class does not exist
                                    //Console.WriteLine(e);
                                }
                            }
                        }
                        else
                        {
                            DaySchedule += "\n" + WeekDayLabel.Text + "\n" + "Нет занятий" + "\n";
                        }
                    }
                    else
                    {
                        DaySchedule += "\n" + Date + "\n" + "Нет занятий" + "\n";

                    }
                }
                return DaySchedule;
            }
            catch (Exception)
            {
                Console.WriteLine("\n Unable to get to the website \n");
                UnableToGetToWebSite = true;
                return "";
            }
        }
    }
}

