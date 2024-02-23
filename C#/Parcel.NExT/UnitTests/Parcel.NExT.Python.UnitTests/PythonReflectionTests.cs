namespace Parcel.NExT.Python.UnitTests
{
    public class PythonReflectionTests
    {
        [Fact]
        public void InstalledPackagesShouldContainPip()
        {
            Assert.Contains("pip", PythonReflection.GetInstalledPackages());
        }
        [Fact]
        public void AvailableModulesShouldContainStandardPythonLibraries()
        {
            string[] available = PythonReflection.GetAllAvailableModules();
            string[] standard = ["pipes", "os", "venv"];
            foreach (var item in available)
                Assert.Contains(item, available);
        }
        [Fact]
        public void GetPyPiModulesShouldContainFamousModules()
        {
            string[] modules = PythonReflection.GetPyPiModules().Select(m => m.Name).ToArray();
            string[] famouseModules = ["numpy"];
            foreach (var item in famouseModules)
                Assert.Contains(item, modules);
        }
    }
}
