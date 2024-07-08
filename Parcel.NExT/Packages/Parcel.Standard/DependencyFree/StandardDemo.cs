// Define a whole suite of standard demo-use nodes, categorized in this case using classes.
namespace Parcel.Standard.DependencyFree
{
    /// <summary>
    /// Primitives from texts
    /// </summary>
    public static class Primitives
    {
        #region The most fundamental type
        public static double Number(string value)
            => double.Parse(value);
        public static bool Bool(string value)
            => bool.Parse(value);
        public static string String(string value)
            => value;
        #endregion

        #region Arrays of Fundamental Types
        public static string[] StringArray(string value)
        {
            // Without [] brackets
            return value.Split(',').Select(v => v.Trim()).ToArray();
        }
        public static double[] NumberArray(string value)
        {
            // Without [] brackets
            return value.Split(',').Select(v => v.Trim()).Select(double.Parse).ToArray();
        }
        public static bool[] BoolArray(string value)
        {
            // Without [] brackets
            return value.Split(',').Select(v => v.Trim()).Select(bool.Parse).ToArray();
        }
        #endregion

        #region Make Array
        public static string[] MakeStringArray(params string[] values)
            => values;
        public static double[] MakeNumberArray(params double[] values)
            => values;
        public static bool[] MakeBoolArray(params bool[] values)
            => values;
        #endregion
    }

    public static class SystemFunctions
    {
        public static void PrintLine(string message)
            => Console.WriteLine(message); // TODO: How do we implement this with the front-end? A possible approach is to utilize seperate front-end service.
    }

    /// <summary>
    /// Functional operators on basic types;
    /// In general, front-ends and back-ends should provide more direct in-place and native evaluations for optimization purpose instead of relying on such functional operators
    /// </summary>
    public static class Operators
    {
        #region Number Operators
        public static double Add(double a, double b)
            => a + b;
        public static double Subtract(double a, double b)
            => a - b;
        public static double Multiply(double a, double b)
            => a * b;
        public static double Divide(double a, double b)
            => a / b;
        public static double Modulo(double a, double b)
            => a % b;
        public static double Power(double a, double b)
            => Math.Pow(a, b);
        #endregion

        #region Boolean Operators
        public static bool And(bool a, bool b)
            => a && b;
        public static bool Or(bool a, bool b)
            => a || b;
        public static bool Not(bool a)
            => !a;
        public static bool Xor(bool a, bool b)
            => a ^ b;
        #endregion

        #region String Operators
        public static string Concat(string a, string b)
            => a + b;
        public static string Substring(string a, int start, double length)
            => a.Substring(start, (int)length);
        public static string Replace(string a, string old, string newStr)
            => a.Replace(old, newStr);
        public static string ToUpper(string a)
            => a.ToUpper();
        public static string ToLower(string a)
            => a.ToLower();
        public static string Format(string format, string[] args)
            => string.Format(format, args);
        #endregion
    }
}
