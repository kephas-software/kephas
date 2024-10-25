// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.Testing.Logging;
using Kephas.Tests.Services.CustomNamedValueAppServiceMetadata;
using Kephas.Tests.Services.CustomValueAppServiceMetadata;
using Kephas.Tests.Services.DefaultAppServiceMetadata;
using Kephas.Tests.Services.DefaultExplicitAppServiceMetadata;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Kephas.Tests.Services
{
    /// <summary>
    /// Test class for <see cref="ServiceCollectionBuilder"/>.
    /// </summary>
    public class ServiceCollectionBuilderTest
    {
        [Fact]
        public void Build_Multiple()
        {
            var parts = new[]
                {
                    typeof(IMultipleTestAppService),
                    typeof(MultipleTestService),
                    typeof(NewMultipleTestService),
                };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(2, services.Count(s => s.ImplementationType is not null));

            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(MultipleTestService)));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(NewMultipleTestService)));
        }

        [Fact]
        public void Build_Multiple_Derived()
        {
            var parts = new[]
            {
                typeof(IMultipleTestAppService),
                typeof(MultipleTestService),
                typeof(NewMultipleTestService),
                typeof(DerivedMultipleTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(3, services.Count(s => s.ImplementationType is not null));

            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(MultipleTestService)));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(NewMultipleTestService)));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(DerivedMultipleTestService)));
        }

        [Fact]
        public void Build_Single_one_service()
        {
            var parts = new[]
                {
                    typeof(ISingleTestAppService),
                    typeof(SingleTestService),
                };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(SingleTestService)));
        }

        [Fact]
        public void Build_Single_one_service_overridden_chain()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
                typeof(ChainSingleOverrideTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(ChainSingleOverrideTestService)));
        }

        [Fact]
        public void Build_Single_one_service_overridden()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(DerivedOverrideSingleTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(DerivedOverrideSingleTestService)));
        }

        [Fact]
        public void Build_Single_override_service_success()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(SingleOverrideTestService)));
        }

        [Fact]
        public void Build_Single_service_multiple_registrations_first()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(SingleTestService)));
            Assert.Empty(services.Where(s => s.ImplementationType == typeof(SingleSameOverrideTestService)));
        }


        [Fact]
        public void Build_Single_service_multiple_registrations_last()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.NotEmpty(services.Where(s => s.ImplementationType == typeof(SingleSameOverrideTestService)));
            Assert.Empty(services.Where(s => s.ImplementationType == typeof(SingleTestService)));
        }

        [Fact]
        public void Build_generic()
        {
            var parts = new[]
            {
                typeof(IGenericAppService<>),
                typeof(GenericAppService<>),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            var match = services.First(s => s.ImplementationType is not null);
            Assert.Equal(match.ImplementationType, typeof(GenericAppService<>));
        }

        [Fact]
        public void Build_default_metadata()
        {
            var parts = new[]
            {
                typeof(IDefaultMetadataAppService),
                typeof(DefaultMetadataAppService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.Single(s => s.ImplementationType == typeof(DefaultMetadataAppService));
            var metadata = testBuilder.Metadata!;

            Assert.Equal(1, metadata.Count);
            Assert.True(metadata.ContainsKey(nameof(ServiceMetadata.ImplementationType)));
            Assert.Equal(typeof(DefaultMetadataAppService), metadata[nameof(ServiceMetadata.ImplementationType)]);
        }

        [Fact]
        public void Build_generic_with_nongeneric_base()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(1, services.Count(s => s.ImplementationType is not null));
            var testBuilder = services.Single(s => s.ImplementationType is not null);
            Assert.Equal(typeof(IOneGenericAppService), testBuilder.ServiceType);
        }

        [Fact]
        public void Build_generic_with_nongeneric_metadata()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.First(s => s.ImplementationType is not null);
            var metadata = testBuilder.Metadata;

            Assert.Equal(2, metadata.Count);
            Assert.True(metadata.ContainsKey("TType"));
            Assert.Equal(typeof(string), metadata["TType"]);
        }

        [Fact]
        public void Build_generic_with_nongeneric_metadata_two()
        {
            var parts = new[]
            {
                typeof(ITwoGenericAppService<,>),
                typeof(TwoGenericAppServiceIntBool),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.First(s => s.ImplementationType is not null);
            var metadata = testBuilder.Metadata;

            Assert.Equal(3, metadata.Count);
            Assert.True(metadata.ContainsKey("FromType"));
            Assert.True(metadata.ContainsKey("ToType"));

            Assert.Equal(typeof(int), metadata["FromType"]);
            Assert.Equal(typeof(bool), metadata["ToType"]);
        }

        [Fact]
        public void Build_as_open_generic_ILogger()
        {
            var log = new StringBuilder();

            var parts = new[]
            {
                typeof(ILogger<>),
                typeof(TypedLogger<>),
            };
            var builder = CreateAppServiceCollectionBuilder(
                GetTestAppServices(m => log.AppendLine(m)));
            var services = builder.AddTypes(parts).Build();

            var testBuilder = services.Single(s => s.ImplementationType is not null);
            var metadata = testBuilder.Metadata;

            // should not warn that metadata attributes are not supported
            Assert.False(log.ToString().Contains(LogLevel.Warning.ToString()));

            Assert.Equal(2, metadata.Count);
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
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(2, services.Count(s => s.ImplementationType is not null));
            var nullBuilderEntry = services.Single(s => s.ImplementationType == typeof(NullExplicitMetadataAppService));
            var explicitBuilderEntry = services.Single(s => s.ImplementationType == typeof(ExplicitMetadataAppService));

            var nullMetadata = nullBuilderEntry.Metadata!;
            var explicitMetadata = explicitBuilderEntry.Metadata!;

            Assert.Equal(1, nullMetadata.Count);
            Assert.False(nullMetadata.ContainsKey(nameof(ServiceMetadata.Priority)));

            Assert.Equal(2, explicitMetadata.Count);
            Assert.True(explicitMetadata.ContainsKey(nameof(ServiceMetadata.Priority)));
            Assert.Equal((Priority)100, explicitMetadata[nameof(ServiceMetadata.Priority)]);
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
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(2, services.Count(s => s.ImplementationType is not null));
            var customEntry = services.First(s => s.ImplementationType == typeof(CustomValueMetadataAppService));
            var nullEntry = services.First(s => s.ImplementationType == typeof(CustomValueNullMetadataAppService));

            var customMetadata = customEntry.Metadata!;

            Assert.Equal(2, customMetadata.Count);
            Assert.Equal("hi there", customMetadata["CustomValueMetadata"]);

            Assert.False(nullEntry.Metadata!.ContainsKey("CustomValueMetadata"));
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
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(2, services.Count(s => s.ImplementationType is not null));

            var customBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomValueMetadataAppService));
            var customMetadata = customBuilderEntry.Metadata!;
            Assert.Equal(2, customMetadata.Count);
            Assert.Equal("hi there", customMetadata["CustomValueMetadata"]);

            var nullBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomValueNullMetadataAppService));
            var nullMetadata = nullBuilderEntry.Metadata!;
            Assert.False(nullMetadata.ContainsKey("CustomValueMetadata"));
        }

        [Fact]
        public void Build_metadata_MetadataNamedValue_properties()
        {
            var parts = new[]
            {
                typeof(ICustomNamedValueMetadataAppService),
                typeof(CustomNamedValueMetadataAppService),
                typeof(CustomNamedValueNullMetadataAppService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            var services = builder.AddTypes(parts).Build();

            Assert.Equal(2, services.Count(s => s.ImplementationType is not null));
            var customBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomNamedValueMetadataAppService));

            var nullBuilderEntry = services.First(s => s.ImplementationType == typeof(CustomNamedValueNullMetadataAppService));

            var customMetadata = customBuilderEntry.Metadata!;
            var nullMetadata = nullBuilderEntry.Metadata!;

            Assert.Equal(3, customMetadata.Count);
            Assert.Equal("hi there", customMetadata["Name"]);
            Assert.Equal("ICXP", customMetadata["IconName"]);
            Assert.False(customMetadata.ContainsKey("Description"));

            Assert.False(nullMetadata.ContainsKey("Name"));
            Assert.False(nullMetadata.ContainsKey("IconName"));
        }

        [Fact]
        public void Build_bad_contract_type()
        {
            var parts = new[]
            {
                typeof(IBadAppService),
                typeof(BadAppService),
            };
            var builder = CreateAppServiceCollectionBuilder();
            Assert.Throws<ServiceException>(() => builder.AddTypes(parts).Build());
        }

        private ServiceCollectionBuilder CreateAppServiceCollectionBuilder(IList<IServiceInfo>? services = null)
        {
            var builder = new ServiceCollectionBuilder(
                services ?? new List<IServiceInfo>(),
                () => [ typeof(ServiceCollectionBuilderTest).Assembly, typeof(ServiceCollectionBuilder).Assembly ]);
            return builder;
        }

        private IList<IServiceInfo> GetTestAppServices(Action<string>? logAction = null)
        {
            var logger = new TestLogger<string>((_, _, message) => logAction?.Invoke(message));
            var logManager = Substitute.For<ILogManager>();
            logManager.GetLogger(Arg.Any<string>()).Returns(logger);

            var appServices = new List<IServiceInfo>
            {
                new ServiceInfo(typeof(ILogManager), logManager)
            };

            return appServices;
        }

        [SingletonService]
        public interface ISingleTestAppService { }

        [SingletonService]
        public interface IMultipleTestAppService { }

        public class SingleTestService : ISingleTestAppService { }

        [Priority(Priority.High)]
        public class SingleOverrideTestService : ISingleTestAppService { }

        public class SingleSameOverrideTestService : ISingleTestAppService { }

        public class MultipleTestService : IMultipleTestAppService { }

        public class NewMultipleTestService : IMultipleTestAppService { }

        public class DerivedMultipleTestService : MultipleTestService { }

        [OverrideBaseImplementation]
        public class ChainSingleOverrideTestService : SingleOverrideTestService { }

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

        [SingletonService(ServiceType = typeof(IDisposable))]
        public interface IBadAppService { }

        public class BadAppService : IBadAppService { }

        public interface ILogManager
        {
            ILogger GetLogger(string name);
        }

        [SingletonService(AsOpenGeneric = true)]
        public interface ITypedLogger<T> {}
        public class TypedLogger<T> : ITypedLogger<T> {}
    }

    namespace CustomValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomValueMetadataAppService { }

        [CustomValueServiceMetadata("hi there")]
        public class CustomValueMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueNullMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueServiceMetadataAttribute : Attribute, IServiceMetadataValue<string>
        {
            public CustomValueServiceMetadataAttribute(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }

    namespace CustomNamedValueAppServiceMetadata
    {
        [SingletonService]
        public interface ICustomNamedValueMetadataAppService { }

        [CustomNamedValueMetadata("hi there", "bob", IconName = "ICXP")]
        public class CustomNamedValueMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueNullMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueMetadataAttribute : Attribute, IServiceMetadataProvider
        {
            public CustomNamedValueMetadataAttribute(string name, string? description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; }

            public string? IconName { get; set; }

            public string? Description { get; }

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

        public class DefaultMetadataAppService : IDefaultMetadataAppService
        {
        }
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
