using Parcel.CoreEngine.Service.Interpretation;
using Parcel.Neo.ViewModels;
using Parcel.NExT.Interpreter.Analyzer;
using System;

namespace Parcel.Neo.PopupWindows
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

            // Test use
            CodeEditor.Text = """
                using System.Net.Http;
                using System.Net.Http.Headers;
                
                string Fetch(string url)
                {
                    using HttpClient client = new();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Parcel REST API - The Very Lean HttpClient");
                    return client.GetStringAsync(url).Result;
                }
                """;
            NodePreview = AnalyzeSnippet(CodeEditor.Text);
        }
        #endregion

        #region Methods
        #endregion

        #region Events
        private void AnalyzeCodeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string code = CodeEditor.Text;
            NodePreview = AnalyzeSnippet(code);
        }
        private void CodeEditor_Initialized(object sender, EventArgs e)
        {
            CodeEditor.SyntaxHighlighting = LiveCodePreviewWindow.ReadCSharpSyntaxHighlightingRules();
        }
        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            string code = CodeEditor.Text;
            NodePreview = AnalyzeSnippet(code);
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
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        private NodesPaletteToolboxNodeItemViewModel _nodePreview;
        public NodesPaletteToolboxNodeItemViewModel NodePreview
        {
            get => _nodePreview;
            set => SetField(ref _nodePreview, value);
        }
        #endregion

        #region Helpers
        private NodesPaletteToolboxNodeItemViewModel AnalyzeSnippet(string code)
        {
            try
            {
                FunctionalNodeDescription? description = CodeAnalyzer.AnalyzeFunctionalNode(code);

                var method = typeof(CreateFunctionWindow).GetMethod(nameof(Fetch));
                return new()
                {
                    // Definition = new Base.Framework.ToolboxNodeExport("Hello World", new CoreEngine.Interfaces.Callable(description)),// Need refactoring.
                    Definition = new Base.Framework.ToolboxNodeExport(method.Name, new CoreEngine.Interfaces.Callable(method)),
                    DisplayName = method.Name,
                    IsConstructor = false,
                    PreviewImage = null
                };
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return NodePreview;
            }
            
        }
        public static string Fetch(string url)
        {
            return string.Empty;
        }
        #endregion
    }
}
