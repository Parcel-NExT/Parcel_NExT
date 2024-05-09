
namespace Parcel.NExT.CodeGen
{
    public static class CodeGenService
    {
        #region Build Environment
        public static bool CheckSDKsInstalled(bool throwExceptions = true)
        {
            if (throwExceptions)
                return CheckSDKsInstalled();
            else
            {
                try { return CheckSDKsInstalled(); }
                catch { return false; }
            }
        }
        public static bool CheckSDKsInstalled()
        {
            // Check dotnet sdk is installed and available in PATH
            // ...

            // Check Python is installed and available in PATH
            // ...

            throw new NotImplementedException();
        }
        #endregion
    }
}
