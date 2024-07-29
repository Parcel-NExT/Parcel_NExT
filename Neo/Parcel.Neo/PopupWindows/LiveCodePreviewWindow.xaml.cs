using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Parcel.Neo.PopupWindows
{
    /// <summary>
    /// Interaction logic for LiveCodePreviewWindow.xaml
    /// </summary>
    public partial class LiveCodePreviewWindow : BaseWindow
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
        public string LiveCodePreview 
        { 
            get => _liveCodePreview; 
            set
            {
                SetField(ref _liveCodePreview, value);
                CodeEditor.Text = LiveCodePreview;
            }
        }
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
            CodeEditor.SyntaxHighlighting = ReadCSharpSyntaxHighlightingRules();
            e.Handled = true;
        }
        private void ChangeLanguageModePythonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentLanguageMode = LanguageMode.Python;
            RegenerateCallback?.Invoke();
            CodeEditor.SyntaxHighlighting = ReadPythonSyntaxHighlightingRules();
            e.Handled = true;
        }
        private void CodeEditor_Initialized(object sender, EventArgs e)
        {
            CodeEditor.SyntaxHighlighting = ReadCSharpSyntaxHighlightingRules();
        }
        #endregion

        #region Helpers
        internal static IHighlightingDefinition ReadCSharpSyntaxHighlightingRules()
        {
            // Remark: For built-in: ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#"); 
            // Alternative: ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(FileName));

            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Parcel.Neo.PopupWindows.csharp.xshd.xml");
            using System.Xml.XmlTextReader reader = new(stream);
            return HighlightingLoader.Load(reader,
                HighlightingManager.Instance);
        }
        internal static IHighlightingDefinition ReadPythonSyntaxHighlightingRules()
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Parcel.Neo.PopupWindows.python.xshd.xml");
            using System.Xml.XmlTextReader reader = new(stream);
            return HighlightingLoader.Load(reader,
                HighlightingManager.Instance);
        }
        #endregion
    }
}
