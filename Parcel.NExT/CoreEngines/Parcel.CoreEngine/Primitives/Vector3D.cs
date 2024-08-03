namespace Parcel.Neo.Base.DataTypes
{
    /// <summary>
    /// Represents a 3D coordinate; Period.
    /// Specifically it's used for representing vectors in 3D geometric space.
    /// </summary>
    public struct Vector3D(double x, double y, double z)
    {
        #region Properties
        public double X = x;
        public double Y = y;
        public double Z = z;
        #endregion

        #region Operator Overloads
        public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        #endregion
    }

    /// <summary>
    /// Represents a 3D extension.
    /// Specifically it's used for representing vectors in 3D geometric space (with a semantic twist.
    /// </summary>
    public struct Extent3D(double width, double height, double depth)
    {
        #region Properties
        /// <summary>
        /// X
        /// </summary>
        public double Width = width;
        /// <summary>
        /// Y
        /// </summary>
        public double Height = height;
        /// <summary>
        /// Z
        /// </summary>
        public double Depth = depth;
        #endregion

        #region Operator Overloads
        public static Extent3D operator +(Extent3D a, Extent3D b) => new(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth);
        public static Extent3D operator -(Extent3D a, Extent3D b) => new(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth);
        #endregion
    }
}
