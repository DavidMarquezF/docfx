// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

public static class AgritecExtensions
{
    public static MarkdownPipelineBuilder UseAgritecExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        return pipeline.UseAnimalKeyword().UseReportSnippet(context);
    }


    public static MarkdownPipelineBuilder UseAnimalKeyword(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready(new AnimalKeywords.AnimalKeywordExtension());
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseReportSnippet(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        pipeline.Extensions.AddIfNotAlready(new ReportSnippetExtension(context));
        return pipeline;
    }
}
