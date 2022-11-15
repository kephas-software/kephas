# MongoDB

## Introduction
This package adds integration between the MongoDB driver and Kephas.

Typically used areas and classes/interfaces/services:
* ``IMongoClientProvider``, ``IMongoSerializer<T>``, ``IMongoConventionsProvider``.

## Usage

Follow these steps:
* Configure the connection, for example in the ``mongoSettings.json``.
* Inject the ``IMongoDatabaseProvider`` into the conjsumer class, typically a repository class. Alternatively, if work with multiple databases is required, the ``IMongoClientProvider`` service can be injected instead.

#### Example configuration (``mongoSettings.json``)

```json
{
  "DataStore": {
    "ConnectionString": "mongodb://my-user:super-password@server:27017",
    "DbName": "my-database"
  }
}
```

#### Example code

```csharp
// consume MongoDB services in the repository
public class EventRepository : IEventRepository, IMongoRepository
{
    private readonly ILogger<EventRepository> logger;
    private readonly IMongoDatabaseProvider databaseProvider;
    private readonly Pluralizer pluralizer;

    public EventRepository(ILogger<EventRepository> logger, IMongoDatabaseProvider databaseProvider)
    {
        this.logger = logger;
        this.databaseProvider = databaseProvider;
        this.pluralizer = new Pluralizer();
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        var db = databaseProvider.GetDatabase();
        return db.GetCollection<T>(pluralizer.Pluralize(collectionName));
    }

    public async Task<T> GetById<T>(string id) where T : IDomain
    {
        var query = await GetCollection<T>().FindAsync(f => f.Id == id);
        return await query.FirstOrDefaultAsync();
    }
}
```

## The ``IMongoClientProvider`` singleton service

This service is the entry point of MongoDB connectivity. It has only one method: ``GetMongoClient(): IMongoClient``.
The default implementation (``MongoClientProvider``) ensures that:
* only one instance of ``MongoClient`` is used as
explained [here](https://stackoverflow.com/questions/63304279/re-use-mongodb-mongoclient-in-asp-net-core-service).
* the convention providers and the BSON serializers are registered properly, in the order of their declared priority,
**before** the first use of the ``MongoClient``.

## Convention providers

Conventions control the mapping between the data in the database and the .NET classes.
A convention provider implements ``IMongoConventionsProvider``, which provides a record with the shape: ``(name, conventions pack, match)``.
* _name_: the conventions registered name.
* _conventions pack_: the pack containing the list of conventions.
* _match_: a function selecting the classes to which the conventions are applied. 

The default conventions provider registers two conventions:
* ``CamelCaseElementNameConvention``: the JSON property names are persisted as camel case.
* ``IgnoreExtraElementsConvention(true)``: if the database entity contains more properties than the .NET class, these properties are ignored.

Additionally, the default conventions provider is registered with a ``Low`` priority, so that the custom conventions providers have by default a higher priority:

The value in the ``MongoSettings.EntityNamespaceConventions`` controls to which classes the default conventions are applied.
* If not specified, all classes are matched.
* If a list of namespaces are specified, all classes in a namespace starting with one of the given namespaces is matched.

#### Example for enabling the default conventions only in certain namespaces

```csharp
{
  "ConnectionString": "mongodb://my-user:super-password@server:27017",
  "DbName": "my-database",
  "EntityNamespaceConventions": [ "My.Domain1.Model", "My.Domain2.Model" ]
}
```

### Extensibility
To add a new conventions provider:
* define a class implementing ``IMongoConventionsProvider``.
  * control the order in which it is registered by providing a ``[ProcessingPriority]`` attribute.

```csharp
// define the conventions provider and register it with the high priority
[ProcessingPriority(Priority.High)]
public class MyMongoConventionsProvider : IMongoConventionsProvider
{
    public MongoConventions GetConventions() =>
        new("my-conventions", GetConventionPack(), t => t.Name.StartsWith("My"));

    private ConventionPack GetConventionPack()
    {
        return new ConventionPack
        {
            new IgnoreExtraElementsConvention(false),
        };
    }
}
```

## BSON serializers
BSON serializers control the serialization of values. By default, serializers for ``Guid`` and ``Nullable<Guid>`` types are provided, which serialize them as strings.

### Extensibility

To add a new custom serializer:
* define a class implementing ``IMongoSerializer<T>``

#### Example of custom serializer using existing MongoDB serializers

```csharp
// define the serializer
public class MongoDateTimeOffsetSerializer : DateTimeOffsetSerializer, IMongoSerializer<DateTimeOffset>
{
    public MongoDateTimeOffsetSerializer()
        : base(BsonType.String)
    {
    }
}
```

## Other resources

* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.
