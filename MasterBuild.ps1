<#
.SYNOPSIS
	Builds the Sandcastle Help File Builder project, supporting documentation, and installer.

.DESCRIPTION
	Builds the Sandcastle Help File Builder project, supporting documentation, and installer.

.PARAMETER BuildReflectionData
	When specified, the reference reflection data will be rebuilt. This switch is always enabled when the reflection data files are missing.

.PARAMETER NoClean
	When specified, the Clean target is not executed before building each project.

.PARAMETER Debug
	When specified, the Debug configuration is built instead of the standard Release configuration.

.NOTES
	Author: Sam Harwell
#>
param (
	[switch]$BuildReflectionData,
	[switch]$NoClean,
	[switch]$Debug
)

if ($NoClean) {
	$BuildTargets = 'Build'
} else {
	$BuildTargets = 'Clean;Build'
}

if ($Debug) {
	$BuildConfig = 'Debug'
} else {
	$BuildConfig = 'Release'
}

$CommandDir = Split-Path -Parent $PSCommandPath

$env:SHFBROOT = "$CommandDir\SHFB\Deploy\"
$msbuild = "$env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$nuget = "$CommandDir\SHFB\Source\.nuget\NuGet.exe"

cd "$CommandDir\SHFB\Source"

&$nuget 'restore' 'SandcastleTools.sln'
&$msbuild '/nologo' '/v:m' '/m' 'SandcastleTools.sln' "/t:$BuildTargets" "/p:Configuration=$BuildConfig;Platform=Any CPU"
If ($LASTEXITCODE -ne 0) {
	echo 'Failed to build SandcastleTools.sln, aborting!'
	cd $CommandDir
	exit $LASTEXITCODE
}

$FrameworkVersions = @{
	'.NETCore' = '4.5.1'
	'.NETFramework' = '4.5.1'
	'.NETMicroFramework' = '4.3'
	'.NETPortable' = '4.6'
	'Silverlight' = '5.0'
	'WindowsPhone' = '8.1'
	'WindowsPhoneApp' = '8.1'
}

ForEach ($pair In $FrameworkVersions.GetEnumerator()) {
	$BuildCurrent = $BuildReflectionData
	If (!(Test-Path "$CommandDir\SHFB\Deploy\Data\$($pair.Key)\System.xml")) {
		$BuildCurrent = $true
	}

	If ($BuildCurrent) {
		cd "$CommandDir\SHFB\Deploy\Data"
		.\BuildReflectionData.ps1 $pair.Key $pair.Value
		If ($LASTEXITCODE -ne 0) {
			echo 'Failed to build the reflection data, aborting!'
			cd $CommandDir
			exit $LASTEXITCODE
		}
	}
}

cd "$CommandDir\SHFB\Source"

&$nuget 'restore' 'SandcastleBuilder.sln'
&$msbuild '/nologo' '/v:m' '/m' 'SandcastleBuilder.sln' "/t:$BuildTargets" "/p:Configuration=$BuildConfig;Platform=Any CPU"
If ($LASTEXITCODE -ne 0) {
	echo 'Failed to build SandcastleBuilder.sln, aborting!'
	cd $CommandDir
	exit $LASTEXITCODE
}

&$nuget 'restore' 'SandcastleBuilderPackage.sln'
&$msbuild '/nologo' '/v:m' '/m' 'SandcastleBuilderPackage.sln' "/t:$BuildTargets" "/p:Configuration=$BuildConfig;Platform=Any CPU"
If ($LASTEXITCODE -ne 0) {
	echo 'Failed to build SandcastleBuilderPackage.sln, aborting!'
	cd $CommandDir
	exit $LASTEXITCODE
}

cd "$CommandDir\Documentation"

&$nuget 'restore' 'AllDocumentation.sln'
&$msbuild '/nologo' '/v:m' '/m' 'AllDocumentation.sln' "/t:$BuildTargets" "/p:Configuration=$BuildConfig;Platform=Any CPU"
If ($LASTEXITCODE -ne 0) {
	echo 'Failed to build AllDocumentation.sln, aborting!'
	cd $CommandDir
	exit $LASTEXITCODE
}

cd "$CommandDir\SHFB\Source"

&$nuget 'restore' 'SHFBSetup.sln'
&$msbuild '/nologo' '/v:m' '/m' 'SHFBSetup.sln' "/t:$BuildTargets" "/p:Configuration=$BuildConfig;Platform=Any CPU"
If ($LASTEXITCODE -ne 0) {
	echo 'Failed to build SHFBSetup.sln, aborting!'
	cd $CommandDir
	exit $LASTEXITCODE
}

CD "$CommandDir\NuGet"

.\BuildNuGet.ps1

cd $CommandDir
