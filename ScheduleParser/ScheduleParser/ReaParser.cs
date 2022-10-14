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
        
        //make it async
        //get schedule for the week
        public void GetSchedule(string _GroupId)
        {
            List <string> schedule = new List<string> ();                                                                     
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
            catch (Exception) { Console.WriteLine("Unable to get to the website"); }

            Thread.Sleep(2000);

            try
            {
                for (int i = 1; i < 6; i++)
                {
                    var AmountOfClasses = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody"), 10).GetAttribute("childElementCount");
                    var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/thead/tr/th/h5"), 10);

                    Console.WriteLine(WeekDayLabel.Text);
                    schedule[i] = WeekDayLabel.Text;

                    Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");

                    for (int j = 1; j < Int32.Parse(AmountOfClasses) + 1; j++)
                    {
                        var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div/table/tbody/tr[{j}]"), 50);

                        //Console.WriteLine(block.Text);
                        schedule.Add(block.Text);
                        //send.message 
                        
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

        //:Override for an exact day 
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
            catch (Exception) { Console.WriteLine("Unable to get to the website"); }

            Thread.Sleep(2000);

            try
            {
                var AmountOfClasses = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/tbody"), 10).GetAttribute("childElementCount");
                var WeekDayLabel = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/thead/tr/th/h5"), 10);

                Console.WriteLine("---------------------CONSOLE OUTPUT:--------------------------");
                Console.WriteLine(WeekDayLabel.Text);

                for (int j = 1; j < Int32.Parse(AmountOfClasses) + 1; j++)
                {
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{_WeekDay}]/div/table/tbody/tr[{j}]"), 50);

                    Console.WriteLine(block.Text);
                    //send.message

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
            try
            {
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    return wait.Until(drv => drv.FindElement(by)); 
                }
            }catch(OpenQA.Selenium.NoSuchElementException ex)
            //message send
            { Console.WriteLine("Невозможно обнаружить элемент"); }                    
            return driver.FindElement(by);
        }
    }
    
}

