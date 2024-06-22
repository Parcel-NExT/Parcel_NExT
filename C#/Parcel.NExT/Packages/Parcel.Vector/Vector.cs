namespace Parcel.Types
{
    public static class VectorHelper
    {
        public static double[] Range(int count = 100, double start = 0, double increment = 1)
        {
            return Enumerable.Range(0, count).Select(x => start + x * increment).ToArray();
        }

        #region Math
        public static double[] Subtract(this double[] vector, double other)
            => [.. vector.Select(v => v - other)];
        public static double[] Add(this double[] vector, double other)
            => [.. vector.Select(v => v + other)];
        public static double[] Divide(this double[] vector, double other)
            => [.. vector.Select(v => v / other)];
        public static double[] Multiply(this double[] vector, double other)
            => [.. vector.Select(v => v * other)];

        public static double[] Sine(this double[] vector)
            => [.. vector.Select(Math.Sin)];
        public static double[] Cosine(this double[] vector)
            => [.. vector.Select(Math.Cos)];
        public static double[] SqaureRoot(this double[] vector)
            => [.. vector.Select(Math.Sqrt)];
        public static double[] AbsoluteValue(this double[] vector)
            => [.. vector.Select(Math.Abs)];
        public static double[] Arccosine(this double[] vector)
            => [.. vector.Select(Math.Acos)];
        public static double[] Arcsine(this double[] vector)
            => [.. vector.Select(Math.Asin)];
        public static double[] Log10(this double[] vector)
            => [.. vector.Select(Math.Log10)];
        public static double[] Tangent(this double[] vector)
            => [.. vector.Select(Math.Tan)];
        public static double[] Modulus(this double[] vector, double divisor)
            => [.. vector.Select(v => v % divisor)];
        public static double[] Log2(this double[] vector)
            => [.. vector.Select(Math.Log2)];
        public static double[] Power(this double[] vector, double exponent)
            => [.. vector.Select(v => Math.Pow(v, exponent))];

        public static double Minimum(this double[] vector)
            => vector.Min();
        public static double Maximum(this double[] vector)
            => vector.Max();
        public static double Mean(this double[] vector)
            => vector.Mean();
        public static double StandardDeviation(this double[] vector)
            => vector.StandardDeviation();
        public static double Variance(this double[] vector)
            => vector.Variance();
        #endregion
    }
}
