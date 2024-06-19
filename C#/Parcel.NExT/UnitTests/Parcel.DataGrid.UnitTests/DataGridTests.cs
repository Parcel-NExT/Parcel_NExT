namespace Parcel.Types
{
    public class Tests
    {
        #region Test Data
        readonly DataGrid TestDataGrid = new();
        #endregion

        #region Construction
        public Tests()
        {
            TestDataGrid.AddColumn("Col1");
            TestDataGrid.AddColumn("Col2");
            TestDataGrid.AddColumn("Col3");
            TestDataGrid.AddColumn("Col4");
            TestDataGrid.AddRow(1, 2.0, "String", true);
            TestDataGrid.AddRow(1, 2.0, "String", false);
            TestDataGrid.AddRow(1, 2.0, "String", true);
            TestDataGrid.AddRow(1, 2.0, "String", false);
            TestDataGrid.AddRow(1, 2.0, "String", true);
        }
        #endregion

        #region Data Grid Storage
        /// <summary>
        /// DataGrid types should be "hidden" as programming model goes, but its internal types should be consistent.
        /// </summary>
        [Fact]
        public void DataGridConsistentTypesRequirement()
        {
            Assert.Throws<ArgumentException>(() => TestDataGrid.AddRow("String Value", 0.2));
        }

        [Fact]
        public void DataGridCorrectColumnTypes()
        {
            Assert.Equal(nameof(Int32), TestDataGrid.Columns[0].TypeName);
            Assert.Equal(nameof(Double), TestDataGrid.Columns[1].TypeName);
            Assert.Equal(nameof(String), TestDataGrid.Columns[2].TypeName);
            Assert.Equal(nameof(Boolean), TestDataGrid.Columns[3].TypeName);
        }
        #endregion

        #region Data Grid Operations
        [Fact]
        public void DataGridMeanOperation()
        {
            Assert.Equal(2.0, TestDataGrid.Columns[1].Mean());
        }
        #endregion
    }
}