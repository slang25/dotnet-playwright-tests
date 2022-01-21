// Copyright (c) Martin Costello, 2021. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections;
using Microsoft.Playwright;

namespace PlaywrightTests;

public sealed class BrowsersTestData : IEnumerable<object[]>
{
    public static (string UserName, string AccessToken) BrowserStackCredentials()
    {
        string browserStackUserName = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
        string browserStackToken = Environment.GetEnvironmentVariable("BROWSERSTACK_TOKEN");

        return (browserStackUserName, browserStackToken);
    }

    public static bool IsRunningInGitHubActions { get; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

    public static bool UseBrowserStack { get; } =
        BrowserStackCredentials() != default &&
        // HACK Something is hanging when using BrowserStack on non-Windows operating systems in GitHub Actions
        (!IsRunningInGitHubActions || OperatingSystem.IsWindows());

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new[] { BrowserType.Chromium, null };

        if (UseBrowserStack || !OperatingSystem.IsWindows())
        {
            yield return new[] { BrowserType.Chromium, "chrome" };
        }

        if (UseBrowserStack || !OperatingSystem.IsLinux())
        {
            yield return new[] { BrowserType.Chromium, "msedge" };
        }

        yield return new object[] { BrowserType.Firefox, null };

        if (UseBrowserStack || OperatingSystem.IsMacOS())
        {
            yield return new object[] { BrowserType.Webkit, null };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
