namespace Zora.Infrastructure.Package
{
    /// <summary>
    /// Helper class for scaffolding new .Net packages
    /// </summary>
    public static class PackageTemplateGenerator
    {
        public static string GenerateTemplate(string packageName, string folderPath)
        {
            string projectFolder = Path.Combine(folderPath, packageName);

            // Generate something that is similar to `dotnet new console` without relying on dotnet sdk, aka. just use plain text
            Directory.CreateDirectory(projectFolder);
            File.WriteAllText($"{packageName}.csproj", $$"""
                <Project Sdk="Microsoft.NET.Sdk">

                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net8.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>

                </Project>
                """);
            File.WriteAllText($"Program.cs", $$"""
                namespace 
                {
                    public static class MyClass
                    {
                        public static string HelloWorld() 
                            => "Hello World!";
                    }
                }
                """);

            return projectFolder;
        }
    }
}
