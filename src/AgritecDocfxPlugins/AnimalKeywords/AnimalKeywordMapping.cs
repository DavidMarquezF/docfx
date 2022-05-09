namespace AgritecDocfxPlugins.AnimalKeywords
{
    public class AnimalKeywordMapping
    {
        internal CompactPrefixTree<string> PrefixTree { get; }
        internal char[] OpeningCharacters { get; }

        public static IDictionary<string, string> GetKeywordsForProfile(string profile)
        {
            var dict = new Dictionary<string, string>() {
                            { "app", Profile.GetProfileName(profile)},
                            { "nullipara", "Nullipara"},
                            { "birthing", "Birthing"},
                            { "prebirthing", "Prebirthing" },
                            { "birth", "Birth"},
                            { "prebirth", "Prebirth" },
                            { "male", "Male" },
                            { "males", "Males"}
                        };

            switch (profile)
            {
                case Profile.POR:
                    dict["birthing"] = "Farrowing";
                    dict["prebirthing"] = "Prefarrowing";
                    dict["birth"] = "Farrow";
                    dict["prebirth"] = "Prefarrow";
                    dict["male"] = "Boar";
                    dict["males"] = "Boars";
                    break;
            }

            return dict;
        }

        public AnimalKeywordMapping(string profile) : this(GetKeywordsForProfile(profile ?? Profile.POR))
        {

        }

        public AnimalKeywordMapping(IDictionary<string, string> keywords)
        {
            int count = keywords.Count;

            // Count * 2 seems to be a good fit for the data set
            PrefixTree = new CompactPrefixTree<string>(count, count * 2, count * 2);

            foreach (var shortcode in keywords)
            {
                if (string.IsNullOrEmpty(shortcode.Key) || string.IsNullOrEmpty(shortcode.Value))
                    throw new ArgumentException("The dictionaries cannot contain null or empty keys/values", nameof(keywords));

                PrefixTree.Add(shortcode);
            }
        }
    }
}
