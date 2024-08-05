using System.Windows;

namespace Parcel.Neo.Prompts
{
    /// <summary>
    /// Pick item from a list of entries
    /// </summary>
    public partial class ListEntryPickPromptDialog : Window
    {
        #region Constructor
        public ListEntryPickPromptDialog(MainWindow mainWindow, string title, string label, string[] values, string defaultValue)
        {
            Owner = mainWindow;

            title = title.Trim();
            label = label.Trim();

            InitializeComponent();

            Title = title;
            EntryLabel.Content = label.EndsWith(':') ? label : $"{label}:";
            
            EntryListBox.ItemsSource = values;
            EntryListBox.SelectedItem = defaultValue;
        }
        #endregion

        #region Properties
        public string Value { get; private set; }
        #endregion

        #region Events
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Value = EntryListBox.SelectedItem as string;
            e.Handled = true;
            DialogResult = true;
        }
        #endregion
    }
}
