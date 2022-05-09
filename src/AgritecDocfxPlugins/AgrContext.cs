using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

internal class AgrContext
{
    internal static string GetProfile(MarkdownContext context)
    {
        return context.GetToken("agr-profile");
    }

    internal static string GetLocale(MarkdownContext context)
    {
        return context.GetToken("agr-lang");
    }

}
