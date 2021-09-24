﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoConventionsRegistrarTest.cs" company="Kephas Software SRL">
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
    using System.Linq;
    using System.Text;

    using Kephas.Core.Tests.Injection;
    using Kephas.Core.Tests.Services.CustomNamedValueAppServiceMetadata;
    using Kephas.Core.Tests.Services.CustomValueAppServiceMetadata;
    using Kephas.Core.Tests.Services.DefaultAppServiceMetadata;
    using Kephas.Core.Tests.Services.DefaultExplicitAppServiceMetadata;
    using Kephas.Injection;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AppServiceInfoConventionsRegistrar"/>.
    /// </summary>
    [TestFixture]
    public class AppServiceInfoConventionsRegistrarTest
    {
        [Test]
        public void RegisterConventions_Multiple()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
                {
                    typeof(IMultipleTestAppService),
                    typeof(MultipleTestService),
                    typeof(NewMultipleTestService),
                };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.Single();

            Assert.IsTrue(builderEntry.Key(typeof(MultipleTestService)));
            Assert.IsTrue(builderEntry.Key(typeof(NewMultipleTestService)));
        }

        [Test]
        public void RegisterConventions_Single_one_service()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
                {
                    typeof(ISingleTestAppService),
                    typeof(SingleTestService),
                };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(SingleTestService)));
        }

        [Test]
        public void RegisterConventions_Single_one_service_overridden_chain()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
                typeof(ChainSingleOverrideTestService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(ChainSingleOverrideTestService)));
        }

        [Test]
        public void RegisterConventions_Single_one_service_overridden()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(DerivedOverrideSingleTestService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(DerivedOverrideSingleTestService)));
        }

        [Test]
        public void RegisterConventions_Single_override_service_success()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleOverrideTestService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(SingleOverrideTestService)));
        }

        [Test]
        public void RegisterConventions_Single_override_service_failure()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ISingleTestAppService),
                typeof(SingleTestService),
                typeof(SingleSameOverrideTestService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            Assert.Throws<AmbiguousServiceResolutionException>(() => registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts))));
        }

        [Test]
        public void RegisterConventions_generic()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IGenericAppService<>),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var match = conventions.MatchingConventionsBuilders.Keys.First();
            Assert.IsTrue(match(typeof(GenericAppService<>)));
            Assert.IsFalse(match(typeof(TwoGenericAppServiceIntBool)));
        }

        [Test]
        public void RegisterConventions_default_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IDefaultMetadataAppService),
                typeof(DefaultMetadataAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)conventions.TypeConventionsBuilders[typeof(DefaultMetadataAppService)];
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(5, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("ProcessingPriority"));
            Assert.IsTrue(metadata.ContainsKey("OverridePriority"));
            Assert.IsTrue(metadata.ContainsKey("Override"));
            Assert.IsTrue(metadata.ContainsKey("ServiceName"));
            Assert.IsTrue(metadata.ContainsKey(nameof(AppServiceMetadata.ServiceInstanceType)));

            var valueGetter = (Func<Type, object>)metadata[nameof(AppServiceMetadata.ServiceInstanceType)];
            Assert.AreEqual(typeof(IDefaultMetadataAppService), valueGetter(typeof(IDefaultMetadataAppService)));
            Assert.AreEqual(null, valueGetter(null));

            valueGetter = (Func<Type, object>)metadata["ProcessingPriority"];
            Assert.AreEqual(null, valueGetter(typeof(IDefaultMetadataAppService)));

            valueGetter = (Func<Type, object>)metadata["OverridePriority"];
            Assert.AreEqual(null, valueGetter(typeof(IDefaultMetadataAppService)));
        }

        [Test]
        public void RegisterConventions_generic_with_nongeneric_base()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)conventions.MatchingConventionsBuilders.Values.First();
            Assert.AreEqual(typeof(IOneGenericAppService), testBuilder.ExportBuilder.ContractType);
        }

        [Test]
        public void RegisterConventions_generic_with_nongeneric_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IOneGenericAppService<>),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)conventions.MatchingConventionsBuilders.Values.First();
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(6, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("TType"));

            var valueGetter = (Func<Type, object>)metadata["TType"];
            Assert.AreEqual(typeof(string), valueGetter(typeof(OneGenericAppServiceString)));
        }

        [Test]
        public void RegisterConventions_generic_with_nongeneric_metadata_two()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ITwoGenericAppService<,>),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)conventions.MatchingConventionsBuilders.Values.First();
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(7, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("FromType"));
            Assert.IsTrue(metadata.ContainsKey("ToType"));

            var valueGetter = (Func<Type, object>)metadata["FromType"];
            Assert.AreEqual(typeof(int), valueGetter(typeof(TwoGenericAppServiceIntBool)));

            valueGetter = (Func<Type, object>)metadata["ToType"];
            Assert.AreEqual(typeof(bool), valueGetter(typeof(TwoGenericAppServiceIntBool)));
        }

        [Test]
        public void RegisterConventions_as_open_generic_ILogger()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var log = new StringBuilder();

            var parts = new[]
            {
                typeof(ILogger<>),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(this.GetTestAmbientServices(m => log.AppendLine(m))).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)conventions.MatchingConventionsBuilders.Values.Single();
            var metadata = testBuilder.ExportBuilder.Metadata;

            // should not warn that metadata attributes are not supported
            Assert.IsFalse(log.ToString().Contains(LogLevel.Warning.ToString()));

            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void RegisterConventions_metadata()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IExplicitMetadataAppService),
                typeof(ExplicitMetadataAppService),
                typeof(NullExplicitMetadataAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(NullExplicitMetadataAppService)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(5, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("ProcessingPriority"));

            var valueGetter = (Func<Type, object>)metadata["ProcessingPriority"];
            Assert.AreEqual(100, valueGetter(typeof(ExplicitMetadataAppService)));
            Assert.IsNull(valueGetter(typeof(NullExplicitMetadataAppService)));
        }

        [Test]
        public void RegisterConventions_metadata_IMetadataValue()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(CustomValueNullMetadataAppService)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(6, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("CustomValueMetadata"));

            var valueGetter = (Func<Type, object>)metadata["CustomValueMetadata"];
            Assert.AreEqual("hi there", valueGetter(typeof(CustomValueMetadataAppService)));
            Assert.IsNull(valueGetter(typeof(CustomValueNullMetadataAppService)));
        }

        [Test]
        public void RegisterConventions_metadata_MetadataValue_properties()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ICustomValueMetadataAppService),
                typeof(CustomValueMetadataAppService),
                typeof(CustomValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(CustomValueNullMetadataAppService)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(6, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("CustomValueMetadata"));

            var valueGetter = (Func<Type, object>)metadata["CustomValueMetadata"];
            Assert.AreEqual("hi there", valueGetter(typeof(CustomValueMetadataAppService)));
            Assert.IsNull(valueGetter(typeof(CustomValueNullMetadataAppService)));
        }

        [Test]
        public void RegisterConventions_metadata_MetadataNamedValue_properties()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(ICustomNamedValueMetadataAppService),
                typeof(CustomNamedValueMetadataAppService),
                typeof(CustomNamedValueNullMetadataAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            registrar.RegisterConventions(
                conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts)));

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(CustomNamedValueNullMetadataAppService)));

            var testBuilder = (InjectorBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var metadata = testBuilder.ExportBuilder.Metadata;

            Assert.AreEqual(7, metadata.Count);
            Assert.IsTrue(metadata.ContainsKey("CustomNamedValueMetadataName"));
            Assert.IsTrue(metadata.ContainsKey("Icon"));
            Assert.IsFalse(metadata.ContainsKey("CustomNamedValueMetadataDescription"));

            var valueGetter = (Func<Type, object>)metadata["CustomNamedValueMetadataName"];
            var iconValueGetter = (Func<Type, object>)metadata["Icon"];
            Assert.AreEqual("hi there", valueGetter(typeof(CustomNamedValueMetadataAppService)));
            Assert.AreEqual("ICXP", iconValueGetter(typeof(CustomNamedValueMetadataAppService)));
            Assert.IsNull(valueGetter(typeof(CustomNamedValueNullMetadataAppService)));
            Assert.IsNull(iconValueGetter(typeof(CustomNamedValueNullMetadataAppService)));
        }

        [Test]
        public void RegisterConventions_bad_contract_type()
        {
            var conventions = new InjectorBuilderBaseTest.TestConventionsBuilder();

            var parts = new[]
            {
                typeof(IBadAppService),
                typeof(BadAppService),
            };
            var registrar = CreateAppServiceInfoConventionsRegistrar();
            Assert.Throws<InjectionException>(
                () => registrar.RegisterConventions(
                    conventions,
                new TestBuildContext(new AmbientServices()).WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts))));
        }

        [Test]
        public void DefaultMetadataAttributeTypes()
        {
            Assert.IsTrue(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(OverridePriorityAttribute)));
            Assert.IsTrue(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(ProcessingPriorityAttribute)));
            Assert.IsTrue(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(ServiceNameAttribute)));
        }

        [Test]
        public void RegisterDefaultMetadataAttributeTypes_success()
        {
            Assert.IsFalse(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(DummyAttribute)));

            AppServiceInfoConventionsRegistrar.RegisterDefaultMetadataAttributeTypes(typeof(DummyAttribute));

            Assert.IsTrue(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(DummyAttribute)));
        }

        [Test]
        public void RegisterDefaultMetadataAttributeTypes_unique_types()
        {
            Assert.IsTrue(AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Contains(typeof(ServiceNameAttribute)));

            AppServiceInfoConventionsRegistrar.RegisterDefaultMetadataAttributeTypes(typeof(ServiceNameAttribute));

            Assert.AreEqual(1, AppServiceInfoConventionsRegistrar.DefaultMetadataAttributeTypes.Count(t => t == typeof(ServiceNameAttribute)));
        }

        private static AppServiceInfoConventionsRegistrar CreateAppServiceInfoConventionsRegistrar()
        {
            var registrar = new AppServiceInfoConventionsRegistrar(new AppServiceMetadataResolver(new RuntimeTypeRegistry()));
            return registrar;
        }

        public class DummyAttribute : Attribute
        {
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

            var ambientServices = new AmbientServices();
            ambientServices.Register(logManager);

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
        [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(CustomValueMetadataAttribute) })]
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

        [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(CustomNamedValueMetadataAttribute) })]
        public interface ICustomNamedValueMetadataAppService { }

        [CustomNamedValueMetadata("hi there", "bob", IconName = "ICXP")]
        public class CustomNamedValueMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueNullMetadataAppService : ICustomNamedValueMetadataAppService { }

        public class CustomNamedValueMetadataAttribute : Attribute, IMetadataProvider
        {
            public CustomNamedValueMetadataAttribute(string value, string description)
            {
                this.Name = value;
                this.Description = description;
            }

            public string Name { get; }

            public string IconName { get; set; }

            public string Description { get; }

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
        [SingletonAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
        public interface IExplicitMetadataAppService { }

        [ProcessingPriority(100)]
        public class ExplicitMetadataAppService : IExplicitMetadataAppService { }

        public class NullExplicitMetadataAppService : IExplicitMetadataAppService { }
    }
}