<a name="4.1.0-rc.10"></a>
# [4.1.0-rc.10](https://github.com/quartz-software/kephas/compare/v4.0.0...master) (2017-12-21)

* Core.Application
  * Made the FeatureInfo.FromMetadata public so that the default calculation of FeatureInfo from metadata is publicly exposed.
  * Made AppContext get the AppManifest (if not provided) from the DI container.
  * Added the AppBase application root class.
  * Added some more DefaultAppManager tests.
  * Added IManifest.ContainsFeature and removed failed features from the app manifest.
* Core.Sets
  * Improved PartialOrderedSet error message upon cycles.
* Mail
  * Moved (System)EmailSenderSettings to Kephas.Mail.
  * Published the Kephas.Mail.MailKit - Kephas.Mail.Services will be marked as obsolete.
* Data
  * Added the ExecuteCommand for issuing commands to the data store.
* Logging.NLog
  * Made NLog target the 4.5.0 version instead of 5.0.0.

* Samples
  * Fix FileNotFound exception in .NET Core samples + fixed debugging problem of .NET Core.
  
* Core
  * Made AppServicesBuilder accept IAmbientServices (interface) instead of AmbientServices (class).
* Core.Application
  *  Refactored the AppBase to not call stop from start.
  * Renamed AppBase.StartApplicationAsync to BootstrapAsync
  * Renamed AppBase.StopApplicationAsync to ShutdownAsync
  * Added SignalShutdown to AppContext.
  * Added tests to AppBase.
* Data
  * ConnectionStringParser.Parse refactored to AsDictionary and AsExpando.
  * Added more unit tests for DataContextExtensions.
  
* API Documentation
  * docFX generated API documentation + docFX tool.
* Data
  * Improved the ExecuteResult constructor.
* Data.IO
  * Added behaviors to the data import service.
* Samples
  * Upgraded samples to Kephas 4.1.0-beta03.
  
* Core
  * Made DefaultTypeResolver.ResolveTypeCore virtual.
  * Fixed the DefaultTypeResolver when the type is not found and it should not throw exception. Also throws TypeLoadException if type not found.
* Data
  * Added constructor for DataValidationResult from exception.
* Data.IO
  * Made DataIOResult expando.
  * Added configuration possibilities for DataExport.
  * Added context configuration possibilities in DataImportService.
* Serialization
  * Added Kephas.Serialization.ServiceStack.Text.

* Core.Application
  *  Improved AppBase behavior on failed Bootstrapping, by calling a forced shutdown if the initialization procedure was started.
* Core.Dynamic
  * Added Expando.Merge extension method.
* Data.Endpoints
  * Added Options to QueryMessage to pass control values to the server.
* Data.IO
  * Added data I/O context extension methods.
  * Added possibility of configuring the SerializationContext in both export and import.
  * Data export accepts now also an enumeration of client data as an alternative to client query.

* Core
  * Made the Expando recognize dictionaries when passed as objects.
* Data.Client
  * Fixed method name WithClientQueryExecutionContext to WithClientQueryExecutionContextConfig.
  * Added ClientQueryExecutionContext extensions.
* Serialization.ServiceStack.Text
  * Made the Kephas.Serialization.ServiceStack.Text.Net45 use the signed version of ServiceStack.Text.
  * Made ServiceStack serialization deserialize objects to dictionaries.

* Core
  * Improved RutimePropertyinfo.ToString().
* Data
  * Improved Ref, added unit tests.
  * FindCommand & FindOneCommand refactorized to use the local cache, if possible; tests added.
  * Added better support for rejecting empty IDs in find.
  
* Core
  * Made the ThreadContextAwaiter not step into when debugging.
  * Made IExpando.Merge return typed expandos.
  * TrySetValue does not throw exceptions anymore for read only properties, instead returns false. Only TrySetMember returns MemberAccessException.
  * RuntimePropertyInfo.CanWrite and CanRead made virtual, taking into consideration whether a setter/getter is available.

* Data
  * Added DataConversionContextBuilder, made properties of IDataConversionContext read-write.
  * Skipping validation of an entity part if no entity type info provided for it (PersistChangesCommand).
  * Added the ChangeSet to Save in PersistChangesContext and also the Iteration.

* Data.Client
  * Added support for Order in ClientQuery.
  
* Data
  * Refactored FindCommandBase for better specialization.
  * Try to resolve the target entity if a non-empty ID provided, even for new entities (DefaultDataConversionService).
  
* NuGet Packages
  * Added src to the symbols packages.
* Logging.NLog
  * Upgraded to NLog 4.5-rc02.
* Data
  * Fixed a bug in the SubstituteTypeExpressionVisitor.
  
* Data
  * Added overridable IRef.IsEmpty, used in RequiredRef attribute.
  
* Data
  * Added IterationChangeSet to PersistChangesContext.
  * Fixed the type of operationContext to be IPersistChangesContext in PersistChangesCommand.ValidateEntityAsync.
  * Added IEntityInfo.IsChanged(property).
* Model
  * Optimized the Property in GetValue and SetValue to use a RuntimePropertyInfo from its parts.
  * Made NamedElementBase.Parts available as protected.
  
* Data.Model
  * Added Loose kind to the EntityPartAttribute.
  
* Data
  *  Added support for custom conversion target resolvers (for example based on natural keys).
  
* Core
  * ExpandoBase fixed the ToDictionary with duplicated property.
  
* Serialization.ServiceStack
  *  ServiceStack.Text: serialized date with JsConfig.DateHandler = DateHandler.ISO8601.
* Data.Linq
  * Added support for simplifying conversion expressions when they convert to a base type.
  
* Data.Linq
  * Exclude the value types (for example nullable comparisons) from simplifying expressions.