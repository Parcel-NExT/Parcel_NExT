namespace Parcel.Standard.Programming
{
    /// <summary>
    /// Offers additional program/graph input-related functions
    /// </summary>
    public class InputHelpers
    {
        /// <summary>
        /// Validates input number falls in expected range; Throw an exception otherwise.
        /// </summary>
        /// <remarks>
        /// This function offers a functional way to do input validation; Useful when graph/program needs to halt when dealing with invalid inputs and thus stop downstream processing.
        /// Useful when running graphs in command line.
        /// </remarks>
        public static double ValidateNumberExclusive(double number, double minExclusive, double maxExclusive, bool shouldBeInteger = false)
        {
            if (number < minExclusive)
                throw new ArgumentException($"Input number {number} is smaller than mininum allowed value {minExclusive}");
            if (number > maxExclusive)
                throw new ArgumentException($"Input number {number} is bigger than mininum allowed value {maxExclusive}");
            if (shouldBeInteger && (number % 1) != 0)
                throw new ArgumentException($"Input number {number} must be an integer.");
            return number;
        }

        /// <summary>
        /// Validates input number falls in expected range; Throw an exception otherwise.
        /// </summary>
        /// <remarks>
        /// This function offers a functional way to do input validation; Useful when graph/program needs to halt when dealing with invalid inputs and thus stop downstream processing.
        /// Useful when running graphs in command line.
        /// </remarks>
        public static double ValidateNumberInclusive(double number, double minInclusive, double maxInclusive, bool shouldBeInteger = false)
        {
            if (number < minInclusive)
                throw new ArgumentException($"Input number {number} is smaller than mininum allowed value {minInclusive}");
            if (number > maxInclusive)
                throw new ArgumentException($"Input number {number} is bigger than mininum allowed value {maxInclusive}");
            if (shouldBeInteger && (number % 1) != 0)
                throw new ArgumentException($"Input number {number} must be an integer.");
            return number;
        }
    }
}
