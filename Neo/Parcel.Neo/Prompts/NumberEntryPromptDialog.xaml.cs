using System.Windows;

namespace Parcel.Neo.Prompts
{
    public partial class NumberEntryPromptDialog : Window
    {
        #region Constructor
        public NumberEntryPromptDialog(Window owner, string title, string label, double defaultValue, double? min = null, double? max = null)
        {
            Owner = owner;

            title = title.Trim();
            label = label.Trim();

            Min = min;
            Max = max;

            InitializeComponent();

            Title = title;
            EntryLabel.Content = label.EndsWith(':') ? label : $"{label}:";
            EntryTextBox.Text = defaultValue.ToString();
        }
        #endregion

        #region Properties
        public double Value { get; private set; }
        public double? Min { get; }
        public double? Max { get; }
        #endregion

        #region Events
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(EntryTextBox.Text, out double d))
            {
                if (d >= Min && d <= Max)
                {
                    Value = d;
                    DialogResult = true;
                }
                else
                    EntryTextBox.Text = "Invalid value range";
            }
            else
                EntryTextBox.Text = "Invalid value";

            e.Handled = true;
        }
        #endregion
    }
}
