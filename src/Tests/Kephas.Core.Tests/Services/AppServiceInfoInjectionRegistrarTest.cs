// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoInjectionRegistrarTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AttributedAppServiceInfoConventionsRegistrar" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Kephas.Core.Tests.Injection;
    using Kephas.Core.Tests.Services.CustomNamedValueAppServiceMetadata;
    using Kephas.Core.Tests.Services.CustomValueAppServiceMetadata;
    using Kephas.Core.Tests.Services.DefaultAppServiceMetadata;
    using Kephas.Core.Tests.Services.DefaultExplicitAppServiceMetadata;
    using Kephas.Services;
    using Kephas.Services.Configuration;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Services;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceInfoInjectionRegistrar"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceInfoInjectionRegistrarTest : TestBase
    {
        [Test]
        public void RegisterServices_Multiple()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
                {
                    typeof(IMultipleTestAppService),
                    typeof(MultipleTestService),
                    typeof(NewMultipleTestService),
                };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(2, conventions.TypeBuilders.Count);

            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(MultipleTestService)));
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(NewMultipleTestService)));
        }

        [Test]
        public void RegisterServices_Multiple_Derived()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IMultipleTestAppService),
                typeof(MultipleTestService),
                typeof(NewMultipleTestService),
                typeof(DerivedMultipleTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(3, conventions.TypeBuilders.Count);

            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(MultipleTestService)));
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(NewMultipleTestService)));
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(DerivedMultipleTestService)));
        }

        [Test]
        public void RegisterServices_Single_one_service()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
                {
                    typeof(ISingleTestAppService),
                    typeof(SingleTestService),
                };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(SingleTestService)));
        }

        [Test]
        public void RegisterServices_Single_one_service_overridden_chain()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
                typeof(ChainSingleOverrideTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(ChainSingleOverrideTestService)));
        }

        [Test]
        public void RegisterServices_Single_one_service_overridden()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(DerivedOverrideSingleTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(DerivedOverrideSingleTestService)));
        }

        [Test]
        public void RegisterServices_Single_override_service_success()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()), new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(SingleOverrideTestService)));
        }

        [Test]
        public void RegisterServices_Single_service_multiple_registrations_first()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()) { Settings = { AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.UseFirst } },
                new List<IAppServiceInfosProvider> { new PartsAppServiceInfosProvider(parts) });

            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(SingleTestService)));
            Assert.IsFalse(conventions.TypeBuilders.ContainsKey(typeof(SingleSameOverrideTestService)));
        }


        [Test]
        public void RegisterServices_Single_service_multiple_registrations_last()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()) { Settings = { AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.UseLast } },
                new List<IAppServiceInfosProvider> { new PartsAppServiceInfosProvider(parts) });

            Assert.IsTrue(conventions.TypeBuilders.ContainsKey(typeof(SingleSameOverrideTestService)));
            Assert.IsFalse(conventions.TypeBuilders.ContainsKey(typeof(SingleTestService)));
        }


        [Test]
        public void RegisterServices_Single_service_multiple_registrations_force_failure()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            Assert.Throws<AmbiguousServiceResolutionException>(() => registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()) { Settings = { AmbiguousResolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority } },
                new List<IAppServiceInfosProvider> { new PartsAppServiceInfosProvider(parts) }));
        }

        [Test]
        public void RegisterServices_generic()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IGenericAppService<>),
                typeof(GenericAppService<>),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            var match = conventions.TypeBuilders.Keys.First();
            Assert.AreEqual(match, typeof(GenericAppService<>));
        }

        [Test]
        public void RegisterServices_default_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IDefaultMetadataAppService),
                typeof(DefaultMetadataAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            var testBuilder = conventions.TypeBuilders[typeof(DefaultMetadataAppService)];
            var metadata = testBuilder.Metadata!;

            Assert.AreEqual(1, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey(nameof(AppServiceMetadata.ServiceType)));

            var valueGetter = metadata[nameof(AppServiceMetadata.ServiceType)];
            Assert.AreEqual(typeof(DefaultMetadataAppService), metadata[nameof(AppServiceMetadata.ServiceType)]);
        }

        [Test]
        public void RegisterServices_generic_with_nongeneric_base()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(1, conventions.TypeBuilders.Count);
            var testBuilder = conventions.TypeBuilders.Values.First();
            Assert.AreEqual(typeof(IOneGenericAppService), testBuilder.ContractType);
        }

        [Test]
        public void RegisterServices_generic_with_nongeneric_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
                typeof(OneGenericAppServiceString),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            var testBuilder = conventions.TypeBuilders.Values.First()!;
            var metadata = testBuilder.Metadata;

            Assert.AreEqual(2, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("TType"));
            Assert.AreEqual(typeof(string), metadata["TType"]);
        }

        [Test]
        public void RegisterServices_generic_with_nongeneric_metadata_two()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ITwoGenericAppService<,>),
                typeof(TwoGenericAppServiceIntBool),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            var testBuilder = conventions.TypeBuilders.Values.First();
            var metadata = testBuilder.Metadata;

            Assert.AreEqual(3, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("FromType"));
            Assert.IsTrue(metadata.ContainsKey("ToType"));

            Assert.AreEqual(typeof(int), metadata["FromType"]);
            Assert.AreEqual(typeof(bool), metadata["ToType"]);
        }

        [Test]
        public void RegisterServices_as_open_generic_ILogger()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var log = new StringBuilder();

            var parts = new[]
            {
                typeof(ILogger<>),
                typeof(TypedLogger<>),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.GetTestAmbientServices(m => log.AppendLine(m))),
                new List<IAppServiceInfosProvider> { new PartsAppServiceInfosProvider(parts) });

            var testBuilder = conventions.TypeBuilders.Values.Single();
            var metadata = testBuilder.Metadata;

            // should not warn that metadata attributes are not supported
            Assert.IsFalse(log.ToString().Contains(LogLevel.Warning.ToString()));

            Assert.AreEqual(2, metadata.Count);
        }

        [Test]
        public void RegisterServices_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IExplicitMetadataAppService),
                typeof(ExplicitMetadataAppService),
                typeof(NullExplicitMetadataAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(2, conventions.TypeBuilders.Count);
            var nullBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(NullExplicitMetadataAppService));
            var explicitBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(ExplicitMetadataAppService));
            Assert.AreEqual(nullBuilderEntry.Value.ServiceType, typeof(NullExplicitMetadataAppService));

            var nullMetadata = nullBuilderEntry.Value.Metadata!;
            var explicitMetadata = explicitBuilderEntry.Value.Metadata!;

            Assert.AreEqual(1, nullMetadata.Count);
            Assert.IsFalse(nullMetadata.ContainsKey(nameof(AppServiceMetadata.ProcessingPriority)));

            Assert.AreEqual(2, explicitMetadata.Count);
            Assert.IsTrue(explicitMetadata.ContainsKey(nameof(AppServiceMetadata.ProcessingPriority)));
            Assert.AreEqual((Priority)100, explicitMetadata[nameof(AppServiceMetadata.ProcessingPriority)]);
        }

        [Test]
        public void RegisterServices_metadata_IMetadataValue()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(2, conventions.TypeBuilders.Count);
            var customEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomValueMetadataAppService));
            var nullEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomValueNullMetadataAppService));
            Assert.AreEqual(customEntry.Value.ServiceType, typeof(CustomValueMetadataAppService));

            var customBuilder = customEntry.Value;
            var customMetadata = customBuilder.Metadata!;

            Assert.AreEqual(2, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["CustomValueMetadata"]);

            Assert.IsFalse(nullEntry.Value.Metadata!.ContainsKey("CustomValueMetadata"));
        }

        [Test]
        public void RegisterServices_metadata_MetadataValue_properties()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(2, conventions.TypeBuilders.Count);

            var customBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomValueMetadataAppService));
            var customMetadata = customBuilderEntry.Value.Metadata!;
            Assert.AreEqual(2, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["CustomValueMetadata"]);

            var nullBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomValueNullMetadataAppService));
            var nullMetadata = nullBuilderEntry.Value.Metadata!;
            Assert.IsFalse(nullMetadata.ContainsKey("CustomValueMetadata"));
        }

        [Test]
        public void RegisterServices_metadata_MetadataNamedValue_properties()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(ICustomNamedValueMetadataAppService),
                typeof(CustomNamedValueMetadataAppService),
                typeof(CustomNamedValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            registrar.RegisterServices(
                conventions,
                new TestBuildContext(this.CreateAmbientServices()),
                new IAppServiceInfosProvider[] { new PartsAppServiceInfosProvider(parts) });

            Assert.AreEqual(2, conventions.TypeBuilders.Count);
            var customBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomNamedValueMetadataAppService));
            Assert.AreEqual(customBuilderEntry.Value.ServiceType, typeof(CustomNamedValueMetadataAppService));

            var nullBuilderEntry = conventions.TypeBuilders.First(kv => kv.Key == typeof(CustomNamedValueNullMetadataAppService));
            Assert.AreEqual(nullBuilderEntry.Value.ServiceType, typeof(CustomNamedValueNullMetadataAppService));

            var customMetadata = customBuilderEntry.Value.Metadata!;
            var nullMetadata = nullBuilderEntry.Value.Metadata!;

            Assert.AreEqual(3, customMetadata.Count);
            Assert.AreEqual("hi there", customMetadata["Name"]);
            Assert.AreEqual("ICXP", customMetadata["IconName"]);
            Assert.IsFalse(customMetadata.ContainsKey("Description"));

            Assert.IsFalse(nullMetadata.ContainsKey("Name"));
            Assert.IsFalse(nullMetadata.ContainsKey("IconName"));
        }

        [Test]
        public void RegisterServices_bad_contract_type()
        {
            var conventions = new InjectorBuilderBaseTest.TestRegistrationInjectorBuilder();

            var parts = new[]
            {
                typeof(IBadAppService),
                typeof(BadAppService),
            };
            var registrar = CreateAppServiceInfoInjectionRegistrar();
            Assert.Throws<InjectionException>(
                () => registrar.RegisterServices(
                    conventions,
                    new TestBuildContext(this.CreateAmbientServices()),
                    new List<IAppServiceInfosProvider> { new PartsAppServiceInfosProvider(parts) }));
        }

        private static AppServiceInfoInjectionRegistrar CreateAppServiceInfoInjectionRegistrar()
        {
            var registrar = new AppServiceInfoInjectionRegistrar();
            return registrar;
        }

        private IAmbientServices GetTestAmbientServices(Action<string>? logAction = null)
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

            var ambientServices = this.CreateAmbientServices();
            ambientServices.Add(logManager);

            return ambientServices;
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
        using System.Collections.Generic;

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