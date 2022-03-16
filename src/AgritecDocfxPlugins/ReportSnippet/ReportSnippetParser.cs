// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Helpers;
using Markdig.Parsers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

/// <summary>
/// Used to expand the markdown to show a pdf and image for a repor
/// </summary>
public class ReportSnippetParser : BlockParser
{
    private const string StartString = "[!agr-report";

    public ReportSnippetParser()
    {
        OpeningCharacters = new[] { '[' };
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        // Sample: [!code-javascript[Main](../jquery.js?name=testsnippet#tag "title")]
        var slice = processor.Line;

        if (!ExtensionsHelper.MatchStart(ref slice, StartString, false))
            return BlockState.None;



        var codeSnippet = new ReportSnippet(this);

        if (!MatchName(ref slice, ref codeSnippet))
        {
            return BlockState.None;
        }


        ExtensionsHelper.SkipWhitespace(ref slice);

        if (slice.CurrentChar == ']')
        {
            var codeSnippetEnd = slice.Start;
            slice.NextChar();
            ExtensionsHelper.SkipWhitespace(ref slice);
            if (slice.CurrentChar == '\0')
            {
                // slice finished its task, re-use it for Raw content
                slice.Start = processor.Line.Start;
                slice.End = codeSnippetEnd;
                //codeSnippet.Raw = slice.ToString();
                codeSnippet.Column = processor.Column;
                codeSnippet.Line = processor.LineIndex;

                processor.NewBlocks.Push(codeSnippet);
                return BlockState.BreakDiscard;
            }
        }


        return BlockState.None;
    }

    private static bool MatchName(ref StringSlice slice, ref ReportSnippet codeSnippet)
    {
        if (slice.CurrentChar != '[')
        {
            return false;
        }

        var c = slice.NextChar();
        var name = StringBuilderCache.Local();
        var hasEscape = false;

        while (c != '\0' && (c != ']' || hasEscape))
        {
            if (c == '\\' && !hasEscape)
            {
                hasEscape = true;
            }
            else
            {
                name.Append(c);
                hasEscape = false;
            }
            c = slice.NextChar();
        }

        codeSnippet.ReportName = name.ToString().Trim();

        if (c == ']')
        {
            slice.NextChar();
            return true;
        }

        return false;
    }

}
