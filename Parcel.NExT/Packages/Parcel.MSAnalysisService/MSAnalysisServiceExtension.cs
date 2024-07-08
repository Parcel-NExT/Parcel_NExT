using Microsoft.AnalysisServices.AdomdClient;
using System.Data;
using Parcel.Types;

namespace Parcel.Services
{
    public static class MSAnalysisServiceExtension
    {
        public static DataTable? ExecuteMDXQuery(string dataSource, string mdxQuery)
        {
            using AdomdConnection conn = new(@$"Data Source={dataSource};Initial Catalog=RiskCube;Provider=MSOLAP.5;Integrated Security=SSPI;Format=Tabular");
            conn.Open();

            DataTable? datatable = new AdomdCommand(mdxQuery, conn)
                .ExecuteCellSet()
                .CellSetToTable();
            return datatable;
        }
    }
}
