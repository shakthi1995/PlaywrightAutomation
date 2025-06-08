using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using PlaywrightTestScripts.Base;
using PlaywrightTestScripts.Pages;
using PlaywrightTestScripts.utils;

namespace PlaywrightTestScripts.Tests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class UITest : TestBase
    {
        [TestCase(TestName = "Verify the standard user logs into sauce demo"), Parallelizable]
        public async Task StandardUserlogin()
        {
            try
            {
                LandingPage lPage = new LandingPage(_page);
                await lPage.EnterAndSubmitUserDetails(Helper.Base64Decode(Settings.username), Helper.Base64Decode(Settings.password));
                
                await Assertions.Expect(_page).ToHaveTitleAsync("Swag Labs");
            }
            catch (Exception e)
            {
                //Log the exception in report and throw it
                throw;
            }
        }

        [TestCase(TestName = "Verify the user adds backpack to the cart"), Parallelizable]
        public async Task AddProductToCart()
        {
            try
            {
                LandingPage lPage = new LandingPage(_page);
                await lPage.EnterAndSubmitUserDetails(Helper.Base64Decode(Settings.username), Helper.Base64Decode(Settings.password));
                
                ProductsPage productsPage = new ProductsPage(_page);
                await productsPage.BackpackAddToCartFunction();
                await Assertions.Expect(_page.Locator(".shopping_cart_badge")).ToHaveTextAsync("1");
            }
            catch (Exception e)
            {
                //Log the exception in report and throw it
                throw;
            }
        }

        [TestCase(TestName = "Verify the user adds backpack to the cart and verifies it in the card"), Parallelizable]
        public async Task CartVerification()
        {
            try
            {
                LandingPage lPage = new LandingPage(_page);
                await lPage.EnterAndSubmitUserDetails(Helper.Base64Decode(Settings.username), Helper.Base64Decode(Settings.password));
                
                ProductsPage productsPage = new ProductsPage(_page);
                await productsPage.BackpackAddToCartFunction();
                await productsPage.OpenCart();
                await Assertions.Expect(_page.Locator(".inventory_item_name")).ToBeVisibleAsync();
                await Assertions.Expect(_page.Locator(".inventory_item_name")).ToHaveTextAsync("Sauce Labs Backpack");
            }
            catch (Exception e)
            {
                //Log the exception in report and throw it
                throw;
            }
        }
    }
}
