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

------------------ InternalsVisibleTo ---------------

  .custom /*0C000004:0A000004*/ instance void [netstandard/*23000001*/]System.Runtime.CompilerServices.InternalsVisibleToAttribute/*01000005*/::.ctor(string) /* 0A000004 */ = ( 01 00 81 5A 4D 6F 6E 67 6F 44 42 2E 44 72 69 76   // ...ZMongoDB.Driv
                                                                                                                                                                                 65 72 2C 20 50 75 62 6C 69 63 4B 65 79 3D 30 30   // er, PublicKey=00
                                                                                                                                                                                 32 34 30 30 30 30 30 34 38 30 30 30 30 30 39 34   // 2400000480000094
                                                                                                                                                                                 30 30 30 30 30 30 30 36 30 32 30 30 30 30 30 30   // 0000000602000000
                                                                                                                                                                                 32 34 30 30 30 30 35 32 35 33 34 31 33 31 30 30   // 2400005253413100
                                                                                                                                                                                 30 34 30 30 30 30 30 31 30 30 30 31 30 30 66 64   // 04000001000100fd
                                                                                                                                                                                 61 62 37 33 39 65 64 64 34 65 61 35 63 30 37 36   // ab739edd4ea5c076
                                                                                                                                                                                 36 62 32 31 36 62 36 33 64 37 39 33 63 33 65 64   // 6b216b63d793c3ed
                                                                                                                                                                                 62 32 36 39 38 32 35 36 35 36 39 61 32 35 64 39   // b2698256569a25d9
                                                                                                                                                                                 35 61 33 61 37 37 37 65 39 39 31 36 66 64 31 65   // 5a3a777e9916fd1e
                                                                                                                                                                                 32 34 65 64 33 63 33 65 62 39 37 39 64 37 39 35   // 24ed3c3eb979d795
                                                                                                                                                                                 65 61 31 33 62 62 34 31 30 61 36 30 63 62 30 34   // ea13bb410a60cb04
                                                                                                                                                                                 61 37 39 30 65 61 65 66 34 34 36 64 65 61 30 66   // a790eaef446dea0f
                                                                                                                                                                                 66 31 34 35 31 62 66 61 34 39 34 61 32 39 36 36   // f1451bfa494a2966
                                                                                                                                                                                 35 39 30 63 37 32 31 36 63 66 30 61 37 31 38 33   // 590c7216cf0a7183
                                                                                                                                                                                 33 64 37 38 65 30 65 30 38 31 33 36 66 39 66 62   // 3d78e0e08136f9fb
                                                                                                                                                                                 66 63 66 36 30 38 38 32 39 32 64 62 38 33 66 66   // fcf6088292db83ff
                                                                                                                                                                                 66 34 62 38 34 64 33 63 33 37 65 39 38 30 61 63   // f4b84d3c37e980ac
                                                                                                                                                                                 34 34 32 33 38 64 31 62 32 63 39 35 38 35 35 64   // 44238d1b2c95855d
                                                                                                                                                                                 65 36 62 38 61 32 31 36 62 39 31 36 33 36 66 31   // e6b8a216b91636f1
                                                                                                                                                                                 34 34 39 63 63 38 64 30 32 37 31 63 36 30 30 30   // 449cc8d0271c6000
                                                                                                                                                                                 36 63 61 33 62 64 36 39 35 61 38 33 61 61 00 00 ) // 6ca3bd695a83aa..
