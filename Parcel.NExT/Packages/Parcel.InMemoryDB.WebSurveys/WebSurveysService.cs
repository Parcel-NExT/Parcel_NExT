using Parcel.Infrastructure;

namespace Parcel.InMemoryDB.WebSurveys
{
    #region Survey Content Types
    public class SurveyField
    {

    }
    public class SurveySection
    {
        public string Header { get;set; }
        public string Description { get; set; }

        public SurveySection(string header, string description)
        {
            Header = header;
            Description = description;
        }
        public SurveySection(string header, string description, SurveyField[]? fIelds)
        {
            Header = header;
            Description = description;
            FIelds = fIelds;
        }

        public SurveyField[]? FIelds { get; set; }
    }
    public class SurveyLayout
    {
        public SurveyLayout(params SurveySection[] sections) => Sections = sections;
        public SurveySection[]? Sections { get; set; }
    }
    #endregion


    #region Survey Construction
    public class SurveyContent
    {
        public string Name { get; set; }
        public string Description{ get; set; }

        public SurveyLayout? Layout { get; set; }
    }

    public static class SurveyBuilder
    {
        #region Layouts
        public static SurveyLayout LinearLayout(SurveySection primarySection) => new(primarySection);
        public static SurveyLayout LinearLayout(params SurveySection[] sections) => new(sections);
        #endregion

        #region Sections
        public static SurveySection AddSection(string header, string description)
        {
            return new SurveySection(header, description);
        }
        #endregion

        #region Fields

        #endregion
        // MakeSection
        // MakeField
    }
    #endregion

    public class SurveySiteConfiguration
    {
        public string Title { get; set; }
        public SurveyContent[] Surveys { get; set; }
    }
    public static class WebSurveysService
    {
        #region Service Bookkeeping

        #endregion

        #region Interface Methods
        /// <summary>
        /// Launch or update existing service.
        /// </summary>
        /// <returns>Returns service url</returns>
        public static string ConfigureSurveyService(SurveyLayout layout, string surveyTitle, string databaseFile, int? port = null)
        {
            DevelopmentServer server = DevelopmentServer.StartServerInNewThread([
                new EndpointDefinition("/", HandleSurveyForm(new SurveySiteConfiguration(), new SurveyContent() { Layout = layout, Name = surveyTitle }))
            ], port);
            Thread.Sleep(1000); // Wait for server to start; Will not work when Windows prompt user for firewall permission
            return server.ServerAddress;
        }
        #endregion

        #region Templates
        private static readonly EndpointDefinition.EndpointHandler DefaultHandler = (Dictionary<string, string> parameters, string body) => """
            <!DOCTYPE html>
            <html lang="en">

            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Main Page</title>
            </head>

            <body>
              <h1>Available Surveys</h1>
            </body>

            </html>
            """;
        private static EndpointDefinition.EndpointHandler HandleSurveyForm(SurveySiteConfiguration siteConfiguration, SurveyContent formContent) => (Dictionary<string, string> parameters, string body) => RazorTemplateEngine.CompileTemplate(formContent.Layout, $$"""
            <!DOCTYPE html>
            <html lang="en">

            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>{{siteConfiguration.Title}} - {{formContent.Name}}</title>
            </head>

            <body>
                <h1>{{formContent.Name}}</h1>
                <strong>{{formContent.Description}}</strong>
                @foreach(var section in @Model.Sections)
                {
                    <h2>@section.Header</h2>
                    <emphasis>@section.Description</emphasis>
                }
            </body>

            </html>
            """, TemplateFormat.HTML);
        #endregion
    }
}
