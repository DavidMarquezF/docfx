// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Renderers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

public class ReportSnippetExtension : IMarkdownExtension
{
    private readonly MarkdownContext _context;

    public ReportSnippetExtension(MarkdownContext context)
    {
        _context = context;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.BlockParsers.AddIfNotAlready<ReportSnippetParser>();
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<HtmlReportSnippetRenderer>())
        {
            // Must be inserted before CodeBlockRenderer
            htmlRenderer.ObjectRenderers.Insert(0, new HtmlReportSnippetRenderer(_context, pipeline));
        }
    }
}
