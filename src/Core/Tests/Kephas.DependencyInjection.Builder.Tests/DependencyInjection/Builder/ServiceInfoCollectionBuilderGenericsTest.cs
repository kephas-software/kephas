using System.Linq.Expressions;
using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Tests.DependencyInjection.TestServices;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Builder;

public class ServiceInfoCollectionBuilderGenericsTest
{
    [Theory]
    // open generic (default)
    [InlineData(new[]
    {
        typeof(IOpenGenericAsDefault<>),
        typeof(OpenGenericAsDefault<>)
    }, new[]
    {
        typeof(OpenGenericAsDefault<>),
    })]
    // open generic (explicit), two generic parameters
    [InlineData(new[]
    {
        typeof(IOpenGenericAsExplicit<,>),
        typeof(OpenGenericAsExplicit<,>),
    }, new[]
    {
        typeof(OpenGenericAsExplicit<,>),
    })]
    // open generic with priority and multiple implementations
    [InlineData(new[]
    {
        typeof(IOpenGenericAsDefault<>),
        typeof(HigherOpenGenericAsDefault<>),
        typeof(OpenGenericAsDefault<>)
    }, new[]
    {
        typeof(OpenGenericAsDefault<>),
        typeof(HigherOpenGenericAsDefault<>),
    })]
    // open generic with priority, multiple implementations, and override
    [InlineData(new[]
    {
        typeof(IOpenGenericAsDefault<>),
        typeof(OpenGenericAsDefault<>),
        typeof(HigherOpenGenericAsDefault<>),
        typeof(OverrideOpenGenericAsDefault<>),
    }, new[]
    {
        typeof(OverrideOpenGenericAsDefault<>),
        typeof(HigherOpenGenericAsDefault<>),
    })]
    public void Build_open_generics(Type[] types, Type[] registeredTypes)
    {
        var serviceType = types[0];
        var implementationTypes = types.Skip(1).ToArray();

        var services = BuildServices(types);
        services.Should().Satisfy(
            registeredTypes.Select(t => (Expression<Func<IServiceInfo, bool>>)(
                s => s.ImplementationType == t && s.ServiceDeclarationType == serviceType)).ToArray());

        var provider = new GenericServiceInfoProvider(serviceType, implementationTypes);
        services = BuildServices(provider);
        services.Should().Satisfy(
            registeredTypes.Select(t => (Expression<Func<IServiceInfo, bool>>)(
                s => s.ImplementationType == t && s.ServiceDeclarationType == serviceType)).ToArray());
    }

    [Theory]
    [InlineData(
        typeof(IGenericCollector<>),
        new[] { typeof(StringGenericCollector) },
        typeof(ICollector),
        new[] { typeof(IGenericCollector<string>) })]
    [InlineData(
        typeof(IGenericTwoCollector<,>),
        new[] { typeof(IntBoolGenericTwoCollector) },
        typeof(ITwoCollector),
        new[] { typeof(IGenericTwoCollector<int, bool>)})]
    public void Build_generics_with_non_generic_service_type(
        Type serviceDeclarationType,
        Type[] implementationTypes,
        Type expectedServiceType,
        Type[] expectedServiceDeclarationTypes)
    {
        var services = BuildServices([
            serviceDeclarationType,
            ..implementationTypes]);

        services.Should().Satisfy(
            implementationTypes.Select((t, i) => (Expression<Func<IServiceInfo, bool>>)(
                s => s.ImplementationType == t &&
                     s.ServiceType == expectedServiceType &&
                     s.ServiceDeclarationType == expectedServiceDeclarationTypes[i])).ToArray());

        var provider = new GenericServiceInfoProvider(serviceDeclarationType, implementationTypes);
        services = BuildServices(provider);
        services.Should().Satisfy(
            implementationTypes.Select((t, i) => (Expression<Func<IServiceInfo, bool>>)(
                s => s.ImplementationType == t && s.ServiceDeclarationType == expectedServiceDeclarationTypes[i])).ToArray());
    }

    [Theory]
    [InlineData(typeof(IGenericCollector<>), typeof(IGenericCollector<string>))]
    [InlineData(typeof(IGenericTwoCollector<,>), typeof(IGenericTwoCollector<int, bool>))]
    public void Build_generic_with_generic_from_family_declaration_attribute(Type serviceDeclarationType, Type expectedServiceDeclarationType)
    {
        var services = CreateBuilder()
            .AddProviders(new DeclareSingletonServiceFamilyAttribute(serviceDeclarationType))
            .AddAssemblies(serviceDeclarationType.Assembly)
            .Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(1);
        var testBuilder = services.Single(s => s.ImplementationType is not null);
        testBuilder.ServiceType.Should().Be(expectedServiceDeclarationType);
    }

    [Theory]
    [InlineData(
        typeof(IGenericCollector<>),
        typeof(StringGenericCollector),
        new[] { "ImplementationType", "ServiceKey", "TType" },
        new[] { (object?)typeof(StringGenericCollector), "String", typeof(string) })]
    [InlineData(
        typeof(IGenericTwoCollector<,>),
        typeof(IntBoolGenericTwoCollector),
        new[] { "ImplementationType", "ServiceKey", "FromType", "ToType" },
        new[] { (object?)typeof(IntBoolGenericTwoCollector), "IntBool", typeof(int), typeof(bool) })]
    public void Build_generic_with_non_generic_metadata(Type serviceDeclarationType, Type implementationType, string[] expectedMetadataKeys, object[] expectedMetadataValues)
    {
        var services = BuildServices(serviceDeclarationType, implementationType);

        var metadata = services.Single().Metadata;
        metadata.Should().Satisfy(
            expectedMetadataKeys.Zip(expectedMetadataValues, (k, v) => (Expression<Func<KeyValuePair<string, object?>, bool>>)(m => m.Key == k && m.Value!.Equals(v))).ToArray());
    }

    private ServiceInfoCollectionBuilder CreateBuilder() =>
        new(
            new ServiceInfosBuilderSettings
            {
                Services = new ServiceInfoCollection(),
                GetAppAssemblies = () => [typeof(ServiceInfoCollectionBuilder).Assembly],
            });

    private IEnumerable<IServiceInfo> BuildServices(params Type[] types) =>
        CreateBuilder()
            .AddTypes(types)
            .Build()
            .Where(s => s.ImplementationType is not null);
    

    private IEnumerable<IServiceInfo> BuildServices(params IServiceInfoProvider[] providers) =>
        CreateBuilder()
            .AddProviders(providers)
            .Build()
            .Where(s => s.ImplementationType is not null);
}