echo off

if [%1] == [] goto ErrorEmptyAssemblyName
if [%2] == [] goto ErrorEmptyKey

echo decompiling...
ildasm /all /out=%1.signed.il %1.dll > %1.decompile.log
echo Decompliling complete, press any key to start with compiling...
pause
ilasm /dll /key=%2 %1.signed.il > %1.compile.log
echo Compiling done to %1.signed.dll.

exit /B

:ErrorEmptyAssemblyName
echo No assembly name was provided! 
echo Usage: sign assembly-without-dll-ending key
exit /B

:ErrorEmptyKey
echo No strong name key was provided! 
echo Usage: sign assembly-without-dll-ending key
exit /B