using Parcel.Types;

namespace Parcel.Types
{
    /// <summary>
    /// Finance/Data analytics oriented.
    /// 
    /// Design principle: Basic operations are on columns of data grids (versus the Vector package) (Those will check and validate column type as Number/Double)
    /// </summary>
    public static class DataGridOperationsHelper
    {
        /// <param name="columnSelection">GUI can provide more specific dynamic behavior for this parameter based on Table Input, e.g. hide it when dataGrid is not hooked, and provide enumeration if input table contains headers</param>
        /// <remarks>I don't remember what's the intention of this</remarks>
        public static void RobustMean(double[] list, DataGrid dataGrid, string columnName, int? columnSelection, out double[] resultList, out DataGrid resultDataGrid, out double resultValue)
        {
            columnSelection ??= -1;

            if (list == null && dataGrid == null)
                throw new ArgumentException("Invalid inputs");
            if (list != null && dataGrid != null)
                throw new ArgumentException("Invalid inputs");
            if (dataGrid != null && columnSelection == -1 && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && columnSelection != -1
                                             && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            resultValue = 0;
            if (list != null)
                resultValue = list.Sum();
            if (dataGrid != null)
            {
                if (columnSelection != -1)
                    resultValue = dataGrid.Columns[columnSelection.Value].Mean();
                else if (!string.IsNullOrWhiteSpace(columnName))
                    resultValue = dataGrid.Columns.Single(c => c.Header == columnName).Mean();
            }

            resultList = list!;
            resultDataGrid = dataGrid!;
        }
        /// <summary>
        /// Outputs Min, Max, and Max-Min;
        /// Also displays those numbers in three lines in the node message content.
        /// </summary>
        /// <param name="message" nodeMessage="nodeMessage">Used for node message</param>
        public static void Range(this DataGrid dataGrid, string columnName, out double min, out double max, out double range, out string message)
        {
            DataColumn? column = dataGrid[columnName];
            if (column != null)
            {
                min = column.Min();
                max = column.Max();
                range = max - min;
                message = $"{min:F2}-{max:F2}[{range:F2}]";
            }
            else
                throw new ArgumentOutOfRangeException($"Column {columnName} not found in datagrid. Available columns: {string.Join(", ", dataGrid.ColumnHeaders)}");
        }
        public static double Mean(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            return dataGrid!.Columns.Single(c => c.Header == columnName).Mean();
        }
        public static double Variance(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column = dataGrid.Columns.Single(c => c.Header == columnName);
            bool usePopulation = column.Length > 50;
            return column.Variance(usePopulation);
        }

        public static double StandardDeviation(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column = dataGrid.Columns.Single(c => c.Header == columnName);
            bool usePopulation = column.Length > 50;
            return column.StandardDeviation(usePopulation);
        }

        public static double Min(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            return dataGrid.Columns.Single(c => c.Header == columnName).Min();
        }

        public static double Max(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            return dataGrid.Columns.Single(c => c.Header == columnName).Max();
        }

        public static double Sum(this DataGrid dataGrid, string columnName)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid != null && string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid != null && !string.IsNullOrWhiteSpace(columnName)
                                             && dataGrid.Columns.All(c => c.Header != columnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            return dataGrid.Columns.Single(c => c.Header == columnName).Sum();
        }
        public static double Correlation(DataGrid dataGrid1, DataGrid dataGrid2, string columnName1, string columnName2)
        {
            // TODO: Remove column name parameters and maybe assume single column
            if (dataGrid1 == null || dataGrid2 == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid1 != null && string.IsNullOrWhiteSpace(columnName1) && dataGrid1.Columns.Count != 1
                || dataGrid2 != null && string.IsNullOrWhiteSpace(columnName2) && dataGrid2.Columns.Count != 1)
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid1 != null && !string.IsNullOrWhiteSpace(columnName1)
                                               && dataGrid1.Columns.All(c => c.Header != columnName1)
                || dataGrid2 != null && !string.IsNullOrWhiteSpace(columnName2)
                                                  && dataGrid2.Columns.All(c => c.Header != columnName2))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column1 = dataGrid1.Columns.Single(c => string.IsNullOrWhiteSpace(columnName1) || c.Header == columnName1);
            var column2 = dataGrid1.Columns.Single(c => string.IsNullOrWhiteSpace(columnName2) || c.Header == columnName2);
            return column1.Correlation(column2);
        }

        public static double Covariance(DataGrid dataGrid1, DataGrid dataGrid2, string columnName1, string columnName2)
        {
            // TODO: Remove column name parameters and maybe assume single column
            if (dataGrid1 == null || dataGrid2 == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid1 != null && string.IsNullOrWhiteSpace(columnName1) && dataGrid1.Columns.Count != 1
                 || dataGrid2 != null && string.IsNullOrWhiteSpace(columnName2) && dataGrid2.Columns.Count != 1)
                throw new ArgumentException("No column selection is given for the table");
            if (dataGrid1 != null && !string.IsNullOrWhiteSpace(columnName1)
                                              && dataGrid1.Columns.All(c => c.Header != columnName1)
                || dataGrid2 != null && !string.IsNullOrWhiteSpace(columnName2)
                                                 && dataGrid2.Columns.All(c => c.Header != columnName2))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column1 = dataGrid1!.Columns.Single(c => string.IsNullOrWhiteSpace(columnName1) || c.Header == columnName1);
            var column2 = dataGrid1!.Columns.Single(c => string.IsNullOrWhiteSpace(columnName2) || c.Header == columnName2);
            return column1.Covariance(column2);
        }

        public static DataGrid CovarianceMatrix(this DataGrid dataGrid)
        {
            // TODO: Redundant, remove this function alltogether - or update DataGrid interface to implement covariance matrix here instead

            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");

            return dataGrid.CovarianceMatrix();
        }

        public static DataGrid PercentReturn(this DataGrid dataGrid, bool LatestAtTop)
        {
            if (dataGrid == null)
                throw new ArgumentException("Missing Data Table input.");
            if (dataGrid.Columns.Any(c => c.Type != typeof(DateTime)
                                                      && c.TypeName != "Number"
                                                      && c.Type != typeof(string)))
                throw new ArgumentException("Data Table contains invalid rows.");

            DataGrid result = new();
            foreach (DataColumn sourceColumn in dataGrid.Columns)
            {
                var resultColumn = result.AddColumn(sourceColumn.Header);
                if (LatestAtTop)
                {
                    for (int i = 0; i < sourceColumn.Length - 1; i++)
                    {
                        if (sourceColumn.Type == typeof(double))
                            resultColumn.Add((sourceColumn[i] - sourceColumn[i + 1]) / sourceColumn[i + 1]);
                        else
                            resultColumn.Add(sourceColumn[i]); // Ignore non-numerical column
                    }
                }
                else
                {
                    for (int i = sourceColumn.Length - 1; i > 0; i--)
                    {
                        if (sourceColumn.Type == typeof(double))
                            resultColumn.Add((sourceColumn[i] - sourceColumn[i - 1]) / sourceColumn[i - 1]);
                        else
                            resultColumn.Add(sourceColumn[i]); // Ignore non-numerical column
                    }
                }
            }

            return result;
        }
    }
}