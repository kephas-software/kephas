// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AttributedAppServiceInfoConventionsbuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Services.Configuration;
    using Kephas.Testing;
    using Kephas.Tests.Services.CustomNamedValueAppServiceMetadata;
    using Kephas.Tests.Services.CustomValueAppServiceMetadata;
    using Kephas.Tests.Services.DefaultAppServiceMetadata;
    using Kephas.Tests.Services.DefaultExplicitAppServiceMetadata;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceCollectionBuilder"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceCollectionBuilderTest : TestBase
    {
        [Test]
        public void Build_Multiple()
        {
            var parts = new[]
                {
                    typeof(IMultipleTestAppService),
                    typeof(MultipleTestService),
                    typeof(NewMultipleTestService),
                };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(2, services.Count(s => s.InstanceType is not null));

            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(MultipleTestService)));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(NewMultipleTestService)));
        }

        [Test]
        public void Build_Multiple_Derived()
        {
            var parts = new[]
            {
                typeof(IMultipleTestAppService),
                typeof(MultipleTestService),
                typeof(NewMultipleTestService),
                typeof(DerivedMultipleTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(3, services.Count(s => s.InstanceType is not null));

            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(MultipleTestService)));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(NewMultipleTestService)));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(DerivedMultipleTestService)));
        }

        [Test]
        public void Build_Single_one_service()
        {
            var parts = new[]
                {
                    typeof(ISingleTestAppService),
                    typeof(SingleTestService),
                };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(SingleTestService)));
        }

        [Test]
        public void Build_Single_one_service_overridden_chain()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
                typeof(ChainSingleOverrideTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(ChainSingleOverrideTestService)));
        }

        [Test]
        public void Build_Single_one_service_overridden()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(DerivedOverrideSingleTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(DerivedOverrideSingleTestService)));
        }

        [Test]
        public void Build_Single_override_service_success()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(SingleOverrideTestService)));
        }

        [Test]
        public void Build_Single_service_multiple_registrations_first()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            builder.Settings.AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.UseFirst;
            var services = builder.WithParts(parts).Build();

            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(SingleTestService)));
            CollectionAssert.IsEmpty(services.Where(s => s.InstanceType == typeof(SingleSameOverrideTestService)));
        }


        [Test]
        public void Build_Single_service_multiple_registrations_last()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            builder.Settings.AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.UseLast;
            var services = builder.WithParts(parts).Build();

            CollectionAssert.IsNotEmpty(services.Where(s => s.InstanceType == typeof(SingleSameOverrideTestService)));
            CollectionAssert.IsEmpty(services.Where(s => s.InstanceType == typeof(SingleTestService)));
        }


        [Test]
        public void Build_Single_service_multiple_registrations_force_failure()
        {
            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            builder.Settings.AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority;
            Assert.Throws<AmbiguousServiceResolutionException>(() => builder.WithParts(parts).Build());
        }

        [Test]
        public void Build_generic()
        {
            var parts = new[]
            {
                typeof(IGenericAppService<>),
                typeof(GenericAppService<>),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            var match = services.First(s => s.InstanceType is not null);
            Assert.AreEqual(match.InstanceType, typeof(GenericAppService<>));
        }

        [Test]
        public void Build_default_metadata()
        {
            var parts = new[]
            {
                typeof(IDefaultMetadataAppService),
                typeof(DefaultMetadataAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            var testBuilder = services.Single(s => s.InstanceType == typeof(DefaultMetadataAppService));
            var metadata = testBuilder.Metadata!;

            Assert.AreEqual(1, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey(nameof(AppServiceMetadata.ServiceType)));
            Assert.AreEqual(typeof(DefaultMetadataAppService), metadata[nameof(AppServiceMetadata.ServiceType)]);
        }

        [Test]
        public void Build_generic_with_nongeneric_base()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(1, services.Count(s => s.InstanceType is not null));
            var testBuilder = services.Single(s => s.InstanceType is not null);
            Assert.AreEqual(typeof(IOneGenericAppService), testBuilder.ContractType);
        }

        [Test]
        public void Build_generic_with_nongeneric_metadata()
        {
            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            var testBuilder = services.First(s => s.InstanceType is not null);
            var metadata = testBuilder.Metadata;

            Assert.AreEqual(2, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("TType"));
            Assert.AreEqual(typeof(string), metadata["TType"]);
        }

        [Test]
        public void Build_generic_with_nongeneric_metadata_two()
        {
            var parts = new[]
            {
                typeof(ITwoGenericAppService<,>),
                typeof(TwoGenericAppServiceIntBool),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            var testBuilder = services.First(s => s.InstanceType is not null);
            var metadata = testBuilder.Metadata;

            Assert.AreEqual(3, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("FromType"));
            Assert.IsTrue(metadata.ContainsKey("ToType"));

            Assert.AreEqual(typeof(int), metadata["FromType"]);
            Assert.AreEqual(typeof(bool), metadata["ToType"]);
        }

        [Test]
        public void Build_as_open_generic_ILogger()
        {
            var log = new StringBuilder();

            var parts = new[]
            {
                typeof(ILogger<>),
                typeof(TypedLogger<>),
            };
            var builder = this.CreateAppServiceCollectionBuilder(
                this.GetTestAppServices(m => log.AppendLine(m)));
            var services = builder.WithParts(parts).Build();

            var testBuilder = services.Single(s => s.InstanceType is not null);
            var metadata = testBuilder.Metadata;

            // should not warn that metadata attributes are not supported
            Assert.IsFalse(log.ToString().Contains(LogLevel.Warning.ToString()));

            Assert.AreEqual(2, metadata.Count);
        }

        [Test]
        public void Build_metadata()
        {
            var parts = new[]
            {
                typeof(IExplicitMetadataAppService),
                typeof(ExplicitMetadataAppService),
                typeof(NullExplicitMetadataAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(2, services.Count(s => s.InstanceType is not null));
            var nullBuilderEntry = services.Single(s => s.InstanceType == typeof(NullExplicitMetadataAppService));
            var explicitBuilderEntry = services.Single(s => s.InstanceType == typeof(ExplicitMetadataAppService));

            var nullMetadata = nullBuilderEntry.Metadata!;
            var explicitMetadata = explicitBuilderEntry.Metadata!;

            Assert.AreEqual(1, nullMetadata.Count);
            Assert.IsFalse(nullMetadata.ContainsKey(nameof(AppServiceMetadata.ProcessingPriority)));

            Assert.AreEqual(2, explicitMetadata.Count);
            Assert.IsTrue(explicitMetadata.ContainsKey(nameof(AppServiceMetadata.ProcessingPriority)));
            Assert.AreEqual((Priority)100, explicitMetadata[nameof(AppServiceMetadata.ProcessingPriority)]);
        }

        [Test]
        public void Build_metadata_IMetadataValue()
        {
            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(2, services.Count(s => s.InstanceType is not null));
            var customEntry = services.First(s => s.InstanceType == typeof(CustomValueMetadataAppService));
            var nullEntry = services.First(s => s.InstanceType == typeof(CustomValueNullMetadataAppService));

            var customMetadata = customEntry.Metadata!;

            Assert.AreEqual(2, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["CustomValueMetadata"]);

            Assert.IsFalse(nullEntry.Metadata!.ContainsKey("CustomValueMetadata"));
        }

        [Test]
        public void Build_metadata_MetadataValue_properties()
        {
            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(2, services.Count(s => s.InstanceType is not null));

            var customBuilderEntry = services.First(s => s.InstanceType == typeof(CustomValueMetadataAppService));
            var customMetadata = customBuilderEntry.Metadata!;
            Assert.AreEqual(2, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["CustomValueMetadata"]);

            var nullBuilderEntry = services.First(s => s.InstanceType == typeof(CustomValueNullMetadataAppService));
            var nullMetadata = nullBuilderEntry.Metadata!;
            Assert.IsFalse(nullMetadata.ContainsKey("CustomValueMetadata"));
        }

        [Test]
        public void Build_metadata_MetadataNamedValue_properties()
        {
            var parts = new[]
            {
                typeof(ICustomNamedValueMetadataAppService),
                typeof(CustomNamedValueMetadataAppService),
                typeof(CustomNamedValueNullMetadataAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            var services = builder.WithParts(parts).Build();

            Assert.AreEqual(2, services.Count(s => s.InstanceType is not null));
            var customBuilderEntry = services.First(s => s.InstanceType == typeof(CustomNamedValueMetadataAppService));

            var nullBuilderEntry = services.First(s => s.InstanceType == typeof(CustomNamedValueNullMetadataAppService));

            var customMetadata = customBuilderEntry.Metadata!;
            var nullMetadata = nullBuilderEntry.Metadata!;

            Assert.AreEqual(3, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["Name"]);
            Assert.AreEqual("ICXP", customMetadata["IconName"]);
            Assert.IsFalse(customMetadata.ContainsKey("Description"));

            Assert.IsFalse(nullMetadata.ContainsKey("Name"));
            Assert.IsFalse(nullMetadata.ContainsKey("IconName"));
        }

        [Test]
        public void Build_bad_contract_type()
        {
            var parts = new[]
            {
                typeof(IBadAppService),
                typeof(BadAppService),
            };
            var builder = this.CreateAppServiceCollectionBuilder();
            Assert.Throws<InjectionException>(() => builder.WithParts(parts).Build());
        }

        private AppServiceCollectionBuilder CreateAppServiceCollectionBuilder(IAppServiceCollection? appServices = null)
        {
            var builder = new AppServiceCollectionBuilder(appServices ?? this.CreateAppServices());
            return builder;
        }

        private IAppServiceCollection GetTestAppServices(Action<string>? logAction = null)
        {
            var logger = Substitute.For<ILogger>();
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            if (logAction != null)
            {
                logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>()))
                    .Do(ci => logAction(ci.Arg<LogLevel>().ToString() + ": " + ci.Arg<string>()));
            }

            var logManager = Substitute.For<ILogManager>();
            logManager.GetLogger(Arg.Any<string>()).Returns(logger);

            var appServices = this.CreateAppServices();
            appServices.Add(logManager);

            return appServices;
        }

        [SingletonAppServiceContract(AllowMultiple = false)]
        public interface ISingleTestAppService { }

        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface IMultipleTestAppService { }

        public class SingleTestService : ISingleTestAppService { }

        [OverridePriority(Priority.High)]
        public class SingleOverrideTestService : ISingleTestAppService { }

        public class SingleSameOverrideTestService : ISingleTestAppService { }

        public class MultipleTestService : IMultipleTestAppService { }

        public class NewMultipleTestService : IMultipleTestAppService { }

        public class DerivedMultipleTestService : MultipleTestService { }

        [Override]
        public class ChainSingleOverrideTestService : SingleOverrideTestService { }

        [Override]
        public class DerivedOverrideSingleTestService : SingleTestService { }


        [SingletonAppServiceContract]
        public interface IGenericAppService<T> { }

        public interface IOneGenericAppService { }

        [SingletonAppServiceContract(ContractType = typeof(IOneGenericAppService))]
        public interface IOneGenericAppService<T> : IOneGenericAppService { }

        public interface ITwoGenericAppService { }

        [SingletonAppServiceContract(ContractType = typeof(ITwoGenericAppService))]
        public interface ITwoGenericAppService<TFrom, ToType> : ITwoGenericAppService { }

        public class GenericAppService<T> : IGenericAppService<T> { }

        public class OneGenericAppServiceString : IOneGenericAppService<string> { }

        public class TwoGenericAppServiceIntBool : ITwoGenericAppService<int, bool> { }

        [SingletonAppServiceContract(ContractType = typeof(IDisposable))]
        public interface IBadAppService { }

        public class BadAppService : IBadAppService { }
    }

    namespace CustomValueAppServiceMetadata
    {
        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface ICustomValueMetadataAppService { }

        [CustomValueMetadata("hi there")]
        public class CustomValueMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueNullMetadataAppService : ICustomValueMetadataAppService { }

        public class CustomValueMetadataAttribute : Attribute, IMetadataValue<string>
        {
            public CustomValueMetadataAttribute(string value)
            {
                this.Value = value;
            }

            public string Value { get; }
        }
    }

    namespace CustomNamedValueAppServiceMetadata
    {
        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface ICustomNamedValueMetadataAppService { }

        [CustomNamedValueMetadata("hi there", "bob", IconName = "ICXP")]
        public class CustomNamedValueMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueNullMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueMetadataAttribute : Attribute, IMetadataProvider
        {
            public CustomNamedValueMetadataAttribute(string name, string? description)
            {
                this.Name = name;
                this.Description = description;
            }

            public string Name { get; }

            public string? IconName { get; set; }

            public string? Description { get; }

            public IEnumerable<(string name, object? value)> GetMetadata()
            {
                yield return (nameof(this.Name), this.Name);
                yield return (nameof(this.IconName), this.IconName);
            }
        }
    }

    namespace DefaultAppServiceMetadata
    {
        [AppServiceContract]
        public interface IDefaultMetadataAppService { }

        public class DefaultMetadataAppService : IDefaultMetadataAppService
        {
        }
    }

    namespace DefaultExplicitAppServiceMetadata
    {
        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface IExplicitMetadataAppService { }

        [ProcessingPriority(100)]
        public class ExplicitMetadataAppService : IExplicitMetadataAppService { }

        public class NullExplicitMetadataAppService : IExplicitMetadataAppService { }
    }
}