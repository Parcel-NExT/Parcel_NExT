using Parcel.Infrastructure;

namespace Parcel.InMemoryDB.WebSurveys
{
    #region Survey Content Types
    public class SurveyField
    {

    }
    public class SurveySection
    {
        public SurveyField[]? FIelds { get; set; }
    }
    public class SurveyLayout
    {
        public SurveySection[]? Sections { get; set; }
    }
    #endregion


    #region Survey Construction
    public class SurveyContent
    {
        public SurveyLayout? Layout { get; set; }
    }

    public static class SurveyBuilder
    {
        #region Layouts
        public static SurveyContent LinearLayout() { return new SurveyContent() { Layout = new SurveyLayout() }; }
        #endregion

        #region Containers

        #endregion

        #region Fields

        #endregion
        // MakeSection
        // MakeField
    }
    #endregion

    public static class WebSurveysService
    {
        #region Service Bookkeeping

        #endregion

        #region Interface Methods
        /// <summary>
        /// Launch or update existing service.
        /// </summary>
        /// <returns>Returns service url</returns>
        public static string ConfigureSurveyService(SurveyContent content, string title, int port)
        {
            var server = DevelopmentServer.StartServerInNewThread(null);
            return server.ServerAddress;
        }
        #endregion
    }
}
