namespace Parcel.Neo.Base.DataTypes
{
    /// <summary>
    /// Represents a 2D coordinate; Period.
    /// Specifically it's used for representing node locations (from PDS perspective).
    /// </summary>
    public struct Vector2D(double x, double y)
    {
        #region Properties
        public double X = x;
        public double Y = y;
        #endregion

        #region Operator Overloads
        public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
        #endregion
    }
}
