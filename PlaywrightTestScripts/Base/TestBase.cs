using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using PlaywrightTestScripts.Model;
using static System.Net.Mime.MediaTypeNames;

namespace PlaywrightTestScripts.Base
{
    [TestFixture]
    public class TestBase
    {
        protected static IPlaywright _playwright;
        protected IBrowser _browser;
        protected IBrowserContext _context;
        protected IPage _page;
        protected EnvVariables _config;

        [OneTimeSetUp]
        public async static Task OneTimeSetup()
        {
            _playwright = await Playwright.CreateAsync();  // Initialize Playwright once for the whole test run
        }

        [SetUp]
        public async Task Setup()
        {
            string jsonFile = File.ReadAllText("Config.json");
            _config = JsonConvert.DeserializeObject<EnvVariables>(jsonFile) ?? throw new InvalidOperationException("Config file could not be deserialized.");
            string browserType = _config.browser ?? "chrome";
            bool headlessMode = _config.headlessMode;

            switch (browserType)
            {
                case "chrome":
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headlessMode,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                case "firefox":
                    _browser = await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headlessMode,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                case "webkit":
                    _browser = await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headlessMode,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

                default:
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headlessMode,
                        Args = new[] {
                                        "--disable-features=PasswordLeakDetection", // Disable password leak popup
                                        "--disable-save-password-bubble" // Disable password manager prompts
                        }
                    });
                    break;

            }
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null, // Maximizes window
                RecordVideoDir = "TestResult/Videos",

            });
            _page = await _context.NewPageAsync();
            await _page.SetViewportSizeAsync(1920, 1080);
            await _page.GotoAsync(_config.Url);
        }

        [OneTimeTearDown]
        public static void OneTimeCleanup()
        {
            
            _playwright?.Dispose();
        }

        [TearDown]
        public async Task TearDown()
        {
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                await CaptureScreenshot(TestContext.CurrentContext.Test.Name);
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
            //await _page.CloseAsync();
            //await _context.CloseAsync();
            //await _browser.CloseAsync();
        }
        public string Base64Encode(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public string Base64Decode(string text) 
        {
            var bit = Convert.FromBase64String(text);
            return Convert.ToBase64String(bit);
        }

        public async Task CaptureScreenshot(string testName)
        {
            string screenshotDir = "Screenshots";
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
        }


    }
}
