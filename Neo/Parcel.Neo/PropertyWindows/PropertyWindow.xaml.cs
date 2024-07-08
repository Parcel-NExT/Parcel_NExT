using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Toolboxes.Basic.Nodes;

namespace Parcel.Neo
{
    public partial class PropertyWindow : BaseWindow
    {
        public PropertyWindow(Window owner, ProcessorNode processor)
        {
            // Support SQL syntax highlight
            using (System.IO.Stream? stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Parcel.Neo.PreviewWindows.sql.xshd.xml"))
            {
                using System.Xml.XmlTextReader reader = new(stream);
                SQLSyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                    HighlightingManager.Instance);
            }

            Processor = processor;
            Owner = owner;
            if (processor is INodeProperty nodeProperty)
                Editors = nodeProperty.Editors;
            if (processor is PrimitiveNode primitiveNode)
            {
                PrimitivePreviewTextBlockVisibility = Visibility.Visible;
                PrimitivePreviewText = primitiveNode.Value;
            }
            InitializeComponent();
            
            if (processor is DataTable dataTable)
            {
                PreviewWindow.PopulateDataGrid(WpfDataGrid, dataTable.InitializeDataGrid(), out string[] dataGridDataColumns, out List<dynamic> dataGridData);
                DataGridDataColumns = dataGridDataColumns;
                DataGridData = dataGridData;
                DataGridVisibility = Visibility.Visible;
            }
        }

        #region DataGrid View Properties
        private List<dynamic> _dataGridData;
        public List<dynamic> DataGridData
        {
            get => _dataGridData;
            set => SetField(ref _dataGridData, value);
        }
        private Visibility _dataGridVisibility = Visibility.Collapsed;
        public Visibility DataGridVisibility
        {
            get => _dataGridVisibility;
            set => SetField(ref _dataGridVisibility, value);
        }
        private string[] _dataGridDataColumns;
        public string[] DataGridDataColumns
        {
            get => _dataGridDataColumns;
            set => SetField(ref _dataGridDataColumns, value);
        }
        #endregion

        #region View Properties
        public ProcessorNode Processor { get; }
        public List<PropertyEditor> Editors { get; }
        public string PrimitivePreviewText { get; }
        public Visibility PrimitivePreviewTextBlockVisibility { get; } = Visibility.Collapsed;
        #endregion
        
        #region Syntax Highlighter

        private IHighlightingDefinition _SQLSyntaxHighlighting;
        public IHighlightingDefinition SQLSyntaxHighlighting
        {
            get => _SQLSyntaxHighlighting;
            set => SetField(ref _SQLSyntaxHighlighting, value);
        }
        #endregion

        #region Events
        private void AvalonEditor_OnInitialized(object sender, EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            PropertyEditor property = editor!.DataContext as PropertyEditor;

            editor.Text = property!.Binding as string;
        }
        private void AvalonEditor_OnTextChanged(object sender, EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            PropertyEditor property = editor!.DataContext as PropertyEditor;
            
            property!.Binding  = editor.Text;
        }
        private void PropertyWindow_OnClosed(object sender, EventArgs e)
        {
            if (Processor is DataTable dataTable && DataGridData.Count > 0)
            {
                // Initialize 2D object array
                IDictionary<string, object> sample = DataGridData[0] as IDictionary<string, object>;
                int colCount = sample!.Keys.Count;
                int rowCount = DataGridData.Count;  // TODO: At the moment if an entry is finished half-way before the window is closed, it produce bad data
                string[] keys = sample!.Keys.ToArray();
                
                // Col first, then Rows; Like Parcel's DataGrid
                dataTable.Data = new object[colCount][];    // Notice despite the syntax, this actually initializes the outer dimension, aka. new object[][colCount]
                for (int i = 0; i < colCount; i++)
                    dataTable.Data[i] = new object[rowCount];
                
                // Populate data
                for (int col = 0; col < colCount; col++)
                {
                    for (int row = 0; row < rowCount; row++)
                    {
                        dataTable.Data[col][row] =  ((IDictionary<string, object>)DataGridData[row])[keys[col]];
                    }
                }
            }
        }
        #endregion
    }
}