using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Renderers;

namespace AgritecDocfxPlugins.AnimalKeywords;


// This extension is inspired by the EmojiMapping extension from Markdig https://github.com/xoofx/markdig/blob/master/src/Markdig/Extensions/Emoji/EmojiMapping.cs
public class AnimalKeywordExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<AnimalKeywordParser>())
        {
            pipeline.InlineParsers.Insert(0, new AnimalKeywordParser(new AnimalKeywordMapping(Profile.POR)));
        }

    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
       
    }
}
