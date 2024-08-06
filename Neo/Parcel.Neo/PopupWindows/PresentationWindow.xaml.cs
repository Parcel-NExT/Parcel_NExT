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
        }
        public Presentation Presentation { get; }
        #endregion

        #region Events
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void BaseWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DialogResult = true;
        }
        #endregion
    }
}
