namespace Parcel.CoreEngine.Primitives
{
    public sealed class Number
    {
        #region Construction
        public Number(double value)
        {
            Value = value;
        }
        #endregion
        
        #region Properties
        public double Value { get; private set; }
        #endregion

        #region Methods (Object)
        public static Number Add(Number a, Number b)
            => new(a.Value + b.Value);
        #endregion

        #region Methods (Primitives)
        public static double Add(params double[] arguments)
            => arguments.Sum();
        #endregion
    }
}
