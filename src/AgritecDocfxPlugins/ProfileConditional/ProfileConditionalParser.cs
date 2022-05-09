using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.ProfileConditional;

internal class ProfileConditionalParser : BlockParser
{
    private const string EndString = "if-profile-end";
    private const char Colon = ':';

    private readonly MarkdownContext _context;

    public ProfileConditionalParser(MarkdownContext context)
    {
        OpeningCharacters = new[] { ':' };
        _context = context;
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        var slice = processor.Line;
        var column = processor.Column;
        var sourcePosition = processor.Start;
        var colonCount = 0;

        var c = slice.CurrentChar;
        while (c == Colon)
        {
            c = slice.NextChar();
            colonCount++;
        }

        if (colonCount < 3)
        {
            return BlockState.None;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, "if-profile", false))
        {
            return BlockState.None;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, "range=\"", false))
        {
            return BlockState.None;
        }

        var range = StringBuilderCache.Local();
        c = slice.CurrentChar;

        while (c != '\0' && c != '"')
        {
            range.Append(c);
            c = slice.NextChar();
        }

        if (c != '"')
        {
            _context.LogWarning("invalid-profile-range", "Profile range does not have ending character (\").", null, line: processor.LineIndex);
            return BlockState.None;
        }

        c = slice.NextChar();
        while (c.IsSpace())
        {
            c = slice.NextChar();
        }

        if (!c.IsZero())
        {
            _context.LogWarning("invalid-profile-range", "Profile range have some invalid chars in the starting.", null, line: processor.LineIndex);
        }

        var monikerRange = new ProfileConditionalBlock(this)
        {
            Closed = false,
            ProfileRange = range.ToString(),
            ColonCount = colonCount,
            Line = processor.LineIndex,
            Column = column,
            Span = new SourceSpan(sourcePosition, slice.End),
        };

        monikerRange.GetAttributes().AddPropertyIfNotExist("range", monikerRange.ProfileRange);

        processor.NewBlocks.Push(monikerRange);

        return BlockState.ContinueDiscard;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (processor.IsBlankLine)
        {
            return BlockState.Continue;
        }

        var slice = processor.Line;
        var monikerRange = (ProfileConditionalBlock)block;

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, new string(':', monikerRange.ColonCount)))
        {
            ExtensionsHelper.ResetLineIndent(processor);
            return BlockState.Continue;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, EndString, false))
        {
            ExtensionsHelper.ResetLineIndent(processor);
            return BlockState.Continue;
        }

        var c = ExtensionsHelper.SkipSpaces(ref slice);

        if (!c.IsZero())
        {
            _context.LogWarning("invalid-profile-range", "Profile have some invalid chars in the ending.", block);
        }

        block.UpdateSpanEnd(slice.End);
        monikerRange.Closed = true;

        return BlockState.BreakDiscard;
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
        var monikerRange = (ProfileConditionalBlock)block;
        if (monikerRange != null)
        {
            if (monikerRange.Closed == false)
                _context.LogWarning("invalid-profile-range", $"No \"::: {EndString}\" found for \"{monikerRange.ProfileRange}\", Profile does not end explicitly.", block);
            else if (monikerRange.ProfileRange != null)
            {
                var profile = AgrContext.GetProfile(_context);
                if (profile == null)
                {
                    _context.LogWarning("missing-profile", $"No agritec profile provided in tokens. Showing the text by default", block);
                    monikerRange.ShouldDisplay = true;
                }
                else

                    monikerRange.ShouldDisplay = monikerRange.ProfileRange.Split(",").Any(a => profile == a);

            }
            else
                _context.LogWarning("invalid-profile-range", $"There was no valid profile range", block);


        }

        return true;
    }
}
