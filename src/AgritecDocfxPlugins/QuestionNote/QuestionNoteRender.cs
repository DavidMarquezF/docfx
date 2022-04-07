using System;
using System.Collections.Generic;
using System.Text;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins.QuestionNote;

internal class QuestionNoteRender : HtmlObjectRenderer<QuestionNoteBlock>
{
    private readonly MarkdownContext _context;
    public QuestionNoteRender(MarkdownContext context)
    {
        _context = context;
    }
    protected override void Write(HtmlRenderer renderer, QuestionNoteBlock obj)
    {
        var noteHeading = _context.GetToken("agr-question-title-text") ?? $"<h5>QUESTION</h5>";
        renderer.Write("<div").Write($" class=\"NOTE\"").WriteAttributes(obj).WriteLine(">");
        var savedImplicitParagraph = renderer.ImplicitParagraph;
        renderer.ImplicitParagraph = false;
        renderer.WriteLine(noteHeading);
        renderer.WriteChildren(obj);
        renderer.ImplicitParagraph = savedImplicitParagraph;
        renderer.WriteLine("</div>");
    }
}
