using SystemConsole = System.Console;
using SystemIList = System.Collections.IList;

namespace Parcel.Standard.System
{
    public static class Console
    {
        /// <summary>
        /// Print any object in string representation; Automatically handles well-known types (e.g. arrays).
        /// </summary>
        public static void Print(object @object)
        {
            // Print string items preview
            if (@object is SystemIList list)
            {
                foreach (var item in list)
                    SystemConsole.WriteLine(item.ToString());
            }
            else
                SystemConsole.WriteLine(@object);
        }
    }
}
