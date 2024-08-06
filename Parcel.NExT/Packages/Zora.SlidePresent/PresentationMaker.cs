namespace Zora.GUI.Feature
{
    public sealed class Slide
    {

    }
    public sealed class Presentation
    {
        public List<Slide> Slides { get; set; } = [];
        public PresentationSetting Settings { get; set; } = new();
    }
    public sealed class PresentationSetting
    {
        public bool ShowSlideNumber { get; set; }
    }

    public static class PresentationMaker
    {
        #region Configuration
        public static PresentationSetting ConfigurePresentation(bool showSlideNumber = true)
        {
            return new()
            {
                ShowSlideNumber = showSlideNumber
            };
        }
        #endregion

        public static Presentation Present(Slide[] slides, PresentationSetting? setting = null)
        {
            setting ??= new();

            return new()
            {
                Slides = [.. slides],
                Settings = setting
            };
        }
    }
}
