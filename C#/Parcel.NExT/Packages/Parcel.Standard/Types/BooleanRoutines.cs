namespace Parcel.Standard.Types
{
    /// <summary>
    /// Exposes binary operations
    /// </summary>
    /// <remarks>
    /// Should also expose everything under System.Boolean
    /// </remarks>
    public static class BooleanRoutines
    {
        #region Binary Operations
        /// <alias>&gt; (Bigger Than)</alias>
        public static bool BiggerThan(double a, double b) => a > b;
        public static bool BiggerThanOrEqual(double a, double b) => a >= b;
        /// <alias>&lt; (Smaller Than)</alias>
        public static bool SmallerThan(double a, double b) => a > b;
        public static bool SmallerThanOrEqual(double a, double b) => a <= b;
        /// <alias>&&</alias>
        public static bool And(bool left, bool right) => left && right;
        /// <alias>Or</alias>
        public static bool Or(bool left, bool right) => left || right;
        /// <alias>Logical Exclusive Or</alias>
        public static bool Xor(bool left, bool right) => left ^ right;
        #endregion
    }
}
