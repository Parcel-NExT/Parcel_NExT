# Paths configurations
$tranquilityServerProjectLocation = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$PSScriptRoot/../../C#/Parcel.NExT/BackEnds/Tranquility") # Better than Resolve-Path
$gospelFrontendProjectLocation = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$PSScriptRoot/../../../Experiments/Parcel.Godot/ParcelGospel")
$outputDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$PSScriptRoot/../../../Publish/Parcel_NExT/Windows")

# TODO
# - pwsh version check
# - dotnet version check

# Paths check
if (!(Test-Path $tranquilityServerProjectLocation -ErrorAction SilentlyContinue))
{
    Write-Host "Tranquility server project path is invalid: $tranquilityServerProjectLocation"
    Exit
}
if (!(Test-Path $gospelFrontendProjectLocation -ErrorAction SilentlyContinue))
{
    Write-Host "Gospel project path is invalid: $gospelFrontendProjectLocation"
    Exit
}
if (!(Test-Path $outputDirectory -ErrorAction SilentlyContinue))
{
    Write-Host "Build output path is invalid: $outputDirectory"
    Exit
}

# Warning
Write-Host "You must review those paths before continuing! If those paths are not desirable, modify this script before proceeding."
Write-Host "Tranquility server project path: $tranquilityServerProjectLocation"
Write-Host "Gospel front-end project path: $gospelFrontendProjectLocation"
Write-Host "Build output directory: $outputDirectory"
if (!(@("yes", "y", "", "true", "t") -contains ((Read-Host "Continue? (Yes/No)").ToLower()))) { Exit }

# Build C# Server
if (Get-Command "dotnet" -ErrorAction SilentlyContinue)
{
    if (!(Test-Path "$outputDirectory/Tranquility.exe"))
    {
        dotnet publish "$tranquilityServerProjectLocation" --output "$outputDirectory" --self-contained --use-current-runtime -p:PublishSingleFile=true -p:PublishReadyToRunShowWarnings=true
    }
}
else { Write-Host "Command `dotnet` doesn't exist." }
# Build Front-end
if (Get-Command "Godot_v4.2.1-stable_win64.exe" -ErrorAction SilentlyContinue)
{
    if (!(Test-Path "$outputDirectory/Gospel.exe"))
    {
        Godot_v4.2.1-stable_win64.exe --headless --path "$gospelFrontendProjectLocation" --export-release "Windows Desktop" "$outputDirectory/Gospel.exe"
    }
}
else { Write-Host "Godot_v4.2.1-stable_win64.exe `dotnet` doesn't exist." }

# Build essential dependancies: StandardLibrary
if (Get-Command "dotnet" -ErrorAction SilentlyContinue)
{
    if (!(Test-Path "$outputDirectory/StandardLibrary.dll"))
    {
        dotnet publish "$tranquilityServerProjectLocation/../../BasicModules/StandardLibrary" --output "$outputDirectory" --use-current-runtime -p:PublishReadyToRunShowWarnings=true
    }
}
else { Write-Host "Command `dotnet` doesn't exist." }

# Compress archive
if (Test-Path "$outputDirectory")
{
    $compress = @{
        Path = "$outputDirectory/*"
        CompressionLevel = "Optimal"
        DestinationPath = "$outputDirectory/../Parcel_NExT_Gospel_Windows_$(Get-Date -Format "yyyyMMdd")_v0.0.0.zip"
        
    }
    Compress-Archive -Update @compress
}