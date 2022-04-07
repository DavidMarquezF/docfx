using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.QuestionNote;

internal class QuestionNoteParser : BlockParser
{
    private const string EndString = "question-end";
    private const char Colon = ':';

    private readonly MarkdownContext _context;

    public QuestionNoteParser(MarkdownContext context)
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

        if (!ExtensionsHelper.MatchStart(ref slice, "question", false))
        {
            return BlockState.None;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        var monikerRange = new QuestionNoteBlock(this)
        {
            Closed = false,
            ColonCount = colonCount,
            Line = processor.LineIndex,
            Column = column,
            Span = new SourceSpan(sourcePosition, slice.End),
        };

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
        var monikerRange = (QuestionNoteBlock)block;

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
            _context.LogWarning("invalid-question", "Question may have some invalid chars in the ending.", block);
        }

        block.UpdateSpanEnd(slice.End);
        monikerRange.Closed = true;

        return BlockState.BreakDiscard;
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
        var monikerRange = (QuestionNoteBlock)block;
        if (monikerRange != null && monikerRange.Closed == false)
        {
            _context.LogWarning("invalid-question", $"No \"::: {EndString}\" found for question. Question does not end explicitly.", block);
        }

        return true;
    }
}
