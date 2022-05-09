// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace AgritecDocFxPlugins.Tests;

public class AnimalKeywordTest
{
    [Fact]
    public void AnimalKeywordTestGeneral()
    {
        var content = @"**content :** {Birthing}";
        var expected = @"<p><strong>content :</strong> Farrowing</p>";

        TestUtility.VerifyMarkup(content, expected);
    }

    [Fact]
    public void AnimalKeywordTestGeneralVac()
    {
        var content = @"**content :** {Birthing}";
        var expected = @"<p><strong>content :</strong> Birthing</p>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "vac" } });
    }


    [Fact]
    public void AnimalKeywordTestGeneralVacES()
    {
        var content = @"**content :** {Birthing}";
        var expected = @"<p><strong>content :</strong> Parto</p>";

        TestUtility.VerifyMarkup(content, expected, tokens: new Dictionary<string, string>() { { "agr-profile", "vac" }, { "agr-lang", "es"} });
    }


    [Fact]
    public void AnimalKeywordTestNoSpace()
    {
        var content = @"**content :** asda{Birthing}asdasd";
        var expected = @"<p><strong>content :</strong> asdaFarrowingasdasd</p>";

        TestUtility.VerifyMarkup(content, expected);
    }

    [Fact]
    public void TestAppLowercase()
    {
        var content = @"**content :** {app}";
        var expected = @"<p><strong>content :</strong> porcitec</p>";

        TestUtility.VerifyMarkup(content, expected);
    }

    [Fact]
    public void TestAppUppercase()
    {
        var content = @"**content :** {App}";
        var expected = @"<p><strong>content :</strong> Porcitec</p>";

        TestUtility.VerifyMarkup(content, expected);
    }
}
