using Parcel.CoreEngine.Layouts;

namespace Parcel.CoreEngine.Document
{
    public sealed class ParcelGraph
    {
        #region Constructors
        public ParcelGraph() 
        {
            MainLayout = new ();
            Layouts = [MainLayout];
        }
        #endregion

        #region Properties
        public List<CanvasLayout> Layouts { get; set; }
        #endregion

        #region Layout Properties
        public CanvasLayout MainLayout { get; set; }
        #endregion
    }
}
