using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class ReaParser
    {
        public bool UnableToGetToWebSite { get; set; }

        private string WeekSchedule;
        private string DaySchedule;

        public async Task<string> RunParser(string _GroupId)
        {


            var firefoxOptions = new FirefoxOptions();

            firefoxOptions.AddArgument("--no-sandbox");
            firefoxOptions.AddArgument("--headless");
            //C:\Users\dolgopolov.kv\Documents\GitHub\ScheduleParser
            // /home/app-admin/
            IWebDriver driver = new FirefoxDriver(@"/home/app-admin/", firefoxOptions);


            string URL = "https://rasp.rea.ru/";

            string GroupId = _GroupId;
            WeekSchedule = "";
            await Task.Run(() =>
            {
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

                        WeekSchedule += "\n &#128309;" + WeekDayLabel.Text;
                        if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody")).GetAttribute("childElementCount")) > 1)
                        {
                            for (int subject = 1; subject <= AmountOfClasses; subject++)
                            {
                                try
                                {

                                    if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]")).GetAttribute("childElementCount")) != 0)
                                    {
                                        //Subject Info
                                        var SubjectInfo = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]/a")).GetAttribute("innerText");
                                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                        var ClassTime = driver.FindElement(By.XPath($"//*[@id=\"zoneTimetable\"]/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]")).GetAttribute("innerText");
                                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                        //result string view
                                        WeekSchedule += "\n "+ "&#9726; " +  + ClassTime + ":    " + SubjectInfo + "\n";
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
                            WeekSchedule += "\n "+ "&#9726; " + "Нет занятий" + "\n";
                        }
                    }

                    return WeekSchedule;
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n Unable to get to the website \n" + e.Message);
                    UnableToGetToWebSite = true;
                    return "";
                }

            });
            driver.Quit();
            return WeekSchedule;
        }


        //:Override for the day 

        public async Task<string> RunParser(string _GroupId, string Date)
        {
            var firefoxOptions = new FirefoxOptions();

            firefoxOptions.AddArgument("--no-sandbox");
            firefoxOptions.AddArgument("--headless");

            IWebDriver driver = new FirefoxDriver(@"/home/app-admin/", firefoxOptions);


            string URL = "https://rasp.rea.ru/";

            string GroupId = _GroupId;

            DaySchedule = "";

            await Task.Run(() =>
            {
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
                            DaySchedule += "\n" + WeekDayLabel.Text;
                            if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody")).GetAttribute("childElementCount")) > 1)
                            {
                                for (int subject = 1; subject <= AmountOfClasses; subject++)
                                {
                                    try
                                    {
                                        if (Int32.Parse(driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]")).GetAttribute("childElementCount")) != 0)
                                        {
                                            //Subject Info
                                            var SubjectInfo = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[2]/a")).GetAttribute("innerText");
                                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                                            //Class Time
                                            var ClassTime = driver.FindElement(By.XPath($"//*[@id=\"zoneTimetable\"]/div/div[{Day}]/div/table/tbody/tr[{subject}]/td[1]")).GetAttribute("innerText");
                                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);


                                            //result string view
                                            //only if WeekDayLabel.Text == Date we fill string 
                                            DaySchedule += "\n" + " &#9726; " + ClassTime + ":    " + SubjectInfo + "\n";
                                        }
                                    }
                                    catch (NoSuchElementException e)
                                    {
                                        //=> Class does not exist
                                    }
                                }
                            }
                            else
                            {
                                DaySchedule += "\n" + "&#9726; " + "Нет занятий" + "\n";
                            }
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
            });
            driver.Quit();
            return DaySchedule;
        }
    }
}
