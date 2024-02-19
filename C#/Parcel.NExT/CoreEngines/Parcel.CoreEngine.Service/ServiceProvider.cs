using System.Reflection;

namespace Parcel.CoreEngine.Service
{
    public abstract class ServiceProvider
    {
        #region Reflection Services
        /// <summary>
        /// Get all available public instance methods in this class that are meaningful for backend use.
        /// </summary>
        /// <returns>Return excludes this method itself and some C# standard instance methods</returns>
        public abstract Dictionary<string, MethodInfo> GetAvailableServices();
        #endregion
    }
}
