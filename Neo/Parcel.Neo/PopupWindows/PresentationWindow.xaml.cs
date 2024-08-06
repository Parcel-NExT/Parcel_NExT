using Parcel.Neo.Helpers;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using Zora.GUI.Feature;

namespace Parcel.Neo.PopupWindows
{
    /// <summary>
    /// Interaction logic for PresentationWindow.xaml
    /// </summary>
    public partial class PresentationWindow : BaseWindow
    {
        #region Construction
        public PresentationWindow(MainWindow parent, Presentation presentation)
        {
            Presentation = presentation;

            Owner = parent;
            InitializeComponent();

            ShowNextSlide();
        }

        public Presentation Presentation { get; }
        #endregion

        #region States
        private int CurrentSlide = -1;
        #endregion

        #region Routines
        private void ShowPreviousSlide()
        {
            if (CurrentSlide > 0)
                CurrentSlide--;
            ShowSlide(CurrentSlide);
        }
        private void ShowNextSlide()
        {
            if (Presentation.Slides.Count > CurrentSlide)
                CurrentSlide++;

            if (Presentation.Slides.Count <= CurrentSlide)
                ShowEndSlide();
            else
                ShowSlide(CurrentSlide);
        }
        private void ShowSlide(int index)
        {
            Slide slide = Presentation.Slides[index];

            switch (slide.LayoutType)
            {
                case Slide.SlideLayoutType.TitlePage:
                    MakeTitleSlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.SectionPage:
                    MakePlaceholderSlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.TableOfContent:
                    MakePlaceholderSlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithImage:
                    MakeHeaderWithImageSlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithOneBody:
                    MakeHeaderWithOneBodySlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.HeadertWithTwoBody:
                    MakePlaceholderSlide(Presentation, slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithThreeBody:
                    MakePlaceholderSlide(Presentation, slide);
                    break;
                default:
                    throw new ArgumentException($"Unkown slide type: {slide.LayoutType}");
            }
        }
        private void ShowEndSlide()
        { 
            MakeEndSlide();
        }
        #endregion

        #region Slides
        private void MakeTitleSlide(Presentation presentation, Slide slide)
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(MediaHelper.ConvertColor(slide.SlideStyle.BackgroundColor));

            // Background
            AddBackground(slide);

            // Container
            StackPanel panel = new()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
            PresentationGrid.Children.Add(panel);

            // Title
            panel.Children.Add(new Label()
            {
                Content = slide.Title,
                Foreground = new SolidColorBrush(Colors.Black),

                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new System.Windows.Thickness(0),
                Padding = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize),

                FontSize = slide.SlideStyle.TitleFontSize,
                FontFamily = new FontFamily(slide.SlideStyle.TitleFont)
            });
            // Subtitle
            if (!string.IsNullOrWhiteSpace(slide.SubTitle))
                panel.Children.Add(new Label()
                {
                    Content = slide.SubTitle,
                    Foreground = new SolidColorBrush(Colors.Black),

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                    FontSize = slide.SlideStyle.SubTitleFontSize,
                    Margin = new System.Windows.Thickness(0),
                    Padding = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize),

                    FontFamily = new FontFamily(slide.SlideStyle.HeaderFont)
                });

            // Footer
            AddFooter(presentation);
            // Slide Number
            AddSlideNumber(presentation, slide);
        }
        private void MakeHeaderWithOneBodySlide(Presentation presentation, Slide slide)
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(MediaHelper.ConvertColor(slide.SlideStyle.BackgroundColor));

            // Background
            AddBackground(slide);

            // Container
            DockPanel panel = new()
            {
                LastChildFill = true,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new System.Windows.Thickness(20)
            };
            PresentationGrid.Children.Add(panel);

            // Header
            var headerLabel = new Label()
            {
                Content = slide.Header,
                Foreground = new SolidColorBrush(Colors.Black),

                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,

                FontSize = slide.SlideStyle.HeaderFontSize,
                Margin = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize, 0),
                Padding = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize),

                FontFamily = new FontFamily(slide.SlideStyle.HeaderFont)
            };
            DockPanel.SetDock(headerLabel, Dock.Top);
            panel.Children.Add(headerLabel);
            
            // Body
            var bodyText = new TextBlock()
            {
                Text = slide.Body1,
                Foreground = new SolidColorBrush(Colors.Black),

                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                TextAlignment = System.Windows.TextAlignment.Left,

                FontSize = slide.SlideStyle.BodyFontSize,
                Margin = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize, 0, slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize),
                Padding = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize, 0, slide.SlideStyle.HeaderFontSize, slide.SlideStyle.HeaderFontSize),

                FontFamily = new FontFamily(slide.SlideStyle.BodyFont)
            };
            DockPanel.SetDock(bodyText, Dock.Bottom);
            panel.Children.Add(bodyText);

            // Footer
            AddFooter(presentation);
            // Slide Number
            AddSlideNumber(presentation, slide);
        }
        private void MakeHeaderWithImageSlide(Presentation presentation, Slide slide)
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(MediaHelper.ConvertColor(slide.SlideStyle.BackgroundColor));

            // Background
            AddBackground(slide);

            // Header
            PresentationGrid.Children.Add(new Label()
            {
                Content = slide.Header,
                Foreground = new SolidColorBrush(Colors.Black),

                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,

                FontSize = slide.SlideStyle.HeaderFontSize,
                Margin = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize),
                Padding = new System.Windows.Thickness(slide.SlideStyle.HeaderFontSize),

                FontFamily = new FontFamily(slide.SlideStyle.HeaderFont)
            });
            // Body
            PresentationGrid.Children.Add(new Image()
            {
                Source = ImageSourceHelper.ConvertToBitmapImage(slide.Image3),
                Width = slide.Image3.Width,

                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            });
            // Footer
            AddFooter(presentation);
            // Slide Number
            AddSlideNumber(presentation, slide);
        }
        private void MakePlaceholderSlide(Presentation presentation, Slide slide)
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(Colors.Black);
            PresentationGrid.Children.Add(new Label()
            {
                Content = $"Slide for {slide.LayoutType} has not been implemented.",
                Foreground = new SolidColorBrush(Colors.Red),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            });
        }
        private void MakeEndSlide()
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(Colors.Black);
            PresentationGrid.Children.Add(new TextBlock()
            {
                Text = "Presentation has ended.\nPress ESC to quit.",
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextAlignment = System.Windows.TextAlignment.Center
            });
        }
        #endregion

        #region Decorators
        private void AddBackground(Slide slide)
        {
            if (slide.SlideStyle.BackgroundColor != null) 
                PresentationGrid.Background = new SolidColorBrush(MediaHelper.ConvertColor(slide.SlideStyle.BackgroundColor));

            if (slide.SlideStyle.BackgroundImage != null)
                PresentationGrid.Children.Add(new Image()
                {
                    Source = ImageSourceHelper.ConvertToBitmapImage(slide.SlideStyle.BackgroundImage),
                    Stretch = Stretch.UniformToFill,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch
                });
        }
        private void AddFooter(Presentation presentation)
        {
            if (presentation.Settings.Footer != null)
                PresentationGrid.Children.Add(new Label()
                {
                    Content = presentation.Settings.Footer,
                    Foreground = new SolidColorBrush(Colors.DarkKhaki),

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom,

                    FontSize = presentation.Settings.FooterFontSize,
                    Margin = new System.Windows.Thickness(presentation.Settings.FooterFontSize),
                    Padding = new System.Windows.Thickness(presentation.Settings.FooterFontSize),

                    FontFamily = new FontFamily(presentation.Settings.FooterFont)
                });
        }
        private void AddSlideNumber(Presentation presentation, Slide slide)
        {
            if (presentation.Settings.ShowSlideNumber)
                if (presentation.Settings.ShowSlideNumberOnTitlePage || slide.LayoutType != Slide.SlideLayoutType.TitlePage) // Show page number on non-title page by default
                    PresentationGrid.Children.Add(new Label()
                    {
                        Content = $"{presentation.Slides.IndexOf(slide)}",
                        Foreground = new SolidColorBrush(Colors.DarkKhaki),

                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        VerticalAlignment = System.Windows.VerticalAlignment.Bottom,

                        FontSize = presentation.Settings.FooterFontSize,
                        Margin = new System.Windows.Thickness(presentation.Settings.FooterFontSize),
                        Padding = new System.Windows.Thickness(presentation.Settings.FooterFontSize),

                        FontFamily = new FontFamily(presentation.Settings.FooterFont)
                    });
        }
        #endregion

        #region Events
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowNextSlide();
        }
        private void BaseWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Left
            if (e.Key == System.Windows.Input.Key.Left)
                ShowPreviousSlide();
            // Right
            else if (e.Key == System.Windows.Input.Key.Right)
                ShowNextSlide();
            // Exit
            else if (e.Key == System.Windows.Input.Key.Escape)
                DialogResult = true;
        }
        #endregion
    }
}
