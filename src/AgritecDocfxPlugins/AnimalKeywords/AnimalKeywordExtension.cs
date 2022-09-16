using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    private AnimalKeywordMapping _mapping;


    public AnimalKeywordExtension(MarkdownContext context)
    {
        _context = context;
    }
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        _mapping = new AnimalKeywordMapping(AgrContext.GetProfile(_context), AgrContext.GetLocale(_context));
        pipeline.InlineParsers.Insert(0,new AnimalKeywordParser(_context, _mapping));
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            htmlRenderer.LinkRewriter = link =>
            {
                return Regex.Replace(link, @"({.*})", match =>
                {
                    string word = match.Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(word))
                        return word;

                    var newWord = word.Trim(new char[] { '{', '}' });
                    if (!_mapping.PrefixTree.TryMatchLongest(newWord.ToLower().AsSpan(), out KeyValuePair<string, string> keyMatch))
                        return word;

                    return keyMatch.Value.ToLower();
                });
            };
        }

    }
}
