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

            if (presentation.Slides.Count == 0)
                ShowEndSlide();
            else
                ShowSlide(CurrentSlide);
        }
        public Presentation Presentation { get; }
        #endregion

        #region States
        private int CurrentSlide = 0;
        #endregion

        #region Routines
        private void ShowSlide(int index)
        {
            Slide slide = Presentation.Slides[index];

            switch (slide.LayoutType)
            {
                case Slide.SlideLayoutType.TitlePage:
                    MakePlaceholderSlide(slide);
                    break;
                case Slide.SlideLayoutType.SectionPage:
                    MakePlaceholderSlide(slide);
                    break;
                case Slide.SlideLayoutType.TableOfContent:
                    MakePlaceholderSlide(slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithImage:
                    MakeHeaderWithImageSlide(slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithOneBody:
                    MakePlaceholderSlide(slide);
                    break;
                case Slide.SlideLayoutType.HeadertWithTwoBody:
                    MakePlaceholderSlide(slide);
                    break;
                case Slide.SlideLayoutType.HeaderWithThreeBody:
                    MakePlaceholderSlide(slide);
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
        private void MakeHeaderWithImageSlide(Slide slide)
        {
            PresentationGrid.Children.Clear();
            PresentationGrid.Background = new SolidColorBrush(Colors.White);
            PresentationGrid.Children.Add(new Label()
            {
                Content = slide.Header,
                Foreground = new SolidColorBrush(Colors.Red),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 26
            });
            PresentationGrid.Children.Add(new System.Windows.Controls.Image()
            {
                Source = ImageSourceHelper.ConvertToBitmapImage(slide.Image),
                Width = slide.Image.Width,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            });
        }
        private void MakePlaceholderSlide(Slide slide)
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
            PresentationGrid.Children.Add(new Label()
            {
                Content = "Presentation has ended.\nPress ESC to quit.",
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            });
        }
        #endregion

        #region Events
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Presentation.Slides.Count > CurrentSlide + 1)
                CurrentSlide++;
        }
        private void BaseWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DialogResult = true;
        }
        #endregion
    }
}
