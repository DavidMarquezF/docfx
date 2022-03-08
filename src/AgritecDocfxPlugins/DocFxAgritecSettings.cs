using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgritecDocfxPlugins
{
    internal class DocFxAgritecSettings
    {
        public string Profile { get; }
        public string Language { get; }
        public DocFxAgritecSettings(IReadOnlyDictionary<string, object> parameters)
        {
            Language = parameters.GetValueOrDefault("agr.lang", "en");
            Profile = parameters.GetValueOrDefault("agr.profile", AgritecDocfxPlugins.Profile.POR);
        }
    }
}
