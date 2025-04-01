using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
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
            await context.CloseAsync();

        }
    }
}
