using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using CreditCards.UITests.PageObjectModels;

namespace CreditCards.UITests
{
    public class CreditCardWebAppShould
    {
        private const string AboutUrl = "http://localhost:44108/Home/About";

        [Test]
        [Category("Smoke")]
        public void LoadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
            }
        }

        [Test]
        [Category("Smoke")]
        public void ReloadHomePageOnBack()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                string initialToken = homePage.GenerationToken;
               
                driver.Navigate().GoToUrl(AboutUrl);               
                driver.Navigate().Back();

                homePage.EnsurePageLoaded();

                string reloadedToken = homePage.GenerationToken;

                Assert.AreNotEqual(initialToken, reloadedToken);
            }
        }

        [Test]
        public void DisplayProductsAndRates()
        {
            using (IWebDriver driver = new ChromeDriver())
            {                
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                DemoHelper.Pause();

                Assert.AreEqual("Easy Credit Card", homePage.Products[0].name);
                Assert.AreEqual("20% APR", homePage.Products[0].interestRate);

                Assert.AreEqual("Silver Credit Card", homePage.Products[1].name);
                Assert.AreEqual("18% APR", homePage.Products[1].interestRate);

                Assert.AreEqual("Gold Credit Card", homePage.Products[2].name);
                Assert.AreEqual("17% APR", homePage.Products[2].interestRate);
            }
        }

        [Test]
        public void OpenContactFooterLinkInNewTab()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickContactFooterLink();

                ReadOnlyCollection<string> allTabs = driver.WindowHandles;
                string homePageTab = allTabs[0];
                string contactTab = allTabs[1];
                driver.SwitchTo().Window(contactTab);

                StringAssert.EndsWith("/Home/Contact", driver.Url);
            }
        }

        [Test]
        public void AlertIfLiveChatClosed()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLiveChatFooterLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                //IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());

                IAlert alert = wait.Until(x => IsAlertPresent(x));

                Assert.AreEqual("Live chat is currently closed.", alert.Text);                

                alert.Accept();
            }
        }

        [Test]
        public void NavigateToAboutUsWhenOkClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                //IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                IAlert alert = wait.Until(x => IsAlertPresent(x));

                alert.Accept();

                StringAssert.EndsWith("/Home/About", driver.Url);
            }
        }

        [Test]
        public void NotNavigateToAboutUsWhenCancelClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(5));
                //IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                IAlert alertBox = wait.Until(x => IsAlertPresent(x));
                alertBox.Dismiss();

                homePage.EnsurePageLoaded();
            }
        }

        [Test]
        public void NotDisplayCookieUseMessage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
                driver.Navigate().Refresh();

                Assert.IsFalse(homePage.IsCookieMessagePresent);

                driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
                driver.Navigate().Refresh();

                Assert.IsTrue(homePage.IsCookieMessagePresent);
            }
        }

        //[Test]
        //[UseReporter(typeof(BeyondCompare4Reporter))]
        //public void RenderAboutPage()
        //{
        //    using (IWebDriver driver = new ChromeDriver())
        //    {
        //        // We could also go and create a page object model for the About page
        //        driver.Navigate().GoToUrl(AboutUrl);

        //        ITakesScreenshot screenShotDriver = (ITakesScreenshot)driver;
        //        Screenshot screenShot = screenShotDriver.GetScreenshot();
        //        screenShot.SaveAsFile("aboutpage.png", ScreenshotImageFormat.Png);

        //        FileInfo file = new FileInfo("aboutpage.png");
        //        Approvals.Verify(file);
        //    }
        //}

        public static IAlert IsAlertPresent(IWebDriver driver)
        {
            try
            {
                return driver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
