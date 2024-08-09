using MathNet.Numerics.Statistics;
using System.Collections;

namespace Parcel.Math.Types
{
    /// <remarks>
    /// Design: Syntax elegancy is more important than performance.
    /// This provides base container and operations.
    /// Data type is immutable.
    /// Tightly tied to double[].
    /// </remarks>
    public partial class Vector : IEnumerable<double>, IEnumerable
    {
        #region Properties
        private double[] Values { get; }
        /// <summary>
        /// Get raw data.
        /// </summary>
        public double[] Raw => Values;
        #endregion

        #region Generation
        public static Vector Range(int count = 100, double start = 0, double increment = 1)
            => Enumerable.Range(0, count).Select(x => start + x * increment).ToArray();
        #endregion

        #region Construction
        /// <summary>
        /// Construct empty.
        /// </summary>
        public Vector() 
            => Values = [];
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector(IEnumerable<double> values)
            => Values = values.ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector(IEnumerable<int> values)
            => Values = values.Select(v => (double)v).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector(IEnumerable<bool> values)
            => Values = values.Select(v => v ? 1.0 : 0.0).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector(IEnumerable<float> values)
            => Values = values.Select(v => (double)v).ToArray();
        /// <summary>
        /// Construct from (copy of) values.
        /// </summary>
        public Vector(IEnumerable<string> values)
            => Values = values.Select(v => double.Parse(v)).ToArray();
        /// <summary>
        /// Construct from param arguments.
        /// </summary>
        public Vector(params double[] values)
            => Values = values;
        /// <summary>
        /// Construct from string, either comma delimited or space delimited.
        /// </summary>
        public Vector(string values)
            => Values = values.Contains(',')
                ? values.Split(',', StringSplitOptions.TrimEntries).Select(double.Parse).ToArray()
                : values.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();

        /// <summary>
        /// Implicit constructor
        /// </summary>
        public static implicit operator Vector(double[] values) => new(values);
        #endregion

        #region Data Loading and Saving
        /// <summary>
        /// Load values from CSV or plain text file
        /// </summary>
        public static Vector LoadFromCSV(string path, bool containsHeaderRow, int takeNthCSVColumn = 0)
        {
            if (!File.Exists(path))
                Console.WriteLine($"File {path} doesn't exist.");

            return new Vector(File.ReadLines(path)
                .Skip(containsHeaderRow ? 1 : 0)
                .Select(line => line.Split(',')[takeNthCSVColumn])
                .Select(double.Parse));
        }
        public void SaveToCSVFile(string filepath, bool includeHeader = false)
        {
            if (includeHeader)
                File.WriteAllLines(filepath, ["Values", .. Values.Select(v => v.ToString())]);
            else
                File.WriteAllLines(filepath, Values.Select(v => v.ToString()));
        }
        #endregion

        #region Query
        /// <summary>
        /// Get length of vector
        /// </summary>
        public int Length => Values.Length;
        /// <summary>
        /// Another name for length
        /// </summary>
        public int Norm() => Length;
        /// <summary>
        /// Get string representation of size.
        /// </summary>
        public string Size => $"Vector: {Values.Length} elements";
        /// <summary>
        /// ToString override
        /// </summary>
        public override string ToString() => $"[{string.Join(", ", Values)}]";
        #endregion

        #region Operator Operations
        /// <summary>
        /// Identity (no copy is made)
        /// </summary>
        public static Vector operator +(Vector a) 
            => a;
        /// <summary>
        /// Gets negative
        /// </summary>
        public static Vector operator -(Vector a) 
            => new(a.Values.Select(v => -v));
        /// <summary>
        /// Adds two vectors
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a + b));
        }
        /// <summary>
        /// Adds a scalar to every element
        /// </summary>
        public static Vector operator +(Vector a, double v)
            => new Vector(a.Select(a => a + v));
        /// <summary>
        /// Subtract two vectors
        /// </summary>
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a - b));
        }
        /// <summary>
        /// Subtract a scalar to every element
        /// </summary>
        public static Vector operator -(Vector a, double v)
            => new Vector(a.Select(a => a - v));
        /// <summary>
        /// Multiply element-wise
        /// </summary>
        public static Vector operator *(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => a * b));
        }
        /// <summary>
        /// Multiply every element by a scalar
        /// </summary>
        public static Vector operator *(Vector a, double v)
            => new Vector(a.Select(a => a * v));
        /// <summary>
        /// Divide element-wise
        /// </summary>
        public static Vector operator /(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector size doesn't match");
            return new Vector(a.Zip(b, (a, b) => b == 0 ? throw new DivideByZeroException() : a / b));
        }
        /// <summary>
        /// Multiply every element by a scalar
        /// </summary>
        public static Vector operator /(Vector a, double v)
        {
            if (v == 0)
                throw new DivideByZeroException();
            return new Vector(a.Select(a => a / v));
        }
        /// <summary>
        /// Exponent element-wise
        /// </summary>
        public static Vector operator ^(Vector a, double v)
            => new Vector(a.Select(a => System.Math.Pow(a, v)));
        /// <summary>
        /// Append an element
        /// </summary>
        public static Vector operator |(Vector a, double v)
            => new Vector(a.Append(v));
        /// <summary>
        /// Append an entire vector
        /// </summary>
        public static Vector operator |(Vector a, Vector b)
            => new Vector(a.Concat(b));
        #endregion

        #region Math
        public Vector Subtract(double other)
            => this - other;
        public Vector Add(double other)
            => this + other;
        public Vector Divide(double other = 1)
            => this / other;
        public Vector Multiply(double other)
            => this * other;
        #endregion

        #region Basic Numerical Operations
        public Vector SqaureRoot()
            => new(Values.Select(System.Math.Sqrt));
        public Vector AbsoluteValue()
            => new(Values.Select(System.Math.Abs));
        public Vector Arccosine()
            => new(Values.Select(System.Math.Acos));
        public Vector Arcsine()
            => new(Values.Select(System.Math.Asin));
        public Vector Log10()
            => new(Values.Select(System.Math.Log10));
        public Vector Tangent()
            => new(Values.Select(System.Math.Tan));
        public Vector Modulus(double divisor)
            => new(Values.Select(v => v % divisor));
        public Vector Log2()
            => new(Values.Select(System.Math.Log2));
        /// <summary>
        /// Apply element-wise arbitrary function
        /// </summary>
        public Vector Apply(Func<double, double> fun)
            => new(Values.Select(fun));
        /// <summary>
        /// Compute cos element-wise
        /// </summary>
        public Vector Cosine()
            => new(Values.Select(System.Math.Cos));
        /// <summary>
        /// Compute cosh element-wise
        /// </summary>
        public Vector Cosh()
            => new(Values.Select(System.Math.Cosh));
        /// <summary>
        /// Compute sin element-wise
        /// </summary>
        public Vector Sine()
            => new(Values.Select(System.Math.Sin));
        /// <summary>
        /// Compute sinh element-wise
        /// </summary>
        public Vector Sinh()
            => new(Values.Select(System.Math.Sinh));
        /// <summary>
        /// Compute pow element-wise
        /// </summary>
        public Vector Power(double exponent)
            => new(Values.Select(v => System.Math.Pow(v, exponent)));
        /// <summary>
        /// Compute sqrt element-wise
        /// </summary>
        public Vector Sqrt()
            => new(Values.Select(System.Math.Sqrt));
        #endregion

        #region Collection/Shape Manipulation
        public Vector Shift(int offset, bool fillZero)
        {
            // TODO: Boundary check
            if (offset > 0) // Shift right
                return Values.Skip(offset).Concat(fillZero ? new double[offset] : Values.Take(offset)).ToArray();
            else // Shift left
                return Values.Skip(Values.Length - 1).Concat(fillZero ? new double[offset] : Values.Take(Values.Length - 1)).ToArray();
        }
        #endregion

        #region Construction
        /// <summary>
        /// Make a copy
        /// </summary>
        public Vector Copy()
            => new(this);
        #endregion

        #region IList Interface (Mutable)
        public double this[int index] { get => Values[index]; set => Values[index] = value; }
        public int Count => Values.Length;
        public bool IsReadOnly { get; set; }

        public Vector AddElement(double item)
        {
            if (IsReadOnly) { throw new InvalidOperationException(); }

            var old = Values;
            var newValues = new double[Count + 1];
            for (int i = 0; i < old.Length; i++)
                newValues[i] = old[i];
            newValues[old.Length] = item;

            return newValues;
        }
        public Vector ClearValues()
            => new double[Count];
        public bool Contains(double item)
            => Values.Contains(item);
        public void CopyTo(double[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < Values.Length; i++)
                array[i] = Values[i];
        }
        public IEnumerator<double> GetEnumerator()
        {
            foreach (var value in Values)
                yield return value;
        }
        public int IndexOf(double item)
            => Array.IndexOf(Values, item);
        public Vector Insert(int index, double item)
            => Values.Take(index)
                .Concat(new double[] { item })
                .Concat(Values.Skip(index + 1))
                .ToArray();
        public bool Remove(double item, out Vector? result)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                result = RemoveAt(index);
                return true;
            }
            result = null;
            return false;
        }
        public Vector RemoveAt(int index)
            => Values.Take(index).Concat(Values.Skip(index + 1)).ToArray();
        IEnumerator IEnumerable.GetEnumerator()
            => Values.GetEnumerator();
        #endregion
    }

    /// <summary>
    /// Additional operations
    /// </summary>
    public partial class Vector: IEnumerable<double>, IEnumerable
    {
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

    /// <summary>
    /// Statistics extension for Vector
    /// </summary>
    public partial class Vector : IEnumerable<double>, IEnumerable
    {
        #region Statistical Measures
        /// <summary>
        /// Get min.
        /// </summary>
        public double Minimum => Values.Min();
        /// <summary>
        /// Get mean.
        /// </summary>
        public double Mean => Values.Average();
        /// <summary>
        /// Get average (same as mean).
        /// </summary>
        public double Average => Values.Average();
        /// <summary>
        /// Get max.
        /// </summary>
        public double Maximum => Values.Max();
        /// <summary>
        /// Get sum.
        /// </summary>
        public double Sum => Values.Sum();
        /// <summary>
        /// Get variance.
        /// </summary>
        public double Variance
        {
            get
            {
                // TODO: Why not just use Values.Variance() from MathNet?

                double variance = 0.0;
                if (Values.Length > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / Values.Length;
            }
        }
        public double StandardDeviation
            => Values.StandardDeviation();
        /// <summary>
        /// Get std.
        /// </summary>
        public double STD
            => System.Math.Sqrt(Variance);
        /// <summary>
        /// Get population variance.
        /// </summary>
        public double PopulationVariance
        {
            get
            {
                double variance = 0.0;
                if (Values.Length > 1)
                {
                    double avg = Values.Average();
                    variance += Values.Sum(value => System.Math.Pow(value - avg, 2.0));
                }
                // For population, use n-1, for sample, use n
                return variance / (Values.Length - 1);
            }
        }
        /// <summary>
        /// Get population std.
        /// </summary>
        public double PopulationSTD
            => System.Math.Sqrt(Variance);
        #endregion

        #region Statistics Methods
        /// <summary>
        /// Compute correlation
        /// </summary>
        public double Correlation(Vector other)
        {
            double covariance = Covariance(other);
            double std1 = PopulationSTD;   // Always use n-1 for population
            double std2 = other.PopulationSTD;
            return covariance / (std1 * std2);
        }
        /// <summary>
        /// Compute covariance
        /// </summary>
        public double Covariance(Vector other)
        {
            if (Values.Length != other.Values.Length)
                throw new ArgumentException("Vector size doesn't match");

            double variance = 0.0;
            if (Values.Length > 1)
            {
                double avg1 = Values.Average();
                double avg2 = Values.Average();
                for (int i = 0; i < Values.Length; i++)
                    variance += (Values[i] - avg1) * (other.Values[i] - avg2);
            }
            return variance / (Values.Length - 1); // Always use n-1 for population
        }
        #endregion
    }
}
