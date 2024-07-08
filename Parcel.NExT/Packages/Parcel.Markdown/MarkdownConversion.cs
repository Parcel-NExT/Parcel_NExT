namespace Parcel.Markdown
{
    public static class MarkdownConversion
    {
        public static string ToHtml(this string markdown) => Markdig.Markdown.ToHtml(markdown);
    }
}
