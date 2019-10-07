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

    $targetAssemblyNet45 = "$targetNugetPath\$package.signed.$version\lib\net452\$package"
	Remove-Item -path "$targetAssemblyNet45.dll"
	Remove-Item -path "$targetAssemblyNet45.xml"
	
	$sourceAssemblyNet45 = ".\$package.$version\lib\net452\$package"
	Copy-Item "$sourceAssemblyNet45.signed.dll" "$targetAssemblyNet45.dll"
	Copy-Item "$sourceAssemblyNet45.xml" "$targetAssemblyNet45.xml"

    $targetAssemblyNetStandard = "$targetNugetPath\$package.signed.$version\lib\netstandard1.5\$package"
	Remove-Item -path "$targetAssemblyNetStandard.dll"
	Remove-Item -path "$targetAssemblyNetStandard.xml"
	
	$sourceAssemblyNetStandard = ".\$package.$version\lib\netstandard1.5\$package"
	Copy-Item "$sourceAssemblyNetStandard.signed.dll" "$targetAssemblyNetStandard.dll"
	Copy-Item "$sourceAssemblyNetStandard.xml" "$targetAssemblyNetStandard.xml"
	
	.\NuGet.exe pack $targetNugetPath$package.signed.$version\$package.signed.nuspec -BasePath $targetNugetPath$package.signed.$version
	
	.\NuGet.exe sign .\$package.signed.$version.nupkg -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"

	Copy-Item .\$package.signed.$version.nupkg $targetNugetPath
}
