using Parcel.Neo.ViewModels;
using System;

namespace Parcel.Neo.PreviewWindows
{
    public partial class CreateFunctionWindow : BaseWindow
    {
        public enum LanguageMode
        {
            CSharp,
            Python
        }

        #region Properties
        public LanguageMode CurrentLanguageMode { get; private set; } = LanguageMode.CSharp;
        #endregion

        #region Construction
        public CreateFunctionWindow()
        {
            InitializeComponent();

            NodePreview = new()
            {
                Definition = new Base.Framework.ToolboxNodeExport("Hello World", new CoreEngine.Interfaces.Callable(GetType().GetMethod(nameof(TestMethod)))),
                DisplayName = "Hello World",
                IsConstructor = false,
                PreviewImage = null
            };
        }
        public static void TestMethod(string input)
        { }

        #endregion

        #region Methods
        #endregion

        #region Events
        private void AnalyzeCodeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string code = CodeEditor.Text;

        }
        private void CodeEditor_Initialized(object sender, EventArgs e)
        {
            CodeEditor.SyntaxHighlighting = LiveCodePreviewWindow.ReadCSharpSyntaxHighlightingRules();
        }
        #endregion

        #region Public View Properties
        private string _sourceCode;
        public string SoureceCode 
        { 
            get => _sourceCode; 
            set
            {
                SetField(ref _sourceCode, value);
                CodeEditor.Text = SoureceCode;
            }
        }

        private NodesPaletteToolboxNodeItemViewModel _nodePreview;
        public NodesPaletteToolboxNodeItemViewModel NodePreview
        {
            get => _nodePreview;
            set => SetField(ref _nodePreview, value);
        }
        #endregion
    }
}
