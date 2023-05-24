param (
    [string]$version = $( Read-Host "Please provide MongoDB package version to sign" ),
    [string]$cryptversion = $( Read-Host "Please provide Libmongocrypt package version to sign" ),
    [string]$CertificateSubjectName = "KEPHAS SOFTWARE SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

$targetNugetPath = "..\..\externals\nugets\"

$packages = @(
    "MongoDB.Libmongocrypt",
    "MongoDB.Bson",
    "MongoDB.Driver",
    "MongoDB.Driver.Core",
    "MongoDB.Driver.GridFS"
)

foreach ($package in $packages) {
	$libversion = If ($package -eq "MongoDB.Libmongocrypt") {$cryptversion} Else {$version}
	
    $targetLicense = "$targetNugetPath\$package.signed.$libversion\License.txt"
	Remove-Item -path "$targetLicense"

	$sourceLicense = ".\$package.$libversion\License.txt"
	Copy-Item "$sourceLicense" "$targetLicense"

    $targetAssemblyNet47 = "$targetNugetPath\$package.signed.$libversion\lib\net472\$package"
	Remove-Item -path "$targetAssemblyNet47.dll"
	Remove-Item -path "$targetAssemblyNet47.xml"
	
	$sourceAssemblyNet47 = ".\$package.$libversion\lib\net472\$package"
	Copy-Item "$sourceAssemblyNet47.signed.dll" "$targetAssemblyNet47.dll"
	Copy-Item "$sourceAssemblyNet47.xml" "$targetAssemblyNet47.xml"

    $targetAssemblyNetStandard20 = "$targetNugetPath\$package.signed.$libversion\lib\netstandard2.0\$package"
	Remove-Item -path "$targetAssemblyNetStandard20.dll"
	Remove-Item -path "$targetAssemblyNetStandard20.xml"
	
	$sourceAssemblyNetStandard20 = ".\$package.$libversion\lib\netstandard2.0\$package"
	Copy-Item "$sourceAssemblyNetStandard20.signed.dll" "$targetAssemblyNetStandard20.dll"
	Copy-Item "$sourceAssemblyNetStandard20.xml" "$targetAssemblyNetStandard20.xml"

    $targetAssemblyNetStandard21 = "$targetNugetPath\$package.signed.$libversion\lib\netstandard2.1\$package"
	Remove-Item -path "$targetAssemblyNetStandard21.dll"
	Remove-Item -path "$targetAssemblyNetStandard21.xml"
	
	$sourceAssemblyNetStandard21 = ".\$package.$libversion\lib\netstandard2.1\$package"
	Copy-Item "$sourceAssemblyNetStandard21.signed.dll" "$targetAssemblyNetStandard21.dll"
	Copy-Item "$sourceAssemblyNetStandard21.xml" "$targetAssemblyNetStandard21.xml"
	
	.\NuGet.exe pack $targetNugetPath$package.signed.$libversion\$package.signed.nuspec -BasePath $targetNugetPath$package.signed.$libversion
	
	.\NuGet.exe sign .\$package.signed.$libversion.nupkg -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"

	Copy-Item .\$package.signed.$libversion.nupkg $targetNugetPath
}
