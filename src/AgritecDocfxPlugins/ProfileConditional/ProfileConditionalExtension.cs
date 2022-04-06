using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.ProfileConditional;

internal class ProfileConditionalExtension : IMarkdownExtension
{
    private readonly MarkdownContext _context;

    public ProfileConditionalExtension(MarkdownContext context)
    {
        _context = context;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<CustomContainerParser>())
        {
            pipeline.BlockParsers.InsertBefore<CustomContainerParser>(new ProfileConditionalParser(_context));
        }
        else
        {
            pipeline.BlockParsers.AddIfNotAlready(new ProfileConditionalParser(_context));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<ProfileConditionalRender>())
        {
            htmlRenderer.ObjectRenderers.Insert(0, new ProfileConditionalRender());
        }
    }
}
