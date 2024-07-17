using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Parcel.Neo.ViewModels
{
    public class NodesPaletteToolboxNodeItemViewModel : ObservableObject
    {
        #region View Components
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
        public ToolboxNodeExport Definition { get; set; }
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
