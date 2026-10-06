$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $projectRoot 'src\FarmhandSpeedController.cs'
$output = Join-Path $projectRoot 'build\FarmhandSpeedController.dll'
$releaseZip = Join-Path $projectRoot 'release\FarmhandSpeedController-0.1.0-TEST.zip'
$gameRoot = 'C:\Program Files (x86)\Steam\steamapps\common\Farm Together 2'
$bepInExCore = Join-Path $gameRoot 'BepInEx\core'
$interop = Join-Path $gameRoot 'BepInEx\interop'

$csc = Get-ChildItem 'C:\Program Files\dotnet\sdk\*\Roslyn\bincore\csc.dll' -ErrorAction SilentlyContinue |
    Sort-Object { [version]$_.Directory.Parent.Parent.Name } -Descending |
    Select-Object -First 1 -ExpandProperty FullName
if ([string]::IsNullOrWhiteSpace($csc)) { throw 'Could not find Roslyn csc.dll.' }

$refPack = Get-ChildItem 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref' -Directory |
    Where-Object { Test-Path (Join-Path $_.FullName 'ref\net6.0') } |
    Select-Object -First 1
if ($null -eq $refPack) { throw 'Could not find Microsoft.NETCore.App.Ref net6.0.' }

$references = @()
$references += Get-ChildItem (Join-Path $refPack.FullName 'ref\net6.0') -Filter '*.dll' |
    ForEach-Object { '/r:' + $_.FullName }
$references += @(
    ('/r:' + (Join-Path $interop 'Assembly-CSharp.dll')),
    ('/r:' + (Join-Path $bepInExCore '0Harmony.dll')),
    ('/r:' + (Join-Path $bepInExCore 'BepInEx.Core.dll')),
    ('/r:' + (Join-Path $bepInExCore 'BepInEx.Unity.IL2CPP.dll')),
    ('/r:' + (Join-Path $bepInExCore 'Il2CppInterop.Runtime.dll')),
    ('/r:' + (Join-Path $interop 'Il2Cppmscorlib.dll')),
    ('/r:' + (Join-Path $interop 'Il2CppSystem.dll')),
    ('/r:' + (Join-Path $interop 'UnityEngine.CoreModule.dll'))
)

New-Item -ItemType Directory -Force -Path (Split-Path $output) | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path $releaseZip) | Out-Null
$compilerArgs = @($csc, '/nologo', '/target:library', '/langversion:latest', '/nullable:disable', "/out:$output")
$compilerArgs += $references
$compilerArgs += $source

& 'C:\Program Files\dotnet\dotnet.exe' @compilerArgs
if ($LASTEXITCODE -ne 0) { throw "Compilation failed with exit code $LASTEXITCODE." }

if (Test-Path -LiteralPath $releaseZip) { Remove-Item -LiteralPath $releaseZip -Force }
Compress-Archive -LiteralPath @(
    $output,
    (Join-Path $projectRoot 'README.md'),
    (Join-Path $projectRoot 'CHANGELOG.md')
) -DestinationPath $releaseZip -CompressionLevel Optimal
Write-Host "Built: $output"
Write-Host "Test package: $releaseZip"
