using System.Collections.ObjectModel;

namespace Parcel.Neo.PopupWindows
{
    public partial class PackagePickerWindow : BaseWindow
    {
        #region Constructor
        public PackagePickerWindow()
        {
            AvailablePackages = new();

            InitializeComponent();
        }
        #endregion

        #region View Binding Properties
        private ObservableCollection<string> _availablePackages;
        public ObservableCollection<string> AvailablePackages
        {
            get => _availablePackages;
            set => SetField(ref _availablePackages, value);
        }
        #endregion

        #region Properties
        public string? Result { get; private set; }
        #endregion

        #region Events
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Result = PackagesListBox.SelectedItem as string;
            e.Handled = true;
            DialogResult = true;
        }
        #endregion
    }
}
