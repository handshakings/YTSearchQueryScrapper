using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace YoutubeScrapperFull
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Entrer youtube search query");
            string searchQuery = Console.ReadLine();
            Console.WriteLine("Enter no of videos to scrap");
            string noOfVideos = Console.ReadLine();
            Console.WriteLine("Press enter to start scrapping");
            Console.ReadLine();

            GetDataFromYoutubeAndFillCsv(searchQuery, noOfVideos);
        }
        public static List<string> GetListOfVideoUrls(IWebDriver driver, string searchQuery, string noOfVideos)
        {
            string searchQueryUrl = "https://www.youtube.com/results?search_query=" + searchQuery;
            driver.Navigate().GoToUrl(searchQueryUrl);

            
            int a = 0;
            while (a < int.Parse(noOfVideos) * 6)
            {
                driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowDown);
                a++;
                Thread.Sleep(new Random().Next(1,100));
            }

            driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowDown);

            var elements = driver.FindElements(By.XPath("//*[@id='contents']/ytd-video-renderer")).ToList();
            List<string> youtubeVideoUrlList = new List<string>();
            int count = 0;
            foreach (var element in elements)
            {
                var vidUrl = element.FindElement(By.XPath("div[1]/ytd-thumbnail/a")).GetAttribute("href");
                youtubeVideoUrlList.Add(vidUrl);
                count++;
                if (count == int.Parse(noOfVideos)+3)
                    break;
            }
            return youtubeVideoUrlList;
        }
        static public void GetDataFromYoutubeAndFillCsv(string searchQuery, string noOfVideos)
        {
            FileStream fileStream = new FileStream("youtube.csv", FileMode.Append);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine("SNo,Channel Name,Channel URL,Channel Views,Subscribers,Total Videos,Video Views,Email,Social Links");

            IWebDriver chromeDriver = CreateChromeDriver();
            IWebDriver firefoxDriver = CreateFirefoxDriver();
            List<string> youtubeVideoUrlList = GetListOfVideoUrls(chromeDriver, searchQuery, noOfVideos);

            int j = 2;
            foreach (string vidUrl in youtubeVideoUrlList)
            {
                string videoUrl = vidUrl.Replace("\r", "");
                List<string> channelData = new List<string>();
                if (j % 2 == 0)
                {
                    channelData = Youtube.GetChannelData(videoUrl, firefoxDriver);
                }
                else
                {
                    channelData = Youtube.GetChannelData(videoUrl, chromeDriver);
                }
                if (channelData.Count > 0)
                {
                    writer.WriteLine(j - 1 + "," + channelData.ElementAt(0) + "," + channelData.ElementAt(1) + "," + channelData.ElementAt(2) + "," + channelData.ElementAt(3) + "," + channelData.ElementAt(4) + "," + channelData.ElementAt(5) + "," + "\"" + channelData.ElementAt(6) + "\"" + "," + "\"" + channelData.ElementAt(7).Trim() + "\"");
                    writer.Flush();
                    fileStream.Flush();

                    Console.WriteLine(j - 1 + "\t" + channelData.ElementAt(0) + "\t" + channelData.ElementAt(6));
                    //if (j > 4 && j % 5 == 0)
                    //{
                    //    Console.WriteLine("Intentially give a pause to do safe scraping");
                    //    Thread.Sleep(random.Next(15, 25) * 1000);
                    //}  
                }
                j++;
            }
            writer.Close();
            fileStream.Close();

            chromeDriver.Quit();
            firefoxDriver.Quit();
        }

        static private ChromeDriver CreateChromeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddExcludedArgument("enable-automation");
            //options.AddAdditionalCapability("useAutomationExtension", false);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            //TimeSpan.FromSeconds is the max time for request to timeout
            ChromeDriver driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(80));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(80));
            driver.Manage().Window.Maximize();
            return driver;
        }
        static private FirefoxDriver CreateFirefoxDriver()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            FirefoxDriver driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(80));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(80));
            driver.Manage().Window.Maximize();
            return driver;
        }
    

    }
}
