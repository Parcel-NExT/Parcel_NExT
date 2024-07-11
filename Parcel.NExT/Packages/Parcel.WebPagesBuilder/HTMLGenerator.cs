using Parcel.CoreEngine.Helpers;
using System.Text;

namespace Parcel.Framework.WebPages
{
    public class HTMLGenerator
    {
        #region Constructor
        public HTMLGenerator(WebsiteBlock block)
            : this(DefaultTemplateName, [block]) { }
        public HTMLGenerator(WebsiteBlock[] blocks)
            : this(DefaultTemplateName, blocks) { }
        public HTMLGenerator(WebsiteConfiguration configuration)
            : this(configuration.TemplateName, configuration.Blocks) { }
        #endregion
        
        #region Master Constructor
        public HTMLGenerator(string template, WebsiteBlock[] blocks)
        {
            StringBuilder body = new();
            const string indentation = "\t";
            foreach (WebsiteBlock block in blocks)
                body.AppendLine(block.ToHTML(indentation));

            string templatedHTML = UseTemplate(DefaultTemplate, body);
            HTMLBuilder.AppendLine(templatedHTML);
        }
        #endregion

        #region Templates
        public const string DefaultTemplateName = "Default";
        private static string GetTemplate(string templateName)
            => templateName switch
            {
                DefaultTemplateName => DefaultTemplate,
                _ => throw new ArgumentOutOfRangeException($"Unknown template: {templateName}")
            };
        private static string DefaultTemplate => EmbeddedResourceHelper.ReadTextResource("Parcel.Framework.WebPages.Templates.Default.html");
        #endregion

        #region Properties
        private StringBuilder HTMLBuilder { get; } = new();
        public string HTML => HTMLBuilder.ToString().TrimEnd();
        #endregion

        #region Helpers
        private static string UseTemplate(string template, StringBuilder bodyBuilder)
            => template.Replace("@Body", bodyBuilder.ToString().TrimEnd());
        private static string UseTemplate(string template, string body)
            => template.Replace("@Body", body);
        #endregion
    }
}
