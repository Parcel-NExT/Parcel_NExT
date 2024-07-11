using System.Text;

namespace Parcel.Framework.WebPages
{
    public class HTMLGenerator
    {
        #region Constructor
        public HTMLGenerator(WebsiteBlock block)
        {
            HTMLBuilder.AppendLine($"""
                <!DOCTYPE html>
                <html lang="en">
                    <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <meta http-equiv="X-UA-Compatible" content="ie=edge">
                    <title>HTML 5 Boilerplate</title>
                    </head>
                    <body>
                    {block.ToHTML("\t")}
                    </body>
                </html>
                """);
        }

        public HTMLGenerator(WebsiteBlock[] blocks)
        {
            HTMLBuilder.AppendLine($"""
                <!DOCTYPE html>
                <html lang="en">
                    <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <meta http-equiv="X-UA-Compatible" content="ie=edge">
                    <title>HTML 5 Boilerplate</title>
                    </head>
                    <body>
                """);
            const string indentation = "\t";
            foreach (WebsiteBlock block in blocks)
                HTMLBuilder.AppendLine(block.ToHTML(indentation));
            HTMLBuilder.AppendLine($"""
                    </body>
                </html>
                """);
        }
        public HTMLGenerator(WebsiteConfiguration configuration)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        private StringBuilder HTMLBuilder { get; } = new();
        public string HTML => HTMLBuilder.ToString().TrimEnd();
        #endregion

    }
}
