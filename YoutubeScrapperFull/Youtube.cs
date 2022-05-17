using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace YoutubeScrapperFull
{
    class Youtube
    {

        static Random random = new Random();

        public static List<string> GetChannelData(string videoUrl, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(videoUrl);
            if (driver.Url != videoUrl)
                return null;
            //Thread.Sleep(random.Next(2, 4) * 1000);

            string channelName = null;
            string subscribers = null;
            string channelUrl = null;
            string totalVideos = null;
            string totalVideoViews = null;
            string description = null;

            try
            {
                channelName = driver.FindElement(By.Id("text-container")).GetAttribute("innerText").Trim().Replace(",", "");
            }
            catch (Exception){}

            try
            {
                subscribers = driver.FindElement(By.Id("owner-sub-count")).GetAttribute("innerText").Split(' ')[0].Trim().Replace(",", "");
            }
            catch (Exception){}


            try
            {          
                channelUrl = driver.FindElement(By.XPath("//*[@id='top-row']/ytd-video-owner-renderer/a")).GetAttribute("href");
            }
            catch (Exception) { }

            try
            {
                totalVideoViews = driver.FindElement(By.XPath("//*[@id='count']/ytd-video-view-count-renderer/span[1]")).GetAttribute("innerText").Replace(",", "").Split(' ')[0];
            }
            catch (Exception) { }

            try
            {
                description = driver.FindElement(By.XPath("//*[@id='primary-inner']")).GetAttribute("innerText");
            }
            catch (Exception) { }


            try
            {
                driver.Navigate().GoToUrl(channelUrl);
                //Thread.Sleep(random.Next(2, 4) * 1000);
                string allvideourl = driver.FindElement(By.XPath("//*[@id='play-button']/ytd-button-renderer/a")).GetAttribute("href");
                driver.Navigate().GoToUrl(allvideourl);
                //Thread.Sleep(random.Next(1, 4) * 1000); 
            }
            catch (Exception)
            {
                goto ABOUT;
            }

            try
            {
                totalVideos = driver.FindElement(By.XPath("//*[@id='publisher-container']/div/yt-formatted-string/span[3]")).GetAttribute("innerText").Replace(",", "");
            }
            catch (Exception) { }






        ABOUT:
            if (channelUrl == "")
                return null;
            string aboutUrl = channelUrl + "/about";
            driver.Navigate().GoToUrl(aboutUrl);
            //Thread.Sleep(random.Next(2, 4) * 1000);

            string aboutPageData = null;
            string totalChannelViews = null;
            try
            {
                totalChannelViews = driver.FindElement(By.XPath("//*[@id='right-column']/yt-formatted-string[3]")).GetAttribute("innerText").Split(' ')[0].Replace(",","");
            }
            catch (Exception) { }
            try
            {
                aboutPageData = driver.FindElement(By.XPath("//*[@id='left-column']")).GetAttribute("innerText");
            }
            catch (Exception) { }
            

            string email = StringManipulation.SearchEmail(description + " " + aboutPageData);
            string sLinks = StringManipulation.SearchSocialLinks(description + " " + aboutPageData);

            if(email == null || email == "")
            {
                email = SearchEmailFromGivenLinks(driver, sLinks);
            }

            List<string> channelData = new List<string>()
            {
                channelName,channelUrl,totalChannelViews,subscribers,totalVideos,totalVideoViews,email,sLinks
            };
            return channelData;
        }

       
        public static string SearchEmailFromGivenLinks(IWebDriver driver, string sLinks)
        {
            string[] links = sLinks.Split('\n');
            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)driver;
            scriptExecutor.ExecuteScript("window.open()");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            string email = "";
            foreach(string link in links)
            {
                if (email == "" && !link.Contains("youtube.com"))
                {
                    try
                    {
                        driver.Navigate().GoToUrl(link);
                        //Thread.Sleep(3000);
                        string pageContent = driver.PageSource;
                        email = StringManipulation.SearchEmail(pageContent);
                    }
                    catch (Exception) { }
                }
                else if(email != "")
                {
                    break;
                }     
            }
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            return email;
        }

    }
}
