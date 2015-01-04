param (
)

$CommandDir = Split-Path -Parent $PSCommandPath

$nuget = "$CommandDir\..\SHFB\Source\.nuget\NuGet.exe"
&$nuget 'pack' 'SHFB.nuspec' '-NoDefaultExcludes' '-NoPackageAnalysis' '-OutputDirectory' "$CommandDir\..\Deployment"
