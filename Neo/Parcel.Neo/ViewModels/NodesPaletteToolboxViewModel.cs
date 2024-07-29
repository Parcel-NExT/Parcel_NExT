using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Parcel.Neo.ViewModels
{
    /// <summary>
    /// A view model for NodesPalette, this contains reference to ToolboxNodeExport as payload for drag-n-drop purpose
    /// </summary>
    public class NodesPaletteToolboxNodeItemViewModel : ObservableObject
    {
        #region View Components - Essential
        private ImageSource? _previewImage;
        public ImageSource? PreviewImage
        {
            get => _previewImage;
            set => SetField(ref _previewImage, value);
        }

        private string? _displayName;
        public string? DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }
        #endregion

        #region Payload
        public ToolboxNodeExport Definition { get; set; }
        #endregion

        #region View Components - Styling
        private bool _isConstructor;
        public bool IsConstructor
        {
            get => _isConstructor;
            set => SetField(ref _isConstructor, value);
        }
        #endregion
    }

    public class NodesPaletteToolboxViewModel: ObservableObject
    {
        #region View Components
        private bool _collapsed = false;
        public bool Collapsed
        {
            get => _collapsed;
            set => SetField(ref _collapsed, value);
        }

        private string _ToolboxName = string.Empty;
        public string ToolboxName
        {
            get => _ToolboxName;
            set => SetField(ref _ToolboxName, value);
        }

        private ObservableCollection<NodesPaletteToolboxNodeItemViewModel> _items = default!;
        public ObservableCollection<NodesPaletteToolboxNodeItemViewModel> Items
        {
            get => _items;
            set => SetField(ref _items, value);
        }
        #endregion
    }
}
