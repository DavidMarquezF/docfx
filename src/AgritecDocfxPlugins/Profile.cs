using Microsoft.Docs.MarkdigExtensions;

namespace AgritecDocfxPlugins
{
    internal class Profile
    {
        public const string POR = "por";
        public const string VAC = "vac";
        public const string CUN = "cun";
        public const string OVI = "ovi";

        public static string GetProfileName(string prof)
        {
            switch (prof)
            {
                case POR:
                    return "Porcitec";
                case VAC:
                    return "Vaquitec";
                case CUN:
                    return "Cunitec";
                case OVI:
                    return "Ovitec";
                default:
                    throw new ArgumentException("Invalid profile");
            }
        }
    }
}
