using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PostSharp.Patterns.Caching;
using SeleniumExtras.WaitHelpers;

namespace CloudPic.Tests.UITests
{
    public class BrowserTest
    {
        // Relative path is used
        private const string CROME_DRIVER_PATH = @"C:\Git\CloudPic\CloudPic.Tests\UITests\chromedriver_win32";

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void ShouldBeAbleToLogin()
        {
            using var driver = new ChromeDriver(CROME_DRIVER_PATH);

            driver.Navigate().GoToUrl("https://localhost:44338/");

            var driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

            var loginEmailElement = driver.FindElement(By.Id("login-email"));
            loginEmailElement.SendKeys("ivana.loncar@gmail.com");

            var loginPasswordElement = driver.FindElement(By.Id("login-password"));
            loginPasswordElement.SendKeys("lozinka");

            driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

            var loginButton = driver.FindElement(By.ClassName("btn-primary"));
            loginButton.Click();

            driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //driverWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Algebra: Naslovnica")));


        }

        [Test]
        public void Should_page_display_an_error_message()
        {
            using (var driver = new ChromeDriver(CROME_DRIVER_PATH))
            {
                driver.Navigate().GoToUrl("https://student.racunarstvo.hr");

                // <input name="login" type="text" id="username">
                IWebElement username = driver.FindElement(By.Id("username"));
                username.SendKeys("wrong user");

                // <input name="password" type="password" id="pass">
                IWebElement password = driver.FindElement(By.Id("pass"));
                password.SendKeys("wrong password");

                // <input type="button" value="Prijava" name="btn_login" onclick="submitIt();">
                driver.FindElement(By.Name("btn_login")).Click();

                // Wait for the error message "Greska 001: Korisničko ime i/ili lozinka nije točna."
                // <td>Greska 001: Korisničko ime i/ili lozinka nije točna.</td>
                new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                    .Until(d => d.PageSource.Contains("Greska 001: Korisničko ime i/ili lozinka nije točna."));
            }
        }
    }
}
