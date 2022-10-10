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
            catch (Exception) { Console.WriteLine("Unable to open browser"); }

            Thread.Sleep(2000);

            
            //Get one day schedule block
            try
            {
                
                /*var slot= driver.FindElement(By.XPath("//*[@id='today']/thead/tr/th/h5"),10);
                
                Console.WriteLine(slot.GetAttribute("innerText"));
                Console.WriteLine(slot.Text);
               */

                //get for the weekend 
                for(int i = 1; i< 6; i++)
                {
                    
                    var block = driver.FindElement(By.XPath($"//*[@id='zoneTimetable']/div/div[{i}]/div"),10);
                    //*[@id="today"]/thead/tr/th/h5
                    //*[@id="zoneTimetable"]/div/div[3]/div/table/thead/tr/th/h5
                    //*[@id="zoneTimetable"]/div/div[4]/div/table/thead/tr/th/h5
                    //var blockDate = driver.FindElement(By.XPath(""), 10);
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

