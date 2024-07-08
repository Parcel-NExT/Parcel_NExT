using System.Text;

namespace Parcel.Data
{
    public static class RandomTexts
    {
        public static string LoremIpsum
            => GenerateLoremIpsum(50, 300, 2, 15, 2);
        public static string GenerateLoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
        {
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            Random rand = new();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new();
            for (int p = 0; p < numParagraphs; p++)
            {
                result.Append("<p>");
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
                result.Append("</p>");
            }

            return result.ToString().TrimEnd();
        }
    }
}
