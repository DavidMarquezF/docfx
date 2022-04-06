using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.AnimalKeywords;


// This extension is inspired by the EmojiMapping extension from Markdig https://github.com/xoofx/markdig/blob/master/src/Markdig/Extensions/Emoji/EmojiMapping.cs
public class AnimalKeywordExtension : IMarkdownExtension
{
    private readonly MarkdownContext _context;

    public AnimalKeywordExtension(MarkdownContext context)
    {
        _context = context;
    }
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.InlineParsers.InsertBefore<AutolineInlineParser>(new AnimalKeywordParser(_context));
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
       
    }
}
