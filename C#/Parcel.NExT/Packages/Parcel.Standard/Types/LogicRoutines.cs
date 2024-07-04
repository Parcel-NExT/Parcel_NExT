namespace Parcel.Standard.Types
{
    /// <summary>
    /// Exposes logic
    /// </summary>
    public static class LogicRoutines
    {
        #region Binary and Multiple Choice
        /// <remarks>
        /// Otherwise known as ternary operator 😄
        /// </remarks>
        public static object? Choose(bool choice, object? a, object? b)
            => choice ? a : b;
        /// <param name="choice">Should we call it "choice" or "selector"?🤔</param>
        public static object? Choose(uint choice, params object?[] options)
        {
            if (choice >= options.Length)
                throw new ArgumentOutOfRangeException($"Choice {choice} out of range (range: 0-{options.Length - 1}).");
            return options[choice];
        }
        #endregion
    }
}
