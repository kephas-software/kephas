nuget pack MongoDB.Bson.signed.2.2.3\MongoDB.Bson.signed.nuspec -BasePath MongoDB.Bson.signed.2.2.3
nuget pack MongoDB.Driver.Core.signed.2.2.3\MongoDB.Driver.Core.signed.nuspec -BasePath MongoDB.Driver.Core.signed.2.2.3
nuget pack MongoDB.Driver.signed.2.2.3\MongoDB.Driver.signed.nuspec -BasePath MongoDB.Driver.signed.2.2.3

nuget add MongoDB.Bson.signed.2.2.3.nupkg -source ..\local-package-source
nuget add MongoDB.Driver.Core.signed.2.2.3.nupkg -source ..\local-package-source
nuget add MongoDB.Driver.signed.2.2.3.nupkg -source ..\local-package-source