using System.Text;
using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.Testing.Logging;
using Kephas.Tests.DependencyInjection.Builder.CustomNamedValueAppServiceMetadata;
using Kephas.Tests.DependencyInjection.Builder.CustomValueAppServiceMetadata;
using Kephas.Tests.DependencyInjection.Builder.DefaultAppServiceMetadata;
using Kephas.Tests.DependencyInjection.Builder.DefaultExplicitAppServiceMetadata;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Builder
{
    public class ServiceInfoCollectionBuilderTest
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

        [Fact]
        public void Build_Single_one_service_overridden_chain()
        {
            var parts = new[]
            {
                typeof(ISingleTestService),
                typeof(ChainSingleOverrideTestService),
                typeof(SingleTestService),
                typeof(SingleOverridenTestService),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            var orderedServices = services
                .Where(s => s.ImplementationType is not null)
                .ToList();
            orderedServices.Count.Should().Be(2);
            orderedServices[0].ImplementationType.Should().Be(typeof(ChainSingleOverrideTestService));
            orderedServices[1].ImplementationType.Should().Be(typeof(SingleTestService));
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
                typeof(SingleTestService),
                typeof(SingleOverridenTestService),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            var orderedServices = services
                .Where(s => s.ImplementationType is not null)
                .ToList();
            orderedServices.Count.Should().Be(2);
            orderedServices[0].ImplementationType.Should().Be(typeof(SingleOverridenTestService));
            orderedServices[1].ImplementationType.Should().Be(typeof(SingleTestService));
        }

        [Fact]
        public void Build_generic()
        {
            var parts = new[]
            {
                typeof(IGenericAppService<>),
                typeof(GenericAppService<>),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            services.Count(s => s.ImplementationType is not null).Should().Be(1);
            var match = services.First(s => s.ImplementationType is not null);
            match.ImplementationType.Should().Be(typeof(GenericAppService<>));
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
        public void Build_generic_with_nongeneric_base()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            services.Count(s => s.ImplementationType is not null).Should().Be(1);
            var testBuilder = services.Single(s => s.ImplementationType is not null);
            testBuilder.ServiceType.Should().Be(typeof(IOneGenericAppService));
        }

        [Fact]
        public void Build_generic_with_nongeneric_metadata()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.First(s => s.ImplementationType is not null);
            var metadata = testBuilder.Metadata;

            metadata.Count.Should().Be(3);
            metadata.Should().ContainKey("TType");
            metadata["TType"].Should().Be(typeof(string));
        }

        [Fact]
        public void Build_generic_with_nongeneric_metadata_two()
        {
            var parts = new[]
            {
                typeof(ITwoGenericAppService<,>),
                typeof(TwoGenericAppServiceIntBool),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.First(s => s.ImplementationType is not null);
            var metadata = testBuilder.Metadata;

            metadata.Count.Should().Be(4);
            metadata.Should().ContainKey("FromType");
            metadata.Should().ContainKey("ToType");

            metadata["FromType"].Should().Be(typeof(int));
            metadata["ToType"].Should().Be(typeof(bool));
        }

        [Fact]
        public void Build_as_open_generic_ITypedLogger()
        {
            var parts = new[]
            {
                typeof(ITypedLogger<>),
                typeof(TypedLogger<>),
            };
            var builder = CreateBuilder();
            var services = builder.AddTypes(parts).Build();

            var serviceInfo = services.Single(s => s.ImplementationType is not null);
            var metadata = serviceInfo.Metadata;

            metadata.Should().Satisfy(
                kv => kv.Key == nameof(ServiceMetadata.ImplementationType) && kv.Value == typeof(TypedLogger<>),
                kv => kv.Key == nameof(ServiceMetadata.ServiceKey) && kv.Value == typeof(TypedLogger<>).Name);
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

        [SingletonService]
        public interface ISingleTestService { }

        [SingletonService]
        public interface IMultipleTestService { }

        public class SingleTestService : ISingleTestService { }

        [Priority(Priority.High)]
        public class SingleOverridenTestService : ISingleTestService { }

        public class SingleSameOverrideTestService : ISingleTestService { }

        public class MultipleTestService : IMultipleTestService { }

        public class NewMultipleTestService : IMultipleTestService { }

        public class DerivedMultipleTestService : MultipleTestService { }

        [OverrideBaseImplementation]
        public class ChainSingleOverrideTestService : SingleOverridenTestService { }

        [OverrideBaseImplementation]
        public class DerivedOverrideSingleTestService : SingleTestService { }

        [SingletonService]
        public interface IGenericAppService<T> { }

        public interface IOneGenericAppService { }

        [SingletonService(ServiceType = typeof(IOneGenericAppService))]
        public interface IOneGenericAppService<T> : IOneGenericAppService { }

        public interface ITwoGenericAppService { }

        [SingletonService(ServiceType = typeof(ITwoGenericAppService))]
        public interface ITwoGenericAppService<TFrom, ToType> : ITwoGenericAppService { }

        public class GenericAppService<T> : IGenericAppService<T> { }

        public class OneGenericAppServiceString : IOneGenericAppService<string> { }

        public class TwoGenericAppServiceIntBool : ITwoGenericAppService<int, bool> { }

        [SingletonService(AsOpenGeneric = true)]
        public interface ITypedLogger<T> { }
        public class TypedLogger<T> : ITypedLogger<T> { }
    }

    namespace CustomValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomValueMetadataAppService { }

        [CustomValueMetadata("hi there")]
        public class CustomValueMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueNullMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueMetadataAttribute(string value) : Attribute, IServiceMetadataValue<string>
        {
            public string Value { get; } = value;
        }
    }

    namespace CustomNamedValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomNamedValueMetadataService { }

        [CustomNamedValueMetadata("hi there", "bob", IconName = "ICXP")]
        public class CustomNamedValueMetadataService : ICustomNamedValueMetadataService { }

        public class CustomNamedValueNullMetadataService : ICustomNamedValueMetadataService { }

        public class CustomNamedValueMetadataAttribute(string name, string? description) : Attribute, IServiceMetadataProvider
        {
            public string Name { get; } = name;

            public string? IconName { get; set; }

            public string? Description { get; } = description;

            public IEnumerable<KeyValuePair<string, object?>> GetMetadata()
            {
                yield return KeyValuePair.Create(nameof(Name), (object?)Name);
                yield return KeyValuePair.Create(nameof(IconName), (object?)IconName);
            }
        }
    }

    namespace DefaultAppServiceMetadata
    {
        [TransientService]
        public interface IDefaultMetadataAppService { }

        public class DefaultMetadataAppService : IDefaultMetadataAppService { }
    }

    namespace DefaultExplicitAppServiceMetadata
    {
        [SingletonService]
        public interface IExplicitMetadataAppService { }

        [Priority(100)]
        public class ExplicitMetadataAppService : IExplicitMetadataAppService { }

        public class NullExplicitMetadataAppService : IExplicitMetadataAppService { }
    }
}