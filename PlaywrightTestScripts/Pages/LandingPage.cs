using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTestScripts.utils;

namespace PlaywrightTestScripts.Pages
{
    public class LandingPage
    {
        private readonly IPage _page; 
        public LandingPage(IPage page)
        {
            _page = page;
        }

        #region WebElements

        private ILocator UserNameTextBox => _page.Locator("#user-name");

        private ILocator PasswordTextBox => _page.Locator("#password");

        private ILocator LoginButton => _page.Locator("#login-button");
        //private ILocator MaleRadioButton => _page.Locator("[value=male]");

        #endregion

        #region Methods

        public async Task EnterAndSubmitUserDetails(string name, string email)
        {
            //Helper _helper = new Helper(_page);
            //await _helper.EnterValues(UserNameTextBox, name);
            //await _helper.EnterValues(PasswordTextBox, email);
            //await _helper.SelectElement(LoginButton);

            await UserNameTextBox.FillAsync(name);
            await PasswordTextBox.FillAsync(email);
            await LoginButton.ClickAsync();

        }

        //public async Task ScrollAndSelectRadioButton()
        //{
        //    await MaleRadioButton.ScrollIntoViewIfNeededAsync();
        //    await MaleRadioButton.ClickAsync();
        //}

        #endregion
    }
}
