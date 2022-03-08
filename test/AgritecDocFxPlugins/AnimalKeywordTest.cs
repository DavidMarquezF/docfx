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
    public void AnimalKeywordTestNoSpace()
    {
        var content = @"**content :** asda{Birthing}asdasd";
        var expected = @"<p><strong>content :</strong> asdaFarrowingasdasd</p>";

        TestUtility.VerifyMarkup(content, expected);
    }
}
