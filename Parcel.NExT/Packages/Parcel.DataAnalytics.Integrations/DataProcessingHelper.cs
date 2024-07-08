using System.Data;
using System.Data.SQLite; // TODO: Change to Microsoft SQLite
using System.Dynamic;
using ExcelDataReader;
using Parcel.CoreEngine.Helpers;
using Parcel.Types;
using DataColumn = Parcel.Types.DataColumn;
using DataTable = System.Data.DataTable;

namespace Parcel.Integration
{
    public static class DataProcessingHelper
    {
        public static DataGrid CSV(string filePath, bool containsHeader)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException("Invalid inputs");
            // TODO: Currently if the CSV File is opened by excel then it's not readable by us, and the File.ReadAllText will throw an exception

            return new DataGrid(Path.GetFileNameWithoutExtension(filePath), CSVHelper.ReadCSVFile(filePath, out string[]? headers, containsHeader), headers);
        }

        public static DataGrid Excel(string filePath, bool containsHeader)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException("Invalid inputs");

            DataSet? result = null;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                result = reader.AsDataSet();
            }

            return new DataGrid(result, containsHeader);
        }
        public static DataGrid AddSum(DataGrid dataGrid)
        {
            DataGrid result = new();
            foreach (DataColumn dataColumn in dataGrid.Columns.Where(c => c.Type == typeof(double)))
            {
                DataColumn newColumn = new($"{dataColumn.Header} (Sum)");
                newColumn.Add(dataColumn.Sum());
                result.Columns.Add(newColumn);
            }
            return result;
        }
        /// <summary>
        /// Similar to "trim"
        /// </summary>
        public static DataGrid TakeRows(DataGrid dataGrid, int rowCount)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");
            if (rowCount <= 0)
                throw new ArgumentException("Invalid take amount");
            if (rowCount > dataGrid.RowCount)
                throw new ArgumentException("Take amount is more than amount of rows in table.");
            if (rowCount == dataGrid.RowCount)
                throw new ArgumentException("Take amount is the same as the amount of rows in table.");

            DataGrid newDataGrid = new();
            foreach (DataColumn inputTableColumn in dataGrid.Columns)
                newDataGrid.AddColumnFrom(inputTableColumn, rowCount);
            return newDataGrid;
        }
        /// <summary>
        /// Can be used to extract or reorder fields
        /// </summary>
        public static DataGrid Extract(DataGrid dataGrid, string[] columnNames)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");
            if (dataGrid != null && columnNames.Length == 0)
                throw new ArgumentException("No columns are given for the table.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = dataGrid.GetColumnInfoForDisplay();
            if (columnNames.Any(cn => !columnInfos.ContainsKey(cn)))
                throw new ArgumentException("Cannot find column with specified name on data table.");

            return dataGrid.Extract(columnNames.Select(cn => columnInfos[cn].ColumnIndex));
        }

        /// <summary>
        /// Opposite of Extract
        /// </summary>
        public static DataGrid Exclude(DataGrid dataGrid, string[] columnNames)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");
            if (columnNames.Length == 0)
                throw new ArgumentException("No columns are given for the table.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = dataGrid.GetColumnInfoForDisplay();
            if (columnNames.Any(cn => !columnInfos.ContainsKey(cn)))
                throw new ArgumentException("Cannot find column with specified name on data table.");

            return dataGrid.Exclude(columnNames.Select(cn => columnInfos[cn].ColumnIndex));
        }

        public static DataGrid Rename(DataGrid dataGrid, string[] originalColumnNames, string[] newColumnNames)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");
            if (originalColumnNames.Any(string.IsNullOrWhiteSpace)
                || newColumnNames.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Empty inputs.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = dataGrid.GetColumnInfoForDisplay();
            if (originalColumnNames.Any(c => !columnInfos.ContainsKey(c)))
                throw new ArgumentException("Input column name doesn't exist on input table.");
            // TODO: At the moment we are allowing renamed column names to already exist on the table, i.e. columns with same headers

            DataGrid copy = dataGrid.MakeCopy();
            for (int i = 0; i < originalColumnNames.Length; i++)
            {
                int oldColumnIndex = columnInfos[originalColumnNames[i]].ColumnIndex;
                string newName = newColumnNames[i];
                copy.Columns[oldColumnIndex].RenameHeader(newName);
            }
            return copy;
        }

        public static DataGrid Sort(DataGrid dataGrid, string anchorColumnName, bool reverseOrder)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(anchorColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(anchorColumnName)
                                             && dataGrid.Columns.All(c => c.Header != anchorColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            DataGrid newTable = dataGrid.MakeCopy();
            newTable.Sort(anchorColumnName, reverseOrder);
            return newTable;
        }

        public static DataGrid Append(DataGrid[] dataGrids)
        {
            if (dataGrids.Any(t => t == null))
                throw new ArgumentException("Missing data grid input.");
            if (dataGrids.Select(t => t.RowCount).Distinct().Count() != 1)
                throw new ArgumentException("Data Table size are not equal (there might be bad data).");

            DataGrid result = dataGrids.First();
            for (int i = 1; i < dataGrids.Length; i++)
                result = result.MakeCopy().Append(dataGrids[i]);
            return result;
        }

        public static DataGrid Transpose(DataGrid dataGrid)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing data grid input.");

            return dataGrid.Transpose();
        }
        /// <summary>
        /// Dynamic connector sequence, With option to transpose
        /// </summary>
        public static DataGrid MatrixMultiply(DataGrid[] dataGrids, bool[] shouldTransposes)
        {
            if (dataGrids.Length != shouldTransposes.Length)
                throw new ArgumentException("Wrong number of inputs.");
            if (dataGrids.Any(t => t == null))
                throw new ArgumentException("Invalid table inputs.");

            DataGrid result = shouldTransposes.First() ? dataGrids.First().Transpose() : dataGrids.First();
            for (int i = 1; i < dataGrids.Length; i++)
            {
                bool shouldTranspose = shouldTransposes[i];
                result = result.MatrixMultiply(shouldTranspose ? dataGrids[i].Transpose() : dataGrids[i]);
            }
            return result;
        }
        /// <summary>
        /// Perform SQL query on selected data grids.
        /// </summary>
        /// <param name="sqlCommand" isHidden="isHidden" quickEdit="quickEdit"></param>
        public static DataGrid SQL(DataGrid[] dataGrids, string[] tableNames, string sqlCommand)
        {
            static void PopulateTable(DataGrid table, string tableName, SQLiteConnection connection)
            {
                SQLiteCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"CREATE TABLE '{tableName}'({string.Join(',', table.Columns.Select(c => $"'{c.Header}'"))})";
                cmd.ExecuteNonQuery();

                // Remark: The API is as shitty as it can get

                SQLiteTransaction transaction = connection.BeginTransaction();

                string sql = $"select * from '{tableName}' limit 1";
                SQLiteDataAdapter adapter = new(sql, connection);
                adapter.InsertCommand = new SQLiteCommandBuilder(adapter).GetInsertCommand();

                DataSet dataSet = new();
                adapter.FillSchema(dataSet, SchemaType.Source, tableName);
                adapter.Fill(dataSet, tableName);     // Load exiting table data (will be empty) 

                // Insert data
                DataTable dataTable = dataSet.Tables[tableName];
                foreach (ExpandoObject row in table.Rows)
                {
                    DataRow dataTableRow = dataTable.NewRow();
                    foreach (KeyValuePair<string, dynamic> pair in (IDictionary<string, dynamic>)row)
                        dataTableRow[pair.Key] = pair.Value;
                    dataTable.Rows.Add(dataTableRow);
                }
                int result = adapter.Update(dataTable);

                transaction.Commit();
                dataSet.AcceptChanges();
                // Release resources 
                adapter.Dispose();
                dataSet.Clear();
            }

            if (dataGrids.Length == 0 || dataGrids == null)
                throw new ArgumentException("Missing data grid input.");

            DataGrid? output;
            using (var connection = new SQLiteConnection("Data Source=:memory:"))
            {
                connection.Open();

                // Initialize
                for (int i = 0; i < dataGrids.Length; i++)
                {
                    PopulateTable(dataGrids[i], tableNames[i], connection);
                }

                // Execute
                string formattedText = sqlCommand.EndsWith(';')
                    ? sqlCommand
                    : sqlCommand + ';';
                for (int i = 0; i < dataGrids.Length; i++)
                    formattedText = formattedText.Replace($"@Table{i + 1}", $"'{tableNames[i]}'"); // Table names can't use parameters, so do it manually
                SQLiteDataAdapter adapter = new(formattedText, connection);
                DataSet result = new();
                adapter.Fill(result);

                output = new DataGrid(result);
                connection.Close();
            }
            return output!;
        }
    }
}