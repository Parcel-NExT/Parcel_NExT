using System.Data;
using System.Data.Odbc;
using Parcel.Types;

namespace Parcel.Services
{
    public static class ODBCServices
    {
        public static DataTable FetchFromODBCDatabase(string query, string dsn, out string[] headers)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            DataTable dt = new();
            dt.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());
            headers = dt.GetHeaders();
            return dt;
        }
        public static void InsertData(this OdbcConnection connection, string tableName, DataTable table)
        {
            DataColumnCollection columns = table.Columns;
            string[] headers = columns.GetHeaders();

            OdbcTransaction transaction = connection.BeginTransaction();
            foreach (DataRow row in table.Rows)
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = $"INSERT INTO '{tableName}' ({string.Join(',', headers.Select(h => $"\"{h}\""))}) VALUES ({string.Join(',', headers.Select(h => FormatValue(row[h])))})";

                command.ExecuteNonQuery();
            }
            transaction.Commit();

            static string FormatValue(object value)
            {
                string? text = value.ToString();
                if (double.TryParse(text, out _))
                    return text;
                else return $"\"{text}\""; // TODO: You sure it's double quote not single quote?
            }
        }
        public static DataTable ExecuteODBC(string sqlQuery, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            try
            {
                if (sqlQuery.ToLower().Trim().StartsWith("select"))
                {
                    DataSet result = new();
                    string formattedQuery = sqlQuery.EndsWith(';') ? sqlQuery : sqlQuery + ';';
                    using var adapter = new OdbcDataAdapter(formattedQuery, odbcConnection);

                    adapter.Fill(result);
                    return result.Tables[0];
                }
                else
                {
                    using var command = new OdbcCommand(sqlQuery, odbcConnection);
                    command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message.Replace(Environment.NewLine, " "));
            }
            finally
            {
                odbcConnection.Close();
            }
        }
    }
}
