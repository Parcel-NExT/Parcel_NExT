using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo
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