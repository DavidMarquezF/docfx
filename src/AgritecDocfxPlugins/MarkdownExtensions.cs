// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig;

namespace AgritecDocfxPlugins;

public static class AgritecExtensions
{
    public static MarkdownPipelineBuilder UseAgritecExtensions(this MarkdownPipelineBuilder pipeline)
    {
        return pipeline.UseAnimalKeyword();
    }


    public static MarkdownPipelineBuilder UseAnimalKeyword(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready(new AnimalKeywords.AnimalKeywordExtension());
        return pipeline;
    }
}
