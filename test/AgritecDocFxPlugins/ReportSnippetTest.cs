using Xunit;

namespace AgritecDocFxPlugins.Tests;

public class ReportSnippetTest
{
    [Fact]
    public void AnimalKeywordTestGeneral()
    {
        var content = @"[!agr-report[test]]";
        var expected = "<p><a href=\"test.pdf\">Download PDF example</a></p><p><img src=\"test.png\" alt=\"Report image\" /></p>";

        TestUtility.VerifyMarkup(content, expected);
    }


}
