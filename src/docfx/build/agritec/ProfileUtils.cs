using Microsoft.Docs.Build;

namespace docfx.build.agritec;

internal static class ProfileUtils
{

    public static bool TocHasProfile(TocNode node, string currentProfile)
    {

        try
        {
            if (node.ExtensionData == null)
                return true;

            var hasFound = node.ExtensionData.TryGetValue("profile", out var profile);
            if (hasFound)
                return profile.ToString().Split(",").Any(a => currentProfile == a);
            else
                return true;
        }
        catch
        {
            return true;
        }
    }


    public static TocNode RemoveTocItemsFromProfile(string currentProfile, TocNode? node)
    {
        if (node == null || !TocHasProfile(node, currentProfile))
            return null;

        
        if (node.Items?.Count > 0)
        {
            node.Items = node.Items.Where(n => TocHasProfile(n, currentProfile)).ToList();
        }

        return node;
    }
    
}
