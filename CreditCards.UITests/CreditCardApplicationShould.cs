using CreditCards.UITests.PageObjectModels;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CreditCards.UITests
{
    [Category("Applications")]
    public class CreditCardApplicationShould
    {
        private static IWebDriver Driver;

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Cookies.DeleteAllCookies();
            Driver.Navigate().GoToUrl("about:blank");
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            Driver.Dispose();
        }

        [Test]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
            var homePage = new HomePage(Driver);
            homePage.NavigateTo();

            ApplicationPage applicationPage = homePage.ClickApplyLowRateLink();

            applicationPage.EnsurePageLoaded();
        }

        [Test]
        public void BeInitiatedFromHomePage_EasyApplication()
        {
            var homePage = new HomePage(Driver);
            homePage.NavigateTo();

            homePage.WaitForEasyApplicationCarouselPage();

            ApplicationPage applicationPage = homePage.ClickApplyEasyApplicationLink();

            applicationPage.EnsurePageLoaded();
        }

        //[Test]
        //public void BeInitiatedFromHomePage_CustomerService()
        //{
        //    var homePage = new HomePage(ChromeDriverFixture.Driver);
        //    homePage.NavigateTo();

        //    homePage.WaitForCustomerServiceCarouselPage();

        //    ApplicationPage applicationPage = homePage.ClickCustomerServiceApplicationLink();

        //    applicationPage.EnsurePageLoaded();
        //}

        [Test]
        public void BeInitiatedFromHomePage_RandomGreeting()
        {
            var homePage = new HomePage(Driver);
            homePage.NavigateTo();

            ApplicationPage applicationPage = homePage.ClickRandomGreetingApplyLink();

            applicationPage.EnsurePageLoaded();
        }

        [Test]
        public void BeSubmittedWhenValid()
        {
            const string FirstName = "Sarah";
            const string LastName = "Smith";
            const string Number = "123456-A";
            const string Age = "18";
            const string Income = "50000";

            var applicationPage = new ApplicationPage(Driver);
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

            Assert.AreEqual("ReferredToHuman", applicationCompletePage.Decision);
            Assert.IsNotEmpty(applicationCompletePage.ReferenceNumber);
            Assert.AreEqual($"{FirstName} {LastName}", applicationCompletePage.FullName);
            Assert.AreEqual(Age, applicationCompletePage.Age);
            Assert.AreEqual(Income, applicationCompletePage.Income);
            Assert.AreEqual("Single", applicationCompletePage.RelationshipStatus);
            Assert.AreEqual("TV", applicationCompletePage.BusinessSource);
        }

        [Test]
        public void BeSubmittedWhenValidationErrorsCorrected()
        {
            const string firstName = "Sarah";
            const string invalidAge = "17";
            const string validAge = "18";

            var applicationPage = new ApplicationPage(Driver);
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
            Assert.AreEqual(2, applicationPage.ValidationErrorMessages.Count);
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
