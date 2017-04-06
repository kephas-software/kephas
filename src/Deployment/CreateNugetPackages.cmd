set BuildConfiguration=Debug
set PackageVersion=3.3.0-preview1
set PackageRefVersion=3.3.0

nuget pack ..\Kephas.Core\Package.nuspec -BasePath ..\Kephas.Core -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Logging.NLog\Package.nuspec -BasePath ..\Kephas.Logging.NLog -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Logging.Log4Net\Package.nuspec -BasePath ..\Kephas.Logging.Log4Net -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Composition.Mef\Package.nuspec -BasePath ..\Kephas.Composition.Mef -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Messaging\Package.nuspec -BasePath ..\Kephas.Messaging -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Messaging.Model\Package.nuspec -BasePath ..\Kephas.Messaging.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Model\Package.nuspec -BasePath ..\Kephas.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data\Package.nuspec -BasePath ..\Kephas.Data -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.Client\Package.nuspec -BasePath ..\Kephas.Data.Client -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.Model\Package.nuspec -BasePath ..\Kephas.Data.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.DocumentDB\Package.nuspec -BasePath ..\Kephas.Data.DocumentDB -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.MongoDB\Package.nuspec -BasePath ..\Kephas.Data.MongoDB -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.InMemory\Package.nuspec -BasePath ..\Kephas.Data.InMemory -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Serialization.Json\Package.nuspec -BasePath ..\Kephas.Serialization.Json -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Platform.Net\Package.nuspec -BasePath ..\Kephas.Platform.Net -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Mail\Package.nuspec -BasePath ..\Kephas.Mail -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Mail.Net46\Package.nuspec -BasePath ..\Kephas.Mail.Net46 -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Web.Owin\Package.nuspec -BasePath ..\Kephas.Web.Owin -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
