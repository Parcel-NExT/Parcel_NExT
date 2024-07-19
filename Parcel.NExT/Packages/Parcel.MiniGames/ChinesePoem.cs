using Parcel.CoreEngine.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace Parcel.MiniGame
{
    public static class ChinesePoemReading
    {
        private record PoemEntry(string Title, string Author, string Content);
        private static readonly PoemEntry[] Poems = LoadPoems();
        public static string ChinesePoem()
        {
            Random random = new();
            int draw = random.Next(Poems!.Length);
            PoemEntry poem = Poems[draw];
            return $"""
                {poem.Title} {poem.Author}

                {poem.Content}
                """;
        }

        private static PoemEntry[] LoadPoems()
        {
            string texts = EmbeddedResourceHelper.ReadTextResource("Parcel.MiniGame.唐诗三百首.txt")
                .Replace("\r\n", "\n")
                .Replace('\r', '\n');
            string[] lines = texts.Split('\n'); // Do not remove empty entries, have significance

            List<PoemEntry> poems = [];
            StringBuilder content = new();
            string? title = null;
            string? author = null;
            foreach (string line in lines)
            {
                // Starting new entry
                Match match = Regex.Match(line, @"\d+(.*?)：(.*)");
                if (match.Success)
                {
                    if (content.Length != 0)
                        AddEntry(poems, content, title, author);

                    author = match.Groups[1].Value;
                    title = match.Groups[2].Value;
                    content = new();
                }
                else
                    content.AppendLine(line);
            }
            if (content.Length != 0) // Trailing entry
                AddEntry(poems, content, title, author);

            return [.. poems];

            static void AddEntry(List<PoemEntry> poems, StringBuilder builder, string? title, string? author)
            {
                string content = builder.ToString().Trim();
                poems.Add(new PoemEntry(title!, author!, content));
            }
        }
    }
}
