using System.Windows;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.PopupWindows
{
    public partial class CommentWindow : BaseWindow
    {
        public CommentWindow(Window owner, CommentNode node)
        {
            CommentNode = node;
            Owner = owner;
            InitializeComponent();
        }

        #region View Properties
        public CommentNode CommentNode { get; }
        #endregion
    }
}