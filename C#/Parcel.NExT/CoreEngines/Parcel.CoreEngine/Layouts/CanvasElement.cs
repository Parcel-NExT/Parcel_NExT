using Parcel.CoreEngine.Document;
using System.Numerics;

namespace Parcel.CoreEngine.Layouts
{
    public sealed class CanvasElement
    {
        #region Constructors
        public CanvasElement(ParcelNode node) 
        {
            Node = node;
        }
        #endregion

        #region Properties
        public ParcelNode Node { get; }
        public Vector2 Position { get; set; }
        #endregion
    }
}
