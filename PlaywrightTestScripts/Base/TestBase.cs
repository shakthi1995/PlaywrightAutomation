using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options; 
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using PlaywrightTestScripts.Model;
using PlaywrightTestScripts.utils;

namespace PlaywrightTestScripts.Base
{
    [TestFixture]
    public class TestBase
    {

        protected static IPlaywright _playwright;
        protected IBrowser? _browser;
        protected IBrowserContext _context;
        protected IPage _page;
        protected static Userdetails _userDetails = new Userdetails();
        public static string workDirectory => Directory.GetCurrentDirectory();

        // Navigate up three levels to get to the project root
        public static string projectDirectory
        {
            get
            {
                var parent = Directory.GetParent(workDirectory);
                if (parent == null || parent.Parent == null || parent.Parent.Parent == null)
                {
                    throw new InvalidOperationException("Unable to determine the project directory. Ensure the directory structure is as expected.");
                }
                return parent.Parent.Parent.ToString();
            }
        }

        [OneTimeSetUp]
        public async static Task OneTimeSetup()
        {
            // Initialize the Reporter and cleanup the old reports
            var reportPath = Path.Combine(projectDirectory, "Report", $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            string testReportDir = Path.Combine(projectDirectory, "Report");
            string testArchiveDir = Path.Combine(projectDirectory, "Archive");

            Helper.CreateReportDirectory(testReportDir);
            Helper.CreateReportDirectory(testArchiveDir);
            Helper.MoveReportDirectory(testReportDir, testArchiveDir, true);

            Reporter.SetupExtentReport("UI Regression Test", "Playwright Test Report", reportPath);

            // Initialize Playwright once for the whole test run
            _playwright = await Playwright.CreateAsync();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectDirectory)
                .AddJsonFile("Config.json", optional: false, reloadOnChange: true)
                .Build();

            configuration.GetSection("userDetails").Bind(_userDetails);

            Settings.environment = configuration["env"] ?? "stage"; // Default to stage if not specified

            //Determining the url of the application
            switch(Settings.environment.ToLower())
            {
                case "stage":
                    Settings.url = configuration["stageUrl"] ?? "https://www.saucedemo.com/"; // Default stage URL if not specified
                    break;
                case "dev":
                    Settings.url = configuration["devUrl"] ?? "https://www.saucedemo.com/"; // Default production URL if not specified
                    break;
                default:
                    Settings.url = configuration["stageUrl"] ?? "https://www.saucedemo.com/"; // Default URL if not specified
                    break;
            }

            Settings.browser = configuration["browser"] ?? "chrome"; // Default to Chrome if not specified
            // Convert the configuration value to a boolean explicitly
            Settings.headlessMode = bool.TryParse(configuration["headlessMode"], out var headlessMode) ? headlessMode : false; // Default to headless mode false if not specified

            //Determining the user details for the application
            switch(Settings.environment.ToLower())
            {
                case "stage":
                    Settings.username = _userDetails.standard_user.username;
                    Settings.password = _userDetails.standard_user.password;                       
                    break;
                case "dev":
                    Settings.username = _userDetails.problem_user.username;
                    Settings.password = _userDetails.problem_user.password;
                    break;
                default:
                    Settings.username = _userDetails.standard_user.username;
                    Settings.password = _userDetails.standard_user.password;
                    break;
            }
        }

        [SetUp]
        public async Task Setup()
        {
            Reporter.CreateTest(TestContext.CurrentContext.Test.Name);
            switch (Settings.browser)
            {
                case "chrome":
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.headlessMode,
                        SlowMo = 1000,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                case "firefox":
                    _browser = await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.headlessMode,
                        SlowMo = 1000,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                case "webkit":
                    _browser = await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.headlessMode,
                        SlowMo = 1000,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                default:
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.headlessMode,
                        SlowMo = 1000,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;
            }
            if (_browser == null)
            {
                throw new InvalidOperationException("Browser instance is not initialized. Ensure OneTimeSetup has been executed successfully.");
            }

            if (string.IsNullOrEmpty(Settings.url))
            {
                throw new InvalidOperationException("The URL is not configured. Ensure the 'url' setting is specified in the configuration file.");
            }

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null, // Maximizes window
                RecordVideoDir = "TestResult/Videos",
            });

            _page = await _context.NewPageAsync();
            await _page.SetViewportSizeAsync(1920, 1080);

            await _page.GotoAsync(Settings.url!); // Use null-forgiving operator since we validated it above
            Reporter.LogToReport(Status.Pass, $"Navigated to {Settings.url} using {Settings.browser} browser in {((bool)Settings.headlessMode ? "headless" : "headed")} mode.");
        }

        [OneTimeTearDown]
        public static void OneTimeCleanup()
        {
            _playwright?.Dispose();
            Reporter.FlushReport();
        }

        [TearDown]
        public async Task TearDown()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status.ToString();

            Status logStatus;

            switch (testStatus)
            {
                case "Passed":
                    logStatus = Status.Pass;
                    break;
                case "Failed":
                    logStatus = Status.Fail;
                    Reporter.LogToReport(logStatus, $"Test failed: {TestContext.CurrentContext.Result.Message}");
                    await CaptureScreenshot(TestContext.CurrentContext.Test.Name);
                    break;
                case "Skipped":
                    logStatus = Status.Skip;
                    break;
                default:
                    logStatus = Status.Info;
                    break;
            }

            //Close the page and context after each test, so other tests have isolated resources
            if (_page != null)
            {
                await _page.CloseAsync();
            }

            if (_context != null)
            {
                await _context.CloseAsync();
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }
        public string Base64Encode(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public string Base64Decode(string text) 
        {
            var bit = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bit);
        }

        public async Task CaptureScreenshot(string testName)
        {
            string screenshotDir = Path.Combine(projectDirectory,"Screenshots");
            if (!Directory.Exists(screenshotDir))
            {
                Directory.CreateDirectory(screenshotDir);
            }

            string screenshotPath = Path.Combine(screenshotDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss)}.png");
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            TestContext.AddTestAttachment(screenshotPath, "Screenshot for Failure");
            Reporter.AddScreenshotToReport(screenshotPath);
        }
    }
}
