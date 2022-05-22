using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CreditCards.UITests.PageObjectModels
{
    class HomePage : Page
    {
        public HomePage(IWebDriver driver)
        {
            Driver = driver;
        }

        protected override string PageUrl => "http://localhost:44108/";
        protected override string PageTitle => "Home Page - Credit Cards";

        public ReadOnlyCollection<(string name, string interestRate)> Products
        {
            get
            {
                var products = new List<(string name, string interestRate)>();

                var productCells = Driver.FindElements(By.TagName("td"));

                for (int i = 0; i < productCells.Count - 1; i += 2)
                {
                    string name = productCells[i].Text;
                    string interestRate = productCells[i + 1].Text;
                    products.Add((name, interestRate));
                }

                return products.AsReadOnly();
            }
        }

        public bool IsCookieMessagePresent => Driver.FindElements(By.Id("CookiesBeingUsed")).Any();

        public string GenerationToken => Driver.FindElement(By.Id("GenerationToken")).Text;

        public void ClickContactFooterLink() => Driver.FindElement(By.Id("ContactFooter")).Click();

        public void ClickLiveChatFooterLink() => Driver.FindElement(By.Id("LiveChat")).Click();

        public void ClickLearnAboutUsLink() => Driver.FindElement(By.Id("LearnAboutUs")).Click();


        public ApplicationPage ClickApplyLowRateLink()
        {
            Driver.FindElement(By.Name("ApplyLowRate")).Click();
            return new ApplicationPage(Driver);
        }

        public ApplicationPage ClickApplyEasyApplicationLink()
        {
            string script = @"document.evaluate('//a[text()[contains(.,\'Easy: Apply Now!\')]]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click();";
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript(script);

            return new ApplicationPage(Driver);
        }
        public ApplicationPage ClickCustomerServiceApplicationLink()
        {
            Driver.FindElement(By.ClassName("customer-service-apply-now")).Click(); 

            return new ApplicationPage(Driver);
        }
        
        public ApplicationPage ClickRandomGreetingApplyLink()
        {
            IWebElement randomGreetingApplyLink = Driver.FindElement(By.PartialLinkText("- Apply Now!"));
            randomGreetingApplyLink.Click();
            return new ApplicationPage(Driver);
        }


        public void WaitForEasyApplicationCarouselPage()
        {
            WebDriverWait wait = new WebDriverWait(Driver, timeout: TimeSpan.FromSeconds(11));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Easy: Apply Now!")));
        }

        public void WaitForCustomerServiceCarouselPage()
        {
            WebDriverWait wait = new WebDriverWait(Driver, timeout: TimeSpan.FromSeconds(22));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));
        }        
    }
}
