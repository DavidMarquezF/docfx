using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AgritecDocFxPlugins.Tests;

public class ReportSnippetTest
{
    [Fact]
    public void AnimalKeywordTestGeneral()
    {
        var content = @"[!agr-report[test]]";
        var expected = "<p><img src=\"test.png\" alt=\"Report image\" /><a href=\"test.pdf\">Download PDF example</a></p>";

        TestUtility.VerifyMarkup(content, expected);
    }


}
