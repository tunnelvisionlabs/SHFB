param (
)

$CommandDir = Split-Path -Parent $PSCommandPath

$nuget = "$CommandDir\..\SHFB\Source\.nuget\NuGet.exe"
&$nuget 'pack' 'SHFB.nuspec' '-NoPackageAnalysis' '-OutputDirectory' "$CommandDir\..\Deployment"
