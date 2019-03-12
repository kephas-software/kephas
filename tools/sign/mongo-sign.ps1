param (
    [string]$version = $( Read-Host "Please provide package version to sign" )
)

$keyPath = "..\..\src\Kephas.snk"
$editorPath = "$Env:ProgramFiles\Notepad++\notepad++.exe"

$assemblies = @(
    "mongodb.bson.$version\lib\net45\MongoDB.Bson",
    "mongodb.bson.$version\lib\netstandard1.5\MongoDB.Bson",
    "mongodb.driver.$version\lib\net45\MongoDB.Driver",
    "mongodb.driver.$version\lib\netstandard1.5\MongoDB.Driver",
    "mongodb.driver.core.$version\lib\net45\MongoDB.Driver.Core",
    "mongodb.driver.core.$version\lib\netstandard1.5\MongoDB.Driver.Core",
    "mongodb.driver.gridfs.$version\lib\net45\MongoDB.Driver.GridFS",
    "mongodb.driver.gridfs.$version\lib\netstandard1.5\MongoDB.Driver.GridFS"
)

foreach ($assembly in $assemblies) {
	Write-Host "Decompiling $assembly.dll..."
	iex "ildasm /all /out=$assembly.signed.il $assembly.dll > $assembly.decompile.log"
	iex "& ""$editorPath"" ""$assembly.signed.il"""
	Read-Host -Prompt "Decompiling complete, press ENTER key to start with compiling..."
	iex "ilasm /dll /key=$keyPath $assembly.signed.il > $assembly.compile.log"
	Write-Host "Compiling done to $assembly.signed.dll."
}
