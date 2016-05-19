nuget pack MongoDB.Bson.signed.2.2.4\MongoDB.Bson.signed.nuspec -BasePath MongoDB.Bson.signed.2.2.4
nuget pack MongoDB.Driver.Core.signed.2.2.4\MongoDB.Driver.Core.signed.nuspec -BasePath MongoDB.Driver.Core.signed.2.2.4
nuget pack MongoDB.Driver.signed.2.2.4\MongoDB.Driver.signed.nuspec -BasePath MongoDB.Driver.signed.2.2.4
nuget pack MongoDB.Driver.GridFS.signed.2.2.4\MongoDB.Driver.GridFS.signed.nuspec -BasePath MongoDB.Driver.GridFS.signed.2.2.4

nuget add MongoDB.Bson.signed.2.2.4.nupkg -source ..\local-package-source
nuget add MongoDB.Driver.Core.signed.2.2.4.nupkg -source ..\local-package-source
nuget add MongoDB.Driver.signed.2.2.4.nupkg -source ..\local-package-source
nuget add MongoDB.Driver.GridFS.signed.2.2.4.nupkg -source ..\local-package-source