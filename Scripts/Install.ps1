#
# Install.ps1
#
# Download and install the application.
#
# Remote install: `Invoke-WebRequest 'https://raw.githubusercontent.com/GaelGirodon/accounts/master/Scripts/Install.ps1' | Invoke-Expression`
#

$ErrorActionPreference = "stop" # Quit if anything goes wrong

$PackageUrl = "https://github.com/GaelGirodon/accounts/releases/latest/download/Accounts.zip"
$PackageChecksumUrl = "$PackageUrl.sha256"
$PackagePath = (Join-Path $env:TEMP "Accounts.zip")
$ProgramsPath = (Join-Path $env:LOCALAPPDATA "Programs")
$InstallPath = (Join-Path $ProgramsPath "Accounts")
$StartMenuPath = (Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs")

# Download
Write-Host "Downloading package from $PackageUrl..."
$WebClient = (New-Object System.Net.WebClient)
$WebClient.Headers.Add("User-Agent: WebClient")
$WebClient.DownloadFile($PackageUrl, $PackagePath)
Write-Host -ForegroundColor Green "Package downloaded to $PackagePath"

# Checksum
Write-Host "Validating checksum..."
$WebClient.Headers.Add("User-Agent: WebClient")
$PackageChecksumExpected = ($WebClient.DownloadString($PackageChecksumUrl)).Trim()
Write-Host "> Expected: $PackageChecksumExpected"
$PackageChecksumActual = (Get-FileHash -Algorithm SHA256 $PackagePath).Hash.ToLower()
Write-Host "> Actual:   $PackageChecksumActual"
if ($PackageChecksumActual -ine $PackageChecksumExpected) {
    Write-Error "Invalid checksum"
    return 2 | Out-Null
}
Write-Host -ForegroundColor Green "Checksum is valid"

# Extract
Write-Host "Extracting archive to $InstallPath..."
if (!(Test-Path $ProgramsPath)) {
    New-Item -ItemType "Directory" -Path $ProgramsPath
}
Expand-Archive -Path $PackagePath -DestinationPath $InstallPath -Force
Write-Host -ForegroundColor Green "Archive extracted to $InstallPath"

# Create a shortcut
Write-Host "Creating a shortcut in the start menu..."
$ShortcutName = "Accounts"
if ((Get-Culture).Name -like "fr*") {
    $ShortcutName = "Comptes"
}
$WSScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WSScriptShell.CreateShortcut("$StartMenuPath\$ShortcutName.lnk")
$Shortcut.TargetPath = "$InstallPath\Accounts.exe"
$Shortcut.Save()
Write-Host -ForegroundColor Green "Shortcut created"

# Clean up
Write-Host "Removing package..."
Remove-Item $PackagePath
Write-Host -ForegroundColor Green "Package removed"
