using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Parcel.CoreEngine.Helpers
{
    /// <summary>
    /// Provides parsing service for .net assembly generated XML documentations.
    /// Generates MD documentation from generated XML documentation files.
    /// </summary>
    public static class DocumentationHelper
    {
        #region Sub-Types
        public enum MemberType
        {
            Class,
            Member
        }
        public record Argument(string Type, string Name);
        public class Member
        {
            #region Constructor
            public Member(string signature, string summary)
            {
                Signature = signature;
                Summary = summary;

                Match regex = Regex.Match(Signature, @"[TMP]:([\w\.]*?)\((.*)\)");
                Name = regex.Groups[1].Value;
                MemberType = Signature.First() switch
                {
                    'M' => MemberType.Member,
                    'P' => MemberType.Member,
                    'T' => MemberType.Class,
                    _ => throw new ArgumentException($"Unknown type: {Signature.First()}")
                };
                string argumentsDeclaration = regex.Groups[2].Value;
                Arguments = argumentsDeclaration.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a =>
                    {
                        var parts = a.Split(' ');
                        return new Argument(parts.First(), parts.Last());
                    })
                    .ToArray();
            }
            #endregion

            #region Properties
            public string Signature { get; }
            public string Summary { get; }
            public string Name { get; }
            public MemberType MemberType { get; } 
            public Argument[] Arguments { get; }
            #endregion

            #region Accesors
            public string FriendlyName => Name[(Name.LastIndexOf('.') + 1)..];
            #endregion
        }
        public record Documentation(string AssemblyName, Member[] Members);
        #endregion

        #region Methods
        public static Documentation ParseXML(string filePath)
        {
            XmlDocument doc = new();
            doc.Load(filePath);

            string assemblyName = doc.SelectSingleNode("/doc/assembly/name").InnerText;
            Member[] members = doc.SelectSingleNode("/doc/members")
                .ChildNodes
                .OfType<XmlElement>()
                .Select(e => {
                    string name = e.GetAttribute("name");
                    string summary = e.SelectSingleNode("summary")?.InnerText.Trim() ?? string.Empty;
                    summary = Regex.Replace(summary, @"^\s+", string.Empty, RegexOptions.Multiline);
                    return new Member(name, summary);
                })
                .ToArray();

            return new Documentation(assemblyName, members);
        }
        public static Documentation[] ParseXMLDocumentation(string lookupFolderPath)
        {
            return Directory.GetFiles(lookupFolderPath, "*.xml")
                .Select(ParseXML)
                .ToArray();
        }
        public static string BuildMarkdownDocumentation(Documentation[] docs)
        {
            // Build Markdown
            StringBuilder builder = new();
            builder.AppendLine("# Pure API Documentation\n");

            foreach (var doc in docs.OrderBy(d => d.AssemblyName))
            {
                builder.AppendLine($"## {doc.AssemblyName}\n");

                foreach (var member in doc.Members.OrderBy(m => m.Name))
                {
                    builder.AppendLine($"### `{member.Name}`\n");
                    builder.AppendLine(member.Summary + '\n');
                }
            }

            // Save MD
            string markdown = builder.ToString().TrimEnd();
            return markdown;
        }
        #endregion

        #region Generation/Conversion Service
        /// <summary>
        /// Pseudo entry
        /// </summary>
        public static void Parse(string lookupFolderPath, string outputFilePath)
        {
            Documentation[] docs = ParseXMLDocumentation(lookupFolderPath);
            string markdown = BuildMarkdownDocumentation(docs);

            string documentationName = Path.GetFileNameWithoutExtension(outputFilePath);
            string outputFolder = Path.GetDirectoryName(outputFilePath);
            File.WriteAllText(Path.Combine(outputFolder, $"{documentationName}.md"), markdown);
        }
        #endregion
    }
}
