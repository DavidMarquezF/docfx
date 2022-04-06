using Xunit;

namespace AgritecDocFxPlugins.Tests;

public class ProfileConditionalTest
{
    [Fact]
    public void SpecificProfileDisplay()
    {
        var content = @"::: if-profile range=""por""
This is a test
::: if-profile-end";
        var expected = "<div range=\"por\"><p>This is a test</p></div>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "por" } });
    }

    [Fact]
    public void SpecificProfileDisplayWithInnerMarkdown()
    {
        var content = @"::: if-profile range=""por""
This is a test
# Test
::: if-profile-end";
        var expected = "<div range=\"por\"><p>This is a test</p><h1 id=\"test\">Test</h1></div>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "por" } });
    }

    [Fact]
    public void SpecificWrongProfileDisplay()
    {
        var content = @"
Hello
::: if-profile range=""por""
This is a test
::: if-profile-end";
        var expected = "<p>Hello</p>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "vac" } });
    }


    [Fact]
    public void MultipleProfileDisplay()
    {
        var content = @"::: if-profile range=""por,vac""
This is a test
::: if-profile-end";
        var expected = "<div range=\"por,vac\"><p>This is a test</p></div>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "por" } });
    }

}
