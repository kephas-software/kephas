param (
    [string]$version = $( Read-Host "Please provide package version to sign" )
)

$targetNugetPath = "..\..\externals\nugets\"

$packages = @(
    "MongoDB.Bson",
    "MongoDB.Driver",
    "MongoDB.Driver.Core",
    "MongoDB.Driver.GridFS"
)

foreach ($package in $packages) {
    $targetAssemblyNet45 = "$targetNugetPath\$package.signed.$version\lib\net45\$package"
	Remove-Item -path "$targetAssemblyNet45.dll"
	Remove-Item -path "$targetAssemblyNet45.xml"
	
	$sourceAssemblyNet45 = ".\$package.$version\lib\net45\$package"
	Copy-Item "$sourceAssemblyNet45.signed.dll" "$targetAssemblyNet45.dll"
	Copy-Item "$sourceAssemblyNet45.xml" "$targetAssemblyNet45.xml"

    $targetAssemblyNetStandard = "$targetNugetPath\$package.signed.$version\lib\netstandard1.5\$package"
	Remove-Item -path "$targetAssemblyNetStandard.dll"
	Remove-Item -path "$targetAssemblyNetStandard.xml"
	
	$sourceAssemblyNetStandard = ".\$package.$version\lib\netstandard1.5\$package"
	Copy-Item "$sourceAssemblyNetStandard.signed.dll" "$targetAssemblyNetStandard.dll"
	Copy-Item "$sourceAssemblyNetStandard.xml" "$targetAssemblyNetStandard.xml"
	
	iex "$targetNugetPath\.\nuget.exe pack $targetNugetPath$package.signed.$version\$package.signed.nuspec -BasePath $targetNugetPath$package.signed.$version"
}
