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
        public async Task CreatingIssueOnJiraUsingPOSTRequest()
        {
            try
            {
                using var client = new HttpClient();
                const string url = "https://(your project).atlassian.net/rest/api/2/issue/";
                const string username = "(Your Username)";
                const string password = "(Your API Tokey Key)";
                var data = new
                {
                    fields = new
                    {
                        project = new
                        {
                            key = "GB"
                        },
                        summary = "AT_IP_FT06_If these source and destination account numbers are same, system displays an error",
                        description = "FT06_If these source and destination account numbers are same, system displays an error",
                        issuetype = new
                        {
                            name = "Story"
                        },
                        customfield_10036 = "A message \"Account number does not exist\" if the entered acount number does not exist",
                        customfield_10037 = "A message \"Account number does not exist\" if the entered acount number does not exist was displayed",
                        customfield_10039 = "PayersAccountNo: 123456\r\nPayeeAccountNo: 123456\r\nAmount: 100.0,\r\nDescription: Send money",
                        customfield_10035 = "1/ Login as Manager\r\n2/ Go to Fund Transfer page\r\n3/ Enter the test data\r\n 4/ Press Submit"
                    }
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{username}:{password}")));
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    Log("Issue created successfully");
                }
                else
                {
                    Log($"Failed to create issue. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log($"Creating issue on Jira error: {ex.Message}");
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
        public async Task PerformFundTransfer(string PayersAccountNo, string PayeeAccountNo, decimal Amount, string Description)
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
            await CreatingIssueOnJiraUsingPOSTRequest();
        }

        public bool CheckLoginSuccess() => driver.Url.Equals(@"https://demo.guru99.com/V4/manager/Managerhomepage.php");

        public void EndTest()
        {

        }

    }
}