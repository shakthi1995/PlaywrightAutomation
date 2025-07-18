using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using PlaywrightTestScripts.Base;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using NUnit.Framework;

namespace PlaywrightTestScripts.utils
{
    public class Reporter
    {
        public static ExtentReports? extentReports;
        public static ExtentSparkReporter? sparkReporter;
        private static ConcurrentDictionary<string, ExtentTest> _testMap = new ConcurrentDictionary<string, ExtentTest>();
        
        public static void SetupExtentReport(string reportName, string documentTitle, dynamic path)
        {
            extentReports = new ExtentReports();
            sparkReporter = new ExtentSparkReporter(path);
            
            sparkReporter.Config.Theme = Theme.Standard;
            sparkReporter.Config.DocumentTitle = documentTitle;
            sparkReporter.Config.ReportName = reportName;

            extentReports.AttachReporter(sparkReporter);
            extentReports.AddSystemInfo("Environment", Settings.environment);
            extentReports.AddSystemInfo("Browser", Settings.browser);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void CreateTest(string testName)
        {
            if (extentReports == null)
            {
                Console.WriteLine("ExtentReports is not initialized. Call SetupExtentReport before CreateTest.");
                return;
            }
            var testId = TestContext.CurrentContext.Test.ID;
            Console.WriteLine($"Creating test: {testName} with ID: {testId}");
            var test = extentReports.CreateTest(testName);
            _testMap[testId] = test;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LogToReport(Status status, string message)
        {
            var testId = TestContext.CurrentContext.Test.ID;
            if (!_testMap.TryGetValue(testId, out var test) || test == null)
            {
                Console.WriteLine($"[ExtentReport] extentTest is null for testId {testId}. Call CreateTest before logging. Message: {message}");
                return;
            }
            test.Log(status, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void FlushReport()
        {
            if (extentReports == null)
            {
                Console.WriteLine("ExtentReports is not initialized. Nothing to flush.");
                return;
            }
            Console.WriteLine("Flushing report...");
            extentReports.Flush();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddScreenshotToReport(string screenshotPath)
        {
            var testId = TestContext.CurrentContext.Test.ID;
            if (_testMap.TryGetValue(testId, out var test) && test != null)
            {
                test.AddScreenCaptureFromPath(screenshotPath);
            }
            else
            {
                Console.WriteLine($"ExtentTest is null for testId {testId}, cannot add screenshot.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void TestStatus(string status)
        {
            var testId = TestContext.CurrentContext.Test.ID;
            if (!_testMap.TryGetValue(testId, out var test) || test == null)
            {
                Console.WriteLine($"[ExtentReport] extentTest is null for testId {testId}. Call CreateTest before setting test status. Status: {status}");
                return;
            }
            if (status.Equals("Passed"))
            {
                test.Pass("Test passed successfully.");
            }
            else if (status.Equals("Failed"))
            {
                test.Fail("Test failed.");
            }
            else if (status.Equals("Skipped"))
            {
                test.Skip("Test was skipped.");
            }
            else
            {
                test.Info($"Test status: {status}");
            }
        }
    }
}
