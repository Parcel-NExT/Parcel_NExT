namespace Parcel.Processing.Utilities
{
    public static class Calculator
    {
        /// <summary>
        /// Simple math parsed string to number
        /// </summary>
        public static double Calculate(string expression)
        {
            object result = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator().Evaluate(expression);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Same as Calculator but with a max of 9 variable number of inputs; Auto-replace with $1-$9 as variable names
        /// </summary>
        public static double Calculate(string expression, params double[] variables)
        {
            string replaced = expression;
            for (int i = 0; i < variables.Length; i++)
            {
                double variable = variables[i];
                replaced = replaced.Replace($"${i}", variable.ToString());
            }

            object result = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator().Evaluate(replaced);
            return Convert.ToDouble(result);
        }
    }
}