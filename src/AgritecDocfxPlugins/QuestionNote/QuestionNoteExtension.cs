using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.QuestionNote;

internal class QuestionNoteExtension : IMarkdownExtension
{
    private readonly MarkdownContext _context;

    public QuestionNoteExtension(MarkdownContext context)
    {
        _context = context;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<CustomContainerParser>())
        {
            pipeline.BlockParsers.InsertBefore<CustomContainerParser>(new QuestionNoteParser(_context));
        }
        else
        {
            pipeline.BlockParsers.AddIfNotAlready(new QuestionNoteParser(_context));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<QuestionNoteRender>())
        {
            htmlRenderer.ObjectRenderers.Insert(0, new QuestionNoteRender(_context));
        }
    }
}
