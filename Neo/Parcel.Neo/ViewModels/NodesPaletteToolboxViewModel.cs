using Parcel.Neo.Base.Framework.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Parcel.Neo.ViewModels
{
    public class NodesPaletteToolboxNodeItemViewModel : ObservableObject
    {
        #region View Components
        private ImageSource? _imageSource;
        public ImageSource? ImageSource
        {
            get => _imageSource;
            set => SetField(ref _imageSource, value);
        }

        private string? _nodeName;
        public string? NodeName
        {
            get => _nodeName;
            set => SetField(ref _nodeName, value);
        }
        #endregion
    }

    public class NodesPaletteToolboxViewModel: ObservableObject
    {
        #region View Components
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
