using System.Text;

namespace InteractivePython
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Instantiate single instance
            Parcel.NExT.Python.InteractivePython python = new();

            // Remark-cz: At the moment we use a simple standard input/output model
            StringBuilder scripts = new();
            while (true)
            {
                string? input = Console.ReadLine();
                if (input != null)
                    scripts.AppendLine(input);
                else if (!string.IsNullOrEmpty(scripts.ToString()))
                {
                    EvaluateScript(python, scripts.ToString().TrimEnd());
                    scripts.Clear();
                }
            }
        }

        private static void EvaluateScript(Parcel.NExT.Python.InteractivePython python, string script)
        {
            object? result = RunScript(python, script);
            if (result != null)
                Console.WriteLine(result.ToString());
        }

        private static object? RunScript(Parcel.NExT.Python.InteractivePython python, string scripts)
        {
            object? obj = python.Evaluate(scripts);
            return obj;
        }
    }
}
