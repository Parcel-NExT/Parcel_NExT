using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcel.Neo.Base.DataTypes
{
    [Serializable]
    public struct Bound
    {
        public Vector2D Location;
        public Vector2D Size;
        public double X => Location.X;
        public double Y => Location.Y;
        public double Width => Size.X;
        public double Height => Size.Y;

        public Bound(double x, double y, double width, double height) : this(new Vector2D(x, y), new Vector2D(width, height))
        {
        }

        public Bound(Vector2D start, Vector2D dimensions)
        {
            Location = start;
            Size = dimensions;
        }
    }
}
