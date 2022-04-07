// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AgritecDocfxPlugins.ProfileConditional;
using AgritecDocfxPlugins.QuestionNote;
using Markdig;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

public static class AgritecExtensions
{
    public static MarkdownPipelineBuilder UseAgritecExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        return pipeline
            .UseAnimalKeyword(context)
            .UseReportSnippet(context)
            .UseProfileConditional(context)
            .UseQuestionNote(context);
    }


    public static MarkdownPipelineBuilder UseAnimalKeyword(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        pipeline.Extensions.AddIfNotAlready(new AnimalKeywords.AnimalKeywordExtension(context));
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseReportSnippet(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        pipeline.Extensions.AddIfNotAlready(new ReportSnippetExtension(context));
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseProfileConditional(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        pipeline.Extensions.AddIfNotAlready(new ProfileConditionalExtension(context));
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseQuestionNote(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
    {
        pipeline.Extensions.AddIfNotAlready(new QuestionNoteExtension(context));
        return pipeline;
    }
}
