using Microsoft.CodeAnalysis;
using Parcel.CoreEngine.Service.Interpretation;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.NExT.Interpreter.Analyzer;
using System;
using System.Linq;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Parcel.Neo.PopupWindows
{
    // TODO: Notice we haven't updated CodeGen to work with this yet.
    // TODO: When a snippet contains multiple functions, we can either treat it as a self-contained unit and expect a main "entry function" or import multiple functions?

    public class NodeItemPreviewViewModel : ObservableObject
    {
        #region Data Binding Properties
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }

        private string[] _inputNames;
        public string[] InputNames
        {
            get => _inputNames;
            set => SetField(ref _inputNames, value);
        }

        private string[] _outputNames;
        public string[] OutputNames
        {
            get => _outputNames;
            set => SetField(ref _outputNames, value);
        }
        #endregion
    }

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
                // Update in RMB context menu and also in the Nodes Palette

                // Register
                ToolboxIndexer.AddTool("Custom", new ToolboxNodeExport(compilation.NodeName, compilation.Method));
                // Inform main window to allocate a new Toolbox with this item as target, ready to be used
                (Owner as MainWindow).UpdatePaletteToolboxes();

                Close();
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
        private void BatchImportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string script = System.IO.File.ReadAllText(path);

                string toolboxName = System.IO.Path.GetFileNameWithoutExtension(path);
                FunctionalNodeDescription[]? functions = GatherFunctionsFromSnippet(script);
                if (functions != null)
                    ToolboxIndexer.AddTools(toolboxName, functions.Select(f => new ToolboxNodeExport(f.NodeName, f.Method)).ToArray());

                (Owner as MainWindow).UpdatePaletteToolboxes();
                Close();
            }
        }
        private void OpenSnippetMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private NodeItemPreviewViewModel _nodePreview;
        public NodeItemPreviewViewModel NodePreview
        {
            get => _nodePreview;
            set => SetField(ref _nodePreview, value);
        }
        #endregion

        #region Helpers
        private NodeItemPreviewViewModel AnalyzeSnippet(string code)
        {
            try
            {
                SingleEntranceCodeSnippetComponents? snippet = CodeAnalyzer.AnalyzeFunctionalNode(code);
                CodeAnalyzer.ExtractFunctionInformation(snippet.EntryFunction, out string functionName, out string[] inputs, out string[] outputs);
                ErrorMessage = null;
                return new()
                {
                    DisplayName = snippet.EntryFunction.Identifier.Text,
                    InputNames = inputs,
                    OutputNames = outputs,
                    // TODO: Need to populate InputNames and OutputNames somehow; Need to refactor NodesPaletteToolboxNodeItemViewModel and ToolboxNodeExport to clarify model dependencies
                };
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }
        private FunctionalNodeDescription? CompileSnippet(string code)
        {
            try
            {
                ErrorMessage = null;
                return CodeAnalyzer.CompileFunctionalNode(code);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }

        }
        private FunctionalNodeDescription[]? GatherFunctionsFromSnippet(string code)
        {
            try
            {
                ErrorMessage = null;
                return CodeAnalyzer.CompileNodeFunctions(code);
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
