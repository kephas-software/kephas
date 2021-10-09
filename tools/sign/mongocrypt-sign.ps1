param (
    [string]$version = $( Read-Host "Please provide package version to sign" )
)

$keyPath = "..\..\src\Kephas.snk"
$editorPath = "$Env:ProgramFiles\Notepad++\notepad++.exe".Replace(" (x86)", "")

$assemblies = @(
    "mongodb.libmongocrypt.$version\lib\net472\MongoDB.Libmongocrypt",
    "mongodb.libmongocrypt.$version\lib\netstandard2.0\MongoDB.Libmongocrypt"
    "mongodb.libmongocrypt.$version\lib\netstandard2.1\MongoDB.Libmongocrypt"
)

foreach ($assembly in $assemblies) {
	Write-Host "Decompiling $assembly.dll..."
	iex ".\ildasm.exe /all /out=$assembly.signed.il $assembly.dll > $assembly.decompile.log"
	iex "& ""$editorPath"" ""$assembly.signed.il"""
	Read-Host -Prompt "Decompiling complete, press ENTER key to start with compiling..."
	iex "ilasm.exe /dll /key=$keyPath /res:$assembly.signed.res $assembly.signed.il > $assembly.compile.log"
	Write-Host "Compiling done to $assembly.signed.dll."
}
