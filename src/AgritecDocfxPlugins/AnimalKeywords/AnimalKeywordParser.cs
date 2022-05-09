using Markdig.Helpers;
using Markdig.Parsers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.AnimalKeywords;

public class AnimalKeywordParser : InlineParser
{

    private readonly AnimalKeywordMapping _mapping;
    private readonly MarkdownContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimalKeywordParser"/> class.
    /// </summary>
    public AnimalKeywordParser(MarkdownContext context)
    {
        OpeningCharacters = new[] { '{' };
        _context = context;
        _mapping = new AnimalKeywordMapping(AgrContext.GetProfile(_context), AgrContext.GetLocale(_context));
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var startPosition = slice.Start;
        slice.NextChar();    // Skip the first {
        var start = slice.Start;
        if (!_mapping.PrefixTree.TryMatchLongest(slice.Text.ToLower().AsSpan(slice.Start, slice.Length), out KeyValuePair<string, string> match))
            return false;

        if (slice.PeekCharExtra(match.Key.Length) != '}')
            return false;


        processor.Inline = new AnimalInline(string.Concat(Char.IsUpper(slice.Text[slice.Start]) ? match.Value[0].ToString().ToUpper() : match.Value[0].ToString().ToLower(), match.Value.Substring(1)))
        {
            Span =
            {
                Start = processor.GetSourcePosition(startPosition, out int line, out int column)
            },
            Line = line,
            Column = column,
            Match = match.Key
        };
        processor.Inline.Span.End = startPosition + match.Key.Length;

        slice.Start += match.Key.Length + 1;

        return true;
    }
}
