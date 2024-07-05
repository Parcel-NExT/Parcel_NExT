using System.Text.RegularExpressions;

namespace Parcel.Processing.Utilities
{
    /// <summary>
    /// Main pipeline interface
    /// </summary>
    public sealed class Pipeline
    {
        #region Construction
        /// <summary>
        /// Return result from previous invokation
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        /// Construct a new pipeline with content at this step
        /// </summary>
        public Pipeline(string? content = null)
        {
            Content = content;
        }
        #endregion

        #region Main Interface Method (Fluent API)
        /// <summary>
        /// Continue or running the pipeline with next invokation
        /// </summary>
        public Pipeline Pipe(string program, string arguments)
        {
            if (Content != null)
            {
                string output = PipelineUtilities.Feed(program, arguments, Content);
                return new Pipeline(output);
            }
            else
                return PipelineUtilities.Pipe(program, arguments);
        }
        #endregion

        #region Operator Overloading
        /// <summary>
        /// Feed pipeline using strings
        /// </summary>
        /// <param name="nextStep">Complete program name plus arguments</param>
        /// <returns>Finished pipe</returns>
        public static Pipeline operator |(Pipeline step, string nextStep)
        {
            if (nextStep.StartsWith('"'))
            {
                // Get program name and arguments
                string program = Regex.Match(nextStep, @"^""(.*?)""").Groups[1].Value;
                string arguments = nextStep.Substring(program.Length).Trim();
                string output = PipelineUtilities.Feed(program, arguments, step.Content);
                return new Pipeline(output);
            }
            else
            {
                // Get program name and arguments
                string program = nextStep.Split(' ')[0];
                string arguments = nextStep.Substring(program.Length).Trim();
                string output = PipelineUtilities.Feed(program, arguments, step.Content);
                return new Pipeline(output);
            }
        }
        #endregion
    }
}
