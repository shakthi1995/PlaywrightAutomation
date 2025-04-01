using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaywrightTestScripts.Base;
using PlaywrightTestScripts.Pages;

namespace PlaywrightTestScripts.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class GoogleTest : TestBase
    {
        //[TestCase(TestName = "Verify the user performs serach in google"), Parallelizable]
        public async Task GoogelSearch()
        {
            try
            {
                GoogleSearchPage lPage = new GoogleSearchPage(_page);
                await lPage.SearchFor("Playwright");
            }
            catch (Exception e)
            {
                //Log the exception in report and throw it
                throw;
            }
        }

        //[TestCase(TestName = "Verify the user performs another search in google"), Parallelizable]
        public async Task GoogelSearch2()
        {
            try
            {
                GoogleSearchPage lPage2 = new GoogleSearchPage(_page);
                await lPage2.SearchFor("Playwright");
            }
            catch (Exception e)
            {
                //Log the exception in report and throw it
                throw;
            }
        }

    }
}
