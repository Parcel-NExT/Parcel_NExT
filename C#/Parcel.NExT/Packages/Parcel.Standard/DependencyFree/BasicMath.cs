namespace Parcel.Standard.DependencyFree
{
    public static class BasicMath
    {
        public static double Add(double a, double b)
            => a + b;
        public static double Subtract(double a, double b)
            => a - b;
        public static double Multiply(double a, double b)
            => a * b;
        public static double Divide(double a, double b)
            => a / b;
        public static double Power(double @base, double exponent)
            => Math.Pow(@base, exponent);
    }
}
