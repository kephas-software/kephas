@echo off
if [%1]==[] goto usage

@echo on
nuget push MongoDB.Bson.signed.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push MongoDB.Driver.Core.signed.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push MongoDB.Driver.signed.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push MongoDB.Driver.GridFS.signed.%1.nupkg -Source https://www.nuget.org/api/v2/package

@echo Done.
pause

goto :eof
:usage
@echo Usage: %0 ^<version-number^>
pause
exit /B 1