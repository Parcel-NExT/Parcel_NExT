using System.Windows;
using System.Windows.Input;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.PopupWindows
{
    public partial class OutputWindow : BaseWindow
    {
        #region Construction
        public OutputWindow(Window owner, ProcessorNode processorNode)
        {
            Owner = owner;
            Node = processorNode;

            InitializeComponent();
            DisplaySurface.Node = Node;
        }
        #endregion

        #region View Properties
        private ProcessorNode _node;
        public ProcessorNode Node
        {
            get => _node;
            set => SetField(ref _node, value);
        }
        #endregion

        #region Interface
        public void Update()
        {
            DisplaySurface.Update();
            UpdateLayout();
        }
        #endregion

        #region Events
        private void OutputWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();    // Allow only LMB, since RMB can cause an exception
        }
        #endregion
    }
}