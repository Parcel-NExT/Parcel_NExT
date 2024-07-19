using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Parcel.Neo.PreviewWindows
{
    /// <summary>
    /// Interaction logic for LiveCodePreviewWindow.xaml
    /// </summary>
    public partial class LiveCodePreviewWindow : Window, INotifyPropertyChanged
    {
        public LiveCodePreviewWindow()
        {
            InitializeComponent();
        }

        #region Methods
        internal void UpdateCode(string code)
        {
            LiveCodePreview = code;
        }
        #endregion

        #region Public View Properties
        private string _liveCodePreview;
        public string LiveCodePreview { get => _liveCodePreview; set => SetField(ref _liveCodePreview, value); }
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<type>(ref type field, type value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<type>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
