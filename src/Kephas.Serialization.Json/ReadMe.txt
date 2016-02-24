WARNING

The Json.NET nuget package has a problem when installing, and instead of adding the reference from 
\packages\Newtonsoft.Json.8.0.2\lib\portable-net45+wp80+win8+wpa81+dnxcore50\Newtonsoft.Json.dll
it adds if from the portable-net40 folder. The latter has a problem when compiling with the ccrewriter
which does not occur in the net45 version.