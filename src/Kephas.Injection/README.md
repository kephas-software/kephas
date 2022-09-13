# Injection

## Introduction
A Kephas application uses internally all kinds of services, built-in and custom ones. Structurally, an **application service** has a *service contract* (an interface) declaring its API and one or more *service implementations* of this service contract. A good design keeps the services loosely coupled, ideally with no dependencies at the contract level; this approach has the big advantage of allowing the replacement of implementations due to new or changed requirements with no or minimum side-effects.

Typically used areas and classes/interfaces/services:
- Injection: ``IInjector``, ``IInjectorBuilder``, ``InjectorBuilderBase``.
- Services: ``IAppServiceInfo``, ``IAppServiceInfosProvider``, ``SingletonAppServiceContractAttribute``, AppServiceContractAttribute, OverridePriorityAttribute, ProcessingPriorityAttribute.
- ``IAmbientServices``, ``AmbientServices``.

> Consuming an application service implies depending on its contract and never on its implementation.

Packages providing specific dependency injection implementations:
* [Kephas.Injection.Autofac](https://www.nuget.org/packages/Kephas.Injection.Autofac)
* [Kephas.Extensions.DependencyInjection](https://www.nuget.org/packages/Kephas.Extensions.DependencyInjection)
* [Kephas.Injection.Lite](https://www.nuget.org/packages/Kephas.Injection.Lite)

## `IAmbientServices` interface
This collection holds the service registrations. 

### Registering default services
The `IAmbientServices.RegisterInitializer` static method registers a callback to be invoked when initializing ambient services.
Typically, ambient services initializers are registered in [assembly initializers](https://www.nuget.org/packages/Kephas.Abstractions#assembly-initialization).

```csharp
/// <summary>
/// Assembly initializer for Kephas.Injection.
/// </summary>
public class InjectionAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.RegisterInitializer(ambient => ambient.Register<ILogManager, NullLogManager>());
        IAmbientServices.RegisterInitializer(ambient => ambient.Register<ILocationsManager, FolderLocationsManager>());
    }
}
```

## `AmbientServices` implementation

When an `AmbientServices` instance is created, the initializer callbacks are invoked to add the default services.

> Note: there is no guarantee about the order in which these services are added.

#### Example
```csharp
var ambientServices = new AmbientServices()
    .BuildWithAutofac();

var logger = ambientServices.Injector.Resolve<ILogger<MyComponent>>();
```

## Aims of application services design
* Provide an internal application SOA.
* Declare the expected behavior at the service contract level, not at the implementation level.
* Support metadata.
* Possibility to override a service implementation in a declarative way.
* Possibility to prioritize the service implementations where a collection of them should be used.

## Steps for defining an application service

* Define the application service contract and configure it using one of the `AppServiceContract` attributes:
`[SingletonAppServiceContract]`, `[ScopeSingletonAppServiceContract]`, or `[AppServiceContract]` attributes.
For the semantics of each of the attributes, please see below.
* Implement one or more application services based on the contract defined in the step above.
  * Note: for contracts not allowing multiple service implementations, it is a recommended practice to decorate the service implementation
with the `[OverridePriority]` attribute. See more on this feature below.
* Consume the service.

## Consuming application services

### Dependency injection

The dependency injection is the recommended way of consuming services, either in the class constructor or in writable public properties.

> By design, the injection infrastructure does not require any attributes to be placed on the imports,
> because it can infer which parameters or attributes need to be imported by their respective type.
> The single requirement is for the types to be registered as service contracts.

```C#
    public class DefaultModelSpaceProvider : IModelSpaceProvider
    {
        // A collection of services imported through the class constructor.
        public DefaultModelSpaceProvider(ICollection<IModelInfoProvider> modelInfoProviders, IModelContainer modelContainer)
        {
            //...
        }

        // Service injected in property.
		public IEmailService EmailService { get; set; }
        
        //...
		
    }
```

### Service location

There are cases when only the injector is known and, starting from it, some services need to be consumed.
In this case, the injector's `Resolve`/`TryResolve` or `ResolveMany` methods may be used. Such cases are static contexts or class' code not included in injection.

```C#
var ambientServices = new AmbientServices()
        .WithSerilogManager()
        .WithDynamicAppRuntime()
        .BuildWithAutofac();

var logManager = ambientServices.Injector.Resolve<ILogManager>();
```

> This is not a recommended approach because the methods using service location are not easily testable.
> Additionally, because the dependency is "hidden" in code, no automatic tools can be used to identify
> the dependency and use it for code analysis and refactoring purposes.

## Conventions

* Null service implementations: these are service implementation that do not do anything.
They are just there so that the injection finds at least one implementation of a service contract.
* Default service implementations: these are service implementations that are defined at the highest abstraction layer where they can be defined.
They provide a useful functionality, but they can be overridden by more specialized services.

## Configuring the application service contracts

The approach Kephas takes is a bit different from what the existing dependency injection frameworks do.
In Kephas, the emphasis is set on the service contract and that's why we configure the services at this level, almost entirely.

### Singleton, scoped, or instance based services

* For a singleton service, one single instance will be available within the injector. Use the `[SingletonAppServiceContract]` attribute in this case.
* For a scoped service, one single instance will be available within a scope created by the injector. Use the `[ScopedAppServiceContract]` attribute in this case.
* For an instance based service, each dependency will trigger the creation of a new instance. Use the `[AppServiceContract]` attribute in this case.

### Single or multiple mode

There are cases when for the same service contract multiple service implementations are required.
An example is when multiple partial converters can be defined between two types, and all these converters must be applied.
In such a case, set the `AllowMultiple` option to true in the contract declaration. By default, the services use the single mode.

#### Example:

```C#
    [AppServiceContract(AllowMultiple = true)]
    public interface IConverter<TSource, TTarget>
    {
        ConversionResult Convert(TSource source, TTarget target);
    }
```

> Generic application service contracts allow multiple registrations by default,
> because it is expected that multiple services will be defined with different actual generic type parameters.

### Custom metadata
Sometimes it is required to provide custom metadata at the service implementation level, especially for services in multiple mode. This metadata can be collected by the injection infrastructure and provided at the injection time for use in the client services. Because the infrastructure collects this metadata, it needs the information about it. In Kephas, this is implemented by setting the **MetadataAttributes** property to an array of attribute types exposing the metadata.

The following conventions are applied:
* The metadata values are collected through metadata attributes:
  * If the attribute implements `IMetadataValue<TValue>` interface, then the `Value` property will provide
the value associated to the metadata key, and the attribute type name without the “Attribute” suffix will be the metadata key.
For example, an `EntityTypeAttribute` will generate metadata with name **EntityType** and value as described previously.
  * All the attribute properties annotated with the `[MetadataValue]` attribute are collected as metadata.
This attribute accepts a value name as parameter. If a value name is provided, this name is used as metadata name,
otherwise `{AttributeMetadataName}{PropertyName}` will be the metadata name, where `AttributeMetadataName` is the attribute type name without the “Attribute” suffix,
and `PropertyName` is the name of the annotated property holding the value.
* The attributes must be set at the service implementations level.

#### Example

```C#
    // This is the attribute providing the metadata
    public class OperationAttribute : Attribute, IMetadataValue<string>
    {
        public OperationAttribute(string operation)
        {
            this.Value = operation;
        }

        object IMetadataValue.Value => this.Value;

        public string Value { get; }

        [MetadataValue]
        public string Name { get; } // this property will generate a metadata with name OperationName.
    }	

    // The two classes below define the custom Operation metadata attribute.
    [Operation("+", Name = "Addition")]
    public class AddOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 + op2;
    }

    [Operation("-", Name = "Subtraction")]
    public class SubtractOperation : IOperation
    {
        public int Compute(int op1, int op2) => op1 - op2;
    }

    // This is the declaration of the Operation metadata in the service contract.
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IOperation
    {
        int Compute(int op1, int op2);
    }
	
    // This is how custom metadata is consumed
    public class Calculator : ICalculator
    {
        private readonly IParser parser;

        public Calculator(ICollection<IExportFactory<IOperation, OperationMetadata>> operationFactories, IParser parser)
        {
            this.parser = parser;
            this.OperationsDictionary = operationFactories.ToDictionary(
                e => e.Metadata.Operation,
                e => e.CreateExport().Value);
        }

        public IDictionary<string, IOperation> OperationsDictionary { get; set; }

        public int Compute(string input)
        {
            var parsedOperation = this.parser.Parse(input, this.OperationsDictionary.Select(op => op.Key));
            var operation = this.OperationsDictionary[parsedOperation.Item2];
            return operation.Compute(parsedOperation.Item1, parsedOperation.Item3);
        }
    }
```

#### Built-in metadata attributes
The metadata attributes indicated below are predefined by the infrastructure and do not need to be specified by the service implementations.
* `ProcessingPriority`
* `OverridePriority`
* `ServiceName`

## Configuring the application service implementations

### Override priority

Let’s take the case when there is need for a service declared in single mode, but, for specialization reasons,
there are multiple service implementations defined in different application layers.
Such a scenario is not supported by default, because choosing one service implementation would be not deterministic.
To overcome this issue, Kephas allows declaring an `OverridePriority` at the service implementation level,
so that the injection infrastructure can make use of this information for detecting the implementation with the highest priority,
which will be used as the one implementation of its declaring contract.

The accepted values are `Lowest`, `Low`, `BelowNormal`, `Normal` (default value), `AboveNormal`, `High`, and `Highest`, but any integer value is accepted
- the lower the value, the highest the priority.

> This is a very powerful feature that allows the replacement of service implementation in a declarative way.
> Another benefit of this approach is that more specialized service implementations can be automatically discovered and used,
> without any other means of wiring up the setup of dependency injection container.

> Kephas exposes its default services either with a `Lowest` override priority for the Null service implementations,
> or with a `Low` priority for the rest of them, to allow an uncomplicated override in higher application layers.

#### Example

```C#
    /// <summary>
    /// Application service for processing requests.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IRequestProcessor
    {
        IResponse Process(IRequest request);
    }

    /// <summary>
    /// Provides the default implementation of the <see cref="IRequestProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRequestProcessor : IRequestProcessor
    {
        //...
    }

    //... 
	
    // the following service is defined at the domain application layer.
    // note that even if an OverridePriority would have not been specified,
    // the CustomRequestProcessor would have been elected as implementation
    // for the IRequestProcessor contract, because in this case the OverridePriority
    // is Normal, which is higher than Low.

    /// <summary>
    /// Provides a custom implementation of the <see cref="IRequestProcessor"/> application service contract.
    /// </summary>
    [OverridePriority(Priority.High)]
    public class CustomRequestProcessor : IRequestProcessor
    {
        //...
    }
```

### Processing priority
When importing services in multiple mode, sometimes it is needed to sort these services to provide a logical order of invoking their functionality.
Such a case may be a list of converters between two types, where they must be applied in a particular order.

#### Example

```C#
    [ScopeSingletonAppServiceContract(Scopes.AuthenticatedScope, AllowMultiple = true, ContractType = typeof(IConverter))]
    public interface IConverter<in TSourceType, in TTargetType> : IConverter
        where TSourceType : class
        where TTargetType : class
    {
    }

    [ProcessingPriority(Priority.AboveNormal)]
    public class BeforeUserToUserViewModelConverter : IConverter<IUser, IUserViewModel>
    {
        //...
    }

    // if no processing priority is specified, Priority.Normal is considered.
    public class DefaultUserToUserViewModelConverter : IConverter<IUser, IUserViewModel>
    {
        //...
    }

    // can also use integer values.
    [ProcessingPriority(100)]
    public class AfterUserToUserViewModelConverter : IConverter<IUser, IUserViewModel>
    {
        //...
    }


    // service consuming the model converters.
    public class DefaultConversionService : IConversionService
    {
        public DefaultConversionService(IList<IExportFactory<IConverter, ConverterMetadata>> lazyConverters)
        {
            this.converters = (from b in lazyConverters ?? new List<IExportFactory<IConverter, ConverterMetadata>>()
                              orderby b.Metadata.ProcessingPriority
                              select b.CreateExport().Value).ToList();
        }
    }
```

> Please read also the section related to the generic service contracts to learn about the metadata collected from the generic parameters.

### AppServiceImplementationType
The `AppServiceImplementationType` is metadata indicating the actual implementation of the application service. One of the most simple use cases is, when debugging, to see what is the actual implementation without creating the service.

## Generic application service contracts

### Generic service contract exported as closed generic type

When exposing generic application service contracts, Kephas will export the parts using the closed generic contract.

#### Example

```C#
    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract]
    public interface IRequestHandler<TRequest>
    {
        //...
    }
	
    public class ComputeRequestHandler : IRequestHandler<ComputeRequest>
    {
        //...
    }
	
    public interface ICalculator
    {
        //...
    }
	
    public class Calculator
    {
        public Calculator(IRequestHandler<ComputeRequest> handler)
        {
            // the handler parameter will receive with these service definitions
            // an instance of the ComputeRequestHandler class.
        }
		
        //...
    }
```

### Generic service contract with non-generic exported contract type

Generic service contracts may be declared with a contract type different than the generic interface itself. This is particularly useful in the multiple mode scenario, when the generic argument (or arguments) type is used as a discriminator later, when consuming the services.

#### Example

```C#
    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    public interface IRequestProcessingFilter
    {
    }

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract(AllowMultiple = true, ContractType = typeof(IRequestProcessingFilter))]
    public interface IRequestProcessingFilter<TRequest> : IRequestProcessingFilter
        where TRequest : IRequest
    {
    }

    // Metadata for <see cref="IRequestProcessingFilter"/>.
    public class RequestProcessingFilterMetadata : AppServiceMetadata
    {
        public RequestProcessingFilterMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            // we get the metadata as (key: string, value: object) pairs
            this.RequestType = (Type)metadata.TryGetValue(nameof(this.RequestType), null);
        }

        public RequestProcessingFilterMetadata(Type requestType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.RequestType = requestType;
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        public Type RequestType { get; }
    }
```

Consuming such services is pretty straightforward:

```C#
    public class RequestProcessor : IRequestProcessor
    {
        public RequestProcessor(ICollection<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>> filterFactories)
        {
        }
        
        // alternatively, could import the services through the means of a collection property.
        public IList<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>> FilterFactories { get; set; }

        // NOTE: both ways of service injection are illustrated above, for demo purposes. In real life use either of them, but not both at the same time for the same dependency.
		
        public ProcessRequest<TRequest>(TRequest request)
        {
            // use the automatically provided RequestType metadata.
            var filter = this.FilterFactories.FirstOrDefault(f => f.Metadata.RequestType == typeof(TRequest));
            filter?.Apply();
        }
    }
```

In the example above, the request processing filters are exported using the non-generic `IRequestProcessingFilter` contract type, so that all of them can be collected by the injection using the non-generic contract, and later decisions may be taken based on the generic type metadata.

#### Collected metadata
Additionally to the metadata collected by using the metadata attributes,
Kephas collects from the service implementations the actual generic types
and adds them to the existing injection metadata. The following rules are applies:
* The actual generic parameter is the metadata value.
* The adjusted name of the generic parameter is the metadata key.
The adjusted name is obtained by stripping the leading “T”, if specified, and appending “Type”, if not already there.

In the example above, for the service `IRequestProcessingFilter<TRequest>` the `RequestType` was collected without
declaring explicitly a metadata attribute for it (note also the transformation `TRequest` -> `RequestType`).

### Generic service contracts exported as open generic types
A special kind of generic service contracts are open generic exported contracts.
The key difference from the other generic contracts discussed previously is that the exported contract
is not a base non-generic contract, but the generic contract itself, and this is marked through setting
the ``AsOpenGeneric`` property to ``true``. Consequently, the exported parts are generic implementations
of that generic contract, and the imports are closed generics of it.

#### Example

```C#
    /// <summary>
    /// Defines a service contract for a logger associated to a specific service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface ILogger<TService> : ILogger
    {
    }

    /// <summary>
    /// NLog logger for the <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    public class NLogger<TService> : ILogger<TService>
    {
    }

    /// <summary>
    /// Model provider based on the .NET runtime and the type system.
    /// </summary>
    public class RuntimeModelInfoProvider : IModelInfoProvider
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger<RuntimeModelInfoProvider> Logger { get; set; }
    }
```

## Service behaviors

When dealing with _single mode services_, the main behavior is controlled by the _override priority_ settings.
However, for _multiple mode services_, there may be cases when some of these services should be disabled,
for example due to some application configuration options.
A typical case may be when some features should be disabled when the licensing model indicates this.
For such purposes, Kephas provides the _IServiceBehaviorProvider_ shared application service, which can be used to filter out disabled services of a kind.
It exposes the following methods:

* `WhereEnabled(services)`: These methods return from a list of services or export factories only those which are enabled.

### Enabled rule services for application services

These services provide the _enabled_ value of an application service
(implement the `IEnabledServiceBehaviorRule<TServiceContract>` contract, the base classes are `EnabledServiceBehaviorRuleBase<TServiceContract>` and `EnabledServiceBehaviorRuleBase<TServiceContract, TServiceImplementation>`)

* `CanApply(context: TContext): boolean`: returns a value indicating whether the rules applies in the provided context.
* `GetValue(context: TContext): IBehaviorValue<boolean>`: returns the behavior value, where additionally to the value itself may be provided explanatory messages.

### `DefaultServiceBehaviorProvider`

The `DefaultServiceBehaviorProvider` is the built-in implementation of the _IServiceBehaviorProvider_ application service.
It aggregates the enabled rule services and applies them to each of the service or export factory in the list, so that in the end only the enabled ones are returned.

### Example

```C#

    [OverridePriority(Priority.Low)]
    public class DefaultAppManager : IAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppManager"/> class.
        /// </summary>
        /// <param name="serviceBehaviorProvider">The service behavior provider.</param>
        /// <param name="appLifecycleBehaviorFactories">The application lifecycle behavior factories.</param>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        /// <param name="featureLifecycleBehaviorFactories">The feature lifecycle behavior factories.</param>
        public DefaultAppManager(
            IServiceBehaviorProvider serviceBehaviorProvider,
            ICollection<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>> appLifecycleBehaviorFactories,
            ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories,
            ICollection<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>> featureLifecycleBehaviorFactories)
        {
            this.AppLifecycleBehaviorFactories = appLifecycleBehaviorFactories == null
                                                     ? new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>()
                                                     : serviceBehaviorProvider.WhereEnabled(appLifecycleBehaviorFactories).ToList();

            this.FeatureManagerFactories = featureManagerFactories == null
                                               ? new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>>()
                                               : this.SortEnabledFeatureManagerFactories(
                                                   serviceBehaviorProvider.WhereEnabled(featureManagerFactories).ToList());

            this.FeatureLifecycleBehaviorFactories = featureLifecycleBehaviorFactories == null
                                                         ? new List<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>>()
                                                         : serviceBehaviorProvider.WhereEnabled(featureLifecycleBehaviorFactories).ToList();
        }

        //...

    }
```

## Other resources

* [Kephas.Injection.Lite](https://www.nuget.org/packages/Kephas.Injection.Lite)
* [Kephas.Injection.Autofac](https://www.nuget.org/packages/Kephas.Injection.Autofac)
* [Kephas.Extensions.DependencyInjection](https://www.nuget.org/packages/Kephas.Extensions.DependencyInjection)

> Kephas Framework ("stone" in aramaic) aims to deliver a solid infrastructure for applications and application ecosystems.

