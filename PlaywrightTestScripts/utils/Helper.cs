using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using PlaywrightTestScripts.Base;

namespace PlaywrightTestScripts.utils
{
    public class Helper
    {
        private readonly IPage page;
        public Helper(IPage page)
        {
            this.page = page;
        }

        /// <summary>
        /// Selects an element on the page
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public async Task SelectElement(ILocator locator)
        {
            //await page.ClickAsync(selector);
            await locator.ClickAsync();
        }

        /// <summary>
        /// Scroll and Selects an element on the page
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public async Task ScrollAndSelectElement(ILocator locator)
        {
            //await page.Locator(selector).ScrollIntoViewIfNeededAsync();
            //await page.ClickAsync(selector);

            await locator.ScrollIntoViewIfNeededAsync();
            await locator.ClickAsync();
        }

        /// <summary>
        /// Double Clicks an element on the page
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public async Task DoubleClick(ILocator locator)
        {
            //await page.DblClickAsync(selector);
            await locator.DblClickAsync();
        }

        /// <summary>
        /// Right Clicks an element on the page
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public async Task RightClick(ILocator locator)
        {
            //await page.ClickAsync(selector, new PageClickOptions { Button = MouseButton.Right });
            await locator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
        }

        /// <summary>
        /// Enters value in a text box
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task EnterValues(ILocator locator, string value)
        {
            //await page.Locator(selector).FillAsync(value);
            await locator.FillAsync(value);
        }

        /// <summary>
        /// Enters value in a text box
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public async Task SelectFromDropDown(ILocator locator, string value)
        {
            await locator.SelectOptionAsync(value);
        }

        /// <summary>
        /// Handles Popup
        /// </summary>
        /// <returns></returns>
        public async Task HandlePopup()
        {
            page.Dialog += async (sender, dialog) =>
            {
                await dialog.AcceptAsync();
            };
            await Task.CompletedTask;
        }


        /// <summary>
        /// Switches to a iframe
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task SwitchFrame(string iFrame, string frameLocator)
        {
            var frame = page.FrameLocator(iFrame);
            await frame.Locator(frameLocator).ClickAsync();
        }

        /// <summary>
        /// Opens New Window
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task NewWindow()
        {
            var context = await page.Context.NewPageAsync();
            //await context.CloseAsync();

        }

        /// <summary>
        /// Encodes a string to Base64 format
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>

        public static string Base64Encode(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }

        /// <summary>
        /// Decodes a Base64 encoded string back to its original format
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Creates a folder in the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void CreateReportDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Moves files from source directory to destination directory, optionally recursively copying subdirectories.
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void MoveReportDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"source directory not found: {dir.FullName} ");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the source directory and copy them to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                if (!file.FullName.Contains("Results/.DS_Store"))
                {
                    string targetFilePath = Path.Combine(destinationDir, file.Name);
                    file.CopyTo(targetFilePath, true);
                }
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subdir.Name);
                    CreateReportDirectory(newDestinationDir);
                    MoveReportDirectory(subdir.FullName, newDestinationDir, true);
                }
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo folder in dir.GetDirectories())
            {
                folder.Delete(true);
            }
        }
    }
}
