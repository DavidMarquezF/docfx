using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins
{
    internal class Profile
    {
        public const string POR = "por";
        public const string VAC = "vac";
        public const string CUN = "cun";
        public const string OVI = "ovi";

        internal static string GetProfile(MarkdownContext context)
        {
            return context.GetToken("agr-profile");
        }
    }
}
