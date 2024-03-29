using Parcel.CoreEngine.Conversion;
using Parcel.CoreEngine.SemanticTypes;
using Parcel.NExT.Interpreter.Scripting;
using System.Text;

namespace StandardLibrary.ParcelCore
{
    public static class Processing
    {
        public static DataGrid? ProcessRows(DataGrid? input, Text script)
        {
            if (input == null) return null;
            if (string.IsNullOrEmpty(input.Raw) || string.IsNullOrEmpty(script.Value))
                return input;

            string[] headers = input.Headers;
            string[][] rows = input.QuickSplit(true).ToArray();

            StringBuilder result = new();
            result.AppendLine(string.Join(",", headers.Where(h => !string.IsNullOrEmpty(h))));

            foreach (string[] row in rows)
            {
                Dictionary<string, object> initialValues = ConvertRow(headers, row);
                Dictionary<string, object> results = ContextFreeRoslyn.EvaluateLocalSnippet(initialValues, script.Value);
                result.AppendLine(string.Join(",", headers.Where(h => !string.IsNullOrEmpty(h)).Select(h => results[h].ToString())));
            }

            return new DataGrid(result.ToString().TrimEnd());

            static Dictionary<string, object> ConvertRow(string[] names, string[] row)
            {
                Dictionary<string, object> values = [];
                for (int i = 0; i < row.Length; i++)
                {
                    string name = names[i];
                    string value = row[i];
                    if (!string.IsNullOrEmpty(name) && !char.IsNumber(name[0]))
                        values.Add(name, StringTypeConverter.ConvertObjectBestGuess(value));
                }
                return values;
            }
        }
    }
}
