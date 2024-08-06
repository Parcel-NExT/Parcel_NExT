using Parcel.Types;

namespace Zora.GUI.Feature
{
    public sealed class SlideStyle
    {
        public int TitleFontSize { get; set; } = 60;
        public int SubTitleFontSize { get; set; } = 24;
        public int HeaderFontSize { get; set; } = 44;
        public int SubHeaderFontSize { get; set; } = 24;
        public int BodyFontSize { get; set; } = 28;

        public string TitleFont { get; set; } = "Helvetica";
        public string SubTitleFont { get; set; } = "Helvetica";
        public string HeaderFont { get; set; } = "Verdana";
        public string SubHeaderFont { get; set; } = "Verdana";
        public string BodyFont { get; set; } = "Arial";

        public Image? BackgroundImage { get; set; }
        public Color BackgroundColor { get; set; } = Colors.White;
    }
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
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Header { get; set; }
        public string? SubHeader1 { get; set; }

        #region Text Body
        public string? Body1 { get; set; }
        public string? Body2 { get; set; }
        public string? Body3 { get; set; }
        #endregion

        #region Image Body
        public Image? Image1 { get; set; }
        public Image? Image2 { get; set; }
        public Image? Image3 { get; set; }
        #endregion

        #region Styling
        public SlideStyle SlideStyle { get; set; }
        #endregion
    }
    public sealed class Presentation
    {
        public List<Slide> Slides { get; set; } = [];
        public PresentationSetting Settings { get; set; } = new();

        #region Additional Utility
        public string ToMarkdown()
        {
            // Remark: For image, just save the image as file then use reference
            throw new NotImplementedException();
        }
        public void SaveAsMarkdownFile(string path)
            => File.WriteAllText(path, ToMarkdown());
        #endregion
    }
    public sealed class PresentationSetting
    {
        public bool ShowSlideNumber { get; set; }
        public bool ShowSlideNumberOnTitlePage { get; set; } = false;

        public string? Footer { get; set; }
        public double FooterFontSize { get; set; } = 12;
        public string FooterFont { get; set; } = "Verdana";
    }

    public static class PresentationMaker
    {
        #region Configuration
        public static SlideStyle ConfigureSlide(int titleFontSize = 60, int subTitleFontSize = 24, int headerFontSize = 44, int subHeaderFontSize = 24, int bodyFontSize = 28, string titleFont = "Helvetica", string subTitleFont = "Helvetica", string headerFont = "Verdana", string subHeaderFont = "Verdana", string bodyFont = "Arial", Image? backgroundImage = null, Color? backgroundColor = null)
        {
            return new SlideStyle()
            {
                TitleFontSize = titleFontSize,
                SubTitleFontSize = subTitleFontSize,
                HeaderFontSize = headerFontSize,
                SubHeaderFontSize = subHeaderFontSize,
                BodyFontSize = bodyFontSize,
                
                TitleFont = titleFont,
                BodyFont = bodyFont,
                SubHeaderFont = subHeaderFont,
                HeaderFont = headerFont,
                SubTitleFont = subTitleFont,

                BackgroundImage = backgroundImage,
                BackgroundColor = backgroundColor ?? Colors.White,
            };
        }
        public static PresentationSetting ConfigurePresentation(bool showSlideNumber = true, string? footer = null)
        {
            return new()
            {
                ShowSlideNumber = showSlideNumber,
                Footer = footer
            };
        }
        #endregion

        #region Slide Making
        public static Slide TitleSlide(string title, string? subtitle, SlideStyle? style = null)
        {
            return new Slide()
            {
                LayoutType = Slide.SlideLayoutType.TitlePage,
                Title = title,
                SubTitle = subtitle,
                SlideStyle = style ?? DefaultSlideStyle
            };
        }
        public static Slide OneBodySlide(string header, string body, SlideStyle? style = null)
        {
            return new Slide()
            {
                LayoutType = Slide.SlideLayoutType.HeaderWithOneBody,
                Header = header,
                Body1 = body,
                SlideStyle = style ?? DefaultSlideStyle
            };
        }
        public static Slide OneBodySlide(string header, Image image, SlideStyle? style = null)
        {
            return new Slide()
            {
                LayoutType = Slide.SlideLayoutType.HeaderWithImage,
                Header = header,
                Image3 = image,
                SlideStyle = style ?? DefaultSlideStyle
            };
        }
        #endregion

        #region Start Presentation
        public static Presentation PresentFromMarkdownFile(string filePath, PresentationSetting? setting = null)
            => PresentFromMarkdown(File.ReadAllText(filePath), setting);
        public static Presentation PresentFromMarkdown(string markdown, PresentationSetting? setting = null)
        {
            setting ??= new();

            throw new NotImplementedException();
        }
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

        #region Helpers
        private static SlideStyle DefaultSlideStyle
            => new();
        #endregion
    }
}
