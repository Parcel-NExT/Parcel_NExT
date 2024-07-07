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
}
