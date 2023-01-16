# Data

## Introduction
Provides abstractions for managing data: retrieval, persistence, query.

Data is one of the most important parts of an application. To leverage working with it, Kephas provides an abstract data infrastructure which allows data manipulation (CRUD), validation and support for various behaviors.

Typically used areas and classes/interfaces/services:
* ```IDataSpace```, ```IDataContext```.
* Capabilities: ```IEntityEntry```, ```EntityEntry```.
* Conversion: ```IDataConversionService```, ```IDataConverter```, ```DataConverterBase```.
* DataSources: ```IDataSourceService```, ```IDataSourceProvider```.
* Behaviors: ```IDataBehavior```, ```DataBehaviorBase```, ```QueryBehaviorBase```.
* Analysis: ```IRefPropertiesProvider```.
* Setup: ```IDataSetupManager```, ```IDataInstaller```.
* Validation: ```IOnValidateBehavior```.

## The general architecture
All data operations are performed through a _data context_. The data context is responsible for holding and managing a local cache of data and for instantiating [[commands|Data-commands]]. The commands are actually the performers of data operations, integrate data behaviors, and are tightly coupled to the data context that created them.

Data contexts are created by a _data context factory_, which is a singleton application service, by providing a data store name. The factory uses configured data store information and associated services to get it and to initialize the data context.

### The _data context_

The _data context_ is the entry point in regard to data operations. Its main responsibilities are:

* _Query the data_ and retrieve the results. There are no restrictions regarding the implementation of the data querying, so it's up to the implementation to use, for example, a first or a second level cache. All the information controlling how the querying should be performed may be specified in the `queryOperationContext` parameter, which is an expando that can be customized freely.

* Create the _commands_. As explained in the following, the strategy for creating the commands will choose the most appropriate one for the calling data context.

* Provide _entity information_. This is information attached to each entity in the internal cache regarding its ID, change state, original data, and many more.

* _Attach_ and _detach_ entities. These operations add/remove entities to/from the internal cache.

Apart from these, the data context:

* _is a contextual object_: the consumers may access the [[app services|Application-Environment-and-Lifetime-Management#ambient-services]] and use the data context as an [[expando|Dynamic-expandable-objects-(expandos)]] object, dynamically adding and accessing values to it/from it.

* _is initializable and disposable_: implements the `IInitializable` and `IDisposable` interfaces to control its lifetime.

### The _data context factory_

Data contexts are dependent on initialization data, which typically contains at least data store connection information. Due to the fact that multiple physical data stores may be served by the same data context implementation and that by design, at one time, a data context instance may be connected to a single physical data store, there must be a factory service that creates a data context for a given connection. This is the _data context factory_ application service.

* _CreateDataContext(dataStoreName, [initializationContext]): IDataContext_: This is the only method of the factory service which creates a data context instance and initializes it.
    * _dataStoreName_: indicates the name of the data store. This identifies typically an entry in the configuration where connection information and other initialization data is provided.
    * _initializationContext_: provides contextual information for data context initialization.

> Note: The consumer of the _CreateDataContext_ method takes the full responsibility of disposing correctly the received _data context_.

### The _data commands_

The [[data commands|Data-commands]] extend the data context functionality beyond the minimalist one defined by the `IDataContext` interface. Kephas provides built-in commands for the following data operations:

* Create entity
* Delete entity
* Persist changes
* Discard changes
* Find by ID
* Find by criteria

These operations provide the basic infrastructure for data manipulation, but for some scenarios this might not be enough. For example, _update_ commands fired against all entities matching a specific criteria, calling stored procedures, or supporting all kinds of _upsert_ variations are specific cases where there is no built-in support. With Kephas, the developer is free to define as many commands as needed, tailored for specific needs and data stores of all kinds. For a step-by-step tutorial, check the [[data commands|Data-commands]] page.

## The `IDataCommand` interface and `DataCommandBase` base class
A data command has basically only one method: `ExecuteAsync`, which takes an operation context as parameters and returns a promise (task) of a result. The base interface to be implemented by a command is `IDataCommand`, which has its generic flavor `IDataCommand<TOperationContext, TResult>`. However, to ease the definition of commands, a base class `DataCommandBase<TOperationContext, TResult>` is provided, where only the generic ExecuteAsync method should be implemented.

```C#
    public interface IDataCommand
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        Task<IDataCommandResult> ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for data commands, with typed operationContext and result.
    /// </summary>
    /// <typeparam name="TContext">Type of the operation context.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IDataCommand<in TOperationContext, TResult> : IDataCommand
        where TOperationContext : IDataOperationContext
        where TResult : IDataCommandResult
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        Task<TResult> ExecuteAsync(TOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
```

## Steps in implementing a data context command

### Define the command application service contract

The data context uses composition to get an instance of the command to use, so the commands will provide an application service contract and one or more service implementations.

Multiple command implementations may be provided targeting specific data context implementations (different data context implementations may be provided for relational databases, NoSQL databases, or graph databases), so that the targeted data context finds the appropriate command to use. For example, a `MongoDBFindOneCommand` will indicate that it targets a `MongoDBDataContext`.

> Note: When the data context creates the command through composition, it must get a new instance, otherwise unexpected behavior may occur. Therefore it is strongly discouraged to mark the application service contracts as _shared_.

> Note: Together with the application service contract, the specific operation context type (where the input parameters will be provided) and the specific expected result type will be defined, if the command requires it. They must specialize the `IDataOperationContext` and `IDataCommandResult` respectively.

```C#
    /// <summary>
    /// Contract for find commands retrieving one entity based on a predicate.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, 
                        MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IFindOneCommand : IDataCommand<IFindOneContext, IFindResult>
    {
        // AllowMultiple = true indicate that multiple implementations may be provided.
        // DataContextTypeAttribute as metadata attribute indicate that
        // the implementations may specify a target data context type.
    }

    /// <summary>
    /// Interface for data operation contexts of the <see cref="IFindOneCommand"/>.
    /// </summary>
    public interface IFindOneContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the criteria of the entity to find.
        /// </summary>
        Expression Criteria { get; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether to throw an exception if an entity is not found.
        /// </summary>
        bool ThrowIfNotFound { get; }
    }

    /// <summary>
    /// Generic interface for data operation contexts of the <see cref="IFindOneCommand"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public interface IFindOneContext<TEntity> : IFindOneContext
    {
        /// <summary>
        /// Gets the criteria of the entity to find.
        /// </summary>
        /// <remarks>
        /// Overrides the untyped expression from the base interface
        /// to provide LINQ-support.
        /// </remarks>
        new Expression<Func<TEntity, bool>> Criteria { get; }
    }

    /// <summary>
    /// Interface for the find result.
    /// </summary>
    public interface IFindResult : IDataCommandResult
    {
        /// <summary>
        /// Gets the found entity or <c>null</c> if no entity could be found.
        /// </summary>
        object Entity { get; }
    }

```

### Provide data context specific implementation of the command

After the service contract was defined, the service implementing it is created. For convenience, the base class `DataCommandBase<TOperationContext, TResult>` may be used.

> Important: Do not forget to annotate the command service with the targeted data context type. The match is not exact, but done on a compatibility basis. This means that if a data context instantiating a command may find multiple being compatible with it (target compatible types, like the base type `DataContextBase`). The current strategy will choose the command targeting the most specific data context.

```C#
    /// <summary>
    /// Base class for find commands retrieving one result.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class FindOneCommand : DataCommandBase<IFindOneContext, IFindResult>, IFindOneCommand
    {
        //... implement the command execution
    }

    // The command below targets a very specific data context, the MongoDataContext,
    // while the one above should work for all specializing the `DataContextBase`.

    /// <summary>
    /// Command for persisting changes targeting <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoPersistChangesCommand : PersistChangesCommand
    {
        //... implement the command execution
    }

```

### Provide data context extension methods to make command consumption easier

Even if we are ready with the new command, it is not very handy to use it. Here is how we would use it now:

```C#
    var command = dataContext.CreateCommand<IFindOneCommand>();
    var findContext = new FindOneContext
                      {
                           Criteria = criteria,
                           ThrowIfNotFound = false,
                      }
    var result = await command.ExecuteAsync(findContext).PreserveThreadContext();
    var foundEntity = result.Entity;
    // yupee, got the entity! but it was a loooong way to get there :(.
```

So, to achieve the simplicity of simply calling the command on the data context, the next step would be to provide an extension method to wrap up all this stuff.

```C#
    public static class DataContextExtensions
    {
        //...

        public static async Task<T> FindOneAsync<T>(
            this IDataContext dataContext, 
            Expression<Func<T, bool>> criteria,
            bool throwIfNotFound = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Requires.NotNull(dataContext, nameof(dataContext));
            Requires.NotNull(criteria, nameof(criteria));

            var findOneContext = new FindOneContext<T>(dataContext, criteria, throwIfNotFound);
            var command = (IFindOneCommand)dataContext.CreateCommand(typeof(IFindOneCommand));
            var result = await command.ExecuteAsync(findOneContext, cancellationToken).PreserveThreadContext();
            return (T)result.Entity;
        }
    }
```
Now we can use the defined command as if it was a method of the data context:

```C#
    var foundEntity = await dataContext.FindOneAsync<Customer>(
                          c => c.Name == "John" && c.FamilyName == "Doe",
                          throwIfNotFound: false).PreserveThreadContext();
    // way better :)
```

## Synchronous commands
Most data operations are by design asynchronous, but some do not need this overhead, for example because they work with data in the local cache, like marking entities for deletion or discarding the in-memory changes. For such data commands, they need to implement the `ISyncDataCommand` interface or, more comfortable, specialize `SyncDataCommandBase`.

## Securing the data

Entity types may be secured by decorating them with the [SupportsPermission] attribute.

Example:

```csharp
    /// <summary>
    /// The customer entity type.
    /// </summary>
    [SupportsPermission(typeof(IAdminPermission))]
    public interface ICustomer : ...
    {
    }
```
> Note: If a mixin declares supported permission types, all entity types inheriting that mixin will support those permissions, too.


## Other resources

* [Kephas.Application.Abstractions](https://www.nuget.org/packages/Kephas.Application.Abstractions)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
