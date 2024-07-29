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
                
                public static string Fetch(string url)
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

        #region Events
        private void CreateNodeFunctionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FunctionalNodeDescription? compilation = CompileSnippet(CodeEditor.Text);
            if (compilation != null)
            {
                // TODO: Inform main window to allocate a new Toolbox with this item as target, ready to be used (update in RMB context menu and also in the Nodes Palette
            }
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
        private void ImportSnippetMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void SaveSnippetAsMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
                CodeSnippetComponents? snippet = CodeAnalyzer.AnalyzeFunctionalNode(code);
                return new()
                {
                    Definition = null,
                    DisplayName = snippet.EntryFunction.Identifier.Text,
                    IsConstructor = false,
                    PreviewImage = null
                    // TODO: Need to populate InputNames and OutputNames somehow; Need to refactor NodesPaletteToolboxNodeItemViewModel and ToolboxNodeExport to clarify model dependencies
                };
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return NodePreview;
            }
            
        }
        private FunctionalNodeDescription? CompileSnippet(string code)
        {
            try
            {
                return CodeAnalyzer.CompileFunctionalNode(code);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }

        }
        #endregion
    }
}
