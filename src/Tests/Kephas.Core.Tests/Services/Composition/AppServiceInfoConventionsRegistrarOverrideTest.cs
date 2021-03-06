﻿namespace Kephas.Core.Tests.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Core.Tests.Composition;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;

    using NUnit.Framework;

    /// <summary>
    /// An application service conventions registrar base test.
    /// </summary>
    [TestFixture]
    public class AppServiceInfoConventionsRegistrarOverrideTest
    {
        [Test]
        public void RegisterConventions_hierarchy_with_properties_with_same_name()
        {
            var serviceContracts = new List<Type>
                                       {
                                           typeof(ICloneable).GetTypeInfo(),
                                           typeof(IConvertible).GetTypeInfo(),
                                           typeof(IServiceProvider).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? new AppServiceContractAttribute { AllowMultiple = ti == typeof(IServiceProvider).GetTypeInfo() } : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<Type>(serviceContracts)
                            {
                                typeof(TestBaseImporter).GetTypeInfo(),
                                typeof(TestDerivedImporter).GetTypeInfo(),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(TestBaseImporter)));
            Assert.IsTrue(builderEntry.Key(typeof(TestDerivedImporter)));
        }

        [Test]
        public void RegisterConventions_hierarchy_with_missing_default_constructor()
        {
            var serviceContracts = new List<Type>
                                       {
                                           typeof(IGeneric2<,>).GetTypeInfo(),
                                           typeof(INonGeneric).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<Type>(serviceContracts)
                            {
                                typeof(Generic2Base<,>).GetTypeInfo(),
                                typeof(Generic2).GetTypeInfo(),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(Generic2)));

            var conventionBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var constructorInfo = conventionBuilder.ConstructorSelector(typeof(Generic2).GetTypeInfo().DeclaredConstructors);
            Assert.IsNull(constructorInfo);
        }

        [Test]
        public void RegisterConventions_hierarchy_with_multiple_constructors_in_abstract_base()
        {
            var serviceContracts = new List<Type>
                                       {
                                           typeof(IClassifierFactory).GetTypeInfo(),
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<Type>(serviceContracts)
                            {
                                typeof(ClassifierFactoryBase).GetTypeInfo(),
                                typeof(StringClassifierFactory).GetTypeInfo(),
                                typeof(IntClassifierFactory).GetTypeInfo(),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(StringClassifierFactory)));
            Assert.IsTrue(builderEntry.Key(typeof(IntClassifierFactory)));
            Assert.IsFalse(builderEntry.Key(typeof(ClassifierFactoryBase)));

            var conventionBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var constructorInfo = conventionBuilder.ConstructorSelector(typeof(StringClassifierFactory).GetTypeInfo().DeclaredConstructors);
            Assert.IsNull(constructorInfo);
            constructorInfo = conventionBuilder.ConstructorSelector(typeof(IntClassifierFactory).GetTypeInfo().DeclaredConstructors);
            Assert.IsNull(constructorInfo);
        }

        [Test]
        public void RegisterConventions_exported_classes()
        {
            var serviceContracts = new List<Type>
                                       {
                                           typeof(ExportedClass).GetTypeInfo(),
                                           typeof(DerivedExportedClass).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<Type>(serviceContracts)
                            {
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            var builderEntry = conventions.TypeConventionsBuilders.First();
            Assert.AreSame(typeof(ExportedClass), builderEntry.Key);

            var builder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            var exportBuilder = builder.ExportBuilder;
            Assert.AreSame(typeof(ExportedClass), exportBuilder.ContractType);
        }

        [Test]
        public void RegisterConventions_hierarchy_with_non_generic_contract_and_non_abstract_generic_implementation()
        {
            var serviceContracts = new List<Type>
                                       {
                                           typeof(IConverter).GetTypeInfo(),
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<Type>(serviceContracts)
                            {
                                typeof(ConverterBase<,>),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsFalse(builderEntry.Key(typeof(ConverterBase<,>)));
        }

        [ExcludeFromComposition]
        public class TestRegistrar : AppServiceInfoConventionsRegistrar
        {
            private readonly Func<Type, AppServiceContractAttribute> attrProvider;

            public TestRegistrar(Func<Type, AppServiceContractAttribute> attrProvider)
            {
                this.attrProvider = attrProvider;
            }

            protected override IEnumerable<IAppServiceInfoProvider> GetAppServiceInfoProviders(IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
            {
                yield return new TestAppServiceInfoProvider(this.attrProvider);
            }

            private class TestAppServiceInfoProvider : AttributedAppServiceInfoProvider
            {
                private readonly Func<Type, AppServiceContractAttribute> attrProvider;

                public TestAppServiceInfoProvider(Func<Type, AppServiceContractAttribute> attrProvider)
                {
                    this.attrProvider = attrProvider;
                }

                protected override IAppServiceInfo TryGetAppServiceInfo(Type type)
                {
                    return this.attrProvider(type);
                }
            }
        }

        public class TestBaseImporter : IServiceProvider
        {
            public ICloneable ImportedService { get; set; }

            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }

        public class TestDerivedImporter : TestBaseImporter
        {
            public new IConvertible ImportedService { get; set; }
        }

        public interface INonGeneric
        {
        }

        [AppServiceContract(ContractType = typeof(INonGeneric), AllowMultiple = true)]
        public interface IGeneric2<TSource, TTarget> : INonGeneric
        {
        }

        public abstract class Generic2Base<TSource, TTarget> : IGeneric2<TSource, TTarget>
        {
            protected Generic2Base()
            {
            }

            protected Generic2Base(string name)
            {
            }
        }

        public class Generic2 : Generic2Base<string, int>
        {
        }

        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface IConverter
        {
        }

        public class ConverterBase<TSource, TTarget> : IConverter
        {
        }

        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface IClassifierFactory
        {
        }

        public abstract class ClassifierFactoryBase : IClassifierFactory
        {
            protected ClassifierFactoryBase(Func<Type> factory)
                : this(factory, t => true)
            {
            }

            protected ClassifierFactoryBase(Func<Type> factory, Func<Type, bool> supportedTypePredicate)
            {
            }
        }

        [ProcessingPriority(Priority.Lowest)]
        public class StringClassifierFactory : ClassifierFactoryBase
        {
            public StringClassifierFactory()
                : base(() => typeof(string), t => t.IsInterface)
            {
            }
        }

        public class IntClassifierFactory : ClassifierFactoryBase
        {
            public IntClassifierFactory()
                : base(() => typeof(int), t => t.IsInterface)
            {
            }
        }

        [AppServiceContract]
        public class ExportedClass { }

        [OverridePriority(Priority.High)]
        public class DerivedExportedClass { }
    }
}