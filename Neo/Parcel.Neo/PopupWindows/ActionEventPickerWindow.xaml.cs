using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using System.Collections.ObjectModel;

namespace Parcel.Neo.PopupWindows
{
    public partial class ActionEventPickerWindow : BaseWindow
    {
        #region Constructor
        public ActionEventPickerWindow(ProcessorNode[] availableEndpoints)
        {
            AvailableEndpoints = new(availableEndpoints);

            InitializeComponent();
        }
        #endregion

        #region View Binding Properties
        private ObservableCollection<ProcessorNode> _availableEndpoints;
        public ObservableCollection<ProcessorNode> AvailableEndpoints
        {
            get => _availableEndpoints;
            set => SetField(ref _availableEndpoints, value);
        }
        #endregion

        #region Properties
        public ProcessorNode? Result { get; private set; }
        #endregion

        #region Events
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Result = EndpointListBox.SelectedItem as ProcessorNode;
            e.Handled = true;
            DialogResult = true;
        }
        #endregion
    }
}
