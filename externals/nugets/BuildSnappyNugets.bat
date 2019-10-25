@echo off

nuget pack Crc32C.NET.signed.1.0.5\Crc32C.NET.signed.nuspec -BasePath Crc32C.NET.signed.1.0.5
nuget pack Snappy.NET.signed.1.1.1.8\Snappy.NET.signed.nuspec -BasePath Snappy.NET.signed.1.1.1.8

@echo Done.
pause

goto :eof
:usage
@echo Usage: %0 ^<version-number^>
pause
exit /B 1