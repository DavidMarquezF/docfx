using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace AgritecDocfxPlugins.AnimalKeywords;

public class AnimalInline : LiteralInline
{
    // Inherit from LiteralInline so that rendering is already handled by default

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimalInline"/> class.
    /// </summary>
    public AnimalInline()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimalInline"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public AnimalInline(string content)
    {
        Content = new StringSlice(content);
    }

    /// <summary>
    /// Gets or sets the original match string 
    /// </summary>
    public string? Match { get; set; }
}
