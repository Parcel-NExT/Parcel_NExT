using System.Data;
using System.Reflection;
using System.Text;

namespace Parcel.Types
{
    #region Object-Oriented Database Table Extension Base
    public abstract class TableEntryBase
    {
    }
    public class GenericStronglyTypedTable<TType> where TType : TableEntryBase
    {
        public GenericStronglyTypedTable(IEnumerable<TType> rows) 
            => Rows = rows.ToList();
        public List<TType> Rows { get; }
    }
    #endregion

    #region Utility Functions
    public static class DataColumnExtensions
    {
        #region Helpers
        public static IEnumerable<object> ExtractValues(this DataColumn column)
        {
            // Remark: DataColumn in DataTable is like a useless construct - it doesn't contain data; We have to go through DataRows.
            DataTable? originalTable = column.Table;
            foreach (DataRow row in originalTable.Rows)
                yield return row[column];
        }
        public static string[] GetHeaders(this DataColumnCollection dataColumnCollection)
            => Enumerable.Range(0, dataColumnCollection.Count).Select(i => dataColumnCollection[i].ColumnName).ToArray();
        #endregion
    }
    public static class DataTableExtensions
    {
        #region Display
        public static string ToCSV(this DataTable dataTable)
        {
            StringBuilder output = new();
            output.AppendLine(string.Join(",", Enumerable.Range(0, dataTable.Columns.Count).Select(i => dataTable.Columns[i].ColumnName.Replace(" ", string.Empty))));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> items = row.ItemArray.Select(item => $"\"{item.ToString().Replace("\"", "\"\"")}\"");
                string csvLine = string.Join(",", items);
                output.AppendLine(csvLine);
            }
            return output.ToString();
        }
        public static string ToCSVFull(this DataTable dataTable)
        {
            StringBuilder sb = new();

            IEnumerable<string> columnNames = dataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                {
                    string value = field.ToString();
                    string escapeQuotes = value.Contains('"')
                        ? string.Concat("\"", value.Replace("\"", "\"\""), "\"")
                        : value;
                    string addQuotes = escapeQuotes.Contains(',') ? $"\"{escapeQuotes}\"" : escapeQuotes;
                    return addQuotes;
                });
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
        #endregion

        #region Queries
        public static string[] GetHeaders(this DataTable dataTable)
            => Enumerable.Range(0, dataTable.Columns.Count).Select(i => dataTable.Columns[i].ColumnName).ToArray();
        public static DataRow Pick(this DataTable table, Func<DataRow, bool> condition)
            => table.AsEnumerable().Where(condition).First();
        public static IEnumerable<TType> Select<TType>(this DataRowCollection rows, Func<DataRow, TType> action)
        {
            List<TType> result = [];
            foreach (DataRow item in rows)
                result.Add(action(item));
            return result;
        }
        public static bool Empty(this DataTable table)
            => table.Rows.Count == 0;
        #endregion

        #region Object-Oriented Wrapper
        /// <summary>
        /// Retrieve a list of objects for target type using reflection from the data table;
        /// Type must have public properties; Property names are case insensitive
        /// </summary>
        public static GenericStronglyTypedTable<Type> Unwrap<Type>(this DataTable table, Dictionary<string, string> columnRenamming = null) where Type : TableEntryBase, new()
        {
            if (table.Rows.Count == 0) return new GenericStronglyTypedTable<Type>(Array.Empty<Type>());

            // Get column name mapping
            if (columnRenamming == null)
            {
                columnRenamming = [];
                foreach (string column in Enumerable.Range(0, table.Columns.Count).Select(i => table.Columns[i].ColumnName))
                    columnRenamming.Add(column.Replace(" ", string.Empty), column);   // Case-sensitive; Do notice though it cannot contain spaces
            }

            // Initialize objects from rows
            List<Type> returnValues = [];
            foreach (DataRow row in table.Rows)
            {
                // Construct instance of row type
                Type instance = new();

                // Initialize properties of the instance
                foreach (PropertyInfo prop in typeof(Type).GetProperties())
                {
                    string propertyName = prop.Name;
                    if (columnRenamming.ContainsKey(propertyName))
                        prop.SetValue(instance, row[columnRenamming[propertyName]] == DBNull.Value
                            ? null
                            : Convert.ChangeType(row[columnRenamming[propertyName]], prop.PropertyType));
                }

                returnValues.Add(instance);
            }

            return new GenericStronglyTypedTable<Type>(returnValues);
        }
        /// <summary>
        /// Get a single value of given type from the single cell of the read result;
        /// Throw exception when invalid
        /// </summary>
        public static TType? Single<TType>(this DataTable table) /*where type : IConvertible - allow nullable*/
        {
            // If there is no row and type is not nullable then throw an exception otherwise return null
            bool canBeNull = !typeof(TType).IsValueType
                || Nullable.GetUnderlyingType(typeof(TType)) != null;
            if (table.Rows.Count == 0)
            {
                if (canBeNull)
                    return default;
                else throw new ArgumentException("Table contains no value.");
            }
            return ChangeType<TType>(table.Rows[0][0]);
        }
        /// <summary>
        /// Extract a list of single type of values from a data table
        /// </summary>
        public static List<TType>? List<TType>(this DataTable table, int columnIndex = 0) where TType : IConvertible
        {
            // If there is no row then return null
            if (table.Rows.Count == 0) return null;

            // Read all the rows and get the first element of each row
            List<TType> list = [];
            foreach (DataRow row in table.Rows)
                list.Add(ChangeType<TType>(row[columnIndex]));

            return list;
        }
        private static T? ChangeType<T>(object value)
        {
            // Get type
            Type t = typeof(T);

            // Determin nullability
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // Return null for nullable
                if (value == null)
                    return default;

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }
        #endregion
    }
    #endregion
}
