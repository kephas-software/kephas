using FluentAssertions;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.Tests.DependencyInjection.TestServices;
using Kephas.Tests.DependencyInjection.TestServices.CustomNamedValueAppServiceMetadata;
using Kephas.Tests.DependencyInjection.TestServices.CustomValueAppServiceMetadata;
using Kephas.Tests.DependencyInjection.TestServices.DefaultAppServiceMetadata;
using Kephas.Tests.DependencyInjection.TestServices.DefaultExplicitAppServiceMetadata;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Builder;

public class ServiceInfoCollectionBuilderNonGenericTest
{
    [Fact]
    public void Build_Multiple()
    {
        var parts = new[]
        {
            typeof(IMultipleTestService),
            typeof(MultipleTestService),
            typeof(NewMultipleTestService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(2);
        services.Should().ContainSingle(s => s.ImplementationType == typeof(MultipleTestService));
        services.Should().ContainSingle(s => s.ImplementationType == typeof(NewMultipleTestService));
    }

    [Fact]
    public void Build_Multiple_Derived()
    {
        var parts = new[]
        {
            typeof(IMultipleTestService),
            typeof(MultipleTestService),
            typeof(NewMultipleTestService),
            typeof(DerivedMultipleTestService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(3);
        services.Should().ContainSingle(s => s.ImplementationType == typeof(MultipleTestService));
        services.Should().ContainSingle(s => s.ImplementationType == typeof(NewMultipleTestService));
        services.Should().ContainSingle(s => s.ImplementationType == typeof(DerivedMultipleTestService));
    }

    [Fact]
    public void Build_Single_one_service()
    {
        var parts = new[]
        {
            typeof(ISingleTestService),
            typeof(SingleTestService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(1);
        services.Should().ContainSingle(s => s.ImplementationType == typeof(SingleTestService));
    }

    [Theory]
    [InlineData(
        new[]
        {
            typeof(ISingleTestService),
            typeof(ChainSingleOverrideTestService),
            typeof(SingleTestService),
            typeof(SingleOverridenTestService),
            typeof(DerivedOverrideSingleTestService),
        },
        new[]
        {
            typeof(ChainSingleOverrideTestService),
            typeof(DerivedOverrideSingleTestService),
        })]
    [InlineData(
        new[]
        {
            typeof(ISingleTestService),
            typeof(SingleTestService),
            typeof(ChainSingleOverrideTestService),
            typeof(SingleOverridenTestService)
        },
        new[]
        {
            typeof(SingleTestService),
            typeof(ChainSingleOverrideTestService),
        })]
    [InlineData(
        new[]
        {
            typeof(ISingleTestService),
            typeof(SingleTestService),
            typeof(DerivedOverrideSingleTestService),
            typeof(SingleOverridenTestService)
        },
        new[]
        {
            typeof(DerivedOverrideSingleTestService),
            typeof(SingleOverridenTestService)
        })]
    public void Build_Single_one_service_overridden_chain(Type[] registeredTypes, Type[] expectedImplementationTypes)
    {
        var builder = CreateBuilder();
        var services = builder.AddTypes(registeredTypes).Build();

        var orderedServices = services
            .Where(s => s.ImplementationType is not null)
            .ToList();
        orderedServices.Select(s => s.ImplementationType).Should().Equal(expectedImplementationTypes);
    }

    [Fact]
    public void Build_Single_one_service_overridden()
    {
        var parts = new[]
        {
            typeof(ISingleTestService),
            typeof(SingleTestService),
            typeof(DerivedOverrideSingleTestService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(1);
        services.Should().ContainSingle(s => s.ImplementationType == typeof(DerivedOverrideSingleTestService));
    }

    [Fact]
    public void Build_Single_override_service_success()
    {
        var parts = new[]
        {
            typeof(ISingleTestService),
            typeof(SingleOverridenTestService),
            typeof(SingleTestService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        var orderedServices = services
            .Where(s => s.ImplementationType is not null)
            .ToList();
        orderedServices.Select(s => s.ImplementationType)
            .Should().Equal(typeof(SingleTestService), typeof(SingleOverridenTestService));
    }

    [Fact]
    public void Build_default_metadata()
    {
        var parts = new[]
        {
            typeof(IDefaultMetadataAppService),
            typeof(DefaultMetadataAppService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        var testBuilder = services.Single(s => s.ImplementationType == typeof(DefaultMetadataAppService));
        var metadata = testBuilder.Metadata!;

        metadata.Count.Should().Be(2);
        metadata.Should().ContainKey(nameof(ServiceMetadata.ImplementationType));
        metadata[nameof(ServiceMetadata.ImplementationType)].Should().Be(typeof(DefaultMetadataAppService));
    }

    [Fact]
    public void Build_metadata()
    {
        var parts = new[]
        {
            typeof(IExplicitMetadataAppService),
            typeof(ExplicitMetadataAppService),
            typeof(NullExplicitMetadataAppService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(2);
        var nullBuilderEntry = services.Single(s => s.ImplementationType == typeof(NullExplicitMetadataAppService));
        var explicitBuilderEntry = services.Single(s => s.ImplementationType == typeof(ExplicitMetadataAppService));

        var nullMetadata = nullBuilderEntry.Metadata!;
        var explicitMetadata = explicitBuilderEntry.Metadata!;

        nullMetadata.Count.Should().Be(2);
        nullMetadata.Should().NotContainKey(nameof(ServiceMetadata.Priority));

        explicitMetadata.Count.Should().Be(3);
        explicitMetadata.Should().ContainKey(nameof(ServiceMetadata.Priority));
        explicitMetadata[nameof(ServiceMetadata.Priority)].Should().Be((Priority)100);
    }

    [Fact]
    public void Build_metadata_IMetadataValue()
    {
        var parts = new[]
        {
            typeof(ICustomValueMetadataAppService),
            typeof(CustomValueMetadataAppService),
            typeof(CustomValueNullMetadataAppService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(2);
        var customEntry = services.First(s => s.ImplementationType == typeof(CustomValueMetadataAppService));
        var nullEntry = services.First(s => s.ImplementationType == typeof(CustomValueNullMetadataAppService));

        var customMetadata = customEntry.Metadata!;

        customMetadata.Count.Should().Be(3);
        customMetadata["CustomValueMetadata"].Should().Be("hi there");

        nullEntry.Metadata!.Should().NotContainKey("CustomValueMetadata");
    }

    [Fact]
    public void Build_metadata_MetadataValue_properties()
    {
        var parts = new[]
        {
            typeof(ICustomValueMetadataAppService),
            typeof(CustomValueMetadataAppService),
            typeof(CustomValueNullMetadataAppService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(2);

        var customBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomValueMetadataAppService));
        var customMetadata = customBuilderEntry.Metadata!;
        customMetadata.Count.Should().Be(3);
        customMetadata["CustomValueMetadata"].Should().Be("hi there");

        var nullBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomValueNullMetadataAppService));
        var nullMetadata = nullBuilderEntry.Metadata!;
        nullMetadata.Should().NotContainKey("CustomValueMetadata");
    }

    [Fact]
    public void Build_metadata_MetadataNamedValue_properties()
    {
        var parts = new[]
        {
            typeof(ICustomNamedValueMetadataService),
            typeof(CustomNamedValueMetadataService),
            typeof(CustomNamedValueNullMetadataService),
        };
        var builder = CreateBuilder();
        var services = builder.AddTypes(parts).Build();

        services.Count(s => s.ImplementationType is not null).Should().Be(2);
        var customBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomNamedValueMetadataService));

        var nullBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomNamedValueNullMetadataService));

        var customMetadata = customBuilderEntry.Metadata!;
        var nullMetadata = nullBuilderEntry.Metadata!;

        customMetadata.Count.Should().Be(4);
        customMetadata["Name"].Should().Be("hi there");
        customMetadata["IconName"].Should().Be("ICXP");
        customMetadata.Should().NotContainKey("Description");

        nullMetadata.Should().NotContainKey("Name");
        nullMetadata.Should().NotContainKey("IconName");
    }

    private ServiceInfoCollectionBuilder CreateBuilder(IServiceInfoCollection? services = null) =>
        new(
            new ServiceInfosBuilderSettings
            {
                Services = services ?? new ServiceInfoCollection(),
                GetAppAssemblies = () => [typeof(ServiceInfoCollectionBuilder).Assembly],
            });
}