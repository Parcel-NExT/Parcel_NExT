# Builds package dlls

# Paths configurations
$packagesOutputDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$PSScriptRoot/../../Publish/Packages")

# TODO
# - pwsh version check
# - dotnet version check

# Paths check
if (!(Test-Path $packagesOutputDirectory -ErrorAction SilentlyContinue))
{
    Write-Host "Publish path is invalid: $packagesOutputDirectory"
    Exit
}
Remove-Item $packagesOutputDirectory -Recurse -Force

# Warning
Write-Host "You must review those paths before continuing! If those paths are not desirable, modify this script before proceeding. (Notice you need to manually create those folders)"
Write-Host "Packages output directory: $packagesOutputDirectory"
if (!(@("yes", "y", "", "true", "t") -contains ((Read-Host "Continue? (Yes/No)").ToLower()))) { Exit }

# Build all packages
$packages = @(
    "C#\Parcel.NExT\Packages\Parcel.CSV"
    "C#\Parcel.NExT\Packages\Parcel.DataAnalytics"
    "C#\Parcel.NExT\Packages\Parcel.DataGrid"
    "C#\Parcel.NExT\Packages\Parcel.Excel"
    "C#\Parcel.NExT\Packages\Parcel.Generators"
    "C#\Parcel.NExT\Packages\Parcel.HistoryData"
    "C#\Parcel.NExT\Packages\Parcel.InMemoryDB"
    "C#\Parcel.NExT\Packages\Parcel.Matrix"
    "C#\Parcel.NExT\Packages\Parcel.MSAnalysisService"
    "C#\Parcel.NExT\Packages\Parcel.REST"
    "C#\Parcel.NExT\Packages\Parcel.Statistics"
    "C#\Parcel.NExT\Packages\Parcel.Vector"
)
if (Get-Command "dotnet" -ErrorAction SilentlyContinue)
{
    foreach ($Item in $packages) {
        dotnet publish $PSScriptRoot\..\..\$Item --use-current-runtime --output $packagesOutputDirectory
    }
}
else { Write-Host "Command `dotnet` doesn't exist." }