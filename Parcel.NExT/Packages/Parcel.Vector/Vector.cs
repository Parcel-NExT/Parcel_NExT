namespace Parcel.Types
{
    public static class VectorHelper
    {
        #region Generation
        public static double[] Range(int count = 100, double start = 0, double increment = 1)
            => Enumerable.Range(0, count).Select(x => start + x * increment).ToArray();
        #endregion

        #region Collection
        public static double[] Shift(this double[] vector, int offset, bool fillZero)
        {
            // TODO: Boundary check
            if (offset > 0) // Shift right
                return vector.Skip(offset).Concat(fillZero ? new double[offset] : vector.Take(offset)).ToArray();
            else // Shift left
                return vector.Skip(vector.Length - 1).Concat(fillZero ? new double[offset] : vector.Take(vector.Length - 1)).ToArray();
        }
        #endregion

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

        #region Serialization
        public static void SaveToFile(this double[] vector, string filePath)
            => File.WriteAllLines(filePath, vector.Select(v => v.ToString()));
        public static double[] ReadFromFile(string filePath)
            => File.ReadAllLines(filePath).Select(double.Parse).ToArray();
        #endregion

        #region Operations
        public enum AccumulateApplyFunction
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }
        /// <summary>
        /// Applies certain operation starting from base value on top of entire vector and return the resulting vector and final value
        /// </summary>
        public static double AccumulateApply(double baseValue, double[] vector, out double[] path, AccumulateApplyFunction function)
        {
            switch (function)
            {
                case AccumulateApplyFunction.Add:
                    return AccumulateApply(baseValue, vector, out path, (previous, currentElement) => previous + currentElement);
                case AccumulateApplyFunction.Subtract:
                    return AccumulateApply(baseValue, vector, out path, (previous, currentElement) => previous - currentElement);
                case AccumulateApplyFunction.Multiply:
                    return AccumulateApply(baseValue, vector, out path, (previous, currentElement) => previous * currentElement);
                case AccumulateApplyFunction.Divide:
                    return AccumulateApply(baseValue, vector, out path, (previous, currentElement) => previous / currentElement);
                default:
                    throw new ArgumentException($"Invalid function: {function}");
            }
        }
        /// <param name="function">Given previous computed result and current element value, compute the next value</param>
        public static double AccumulateApply(double baseValue, double[] vector, out double[] path, Func<double, double, double> function)
        {
            double finalValue = baseValue;
            path = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                double newValue = function(finalValue, vector[i]);
                path[i] = newValue;
            }
            return finalValue;
        }
        #endregion
    }
}
