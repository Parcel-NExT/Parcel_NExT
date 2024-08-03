using Parcel.NExT.Python.Helpers;
using Python.Runtime;

namespace PythonNetPrintImplementation
{
    public static class PrintImplementation
    {
        #region Print
        public static void Print(object o)
        {
            Console.WriteLine(o.ToString());
        }
        #endregion
    }
}

namespace Parcel.NExT.Python
{
    public sealed class InteractivePython
    {
        #region Construction
        public InteractivePython()
        {
            PythonRuntimeHelper.InitializeEngine();
            using (Py.GIL())
            {
                PythonScope = Py.CreateScope();
                // Remark: Below seems no longer necessary since latest Python.net does implement print()
                //PythonScope.Exec("""
                //    import clr
                //    clr.AddReference("Parcel.NExT.Python")
                //    from PythonNetPrintImplementation import PrintImplementation
                //    import sys
                //    sys.displayhook = PrintImplementation.Print
                //    """);
            }
        }
        private PyModule PythonScope { get; }
        #endregion

        #region Methods
        public object? Evaluate(string scripts)
        {
            if (PythonScope == null)
                throw new ApplicationException("Python runtime is not initialized.");

            using (Py.GIL())
            {
                // Remark: Notice ipython is able to retrieve last result, however this is not the typical behavior of python repl
                // Remark-cz: At the moment we are not able to guess "last result"; But we definitely want to
                try
                {
                    return PythonScope.Eval(scripts);
                }
                catch (Exception) // When exception first get raised here, it could indicate the scripts is not an expression, so we proceed to "Exec" it instead
                {
                    try
                    {
                        PythonScope.Exec(scripts);
                    }
                    catch (Exception realException)
                    {
                        Console.WriteLine(realException.Message);
                    }

                    return null;
                }

                // Clue: Ipython does something like this: https://github.com/ipython/ipython/blob/main/IPython/core/displayhook.py which might involve sys.displayhook and builtins._
                // But do notice that `a = 5` in both python repl and ipython returns None
            }
        }
        #endregion
    }
}
