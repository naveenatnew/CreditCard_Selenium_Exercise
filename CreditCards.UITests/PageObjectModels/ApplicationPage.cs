using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CreditCards.UITests.PageObjectModels
{
    class ApplicationPage : Page
    {
        public ApplicationPage(IWebDriver driver)
        {
            Driver = driver;
        }

        protected override string PageUrl => "http://localhost:44108/Apply";
        protected override string PageTitle => "Credit Card Application - Credit Cards";

        public ReadOnlyCollection<string> ValidationErrorMessages
        {
            get
            {
                return Driver.FindElements(
                             By.CssSelector(".validation-summary-errors > ul > li"))
                             .Select(x => x.Text)
                             .ToList()
                             .AsReadOnly();
            }
        }

        public void ClearAge() => Driver.FindElement(By.Id("Age")).Clear();

        public void EnterFirstName(string firstName) => Driver.FindElement(By.Id("FirstName")).SendKeys(firstName);

        public void EnterLastName(string lastName) => Driver.FindElement(By.Id("LastName")).SendKeys(lastName);

        public void EnterFrequentFlyerNumber(string number) => Driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys(number);

        public void EnterAge(string age) => Driver.FindElement(By.Id("Age")).SendKeys(age);

        public void EnterGrossAnnualIncome(string income) => Driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys(income);

        public void ChooseMaritalStatusSingle() => Driver.FindElement(By.Id("Single")).Click();

        public void AcceptTerms() => Driver.FindElement(By.Id("TermsAccepted")).Click();

        public void ChooseBusinessSourceTV()
        {
            IWebElement businessSourceSelectElement = Driver.FindElement(By.Id("BusinessSource"));
            SelectElement businessSource = new SelectElement(businessSourceSelectElement);
            businessSource.SelectByValue("TV");
        }

        public ApplicationCompletePage SubmitApplication()
        {
            Driver.FindElement(By.Id("SubmitApplication")).Click();

            return new ApplicationCompletePage(Driver);
        }
    }
}
