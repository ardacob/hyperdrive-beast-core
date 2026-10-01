$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework C# derleyicisi bulunamadı.' }
Push-Location $projectRoot
try {
    New-Item -ItemType Directory -Path dist -Force | Out-Null
    & $compiler /nologo /target:winexe /platform:anycpu /win32manifest:windows\HyperDrive.manifest /resource:assets\beast-core.png,Beast /out:dist\HyperDrive.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll windows\HyperDrive.cs
    if ($LASTEXITCODE -ne 0) { throw 'Derleme başarısız.' }
    Write-Host 'dist/HyperDrive.exe hazır.'
} finally { Pop-Location }
