using System.Windows;

namespace Parcel.Neo.Prompts
{
    /// <summary>
    /// Interaction logic for StringEntryPromptDialog.xaml
    /// </summary>
    public partial class StringEntryPromptDialog : Window
    {
        #region Constructor
        public StringEntryPromptDialog(string title, string label)
        {
            InitializeComponent();

            Title = title;
            EntryLabel.Content = label;
        }
        #endregion

        #region Properties
        public string Value { get; private set; }
        #endregion

        #region Events
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Value = EntryTextBox.Text;
            e.Handled = true;
            DialogResult = true;
        }
        #endregion
    }
}
