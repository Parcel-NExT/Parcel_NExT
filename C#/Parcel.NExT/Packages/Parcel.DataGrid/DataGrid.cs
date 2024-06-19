using System.Data;
using System.Dynamic;
using System.Text;

namespace Parcel.Types
{
    #region Alternative Data Grid Construct
    public class DataColumn
    {
        #region Construction
        public DataColumn() { }
        public DataColumn(string header) => Header = header.Trim().Trim('"');
        public DataColumn(DataColumn other)
        {
            Header = other.Header;
            ColumnData = other.ColumnData.ToList();
            ColumnType = other.ColumnType;
        }
        public DataColumn MakeCopy()
            => new(this);
        public void RenameHeader(string newName)
            => Header = newName;
        #endregion

        #region Properties
        public string Header { get; private set; }
        private List<dynamic> ColumnData { get; } = [];
        private Type ColumnType { get; set; }
        #endregion

        #region Accessor
        public void Add<T>(T value)
        {
            if (ColumnData.Count == 0)
                ColumnType = value.GetType();

            if (value.GetType() != ColumnType)
                ColumnType = null; // throw new ArgumentException("Wrong type.");
            ColumnData.Add(value);
        }

        public void RemoveAt(int index)
        {
            if (ColumnData.Count == 0) return;
            ColumnData.RemoveAt(index);
        }
        public int Length => ColumnData.Count;
        public dynamic this[int index] => ColumnData[index];
        public IEnumerable<T> GetDataAs<T>() => ColumnData.OfType<T>();
        public string TypeName
        {
            get
            {
                if (ColumnType == null) return "Mixed";
                else if (ColumnType == typeof(double)) return "Number";
                return ColumnType.Name;
            }
        }
        public Type Type => ColumnType;
        #endregion

        #region Column Operations (Math)
        public double Mean()
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");

            IEnumerable<double> list = ColumnData.Cast<double>();
            return list.Average();
        }
        public double Variance(bool population)
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");

            double[] values = ColumnData.Cast<double>().ToArray();
            double variance = 0.0;
            if (values.Length > 1)
            {
                double avg = values.Average();
                variance += values.Sum(value => Math.Pow(value - avg, 2.0));
            }
            return variance / (population ? values.Length - 1 : values.Length); // For population, use n-1, for sample, use n
        }
        public double StandardDeviation(bool population)
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");
            return Math.Sqrt(Variance(population));
        }
        public double Min()
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");

            IEnumerable<double> list = ColumnData.Cast<double>();
            return list.Min();
        }
        public double Max()
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");

            IEnumerable<double> list = ColumnData.Cast<double>();
            return list.Max();
        }
        public double Sum()
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");

            IEnumerable<double> list = ColumnData.Cast<double>();
            return list.Sum();
        }
        public double Correlation(DataColumn other)
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");
            if (Length != other.Length)
                throw new InvalidOperationException("Columns are not of same length.");

            double covariance = Covariance(other);
            double std1 = StandardDeviation(true);   // Always use n-1 for population
            double std2 = other.StandardDeviation(true);
            return covariance / (std1 * std2);
        }
        public double Covariance(DataColumn other)
        {
            if (ColumnType != typeof(double))
                throw new InvalidOperationException("Column is not of numerical type.");
            if (Length != other.Length)
                throw new InvalidOperationException("Columns are not of same length.");

            double[] values1 = ColumnData.Cast<double>().ToArray();
            double[] values2 = other.ColumnData.Cast<double>().ToArray();
            double variance = 0.0;
            if (values1.Length > 1)
            {
                double avg1 = values1.Average();
                double avg2 = values1.Average();
                for (int i = 0; i < values1.Length; i++)
                    variance += (values1[i] - avg1) * (values2[i] - avg2);
            }
            return variance / (values1.Length - 1); // Always use n-1 for population
        }
        #endregion
    }
    public class DataGrid
    {
        #region Helper
        /// <summary>
        /// Automatically parse a string value to plausible strongly typed objects
        /// </summary>
        public static object Preformatting(string inputValue)
        {
            // Perform pre-formatting
            if (double.TryParse(inputValue, out double number))
                return number;
            else if (DateTime.TryParse(inputValue, out DateTime dateTime))
                return dateTime;
            else return inputValue;
        }
        #endregion

        #region Constructors
        public DataGrid() { }
        public DataGrid(ExpandoObject expando)
        {
            IDictionary<string, object> dict = expando;
            foreach (string key in dict.Keys)
            {
                DataColumn col = new(key);
                col.Add(dict[key]);
                Columns.Add(col);
            }
        }
        /// <remarks>
        /// Instead of providing a constructor that reads a CSV file/CSV text, this constructor should suffice
        /// </remarks>
        public DataGrid(string name, IEnumerable<string[]> csvLines, string[]? headers, bool noParsing = false)
        {
            TableName = name;
            // Initialize columns
            if (headers != null)
                foreach (string header in headers)
                    Columns.Add(new DataColumn(header));

            foreach (string[] line in csvLines)
            {
                // Add data to columns
                for (var i = 0; i < line.Length; i++)
                    Columns[i].Add(noParsing ? line[i] : Preformatting(line[i]));
            }
        }
        public DataGrid(DataSet dataset, bool forceFirstLineAsHeader = false)
        {
            DataTable table = dataset.Tables[0];
            TableName = table.TableName;
            LoadFromDataTable(forceFirstLineAsHeader, table);
        }
        public DataGrid(DataTable dataTable)
        {
            LoadFromDataTable(false, dataTable);
        }
        #endregion

        #region Array Constructors
        public DataGrid(IEnumerable<string> values)
        {
            DataColumn col = new("Array");
            foreach (string value in values)
                col.Add(value);
            Columns.Add(col);
        }
        public DataGrid(IEnumerable<int> values)
        {
            DataColumn col = new("Array");
            foreach (int value in values)
                col.Add(value);
            Columns.Add(col);
        }
        public DataGrid(IEnumerable<double> values)
        {
            DataColumn col = new("Array");
            foreach (double value in values)
                col.Add(value);
            Columns.Add(col);
        }
        #endregion

        #region Members
        public string TableName { get; set; }
        public List<DataColumn> Columns { get; set; } = [];
        public DataColumn OptionalRowHeaderColumn { get; set; }
        #endregion

        #region Accessors
        public int ColumnCount => Columns.Count;
        public int RowCount => Columns.First().Length;
        /// <remarks>
        /// When used in a foreach statement, can retrieve directly as IDictionary<string, object> instead of dynamic
        /// </remarks>
        public List<dynamic> Rows
        {
            get
            {
                string[] columnHeaders = Columns.Select(c => c.Header).ToArray();
                List<dynamic> rows = [];
                for (int row = 0; row < RowCount; row++)
                {
                    dynamic temp = ConstructRow(columnHeaders, row);
                    rows.Add(temp);
                }
                return rows;
            }
        }
        public string ToCSV(bool withColumnHeader = true)
        {
            Dictionary<string, int> repeatNameCounter = [];
            StringBuilder builder = new();
            if (OptionalRowHeaderColumn != null) builder.Append($"{PreProcessColumnNameForDisplay(OptionalRowHeaderColumn.Header, repeatNameCounter)},");
            if (withColumnHeader)
                builder.AppendLine(string.Join(',', Columns
                    .Select(c => PreProcessColumnNameForDisplay(c.Header, repeatNameCounter))
                    .Select(n => EscapeNameForCSV(n))));

            for (int row = 0; row < RowCount; row++)
            {
                if (OptionalRowHeaderColumn != null)
                    builder.Append(OptionalRowHeaderColumn[row].ToString() + ',');
                for (int col = 0; col < ColumnCount; col++)
                    builder.Append($"\"{Columns[col][row].ToString()}\"" + ',');

                builder.Remove(builder.Length - 1, 1);
                builder.AppendLine();
            }

            return builder.ToString();

            static string EscapeNameForCSV(string name)
            {
                if (name.Contains(','))
                    name = name.Replace("\"", "\"\"");
                return $"\"{name.Trim()}\"";
            }
        }
        internal DataTable ToDataTable()
        {
            Dictionary<string, int> repeatNameCounter = [];
            List<string> headers = [];
            if (OptionalRowHeaderColumn != null) headers.Add($"{PreProcessColumnNameForDisplay(OptionalRowHeaderColumn.Header, repeatNameCounter)},");
            headers.AddRange(Columns
                .Select(c => PreProcessColumnNameForDisplay(c.Header, repeatNameCounter))
                .Select(n => EscapeDataGridViewInvalidCharacters(n))
            );

            DataTable dataTable = new(TableName);
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                dataTable.Columns.Add(new System.Data.DataColumn(header, typeof(string)));
            }

            for (int row = 0; row < RowCount; row++)
            {
                List<object> rowValues = [];
                if (OptionalRowHeaderColumn != null)
                    rowValues.Add(OptionalRowHeaderColumn[row]);
                for (int col = 0; col < ColumnCount; col++)
                    rowValues.Add(Columns[col][row]);

                DataRow dataRow = dataTable.NewRow();
                dataRow.ItemArray = [.. rowValues];
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;

            static string EscapeDataGridViewInvalidCharacters(string header)
            {
                var invalidChar = "()[]!@#$%^&*-_=\\/,. ".ToArray();
                if (invalidChar.Any(c => header.Contains(c)))
                    return $"\"{header}\"";
                else return header;
            }
        }
        #endregion

        #region Dynamic Programming - Dynamic Fields Construction
        public void Calculate(string newFieldName, Func<IDictionary<string, object>, object> computeValue)
        {
            if (Columns.Any(c => c.Header == newFieldName))
                throw new ArgumentException($"Field {newFieldName} already exists.");

            string[] existingColumnHeaders = Columns.Select(c => c.Header).ToArray();
            var column = AddColumn(newFieldName);
            for (int row = 0; row < RowCount; row++)
                column.Add(computeValue(ConstructRow(existingColumnHeaders, row)));
        }
        #endregion

        #region Convinience
        public void Save(string path)
            => File.WriteAllText(path, ToCSV());
        #endregion

        #region Editors (In-Place Operations)
        public void AddRow(params object[] values)
        {
            if (values.Length > Columns.Count)
                throw new ArgumentException("Wrong number of row elements.");

            for (int i = 0; i < values.Length; i++)
            {
                Columns[i].Add(values[i]);
            }
        }
        public DataColumn AddColumn(string columnName)
        {
            var newColumn = new DataColumn(columnName);
            Columns.Add(newColumn);
            return newColumn;
        }
        public DataColumn RemoveColumn(string extra)
        {
            if (Columns.Any(c => c.Header == extra))
                Columns.Remove(Columns.First(c => c.Header == extra));
            return null;
        }
        public void AddOptionalRowHeaderColumn(string columnName)
        {
            OptionalRowHeaderColumn = new DataColumn(columnName);
        }
        public DataColumn AddColumnFrom(DataColumn refColumn, int rowCount)
        {
            var column = new DataColumn(refColumn.Header);
            var count = rowCount == 0 ? refColumn.Length : rowCount;
            for (int i = 0; i < count; i++)
                column.Add(refColumn[i]);
            Columns.Add(column);
            return column;
        }
        public void Sort(string anchorColumnName, bool reverseOrder)
        {
            var result = reverseOrder
                ? Rows.OrderByDescending(r => ((IDictionary<string, object>)r)[anchorColumnName]).ToArray()
                : Rows.OrderBy(r => ((IDictionary<string, object>)r)[anchorColumnName]).ToArray();
            var names = Columns.Select(c => c.Header);
            Columns = names.Select(name =>
            {
                var col = new DataColumn(name);
                foreach (dynamic expando in result)
                    col.Add(((IDictionary<string, object>)expando)[name]);
                return col;
            }).ToList();
        }
        #endregion

        #region Copy Operations
        public DataGrid MakeCopy()
        {
            DataGrid result = new();
            IEnumerable<DataColumn> columnCopies = Columns
                .Select(c => c.MakeCopy());
            result.Columns = columnCopies.ToList();
            return result;
        }
        public DataGrid Append(DataGrid other)
        {
            DataGrid result = MakeCopy();
            result.Columns.AddRange(other.Columns.Select(c => c.MakeCopy()));
            return result;
        }
        public DataGrid Extract(IEnumerable<int> columnIndex)
        {
            DataGrid result = new();
            IEnumerable<DataColumn> columnCopies = columnIndex.Select(i => Columns[i].MakeCopy());
            result.Columns = columnCopies.ToList();
            return result;
        }
        public DataGrid Extract(string[] names)
        {
            DataGrid result = new();
            IEnumerable<DataColumn> columnCopies = Columns
                .Where(c => names.Contains(c.Header))
                .Select(c => c.MakeCopy());
            result.Columns = columnCopies.ToList();
            return result;
        }
        public DataGrid Exclude(IEnumerable<int> columnIndex)
            => Extract(Columns.Select((c, i) => i).Except(columnIndex));
        public DataGrid Exclude(string[] names)
            => Extract(Columns.Select(c => c.Header).Except(names).ToArray());
        public DataGrid Transpose()
        {
            DataGrid result = new();
            // Create optional column to hold existing headers
            result.AddOptionalRowHeaderColumn("Header");
            foreach (DataColumn column in Columns)
                result.OptionalRowHeaderColumn.Add(column.Header);

            // Create data columns
            for (int i = 0; i < Columns.First().Length; i++)
                result.AddColumn($"Value {i + 1}");

            // Copy values over
            foreach (DataColumn column in Columns)
                for (int row = 0; row < column.Length; row++)
                    result.Columns[row].Add(column[row]);

            return result;
        }
        public DataGrid MatrixMultiply(DataGrid other)
        {
            // Make use of only numerical columns
            var numericalColumns = Columns.Where(c => c.Type == typeof(double)).ToArray();
            var numericalColumnsOther = other.Columns.Where(c => c.Type == typeof(double)).ToArray();
            int firstMatrixColumnCount = numericalColumns.Length;
            int secondMatrixRowCount = other.RowCount;

            if (firstMatrixColumnCount != secondMatrixRowCount)
                throw new InvalidOperationException("Matrix dimensions don't match for multiplication operation.");

            // Initialize columns
            DataGrid result = new();
            for (var i = 0; i < numericalColumnsOther.Length; i++)
                result.AddColumn($"Column {i + 1}");

            // Compute rows
            for (int row = 0; row < RowCount; row++)
            {
                List<double> rowElements = [];
                for (int col = 0; col < numericalColumnsOther.Length; col++)
                {
                    double sum = 0;
                    for (int i = 0; i < firstMatrixColumnCount; i++)
                        sum += Columns[i][row] * other.Columns[col][i];
                    rowElements.Add(sum);
                }
                result.AddRow(rowElements.OfType<object>().ToArray());
            }

            return result;
        }
        #endregion

        #region Numerical Computation
        public DataGrid CovarianceMatrix()
        {
            DataGrid result = new();
            DataColumn[] numericalColumns = Columns.Where(c => c.Type == typeof(double)).ToArray();

            // Define columns
            result.AddOptionalRowHeaderColumn("Relation");
            foreach (DataColumn column in numericalColumns)
                result.AddColumn(column.Header);
            // Compute data
            foreach (DataColumn column in numericalColumns)
            {
                result.OptionalRowHeaderColumn.Add(column.Header);
                result.AddRow(numericalColumns.Select(other => other.Covariance(column)).OfType<object>().ToArray());
            }

            return result;
        }
        #endregion

        #region Routines
        public struct ColumnInfo
        {
            public string NewKey { get; set; }
            public string OriginalHeader { get; set; }
            public int ColumnIndex { get; set; }
            public string TypeName { get; set; }
        }
        public Dictionary<string, ColumnInfo> GetColumnInfoForDisplay()
        {
            Dictionary<string, int> nameCounter = [];
            IEnumerable<Tuple<string, string, int, string>> infoTuple = Columns.Select((c, i) =>
                new Tuple<string, string, int, string>(
                    PreProcessColumnNameForDisplay(c.Header, nameCounter),
                    c.Header,
                    i,
                    c.TypeName));
            Dictionary<string, ColumnInfo> dict = infoTuple.ToDictionary(
                t => t.Item1,
                t => new ColumnInfo()
                {
                    NewKey = t.Item1,
                    OriginalHeader = t.Item2,
                    ColumnIndex = t.Item3,
                    TypeName = t.Item4
                });
            if (OptionalRowHeaderColumn != null)
                dict[OptionalRowHeaderColumn.Header] = new ColumnInfo()
                {
                    NewKey = OptionalRowHeaderColumn.Header,
                    OriginalHeader = OptionalRowHeaderColumn.Header,
                    ColumnIndex = -1,
                    TypeName = OptionalRowHeaderColumn.TypeName
                };
            return dict;
        }
        private static string PreProcessColumnNameForDisplay(string original, Dictionary<string, int> nameCounter)
        {
            if (!nameCounter.TryGetValue(original, out int value))
                nameCounter[original] = 1;
            else
                nameCounter[original] = value + 1;

            return
                $"{original}{(nameCounter[original] == 1 ? string.Empty : $"{nameCounter[original]}")}";
        }
        private dynamic ConstructRow(string[] columnHeaders, int rowIndex)
        {
            Dictionary<string, int> repeatNameCounter = [];
            dynamic temp = new ExpandoObject();
            if (OptionalRowHeaderColumn != null)
                ((IDictionary<string, object>)temp)[PreProcessColumnNameForDisplay(OptionalRowHeaderColumn.Header, repeatNameCounter)] = OptionalRowHeaderColumn[rowIndex];
            for (int col = 0; col < columnHeaders.Length; col++)
                ((IDictionary<string, object>)temp)[PreProcessColumnNameForDisplay(columnHeaders[col], repeatNameCounter)] = Columns[col][rowIndex];
            return temp;
        }
        private void LoadFromDataTable(bool forceFirstLineAsHeader, DataTable table)
        {
            // Initialize columns
            List<string> headers = [];
            if (!forceFirstLineAsHeader)
            {
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    headers.Add(column.Caption);
                    Columns.Add(new DataColumn(column.Caption));
                }
            }
            else
            {
                DataRow row = table.Rows[0];
                for (int index = 0; index < table.Columns.Count; index++)
                {
                    string text = row[index].ToString();
                    headers.Add(text);
                    Columns.Add(new DataColumn(text));
                }
            }

            // Populate row data
            int startingIndex = forceFirstLineAsHeader ? 1 : 0;
            for (int i = startingIndex; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                // Add data to columns
                for (var col = 0; col < headers.Count; col++)
                    Columns[col].Add(row[col]);
            }
        }
        #endregion
    }
    #endregion
}
