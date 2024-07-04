namespace Parcel.Standard.Types
{
    /// <summary>
    /// Exposes double-point value type, provides all functions related to scalar numerical math.
    /// Includes basic C# operations plus everything from System.Math
    /// </summary>
    /// <remarks>
    /// For culture reasons, we are not simply naming arguments as a, b, or number1, number 2, etc.
    /// </remarks>
    public static class NumberRoutines
    {
        #region Basic Operations
        public static double Add(double summand1, double summand2)
            => summand1 + summand2;
        public static double Subtract(double minuend, double subtrahend)
            => minuend - subtrahend;
        public static double Multiply(double factor1, double factor2)
            => factor1 * factor2;
        public static double Divide(double dividend, double divisor)
        {
            if (divisor == 0)
                throw new ArgumentException("Second input cannot be zero.");

            return dividend / divisor;
        }
        public static double Modulus(double dividend, double divisor)
            => dividend % divisor;
        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        public static double Power(double @base, double exponent)
            => Math.Pow(@base, exponent);
        public static double Root(double radicand, double degree)
            => Math.Pow(radicand, 1.0 / degree); // Mathematically equivalent
        #endregion

        #region System.Math
        /// <summary>
        /// Returns the absolute value of a double-precision floating-point number.
        /// </summary>
        public static double AbsoluteValue(double number) => Math.Abs(number);
        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        public static double Acos(double number) => Math.Acos(number);
        /// <summary>
        /// Returns the angle whose hyperbolic cosine is the specified number.
        /// </summary>
        public static double Acosh(double number) => Math.Acosh(number);
        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        public static double Asin(double number) => Math.Asin(number);
        /// <summary>
        /// Returns the angle whose hyperbolic sine is the specified number.
        /// </summary>
        public static double Asinh(double number) => Math.Asinh(number);
        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        public static double Atan(double number) => Math.Atan(number);
        /// <summary>
        /// Returns the angle whose tangent is the quotient of two specified numbers.
        /// </summary>
        public static double Atan2(double y, double x) => Math.Atan2(y, x);
        /// <summary>
        /// Returns the angle whose hyperbolic tangent is the specified number.
        /// </summary>
        public static double Atanh(double number) => Math.Atanh(number);
        /// <summary>
        /// Returns the largest value that compares less than a specified value.
        /// </summary>
        public static double BitDecrement(double number) => Math.BitDecrement(number);
        /// <summary>
        /// Returns the smallest value that compares greater than a specified value.
        /// </summary>
        public static double BitIncrement(double number) => Math.BitIncrement(number);
        /// <summary>
        /// Returns the cube root of a specified number.
        /// </summary>
        public static double CubeRoot(double number) => Math.Cbrt(number);
        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified double-precision floating-point number.
        /// </summary>
        public static double Ceiling(double number) => Math.Ceiling(number);
        /// <summary>
        /// Returns value clamped to the inclusive range of min and max.
        /// </summary>
        public static double Clamp(double number, double min, double max) => Math.Clamp(number, min, max);
        /// <summary>
        /// Returns a value with the magnitude of x and the sign of y.
        /// </summary>
        public static double CopySign(double number1, double number2) => Math.CopySign(number1, number2);
        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        public static double Cos(double number) => Math.Cos(number);
        /// <summary>
        /// Returns the hyperbolic cosine of the specified angle.
        /// </summary>
        public static double Cosh(double number) => Math.Cosh(number);
        /// <summary>
        /// Returns e raised to the specified power.
        /// </summary>
        public static double Exp(double number) => Math.Exp(number);
        /// <summary>
        /// Returns the largest integral value less than or equal to the specified double-precision floating-point number.
        /// </summary>
        public static double Floor(double number) => Math.Floor(number);
        /// <summary>
        /// Returns (x * y) + z, rounded as one ternary operation.
        /// </summary>
        public static double FusedMultiplyAdd(double x, double y, double z) => Math.FusedMultiplyAdd(x, y, z);
        /// <summary>
        /// Returns the remainder resulting from the division of a specified number by another specified number.
        /// </summary>
        public static double IEEERemainder(double x, double y) => Math.IEEERemainder(x, y);
        /// <summary>
        /// Returns the base 2 integer logarithm of a specified number.
        /// </summary>
        public static double ILogB(double number) => Math.ILogB(number);
        /// <summary>
        /// Returns the natural (base e) logarithm of a specified number.
        /// </summary>
        public static double Log(double number) => Math.Log(number);
        /// <summary>
        /// Returns the logarithm of a specified number in a specified base.
        /// </summary>
        public static double Log(double antilogarithm, double @base) => Math.Log(antilogarithm, @base);
        /// <summary>
        /// Returns the base 10 logarithm of a specified number.
        /// </summary>
        public static double Log10(double number) => Math.Log10(number);
        /// <summary>
        /// Returns the base 2 logarithm of a specified number.
        /// </summary>
        public static double Log2(double number) => Math.Log2(number);
        /// <summary>
        /// Returns the larger of two double-precision floating-point numbers.
        /// </summary>
        public static double Max(double a, double b) => Math.Max(a, b);
        /// <summary>
        /// Returns the larger magnitude of two double-precision floating-point numbers.
        /// </summary>
        public static double MaxMagnitude(double a, double b) => Math.MaxMagnitude(a, b);
        /// <summary>
        /// Returns the smaller of two double-precision floating-point numbers.
        /// </summary>
        public static double Min(double a, double b) => Math.Min(a, b);
        /// <summary>
        /// Returns the smaller magnitude of two double-precision floating-point numbers.
        /// </summary>
        public static double MinMagnitude(double a, double b) => Math.MinMagnitude(a, b);
        /// <summary>
        /// Returns an estimate of the reciprocal of a specified number.
        /// </summary>
        public static double ReciprocalEstimate(double d) => Math.ReciprocalEstimate(d);
        /// <summary>
        /// Returns an estimate of the reciprocal square root of a specified number.
        /// </summary>
        public static double ReciprocalSqrtEstimate(double d) => Math.ReciprocalSqrtEstimate(d);
        /// <summary>
        ///Rounds a double-precision floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
        /// </summary>
        public static double Round(double number) => Math.Round(number);
        /// <summary>
        ///Rounds a double-precision floating-point value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.
        /// </summary>
        public static double Round(double number, int digits) => Math.Round(number, digits);
        /// <summary>
        ///Rounds a double-precision floating-point value to a specified number of fractional digits using the specified rounding convention.
        /// </summary>
        public static double Round(double number, int digits, MidpointRounding mode) => Math.Round(number, digits, mode);
        /// <summary>
        ///Rounds a double-precision floating-point value to an integer using the specified rounding convention.
        /// </summary>
        public static double Round(double number, MidpointRounding mode) => Math.Round(number, mode);
        /// <summary>
        /// Returns x * 2^n computed efficiently.
        /// </summary>
        public static double ScaleB(double number, int n) => Math.ScaleB(number, n);
        /// <summary>
        /// Returns an integer that indicates the sign of a double-precision floating-point number.
        /// </summary>
        public static double Sign(double number) => Math.Sign(number);
        /// <summary>
        /// Returns the sine of the specified angle.
        /// </summary>
        public static double Sin(double angleRadians) => Math.Sin(angleRadians);
        /// <summary>
        /// Returns the sine of the specified angle.
        /// </summary>
        public static double SinDegrees(double angleDegrees) => Math.Sin(angleDegrees / 180.0 * Math.PI);
        /// <summary>
        /// Returns the sine and cosine of the specified angle.
        /// </summary>
        public static (double, double) SinCos(double angle) => Math.SinCos(angle);
        /// <summary>
        /// Returns the hyperbolic sine of the specified angle.
        /// </summary>
        public static double Sinh(double angle) => Math.Sinh(angle);
        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        public static double SquareRoot(double number) => Math.Sqrt(number);
        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        public static double Tan(double number) => Math.Tan(number);
        /// <summary>
        /// Returns the hyperbolic tangent of the specified angle.
        /// </summary>
        public static double Tanh(double number) => Math.Tanh(number);
        /// <summary>
        ///Calculates the integral part of a specified double-precision floating-point number.
        /// </summary>
        public static double Truncate(double number) => Math.Truncate(number);

        #endregion
    }
}
