# 4.5.0 onward
For version 4.5.0 onward please check the notes attached to the releases here: https://github.com/kephas-software/kephas/releases.

<a name="4.5.0"></a>
# [4.5.0-beta03](https://github.com/quartz-software/kephas/compare/v4.5.0-beta03...master) (2018-04-12)

* Core
  * Removed the IIdentityProvider service. Made the DataContext accept an IIdentity instead of IIdentityProvider in constructor.
  * Fixed the TypeExtensions.IsCollection.
  * Added TypeExtensions.IsDictionary extension method.

* Data.Linq
  * Add support for Join, GroupJoin, Select in the SubstituteTypeExpressionVisitor.
  * Removed obsolete NetStandard 1.3 code in SubstituteTypeExpressionVisitor.

* Data.MongoDB
  * Fixed virtual members in MongoDataContext.
  
* Data
  * The data context retrievs its context identity from the provided initialization context.
  
* Messaging
  * Added the context to the message broker methods.

* Other
  * Remove Kephas.Web.Owin - will be replaced by Kephas.AspNetCore.
  * Made Kephas .NET Standard project reference v. 2.0.
  * Converted all .NET standard tests to .NET Core 2.0.
  * Merged Kephas.Platform into Kephas.Core.

# [4.2.0-beta02](https://github.com/quartz-software/kephas/compare/v4.2.0-beta02...v4.5.0-beta03) (2018-03-27)

* Messaging
  * Refactored the distributed Endpoint to accept a URL. This opens up the possibility of http/s communication. The message broker will have multiple registered dispatchers, handling a specific URI scheme (app - the app farm, http/s - REST API, so on).
  * Made the MessageProcessingContext less restrictive regarding the Message and handler.

* Core.Application
  * Added the RequiredFeature attribute to Core 
  
* Core.Services
  * Added the ServiceNameAttribute and made the registration od default metadata attribute types public in the AppServiceContractAttribute.
  * Added the named service provider service. 
  
* Core.Dynamic
  * Made setting a value to null in a dictionary based expando remove the entry from the dictionary. This ensures that the dynamic values contain only meaningful data, reducing the size of them.
  * Added the LazyExpando variation to allow lazy value evaluation/providing.

* Model
  * Added a protected logger inside the NamedElementBase.
  * Made the AppService "base type"-less. Any possible base types are considered only mixins.
  * Unified the projected type resolvers: IProjectedTypeResolver, IEntityTypeResolver, IDataIOProjectedTypeResolver, moved to Core.Model.
  
* Data
  * Added support for initial data handling.
  * Improved Ref - GetEntityInfo must not return null.
  * Improved the extensibility of the DefaultDataConversionService.
  * Fixed CollectionAdapter.CopyTo.
  * Fixed the DefaultDataConversionService to identify the most specific type for source and target, to be able to identify most converters applicable.
  * Added a ServiceRef primitive type similar to Ref, but referencing runtime services. The name of the service (ServiceName attribute) will be persisted.
  
* Data.Client
  * Added more information when member not found in client query conversion.
  * Optimized DataContextQueryableSubstituteTypeConstantHandler.
  * Made SubstituteTypeExpressionVisitor better specializable: TryGetImplementationType/Core made virtual protected.
  * Added more options/flexibility to the SubstituteTypeExpressionVisitor.
  
* Data.Model.Abstractions (new)
  * Added the IIdentifiable and IHierarchyNode abstractions in DataModel.
  
* Serialization.ServiceStack.Text
  * Added JsonExpando class to Kephas.Serialization.ServiceStack.Text.
  * Made the TypeAttr in DefaultJsonSerializerConfigurator configurable, added localization.
  
* Kephas.Web.ServiceStack (new)

* Logging.NLog
  * Upgraded to NLog 4.5 for net standard.

# [4.2.0-beta01](https://github.com/quartz-software/kephas/compare/v4.2.0-beta01...v4.2.0-beta02) (2018-02-15)

* Platform
  * Removed obsolete WithNet45AppEnvironment extension method .
* Model
  * Added the AppService classifier with the corresponding registry. Now it is possible to iterate over all declared app services from the ModelSpace.
* Mail
  * **BREAKING CHANGE** Refactored the mailing infrastructure to allow a better extensibility 
  * Added EmailSenderServiceExtensions for simplified email sending.
  * Modified the EmailSettings.Port to be nullable int, not string.
  * Added IContext to IEmailSendService.SendAsync for further control.
  * Removed the obsolete Kephas.Mail.Services.Net45 project.
  * Added unit tests for mailing.
* Unit Testing
  * Upgraded NUnit to 3.9 and NSubstitute to 3.1.0 

<a name="4.1.1"></a>
# [4.1.1](https://github.com/quartz-software/kephas/compare/v4.1.0...v4.1.1) (2018-02-01)

* Core.Services
  * Added the IFinalizable and IAsyncFinalizable interfaces to pair the IInitializable and IAsyncFinalizable.

<a name="4.1.0"></a>
# [4.1.0](https://github.com/quartz-software/kephas/compare/v4.0.0...v4.1.0) (2018-01-21)

* Core
  * Made AppServicesBuilder accept IAmbientServices (interface) instead of AmbientServices (class).
  * Made DefaultTypeResolver.ResolveTypeCore virtual.
  * Fixed the DefaultTypeResolver when the type is not found and it should not throw exception. Also throws TypeLoadException if type not found.
  * Made the ThreadContextAwaiter not step into when debugging.
  * RuntimePropertyInfo.CanWrite and CanRead made virtual, taking into consideration whether a setter/getter is available.
  * Improved RuntimePropertyinfo.ToString().

* Core.Application
  * Refactored the AppBase to not call stop from start.
  * Renamed AppBase.StartApplicationAsync to BootstrapAsync
  * Renamed AppBase.StopApplicationAsync to ShutdownAsync
  * Added SignalShutdown to AppContext.
  * Added tests to AppBase.
  * Made the FeatureInfo.FromMetadata public so that the default calculation of FeatureInfo from metadata is publicly exposed.
  * Made AppContext get the AppManifest (if not provided) from the DI container.
  * Added the AppBase application root class.
  * Added some more DefaultAppManager tests.
  * Added IManifest.ContainsFeature and removed failed features from the app manifest.
  * Improved AppBase behavior on failed Bootstrapping, by calling a forced shutdown if the initialization procedure was started.

* Core.Dynamic
  * Made the Expando recognize dictionaries when passed as objects.
  * Made IExpando.Merge return typed expandos.
  * TrySetValue does not throw exceptions anymore for read only properties, instead returns false. Only TrySetMember returns MemberAccessException.
  * ExpandoBase fixed the ToDictionary with duplicated property.
  * Added Expando.Merge extension method.

* Core.Sets
  * Improved PartialOrderedSet error message upon cycles.

* Data
  * Added the ExecuteCommand for issuing commands to the data store.
  * ConnectionStringParser.Parse refactored to AsDictionary and AsExpando.
  * Added more unit tests for DataContextExtensions.
  * Improved the ExecuteResult constructor.
  * Added constructor for DataValidationResult from exception.
  * Improved Ref, added unit tests.
  * FindCommand & FindOneCommand refactorized to use the local cache, if possible; tests added.
  * Added better support for rejecting empty IDs in find.
  * Added DataConversionContextBuilder, made properties of IDataConversionContext read-write.
  * Skipping validation of an entity part if no entity type info provided for it (PersistChangesCommand).
  * Added the ChangeSet to Save in PersistChangesContext and also the Iteration.
  * Refactored FindCommandBase for better specialization.
  * Try to resolve the target entity if a non-empty ID provided, even for new entities (DefaultDataConversionService).
  * Fixed a bug in the SubstituteTypeExpressionVisitor.
  * Added overridable IRef.IsEmpty, used in RequiredRef attribute.
  * Added IterationChangeSet to PersistChangesContext.
  * Fixed the type of operationContext to be IPersistChangesContext in PersistChangesCommand.ValidateEntityAsync.
  * Added IEntityInfo.IsChanged(property).
  * Added support for custom conversion target resolvers (for example based on natural keys).
  
* Data.Linq
  * Added support for simplifying conversion expressions when they convert to a base type.
  * Exclude the value types (for example nullable comparisons) from simplifying expressions.* Data.Endpoints
  * Added Options to QueryMessage to pass control values to the server.

* Data.IO
  * Added behaviors to the data import service.
  * Made DataIOResult expando.
  * Added configuration possibilities for DataExport.
  * Added context configuration possibilities in DataImportService.
  * Added data I/O context extension methods.
  * Added possibility of configuring the SerializationContext in both export and import.
  * Data export accepts now also an enumeration of client data as an alternative to client query.
  
* Data.Model
  * Added Loose kind to the EntityPartAttribute.

* Data.Client
  * Fixed method name WithClientQueryExecutionContext to WithClientQueryExecutionContextConfig.
  * Added ClientQueryExecutionContext extensions.
  * Added support for Order in ClientQuery.
 
* Model
  * Optimized the Property in GetValue and SetValue to use a RuntimePropertyInfo from its parts.
  * Made NamedElementBase.Parts available as protected.

* Serialization
  * Added Kephas.Serialization.ServiceStack.Text.

* Serialization.ServiceStack.Text
  * Made the Kephas.Serialization.ServiceStack.Text.Net45 use the signed version of ServiceStack.Text.
  * Made ServiceStack serialization deserialize objects to dictionaries.
  * ServiceStack.Text: serialized date with JsConfig.DateHandler = DateHandler.ISO8601.

* Logging.NLog
  * Made NLog target the 4.5.0 version instead of 5.0.0.
  * Upgraded to NLog 4.5-rc02.

* Mail
  * Moved (System)EmailSenderSettings to Kephas.Mail.
  * Published the Kephas.Mail.MailKit - Kephas.Mail.Services will be marked as obsolete.

* Samples
  * Fix FileNotFound exception in .NET Core samples + fixed debugging problem of .NET Core.
  * Upgraded samples to Kephas 4.1.0-beta03.
  
* API Documentation
  * docFX generated API documentation + docFX tool.

* NuGet Packages
  * Added src to the symbols packages.
