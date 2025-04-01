using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTestScripts.utils;

namespace PlaywrightTestScripts.Pages
{
    public class ProductsPage
    {
        private readonly IPage _page; 
        public ProductsPage(IPage page)
        {
            _page = page;
        }

        #region WebElements

        private ILocator BackpackAddToCartButton => _page.Locator("#add-to-cart-sauce-labs-backpack");

        private ILocator CartButton => _page.Locator(".shopping_cart_badge");

        #endregion

        #region Methods

        public async Task BackpackAddToCartFunction()
        {
            await BackpackAddToCartButton.ClickAsync();
        }

        public async Task OpenCart()
        {
            await CartButton.ClickAsync();
        }



        #endregion
    }
}
