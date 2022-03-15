// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Text;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace AgritecDocfxPlugins;

public class ReportSnippet : LeafBlock
{
    public ReportSnippet(BlockParser parser)
        : base(parser)
    {
    }

   
    public string ReportName { get; set; }




}
