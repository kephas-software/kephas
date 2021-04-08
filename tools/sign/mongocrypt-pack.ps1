param (
    [string]$version = $( Read-Host "Please provide package version to pack" ),
    [string]$CertificateSubjectName = "KEPHAS SOFTWARE SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

$targetNugetPath = "..\..\externals\nugets\"

$packages = @(
    "MongoDB.Libmongocrypt"
)

foreach ($package in $packages) {
    $targetLicense = "$targetNugetPath\$package.signed.$version\License.txt"
	Remove-Item -path "$targetLicense"

	$sourceLicense = ".\$package.$version\License.txt"
	Copy-Item "$sourceLicense" "$targetLicense"

    $targetAssemblyNet45 = "$targetNugetPath\$package.signed.$version\lib\net452\$package"
	Remove-Item -path "$targetAssemblyNet45.dll"
#	Remove-Item -path "$targetAssemblyNet45.xml"
	
	$sourceAssemblyNet45 = ".\$package.$version\lib\net452\$package"
	Copy-Item "$sourceAssemblyNet45.signed.dll" "$targetAssemblyNet45.dll"
#	Copy-Item "$sourceAssemblyNet45.xml" "$targetAssemblyNet45.xml"

    $targetAssemblyNetStandard15 = "$targetNugetPath\$package.signed.$version\lib\netstandard1.5\$package"
	Remove-Item -path "$targetAssemblyNetStandard15.dll"
#	Remove-Item -path "$targetAssemblyNetStandard15.xml"
	
	$sourceAssemblyNetStandard15 = ".\$package.$version\lib\netstandard1.5\$package"
	Copy-Item "$sourceAssemblyNetStandard15.signed.dll" "$targetAssemblyNetStandard15.dll"
#	Copy-Item "$sourceAssemblyNetStandard15.xml" "$targetAssemblyNetStandard15.xml"

    $targetAssemblyNetStandard20 = "$targetNugetPath\$package.signed.$version\lib\netstandard2.0\$package"
	Remove-Item -path "$targetAssemblyNetStandard20.dll"
#	Remove-Item -path "$targetAssemblyNetStandard20.xml"
	
	$sourceAssemblyNetStandard20 = ".\$package.$version\lib\netstandard2.0\$package"
	Copy-Item "$sourceAssemblyNetStandard20.signed.dll" "$targetAssemblyNetStandard20.dll"
#	Copy-Item "$sourceAssemblyNetStandard20.xml" "$targetAssemblyNetStandard20.xml"

    $targetAssemblyNetStandard21 = "$targetNugetPath\$package.signed.$version\lib\netstandard2.1\$package"
	Remove-Item -path "$targetAssemblyNetStandard21.dll"
#	Remove-Item -path "$targetAssemblyNetStandard21.xml"
	
	$sourceAssemblyNetStandard21 = ".\$package.$version\lib\netstandard2.1\$package"
	Copy-Item "$sourceAssemblyNetStandard21.signed.dll" "$targetAssemblyNetStandard21.dll"
#	Copy-Item "$sourceAssemblyNetStandard21.xml" "$targetAssemblyNetStandard21.xml"
	
	.\NuGet.exe pack $targetNugetPath$package.signed.$version\$package.signed.nuspec -BasePath $targetNugetPath$package.signed.$version
	
	.\NuGet.exe sign .\$package.signed.$version.nupkg -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"

	Copy-Item .\$package.signed.$version.nupkg $targetNugetPath
}
