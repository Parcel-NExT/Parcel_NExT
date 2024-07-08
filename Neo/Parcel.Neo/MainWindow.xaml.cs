using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using Parcel.Neo.Base.Algorithms;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Neo.Base.Toolboxes.Basic.Nodes;
using Parcel.Neo.PopupWindows;
using BaseConnection = Parcel.Neo.Base.Framework.ViewModels.BaseConnection;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : BaseWindow
    {
        private const string _parcelWorkflowFileNameFilter = "Parcel workflow file (*.parcel)|*.parcel|YAML file (*.yaml)|*.yaml";

        #region Constructor
        public MainWindow()
        {
            RepeatLastCommand = new DelegateCommand(() => SpawnNode(LastTool, new Vector2D(Editor.MouseLocation.X, Editor.MouseLocation.Y)), 
                () => LastTool != null && !(FocusManager.GetFocusedElement(this) is TextBox) && !(Keyboard.FocusedElement is TextBox));
            SaveCanvasCommand = new DelegateCommand(() => SaveCanvas(false), () => true);
            NewCanvasCommand = new DelegateCommand(() => { Canvas.Nodes.Clear(); Canvas.Connections.Clear(); SaveCanvas(true);}, () => true);
            OpenCanvasCommand = new DelegateCommand(OpenCanvas, () => true);
            
            InitializeComponent();
            
            EventManager.RegisterClassHandler(typeof(Nodify.BaseConnection), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnConnectionInteraction));
            EventManager.RegisterClassHandler(typeof(Nodify.Node), MouseLeftButtonDownEvent, new MouseButtonEventHandler(NodeDoubleclick_OpenProperties));
            EventManager.RegisterClassHandler(typeof(Nodify.GroupingNode), MouseLeftButtonDownEvent, new MouseButtonEventHandler(NodeDoubleclick_OpenProperties));
        }
        public NodesCanvas Canvas { get; set; } = new NodesCanvas();
        #endregion

        #region Commands
        /// <summary>
        /// Provides bookkeeping for Repeat command
        /// </summary>
        private ToolboxNodeExport LastTool { get; set; }
        
        public ICommand RepeatLastCommand { get; }
        
        public ICommand CreateCalculatorNodeCommand { get; }
        public ICommand SaveCanvasCommand { get; }
        public ICommand NewCanvasCommand { get; }
        public ICommand OpenCanvasCommand { get; }
        
        public ICommand ShowHelpCommand { get; }
        #endregion

        #region View Properties
        private const string TitlePrefix = "PV1 Neo by Methodox - The Workflow Engine (Parcel NExT)";
        private string _dynamicTitle = TitlePrefix;
        public string DynamicTitle { get => _dynamicTitle; set => SetField(ref _dynamicTitle, value); }
        private string _currentFilePath;
        public string CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                SetField(ref _currentFilePath, value);
                DynamicTitle = $"{(Owner != null ? "<Reference>" : "<Main>")} {System.IO.Path.GetFileNameWithoutExtension(value)}";
            }
        }

        #endregion

        #region Advanced Node Graph Behaviors
        private void OnConnectionInteraction(object sender, MouseButtonEventArgs e)
        {
            if (sender is Nodify.BaseConnection ctrl && ctrl.DataContext is BaseConnection connection)
            {
                if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    connection.Remove();
                }
                else if (e.ClickCount > 1)
                {
                    var pos = e.GetPosition(ctrl) - new Vector(30, 15);
                    connection.Split(new Vector2D(pos.X, pos.Y));
                }
            }
        }
        #endregion

        #region Events
        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.F3)
            {
                ShowSearchNodePopup();
                e.Handled = true;
            }
        }
        private void Editor_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Use MouseLeftButtonDown instead of MouseDoubleClick event to deal with WPF's e.handled not effective issue
            if (e.ClickCount != 2) return;
            
            OpenCanvas();
            e.Handled = true;

        }
        private void Editor_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowSearchNodePopup();
            e.Handled = true;
        }
        private void NodeDoubleclick_OpenProperties(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            if (!(e.Source is Nodify.Node {DataContext: ProcessorNode processorNode})
                && !(e.Source is Nodify.GroupingNode {DataContext: CommentNode commentNode})) return;

            if (e.Source is Nodify.Node node)
                SpawnPropertyWindow(node.DataContext as BaseNode);
            else if (e.Source is Nodify.GroupingNode groupingNode)
                SpawnPropertyWindow(groupingNode.DataContext as CommentNode);
            
            e.Handled = true;
        }
        private void OpenFileNode_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button {DataContext: OpenFileNode node})) return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Open",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                node.Path = openFileDialog.FileName;
            }
            e.Handled = true;
        }
        private void SaveFileNode_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button {DataContext: SaveFileNode node})) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Select Path to Save",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                node.Path = saveFileDialog.FileName;
            }
            e.Handled = true;
        }
        private void ProcessorNodeTogglePreviewButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Border {Tag: ProcessorNode node} border)) return;

            node.IsPreview = !node.IsPreview;
            e.Handled = true;
        }
        private void ProcessorNodePreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button {Tag: ProcessorNode node} button)) return;

            // TODO: Somehow recover the following behavior
            // Auto-Generate
            //if ((node is CSV || node is Excel) && node.ShouldHaveAutoConnection)
            //{
            //    OpenFileNode filePathNode = SpawnNode(new ToolboxNodeExport("File Input", typeof(OpenFileNode)),
            //        node.Location + new Vector2D(-200, -60)) as OpenFileNode;
            //    Canvas.Schema.TryAddConnection(filePathNode!.MainOutput, node.Input.First());

            //    OpenFileDialog openFileDialog = new OpenFileDialog() { Title = "Select File to Open" };
            //    openFileDialog.Filter = node switch
            //    {
            //        CSV _ => "CSV file (*.csv)|*.csv|All types (*.*)|*.*",
            //        Excel _ => "Excel file (*.xlsx)|*.xlsx|All types (*.*)|*.*",
            //        _ => openFileDialog.Filter
            //    };
            //    if (openFileDialog.ShowDialog() == true)
            //    {
            //        filePathNode.Path = openFileDialog.FileName;
            //    }
            //}
            //if (node is WriteCSV && node.ShouldHaveAutoConnection)
            //{
            //    SaveFileNode filePathNode = SpawnNode(new ToolboxNodeExport("File Path", typeof(SaveFileNode)),
            //        node.Location + new Vector2D(-200, -60)) as SaveFileNode;
            //    Canvas.Schema.TryAddConnection(filePathNode!.MainOutput, node.Input.First());

            //    SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = "Select Path to Save" };
            //    saveFileDialog.Filter = "CSV file (*.csv)|*.csv|All types (*.*)|*.*";
            //    if (saveFileDialog.ShowDialog() == true)
            //    {
            //        filePathNode.Path = saveFileDialog.FileName;
            //    }
            //}
            else if (node is IAutoConnect autoConnect && autoConnect.ShouldHaveAutoConnection && node.AutoPopulatedConnectionNodes != null)
            {
                foreach (Tuple<ToolboxNodeExport,Vector2D,InputConnector> generateNode in autoConnect.AutoPopulatedConnectionNodes)
                {
                    BaseNode temp = SpawnNode(generateNode.Item1, node.Location + generateNode.Item2); // TODO: Notice this can cause problems to function arguments like string[]
                    if(temp is IMainOutputNode outputNode)
                        Canvas.Schema.TryAddConnection(outputNode.MainOutput, generateNode.Item3);
                }
                
                e.Handled = true;
                return;
            }
            
            // Connection check
            if (node.ShouldHaveAutoConnection && node.AutoPopulatedConnectionNodes == null)
            {
                node.Message.Content = "Require Connection.";
                node.Message.Type = NodeMessageType.Error;
                
                e.Handled = true;
                return;
            }
            
            // This is a good chance to auto-save, before anything can crash
            if (CurrentFilePath != null || System.IO.File.Exists(CurrentFilePath))
                Canvas.Save(GetAutoSavePath(CurrentFilePath));
            
            node.IsPreview = true;
            if (node is not GraphReferenceNode reference || _graphPreviewWindows.ContainsKey(reference) || _previewWindows.ContainsKey(reference))  // For graph reference we really don't want to execute it during preview the first time
                ExecuteAll();
            SpawnPreviewWindow(node);

            e.Handled = true;
        }
        private void PrimitiveInputTextbox_OnPreviewKeyDown_CommandOverride(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            // Deal with shortcut key commands issues
            if (e.Key == Key.C || e.Key == Key.R)
            {
                box!.RaiseEvent(new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice, 
                    new TextComposition(InputManager.Current, box, 
                        (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) 
                        ? e.Key.ToString()
                        : e.Key.ToString().ToLower())) 
                {
                    RoutedEvent = TextCompositionManager.TextInputEvent 
                });
                
                e.Handled = true;
            }
        }
        private bool _consoleIsOpen;
        private void ToggleConsoleWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_consoleIsOpen)
            {
                FreeConsole();
                _consoleIsOpen = false;
            }
            else
            {
                AllocConsole();
                _consoleIsOpen = true;
            }
        }
        private void ExportExecutableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExperimentalFeatureWarningWindow warning = new() { Owner = this };
            if (!warning.ShowDialog() == true)
                return;

            SaveFileDialog saveFileDialog = new()
            {
                Title = "Choose Path to Save Executable (Experimental)",
                AddExtension = true,
                DefaultExt = ".exe",
                Filter = "Executables (.exe)|*.exe"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                AlgorithmHelper.CompileGraphAOT(filePath, Canvas);

                // TODO: Show output file in file explorer after done
            }
        }
        private void ExportPureScriptsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Remark: Notice exporting like this will NOT cause the graph to be executed (thus avoiding any potential execution-time errors but also means we won't check whether the script will work during runtime - including empty input/invalid parameters check)
            // The recommendation here (to the user) is to run and make sure things runs before exporting

            ExperimentalFeatureWarningWindow warning = new() { Owner = this };
            if (!warning.ShowDialog() == true)
                return;

            OpenFolderDialog folderDialog = new()
            {
                Title = "Choose Folder to Save Exported Scripts"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string folderName = folderDialog.FolderName;
                string mainScriptFilename = $"{System.IO.Path.GetFileNameWithoutExtension(CurrentFilePath)}.cs";
                AlgorithmHelper.GenerateGraphScripts(folderName, mainScriptFilename, Canvas);

                // TODO: Open output folder in file explorer after done
            }
        }
        private void ExportPythonScriptsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExperimentalFeatureWarningWindow warning = new() { Owner = this };
            if (!warning.ShowDialog() == true)
                return;

            OpenFolderDialog folderDialog = new()
            {
                Title = "Choose Folder to Save Exported Scripts"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string folderName = folderDialog.FolderName;
                // Do something with the result
            }
        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow()
            {
                Owner = this
            }.ShowDialog();
        }
        #endregion

        #region Routine
        private BaseNode SpawnNode(ToolboxNodeExport tool, Vector2D spawnLocation)
        {
            BaseNode node = tool.InstantiateNode();

            if (node is ProcessorNode processorNode)
                processorNode.Title = tool.Name;

            node!.Location = spawnLocation;
            Canvas.Nodes.Add(node);
            return node;
        }
        private void SpawnPreviewWindow(ProcessorNode node)
        {
            if (_previewWindows.ContainsKey(node))
            {
                _previewWindows[node].Activate();
            }
            else
            {
                if (node.MainOutput == null || !node.HasCache(node.MainOutput)) return;
                
                if (node is GraphReferenceNode graph)
                {
                    if (graph.GraphPath == null)
                        InitializeGraphReferenceNode(graph);
                    if (graph.GraphPath == null) return;
                
                    MainWindow graphPreview = new MainWindow()
                    {
                        Owner = this,
                        CurrentFilePath = graph.GraphPath,
                    };
                    graphPreview.Canvas.Open(graph.GraphPath);
                    _graphPreviewWindows[graph] = graphPreview;
                    graphPreview.Closed += (sender, args) => _graphPreviewWindows.Remove(graph);
                    graphPreview.Show();
                }
                
                PreviewWindow preview = new(this, node);
                _previewWindows.Add(node, preview);
                preview.Closed += (sender, args) => _previewWindows.Remove((sender as PreviewWindow)!.Node); 
                preview.Show();   
            }
        }
        private void SpawnPropertyWindow(BaseNode node)
        {
            if (node is GraphReferenceNode graphReference)
            {
                InitializeGraphReferenceNode(graphReference);
                return;
            }
            
            Point cursor = GetCurosrWindowPosition();
            if(node is ProcessorNode processorNode)
                new PropertyWindow(this, processorNode)
                {
                    Left = cursor.X,
                    Top = cursor.Y
                }.Show();
            else if(node is CommentNode commentNode)
                new CommentWindow(this, commentNode)
                {
                    Left = cursor.X,
                    Top = cursor.Y
                }.Show();
        }
        private static void InitializeGraphReferenceNode(GraphReferenceNode graphReference)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Existing Workflow",
                Filter = _parcelWorkflowFileNameFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                graphReference.GraphPath = openFileDialog.FileName;
                graphReference.Title = System.IO.Path.GetFileNameWithoutExtension(graphReference.GraphPath);
            }
        }
        private void ExecuteAll()
        {
            AlgorithmHelper.ExecuteGraph(Canvas);
            foreach (PreviewWindow p in _previewWindows.Values)
                p.Update();
        }
        private void ShowSearchNodePopup()
        {
            Point cursor = GetCurosrWindowPosition();
            Point spawnLocation = Editor.MouseLocation;

            PopupTab popupTab = new(this)
            {
                Left = cursor.X,
                Top = cursor.Y,
                Topmost = true
            };
            void CreateNodeFromSelectedSearchItem(ToolboxNodeExport? toolboxNodeExport)
            {
                if (toolboxNodeExport != null)
                {
                    LastTool = toolboxNodeExport;
                    SpawnNode(LastTool, new Vector2D(spawnLocation.X, spawnLocation.Y));
                }
            }
            popupTab.ItemSelectedAdditionalCallback += CreateNodeFromSelectedSearchItem;
            popupTab.Show();
        }
        private void OpenCanvas()
        {
            // Close previews
            foreach (KeyValuePair<ProcessorNode,PreviewWindow> previewWindow in _previewWindows)
                previewWindow.Value.Close();
            foreach (KeyValuePair<GraphReferenceNode,MainWindow> graphPreviewWindow in _graphPreviewWindows)
                graphPreviewWindow.Value.Close();
            
            // Open
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Parcel Workflow File",
                Filter = _parcelWorkflowFileNameFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = openFileDialog.FileName;
                Canvas.Open(CurrentFilePath);
            }
        }
        private void SaveCanvas(bool createNewFile = false)
        {
            if (createNewFile || CurrentFilePath == null || !System.IO.File.Exists(CurrentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Choose Where to Save Current Workflow",
                    Filter = _parcelWorkflowFileNameFilter
                };
                if (saveFileDialog.ShowDialog() == true)
                    CurrentFilePath = saveFileDialog.FileName;
                else return;
            }

            Canvas.Save(CurrentFilePath);
        }
        private Point GetCurosrWindowPosition()
        {
            Point cursor = Mouse.GetPosition(this);
            RECT rect = GetWindowRectangle(this);
            return new Point((this.WindowState == WindowState.Maximized ? rect.Left : this.Left) + cursor.X,
                (this.WindowState == WindowState.Maximized ? rect.Top : this.Top) + cursor.Y);
        }
        private string GetAutoSavePath(string currentFilePath)
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            string fileFolder = System.IO.Path.GetDirectoryName(currentFilePath);
            string extension = System.IO.Path.GetExtension(currentFilePath);

            string dateString = DateTime.Now.ToString("yyyyMMdd HH-mm-ss");
            string newFilePath = System.IO.Path.Combine(fileFolder!, $"{fileNameWithoutExtension}_autosave_{dateString}{extension}");
            return newFilePath;
        }
        #endregion

        #region State
        private readonly Dictionary<ProcessorNode, PreviewWindow> _previewWindows =
            new Dictionary<ProcessorNode, PreviewWindow>();
        private readonly Dictionary<GraphReferenceNode, MainWindow> _graphPreviewWindows =
            new Dictionary<GraphReferenceNode, MainWindow>();
        #endregion

        #region Interop
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // Make sure RECT is actually OUR defined struct, not the windows rect.
        public static RECT GetWindowRectangle(Window window)
        {
            RECT rect;
            GetWindowRect((new WindowInteropHelper(window)).Handle, out rect);

            return rect;
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
        #endregion
    }
}