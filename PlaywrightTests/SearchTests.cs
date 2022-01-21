// Copyright (c) Martin Costello, 2021. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace PlaywrightTests;

public class SearchTests
{
    public SearchTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    [Theory]
    [ClassData(typeof(BrowsersTestData))]
    public async Task Search_For_DotNet_Core(string browserType, string browserChannel)
    {
        // Configure the options to use with the fixture for this test
        var options = new BrowserFixtureOptions()
        {
            BrowserType = browserType,
            BrowserChannel = browserChannel,
            Build = GetType().Assembly.GetName().Version.ToString(3),
            ProjectName = "dotnet-playwright-tests",
        };

        // Use BrowserStack if credentials are configured
        string browserStackUserName = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
        string browserStackToken = Environment.GetEnvironmentVariable("BROWSERSTACK_TOKEN");

        if (!string.IsNullOrEmpty(browserStackUserName) &&
            !string.IsNullOrEmpty(browserStackToken))
        {
            options.BrowserStackCredentials = (browserStackUserName, browserStackToken);
            options.UseBrowserStack = true;
        }

        // Create fixture that will provide an IPage to use for the test
        var browser = new BrowserFixture(options, OutputHelper);
        await browser.WithPageAsync(async (page) =>
        {
            // Open the search engine
            await page.GotoAsync("https://www.google.com/");
            await page.WaitForLoadStateAsync();

            // Dismiss any cookies dialog
            IElementHandle element = await page.QuerySelectorAsync("text='I agree'");

            if (element is not null)
            {
                await element.ClickAsync();
            }

            // Search for the desired term
            await page.TypeAsync("[name='q']", ".net core");
            await page.Keyboard.PressAsync("Enter");

            // Wait for the results to load
            await page.WaitForSelectorAsync("id=main");

            // Click through to the desired result
            await page.ClickAsync("a:has-text(\".NET\")");
        });
    }
}
