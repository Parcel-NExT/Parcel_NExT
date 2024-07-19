using Microsoft.Win32;
using System;
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
        public enum LanguageMode
        {
            CSharp,
            Python
        }

        #region Properties
        private Action RegenerateCallback { get; }
        public LanguageMode CurrentLanguageMode { get; private set; } = LanguageMode.CSharp;
        #endregion

        #region Construction
        public LiveCodePreviewWindow(Action regenerateCallback)
        {
            InitializeComponent();

            RegenerateCallback = regenerateCallback;
        }
        #endregion

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

        #region Events
        private void RefreshScriptMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RegenerateCallback?.Invoke();
        }
        private void CopyToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Instead of menu item, provide a clickable clipboard icon for button

            for (int i = 0; i < 10; i++) // Remark: Clipboard.SetText can fail. See https://stackoverflow.com/questions/68666/clipbrd-e-cant-open-error-when-setting-the-clipboard-from-net
            {
                try
                {
                    Clipboard.SetText(_liveCodePreview);
                    break;
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }

            e.Handled = true;
        }
        private void SaveScriptAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                AddExtension = true,
                Filter = "Text Files (.txt)| *.txt|Pure Script File (.cs)| *.cs|Python Script File (.py)| *.py|All Files| *.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;
                System.IO.File.WriteAllText(path, _liveCodePreview);
            }
            e.Handled = true;
        }
        private void ChangeLanguageModePureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentLanguageMode = LanguageMode.CSharp;
            RegenerateCallback?.Invoke();
            e.Handled = true;
        }
        private void ChangeLanguageModePythonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentLanguageMode = LanguageMode.Python;
            RegenerateCallback?.Invoke();
            e.Handled = true;
        }
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
