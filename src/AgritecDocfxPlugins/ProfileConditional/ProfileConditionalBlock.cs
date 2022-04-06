using Markdig.Parsers;
using Markdig.Syntax;

namespace AgritecDocfxPlugins.ProfileConditional;

internal class ProfileConditionalBlock : ContainerBlock
{

    public string ProfileRange { get; set; }

    public bool ShouldDisplay { get; set; }

    public int ColonCount { get; set; }

    public bool Closed { get; set; }

    public ProfileConditionalBlock(BlockParser parser)
        : base(parser)
    {
    }


}
