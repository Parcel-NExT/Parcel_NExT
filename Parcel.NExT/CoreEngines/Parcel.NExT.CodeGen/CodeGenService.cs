
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
            bool allPassed = true;

            // Check dotnet sdk is installed and available in PATH
            allPassed = allPassed && CSharpScriptExecutableGenerator.AssertSDKVersion(CSharpScriptExecutableGenerator.GetDotnetSDKVersion());

            // Check Python is installed and available in PATH
            // ...

            // Check Parcel standard library dlls/nugets are available
            // ...

            return allPassed;
        }
        #endregion
    }
}
