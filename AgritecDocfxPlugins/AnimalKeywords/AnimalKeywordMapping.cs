namespace AgritecDocfxPlugins.AnimalKeywords
{
    internal class AnimalKeywordMapping
    {
        internal CompactPrefixTree<string> PrefixTree { get; }
        internal char[] OpeningCharacters { get; }

        public static IDictionary<string, string> GetKeywordsForProfile(string profile)
        {
            return new Dictionary<string, string>()
                {
                    { "Birthing", "Farrowing"},
                    { "Birth", "Farrow"}
                };
        }

        public AnimalKeywordMapping(string profile): this(GetKeywordsForProfile(profile))
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
