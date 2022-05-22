using System;
using CreditCards.UITests.PageObjectModels;
using Xunit;

namespace CreditCards.UITests
{
    [Trait("Category", "Applications")]
    public class CreditCardApplicationShould : IClassFixture<ChromeDriverFixture>
    {                
        private readonly ChromeDriverFixture ChromeDriverFixture;

        public CreditCardApplicationShould(ChromeDriverFixture chromeDriverFixture)
        {
            ChromeDriverFixture = chromeDriverFixture;
            ChromeDriverFixture.Driver.Manage().Cookies.DeleteAllCookies();
            ChromeDriverFixture.Driver.Navigate().GoToUrl("about:blank");
        }

        [Fact]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
            var homePage = new HomePage(ChromeDriverFixture.Driver);
            homePage.NavigateTo();

            ApplicationPage applicationPage = homePage.ClickApplyLowRateLink();

            applicationPage.EnsurePageLoaded();
        }

        [Fact]
        public void BeInitiatedFromHomePage_EasyApplication()
        {
            var homePage = new HomePage(ChromeDriverFixture.Driver);
            homePage.NavigateTo();

            homePage.WaitForEasyApplicationCarouselPage();

            ApplicationPage applicationPage = homePage.ClickApplyEasyApplicationLink();

            applicationPage.EnsurePageLoaded();
        }

        [Fact]
        public void BeInitiatedFromHomePage_CustomerService()
        {
            var homePage = new HomePage(ChromeDriverFixture.Driver);
            homePage.NavigateTo();

            homePage.WaitForCustomerServiceCarouselPage();

            ApplicationPage applicationPage = homePage.ClickCustomerServiceApplicationLink();

            applicationPage.EnsurePageLoaded();
        }

        [Fact]
        public void BeInitiatedFromHomePage_RandomGreeting()
        {
            var homePage = new HomePage(ChromeDriverFixture.Driver);
            homePage.NavigateTo();

            ApplicationPage applicationPage = homePage.ClickRandomGreetingApplyLink();

            applicationPage.EnsurePageLoaded();
        }

        [Fact]
        public void BeSubmittedWhenValid()
        {
            const string FirstName = "Sarah";
            const string LastName = "Smith";
            const string Number = "123456-A";
            const string Age = "18";
            const string Income = "50000";

            var applicationPage = new ApplicationPage(ChromeDriverFixture.Driver);
            applicationPage.NavigateTo();

            applicationPage.EnterFirstName(FirstName);
            applicationPage.EnterLastName(LastName);
            applicationPage.EnterFrequentFlyerNumber(Number);
            applicationPage.EnterAge(Age);
            applicationPage.EnterGrossAnnualIncome(Income);
            applicationPage.ChooseMaritalStatusSingle();
            applicationPage.ChooseBusinessSourceTV();
            applicationPage.AcceptTerms();
            ApplicationCompletePage applicationCompletePage =
                applicationPage.SubmitApplication();

            applicationCompletePage.EnsurePageLoaded();

            Assert.Equal("ReferredToHuman", applicationCompletePage.Decision);
            Assert.NotEmpty(applicationCompletePage.ReferenceNumber);
            Assert.Equal($"{FirstName} {LastName}", applicationCompletePage.FullName);
            Assert.Equal(Age, applicationCompletePage.Age);
            Assert.Equal(Income, applicationCompletePage.Income);
            Assert.Equal("Single", applicationCompletePage.RelationshipStatus);
            Assert.Equal("TV", applicationCompletePage.BusinessSource);
        }

        [Fact]
        public void BeSubmittedWhenValidationErrorsCorrected()
        {
            const string firstName = "Sarah";
            const string invalidAge = "17";
            const string validAge = "18";

            var applicationPage = new ApplicationPage(ChromeDriverFixture.Driver);
            applicationPage.NavigateTo();

            applicationPage.EnterFirstName(firstName);
            // Don't enter lastname
            applicationPage.EnterFrequentFlyerNumber("123456-A");
            applicationPage.EnterAge(invalidAge);
            applicationPage.EnterGrossAnnualIncome("50000");
            applicationPage.ChooseMaritalStatusSingle();
            applicationPage.ChooseBusinessSourceTV();
            applicationPage.AcceptTerms();
            applicationPage.SubmitApplication();

            // Assert that validation failed                                
            Assert.Equal(2, applicationPage.ValidationErrorMessages.Count);
            Assert.Contains("Please provide a last name", applicationPage.ValidationErrorMessages);
            Assert.Contains("You must be at least 18 years old", applicationPage.ValidationErrorMessages);

            // Fix errors
            applicationPage.EnterLastName("Smith");
            applicationPage.ClearAge();
            applicationPage.EnterAge(validAge);

            // Resubmit form
            ApplicationCompletePage applicationCompletePage = applicationPage.SubmitApplication();

            // Check form submitted
            applicationCompletePage.EnsurePageLoaded();
        }
    }
}
