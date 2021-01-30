#
# Package.ps1
#
# Build and package the application.
#

$Configuration = "Release"
$Output = ".\publish"
$Name = "Accounts"

# Clean
if (Test-Path $Output) {
    Remove-Item -Recurse "$Output"
}

# Build
dotnet publish --configuration "$Configuration" --output "$Output"

# Package
Compress-Archive -Path "$Output\*" -DestinationPath "$Output\$Name.zip" -CompressionLevel "Optimal"

# Checksum
(Get-FileHash -Path "$Output\$Name.zip" -Algorithm SHA256).Hash.ToLower() + "`n" `
 | Set-Content -Path "$Output\$Name.zip.sha256" -NoNewLine -Encoding ascii
