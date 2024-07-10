using SystemConsole = System.Console;

namespace Parcel.Standard.System
{
    public static class Console
    {
        /// <summary>
        /// Print any object in string representation
        /// </summary>
        public static void Print(object @object)
            => SystemConsole.WriteLine(@object);
    }
}
