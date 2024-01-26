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

        #region Methods
        public static Number Add(Number A, Number B)
            => new Number(A.Value + B.Value);
        #endregion
    }
}
