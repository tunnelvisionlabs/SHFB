param(
	[string]$SHFBSource = '..\..\SHFB',
	[switch]$Clean
)

$folders = @(
	@("$SHFBSource\Documentation\SandcastleBuilder",	'SandcastleBuilder'),
	@("$SHFBSource\Documentation\SandcastleMAMLGuide",	'MAMLGuide'),
	@("$SHFBSource\Documentation\SandcastleTools",		'SandcastleTools'),
	@("$SHFBSource\Documentation\XMLCommentsGuide",		'XMLCommentsGuide'),
	@("$SHFBSource\SHFB\Source\HtmlToMaml\Doc",			'HtmlToMaml'),
	@("$SHFBSource\SHFB\Source\WebCodeProviders\Doc",	'EWSoftwareCodeDom')
)

ForEach ($folder in $folders)
{
	$source = "$($folder[0])\Help"
	$target = $folder[1]

	If ((Test-Path $target) -and $Clean)
	{
		Write-Host "Removing files from existing output path: $target"
		Remove-Item $target -Recurse
	}

	If (!(Test-Path $target))
	{
		Write-Host "Creating target folder: $target"
		New-Item -ItemType directory $target | Out-Null
	}

	$excludes = @(
		# Skip server-side scripting features
		'FillNode.*', 'LoadIndexKeywords.*', 'SearchHelp.*',

		# Data used by server-side scripting features
		'fti',

		# Working data folder (in case it wasn't removed)
		'Working',

		# Build log and other output formats
		'*.chm', 'LastBuild.log'
	)
	Copy-Item "$source\*" -Recurse $target -Exclude $excludes -Force
}
