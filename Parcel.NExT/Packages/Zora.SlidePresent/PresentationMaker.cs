using Parcel.Types;

namespace Zora.GUI.Feature
{
    public sealed class Slide
    {
        public enum SlideLayoutType
        {
            TitlePage,
            SectionPage,
            TableOfContent,
            HeaderWithImage,
            HeaderWithOneBody,
            HeadertWithTwoBody,
            HeaderWithThreeBody
        }

        public SlideLayoutType LayoutType { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }

        #region Text Body
        public string Body1 { get; set; }
        public string Body2 { get; set; }
        public string Body3 { get; set; }
        #endregion

        #region Image Body
        public Image Image { get; set; }
        #endregion
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

        #region Slide Making
        public static Slide OneBodySlide(string header, string body)
        {
            return new Slide()
            {
                LayoutType = Slide.SlideLayoutType.HeaderWithOneBody,
                Header = header,
                Body1 = body
            };
        }
        public static Slide OneBodySlide(string header, Image image)
        {
            return new Slide()
            {
                LayoutType = Slide.SlideLayoutType.HeaderWithImage,
                Header = header,
                Image = image,
            };
        }
        #endregion

        #region Start Presentation
        public static Presentation Present(Slide[] slides, PresentationSetting? setting = null)
        {
            setting ??= new();

            return new()
            {
                Slides = [.. slides],
                Settings = setting
            };
        }
        #endregion
    }
}
