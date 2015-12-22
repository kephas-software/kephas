set DropFolder="Z:\Software\KephasFramework\"

copy /y ..\NuGetPackagers\Kephas.Core.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Composition.Mef.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Data.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Hosting.Net45.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Hosting.NetCore.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Logging.NLog.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Messaging.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Model.NuGet.Packager\*.nupkg %DropFolder%
copy /y ..\NuGetPackagers\Kephas.Serialization.Json.NuGet.Packager\*.nupkg %DropFolder%

del /Q %DropFolder%*.symbols.*