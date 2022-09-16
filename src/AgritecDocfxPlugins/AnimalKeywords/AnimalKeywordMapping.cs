namespace AgritecDocfxPlugins.AnimalKeywords
{
    public class AnimalKeywordMapping
    {
        internal CompactPrefixTree<string> PrefixTree { get; }
        internal char[] OpeningCharacters { get; }

        public static IDictionary<string, string> GetKeywordsForProfile(string profile, string locale)
        {
            if (locale == "es")
            {
                var dict = new Dictionary<string, string>() {
                            { "app", Profile.GetProfileName(profile)},
                            { "profile", profile},
                            {"culture", locale},
                            { "nullipara", "Nulipara"},
                            { "birthing", "Parto"},
                            { "prebirthing", "Preparto" },
                            { "birth", "Parir"},
                            { "prebirth", "Preparto" },
                            { "male", "Macho" },
                            { "males", "Machos"}
                        };

                return dict;
            }
            else
            {
                var dict = new Dictionary<string, string>() {
                            { "app", Profile.GetProfileName(profile)},
                            { "profile", profile},
                            {"culture", locale},
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

        }

        public AnimalKeywordMapping(string profile, string locale) : this(GetKeywordsForProfile(string.IsNullOrEmpty(profile) ? Profile.POR : profile, locale ?? "en"))
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
