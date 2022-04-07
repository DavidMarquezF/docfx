using Markdig.Parsers;
using Markdig.Syntax;

namespace AgritecDocfxPlugins.QuestionNote;

internal class QuestionNoteBlock : ContainerBlock
{

    public string Question { get; set; }


    public int ColonCount { get; set; }

    public bool Closed { get; set; }

    public QuestionNoteBlock(BlockParser parser)
        : base(parser)
    {
    }


}
