using System.Diagnostics;
using System.Windows;

namespace Parcel.Neo.PopupWindows
{
    public partial class ExperimentalFeatureWarningWindow : Window
    {
        #region Construction
        public ExperimentalFeatureWarningWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
        #endregion
    }
}
