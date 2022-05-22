using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using System.IO;
using ApprovalTests;
using CreditCards.UITests.PageObjectModels;

namespace CreditCards.UITests
{
    public class CreditCardWebAppShould
    {
        private const string AboutUrl = "http://localhost:44108/Home/About";

        [Fact]
        [Trait("Category", "Smoke")]
        public void LoadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();
            }
        }

        [Fact]
        [Trait("Category", "Smoke")]
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

                Assert.NotEqual(initialToken, reloadedToken);
            }
        }

        [Fact]
        public void DisplayProductsAndRates()
        {
            using (IWebDriver driver = new ChromeDriver())
            {                
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                DemoHelper.Pause();

                Assert.Equal("Easy Credit Card", homePage.Products[0].name);
                Assert.Equal("20% APR", homePage.Products[0].interestRate);

                Assert.Equal("Silver Credit Card", homePage.Products[1].name);
                Assert.Equal("18% APR", homePage.Products[1].interestRate);

                Assert.Equal("Gold Credit Card", homePage.Products[2].name);
                Assert.Equal("17% APR", homePage.Products[2].interestRate);
            }
        }

        [Fact]
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

                Assert.EndsWith("/Home/Contact", driver.Url);
            }
        }

        [Fact]
        public void AlertIfLiveChatClosed()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLiveChatFooterLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());

                Assert.Equal("Live chat is currently closed.", alert.Text);                

                alert.Accept();
            }
        }

        [Fact]
        public void NavigateToAboutUsWhenOkClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                alert.Accept();

                Assert.EndsWith("/Home/About", driver.Url);
            }
        }

        [Fact]
        public void NotNavigateToAboutUsWhenCancelClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLearnAboutUsLink();

                WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(5));
                IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                alertBox.Dismiss();

                homePage.EnsurePageLoaded();
            }
        }

        [Fact]
        public void NotDisplayCookieUseMessage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
                driver.Navigate().Refresh();

                Assert.False(homePage.IsCookieMessagePresent);

                driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
                driver.Navigate().Refresh();

                Assert.True(homePage.IsCookieMessagePresent);
            }
        }

        [Fact]
        [UseReporter(typeof(BeyondCompare4Reporter))]
        public void RenderAboutPage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                // We could also go and create a page object model for the About page
                driver.Navigate().GoToUrl(AboutUrl);

                ITakesScreenshot screenShotDriver = (ITakesScreenshot)driver;
                Screenshot screenShot = screenShotDriver.GetScreenshot();
                screenShot.SaveAsFile("aboutpage.bmp", ScreenshotImageFormat.Bmp);

                FileInfo file = new FileInfo("aboutpage.bmp");
                Approvals.Verify(file);
            }
        }
    }
}
