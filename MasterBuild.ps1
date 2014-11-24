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

.PARAMETER FrameworkPlatform
	Specifies the framework platform for the reflection data. The default is ".NETFramework". This parameter is only used when building the reference reflection data.

.PARAMETER FrameworkVersion
	Specifies the framework version for the reflection data. The default is "4.5". This parameter is only used when building the reference reflection data.

.NOTES
	Author: Sam Harwell
#>
param (
	[switch]$BuildReflectionData,
	[switch]$NoClean,
	[switch]$Debug,
	[string]$FrameworkPlatform = '.NETFramework',
	[string]$FrameworkVersion = '4.5'
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

If (!(Test-Path "$CommandDir\SHFB\Deploy\Data\Reflection\System.xml")) {
	$BuildReflectionData = $true
}

If ($BuildReflectionData) {
	cd "$CommandDir\SHFB\Deploy\Data"
	.\BuildReflectionData.ps1 $FrameworkPlatform $FrameworkVersion
	If ($LASTEXITCODE -ne 0) {
		echo 'Failed to build the reflection data, aborting!'
		cd $CommandDir
		exit $LASTEXITCODE
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

cd $CommandDir
