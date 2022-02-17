param (
    [string]$version = $( Read-Host "Please provide package version to sign" ),
    [string]$CertificateSubjectName = "KEPHAS SOFTWARE SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

$targetNugetPath = "..\..\externals\nugets\"

$packages = @(
    "MongoDB.Bson",
    "MongoDB.Driver",
    "MongoDB.Driver.Core",
    "MongoDB.Driver.GridFS"
)

foreach ($package in $packages) {
    $targetLicense = "$targetNugetPath\$package.signed.$version\License.txt"
	Remove-Item -path "$targetLicense"

	$sourceLicense = ".\$package.$version\License.txt"
	Copy-Item "$sourceLicense" "$targetLicense"

    $targetAssemblyNet47 = "$targetNugetPath\$package.signed.$version\lib\net472\$package"
	Remove-Item -path "$targetAssemblyNet47.dll"
	Remove-Item -path "$targetAssemblyNet47.xml"
	
	$sourceAssemblyNet47 = ".\$package.$version\lib\net472\$package"
	Copy-Item "$sourceAssemblyNet47.signed.dll" "$targetAssemblyNet47.dll"
	Copy-Item "$sourceAssemblyNet47.xml" "$targetAssemblyNet47.xml"

    $targetAssemblyNetStandard20 = "$targetNugetPath\$package.signed.$version\lib\netstandard2.0\$package"
	Remove-Item -path "$targetAssemblyNetStandard20.dll"
	Remove-Item -path "$targetAssemblyNetStandard20.xml"
	
	$sourceAssemblyNetStandard20 = ".\$package.$version\lib\netstandard2.0\$package"
	Copy-Item "$sourceAssemblyNetStandard20.signed.dll" "$targetAssemblyNetStandard20.dll"
	Copy-Item "$sourceAssemblyNetStandard20.xml" "$targetAssemblyNetStandard20.xml"

    $targetAssemblyNetStandard21 = "$targetNugetPath\$package.signed.$version\lib\netstandard2.1\$package"
	Remove-Item -path "$targetAssemblyNetStandard21.dll"
	Remove-Item -path "$targetAssemblyNetStandard21.xml"
	
	$sourceAssemblyNetStandard21 = ".\$package.$version\lib\netstandard2.1\$package"
	Copy-Item "$sourceAssemblyNetStandard21.signed.dll" "$targetAssemblyNetStandard21.dll"
	Copy-Item "$sourceAssemblyNetStandard21.xml" "$targetAssemblyNetStandard21.xml"
	
	.\NuGet.exe pack $targetNugetPath$package.signed.$version\$package.signed.nuspec -BasePath $targetNugetPath$package.signed.$version
	
	.\NuGet.exe sign .\$package.signed.$version.nupkg -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"

	Copy-Item .\$package.signed.$version.nupkg $targetNugetPath
}
