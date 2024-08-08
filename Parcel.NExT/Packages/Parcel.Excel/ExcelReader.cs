using ExcelDataReader;
using Parcel.Integration;
using Parcel.Types;
using System.Data;

namespace Parcel.FileFormats.Excel
{
    /// <summary>
    /// Reading excel files.
    /// </summary>
    /// <remarks>
    /// We have dedicated reader/writer because implementation may vary.
    /// </remarks>
    public static class ExcelReader
    {
        public static DataGrid LoadFromExcelWorksheet(string path, string? worksheet = null)
        {
            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
            using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            string csv = string.Empty;
            if (string.IsNullOrWhiteSpace(worksheet))
                csv = result.Tables[0].ToCSV();
            else
                csv = result.Tables[worksheet].ToCSV();

            return DataGridIntegration.InitializeDataGridFromCsvText(worksheet ?? Path.GetFileNameWithoutExtension(path), csv);
        }
    }
}
