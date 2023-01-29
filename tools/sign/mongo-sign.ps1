param (
    [string]$version = $( Read-Host "Please provide MongoDB package version to sign" ),
    [string]$cryptversion = $( Read-Host "Please provide Libmongocrypt package version to sign" )
)

$keyPath = "..\..\src\Kephas.snk"
$editorPath = "$Env:ProgramFiles\Notepad++\notepad++.exe".Replace(" (x86)", "")

$assemblies = @(
    "mongodb.libmongocrypt.$cryptversion\lib\net472\MongoDB.Libmongocrypt",
    "mongodb.libmongocrypt.$cryptversion\lib\netstandard2.0\MongoDB.Libmongocrypt",
    "mongodb.libmongocrypt.$cryptversion\lib\netstandard2.1\MongoDB.Libmongocrypt",
    "mongodb.bson.$version\lib\net472\MongoDB.Bson",
    "mongodb.bson.$version\lib\netstandard2.0\MongoDB.Bson",
    "mongodb.bson.$version\lib\netstandard2.1\MongoDB.Bson",
    "mongodb.driver.$version\lib\net472\MongoDB.Driver",
    "mongodb.driver.$version\lib\netstandard2.0\MongoDB.Driver",
    "mongodb.driver.$version\lib\netstandard2.1\MongoDB.Driver",
    "mongodb.driver.core.$version\lib\net472\MongoDB.Driver.Core",
    "mongodb.driver.core.$version\lib\netstandard2.0\MongoDB.Driver.Core",
    "mongodb.driver.core.$version\lib\netstandard2.1\MongoDB.Driver.Core",
    "mongodb.driver.gridfs.$version\lib\net472\MongoDB.Driver.GridFS",
    "mongodb.driver.gridfs.$version\lib\netstandard2.0\MongoDB.Driver.GridFS"
    "mongodb.driver.gridfs.$version\lib\netstandard2.1\MongoDB.Driver.GridFS"
)

foreach ($assembly in $assemblies) {
	Write-Host "Decompiling $assembly.dll..."
	iex ".\ildasm.exe /all /out=$assembly.signed.il $assembly.dll > $assembly.decompile.log"
	iex "& ""$editorPath"" ""$assembly.signed.il"""
	Read-Host -Prompt "Decompiling complete, press ENTER key to start with compiling..."
	iex "ilasm.exe /dll /key=$keyPath /res:$assembly.signed.res $assembly.signed.il > $assembly.compile.log"
	Write-Host "Compiling done to $assembly.signed.dll."
}
