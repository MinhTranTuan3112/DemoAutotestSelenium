using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Atlassian.Jira;
using NUnit.Framework;
using System.Collections;

namespace NUnitWithSeleniumTestDemo
{
    [TestFixture]
    public class TestingUtils
    {
        //Fields
        public IWebDriver driver { get; set; }
        private delegate void LogDelegate(string text);
        private LogDelegate Log;
        //Constructor
        public TestingUtils()
        {
            ////Create a reference to Google Chrome Browser
            //const string profilePath = @"C:\Users\Asus\AppData\Local\Google\Chrome\User Data\Profile 1";
            //ChromeOptions options = new ChromeOptions();
            //options.AddArgument($"--user-data-dir={profilePath}");
            Log = MyLogger.Instance.LogToFile;
            this.driver = new ChromeDriver();
        }
        private readonly IDictionary CustomFieldsMapID = new Dictionary<string, string>()
        {
            {"Test Data","customfield_10039"},
            {"Test Steps", "customfield_10035"},
            {"Expected Result","customfield_10036" },
            {"Actual Result", "customfield_10037" },
        };
       
        //Methods

        public void StartTest()
        {

        }
        public void CreateIssueOnJira()
        {
            try
            {
                Log("Now creating issue on Jira");
                const string url = "https://minhttse.atlassian.net/";
                const string username = "minhttse172842@fpt.edu.vn";
                const string password = "ATATT3xFfGF0OAgqK-XqwgCQbb03kwvygCZyCGURHPxTNKwYLxReryqBbAQRFnHbDwBJcnALj7tizy0GLROtFUUK6dNgbp3dLT3402PrVb1kB4LUHYthJBRuPQwa0H0hAKOidTbTgoOYSOLz9dT53z9Zo7JNhe3GwFAhy48iKbW0VhDutDr4PXg=608BB3B9";
                const string AssigneeID = "712020:5c6999e8-8bf6-4d69-b493-8483b981cde1";
                var jira = Jira.CreateRestClient(url, username, password);
                var issue = jira.CreateIssue("GB");
                issue.Type = "10009";
                issue.Summary = "AT_IP_FT06_If these source and destination account numbers are same, system displays an error";
                issue.Description = "FT06_If these source and destination account numbers are same, system displays an error";
                issue.Assignee = AssigneeID;
                //issue["customfield_10035"] = "1/ Login as Manager" +
                //    "\n2/ Go to Fund Transfer page" +
                //    "\n3/ Enter the test data";
                //issue["customfield_10039"] = "PayersAccountNo: 123456\r\nPayeeAccountNo: 123456\r\nAmount: 100.0,\r\nDescription: Send money";
                //issue["customfield_10036"] = "A message \"Account number does not exist\" if the entered acount number does not exist";
                //issue["customfield_10037"] = "A message \"Account number does not exist\" if the entered acount number does not exist was displayed";
                issue.SaveChanges();
                Log("Created new issue on Jira");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
        [Test, Order(1)]
        public void PerformLoginAsManager()
        {
            //Navigate to the URL of Guru Banking Project v4
            MyLogger.Instance.LogToFile(@"Going to https://demo.guru99.com/V4/");
            driver.Navigate().GoToUrl(@"https://demo.guru99.com/V4/");

            //Wait for the page to load 
            Thread.Sleep(3000);

            //Find UserID input element using the name attribute with value uid
            IWebElement UserIDInputElement = driver.FindElement(By.Name("uid"));
            //Send Manager Account UserID 
            UserIDInputElement.SendKeys("mngr527204");
            MyLogger.Instance.LogToFile("Sent UserID");

            //Find Password input element using the name attribute with value password
            IWebElement PasswordInputElement = driver.FindElement(By.Name("password"));

            //Send Manager Account Password
            PasswordInputElement.SendKeys("ahYtaju");
            MyLogger.Instance.LogToFile("Sent Password");

            //Get login button
            IWebElement LoginButton = driver.FindElement(By.Name("btnLogin"));

            //Click on login button to login as manager account
            MyLogger.Instance.LogToFile("Login Operation Started");
            LoginButton.Click();

            //Check login success (the mananger page)
            Assert.IsTrue(CheckLoginSuccess());
            if (CheckLoginSuccess())
            {
                MyLogger.Instance.LogToFile("Login Success!!!");
            }
            else
            {
                MyLogger.Instance.LogToFile("Login Failed!!!");
            }
            MyLogger.Instance.LogToFile("Login Step is done");
        }
        [Test, Order(2)]
        [TestCase("123456", "123456", 100.0, "Send money")]
        public void PerformFundTransfer(string PayersAccountNo, string PayeeAccountNo, decimal Amount, string Description)
        {
            MyLogger.Instance.LogToFile("------------- Fund Transfer Started -------------------");
            IWebElement FundTransferPageLink = driver.FindElement(By.XPath("//a[@href='FundTransInput.php']"));
            FundTransferPageLink.Click();
            MyLogger.Instance.LogToFile("Went to Fund Transfer page");
            //Skip ads
            Thread.Sleep(6000);
            // Switch back to the default content
            driver.SwitchTo().DefaultContent();
            IWebElement PayersAccountElement = driver.FindElement(By.Name("payersaccount"));
            PayersAccountElement.SendKeys(PayersAccountNo);
            Thread.Sleep(1000);
            IWebElement PayeesAccountElement = driver.FindElement(By.Name("payeeaccount"));
            PayeesAccountElement.SendKeys(PayeeAccountNo);
            Thread.Sleep(1000);
            IWebElement AmountElement = driver.FindElement(By.Name("ammount"));
            AmountElement.SendKeys(Amount.ToString());
            Thread.Sleep(1000);
            IWebElement DescriptionElement = driver.FindElement(By.Name("desc"));
            DescriptionElement.SendKeys(Description);
            Thread.Sleep(1000);
            IWebElement SubmitButton = driver.FindElement(By.Name("AccSubmit"));
            SubmitButton.Click();

            //Get the alert
            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            Assert.That(!string.IsNullOrEmpty(alertText));
            MyLogger.Instance.LogToFile($"Alert text: {alertText}");
            // Accept the alert (or you can dismiss it if needed)
            alert.Accept();
            // Switch back to the default content
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(3000);
            driver.Close();
            CreateIssueOnJira();
        }

        public bool CheckLoginSuccess() => driver.Url.Equals(@"https://demo.guru99.com/V4/manager/Managerhomepage.php");

        public void EndTest()
        {

        }

    }
}