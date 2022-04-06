using System;
using System.Collections.Generic;
using System.Text;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.ProfileConditional;

internal class ProfileConditionalRender : HtmlObjectRenderer<ProfileConditionalBlock>
{
    protected override void Write(HtmlRenderer renderer, ProfileConditionalBlock obj)
    {
        if (obj.ShouldDisplay)
        {
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</div>");
        }
    }
}
