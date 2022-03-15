using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins;

internal class HtmlReportSnippetRenderer : HtmlObjectRenderer<ReportSnippet>
{

    private const string TagPrefix = "snippet";
    private const string WarningMessageId = "agrReportNotFound";
    private const string DefaultWarningMessage = "It looks like the report image and pdf you are looking for does not exist.";
    private const string WarningTitleId = "warning";
    private const string DefaultWarningTitle = "<h5>WARNING</h5>";

    private MarkdownContext _context;
    private MarkdownPipeline _pipeline;
    public HtmlReportSnippetRenderer(MarkdownContext context, MarkdownPipeline pipeline)
    {
        _context = context;
    }

    protected override void Write(HtmlRenderer renderer, ReportSnippet obj)
    {
        string imagePath = obj.ReportName + ".png";
        string pdfPath = obj.ReportName + ".pdf";
        /*  var (content, codeSnippetPath) = _context.ReadFile(obj., obj, true);

          if (content == null)
          {
              _context.LogWarning("codesnippet-not-found", $"Invalid code snippet link: '{codeSnippet.CodePath}'.", codeSnippet);
              renderer.Write(GetWarning());
              return;
          }*/

        var content = new StringBuilder();
        content.AppendLine($"[Download PDF example]({pdfPath})"); // TODO extract TOKEN from context with the translated string
        content.AppendLine("");
        content.AppendLine($"![Report image]({imagePath})");
        renderer.Write(Markdown.ToHtml(content.ToString(), _pipeline));

    }

    private string GetWarning()
    {
        var warningTitle = _context.GetToken(WarningTitleId) ?? DefaultWarningTitle;
        var warningMessage = _context.GetToken(WarningMessageId) ?? DefaultWarningMessage;

        return $@"<div class=""WARNING"">
{warningTitle}
<p>{warningMessage}</p>
</div>";
    }
}
