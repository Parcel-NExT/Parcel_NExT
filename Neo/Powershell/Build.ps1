# Save pwd for later recover
$PrevPath = Get-Location

Write-Host "Publish for Final Packaging build."
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
# Delete current data
if (Test-Path -Path $PublishFolder) {
    Remove-Item $PublishFolder -Recurse -Force
}

# Publish Executable
$PublishExecutables = @(
    "Parcel.Neo\Parcel.Neo.csproj"
)
foreach ($Item in $PublishExecutables) {
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --self-contained --output $PublishFolder
}

# Validation
$pureExePath = Join-Path $PublishFolder "Neo.exe"
if (-Not (Test-Path $pureExePath)) {
    Write-Host "Build failed."
    Exit
}

# Create archive
$Date = Get-Date -Format yyyyMMdd
$ArchiveFolder = "$PublishFolder\..\Packages"
$ArchivePath = "$ArchiveFolder\PV1_Neo_DistributionBuild_B$Date.zip"
New-Item -ItemType Directory -Force -Path $ArchiveFolder
Compress-Archive -Path $PublishFolder\* -DestinationPath $ArchivePath -Force

# Recover pwd
Set-Location $PrevPath