1. Start the Visual Studio Command Prompt (for ildasm and ilasm).
2. Copy in some subfolder the assembly to sign.
3. Optional: copy in the same folder with sign.bat the strong name key file (*.snk). You can however indicate a path when starting signing.
4. Type in command prompt: 
sign assembly-without-dll-ending key
and then hit ENTER.
5. After decompiling, open with an editor the generated *.il file and:
  a. Remove all InternalsVisibleToAttribute references to unit tests.
  b. Add to the non-signed referenced assemblies the public key token, like in the example below.

.assembly extern /*23000002*/ MongoDB.Bson
{
  .publickeytoken = (15 B1 11 55 99 98 3C 50 )                         // this was added
  .ver 2:3:0:157
}

6. Go back in the command prompt and hit any key, to assemble the assembly again. The output file will have the ending <original>.signed.dll.