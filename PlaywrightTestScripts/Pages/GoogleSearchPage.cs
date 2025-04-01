using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTestScripts.Pages
{
    public class GoogleSearchPage
    {
        private readonly IPage _page;

        public GoogleSearchPage(IPage page)
        {
            _page = page;
        }

        #region WebElements

        //private ILocator searchBox => _page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "Search"});

        //private ILocator searchButton => _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Google Search" });
        private ILocator SearchBox => _page.Locator("[name=q]");
        private ILocator SearchButton => _page.Locator("[name=btnK]").First;


        #endregion

        #region Methods

        public async Task SearchFor(string searchQuery)
        {
            await SearchBox.FillAsync(searchQuery);
            await SearchButton.ClickAsync();
        }

        #endregion
    }
}
