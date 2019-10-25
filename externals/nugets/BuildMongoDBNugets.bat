@echo off
if [%1]==[] goto usage

nuget pack MongoDB.Bson.signed.%1\MongoDB.Bson.signed.nuspec -BasePath MongoDB.Bson.signed.%1
nuget pack MongoDB.Driver.Core.signed.%1\MongoDB.Driver.Core.signed.nuspec -BasePath MongoDB.Driver.Core.signed.%1
nuget pack MongoDB.Driver.signed.%1\MongoDB.Driver.signed.nuspec -BasePath MongoDB.Driver.signed.%1
nuget pack MongoDB.Driver.GridFS.signed.%1\MongoDB.Driver.GridFS.signed.nuspec -BasePath MongoDB.Driver.GridFS.signed.%1

@echo Done.
pause

goto :eof
:usage
@echo Usage: %0 ^<version-number^>
pause
exit /B 1