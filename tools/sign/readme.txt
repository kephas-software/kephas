1. Start the Visual Studio Developer Command Prompt (for ildasm and ilasm).
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

-------------

For MongoDB packages:

1. Start the Visual Studio Developer Command Prompt (for ildasm and ilasm).
2. Copy in the sign folder the MongoDB packages and unzip them.
3. Run: powershell .\mongo-sign.ps1
4. Indicate the version of the packages.

After building the signed packages:

1. Go to ..\..\externals\nuget folder.
2. Create the nuget source folders for the new version.
3. Update the *.signed.nuspec packages.
4. Return to this folder.
5. Run: powershell .\mongo-pack.ps1.

After creating the nuget signed packages, publish them on nuget.org.

--------------

User kaloyank <Kaloyan.Kalchev@progress.com> sends the following message to the owners of Package 'MongoDB.Driver.signed 2.12.2'.
Hi guys!
I noticed something while using your signed MongoDB packages. The FileVersion of the signed libraries is missing from their metadata (or the meta overall is missing). I need this property in order to be able to redeploy an already deployed assembly through my installer app - I compare FileVersions, and redeploy if FileVersion is newer (not possible if FileVersion is null). Fortunately, I think I came up with a solution to that, in case it's an unintentional behavior. After some fiddling around with ildasm and ilasm tools (I guess you're using these to disassemble and reassemble MongoDB assemblies with a snk, respectively), I found out that if the reassembly tool does not know what metadata to add to the newly reassembled libraries, it, quite expectedly, produces an assembly without any metadata (FileVersion, and the like). The disassembly tool (ildasm), though, produces a metadata file - a <assembly-name>.res file, which can be used in the reassembly process using the /res key like this:
ilasm ... /res:<assembly-name>.res ...
This produces the desired result - a signed assembly with the proper metadata attached to it (FileVersion included). Is this a viable solution for you, so that you can, eventually, release next versions of the signed MongoDB assemblies with the correct metadata?
Let me know if I can be of assistance.
Greetings, Kaloyan
