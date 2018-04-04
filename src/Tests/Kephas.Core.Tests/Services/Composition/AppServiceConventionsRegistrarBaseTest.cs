// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConventionsRegistrarBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service conventions registrar base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition.AttributedModel;
    using Kephas.Core.Tests.Composition;
    using Kephas.Services;
    using Kephas.Services.Composition;

    using NUnit.Framework;

    /// <summary>
    /// An application service conventions registrar base test.
    /// </summary>
    [TestFixture]
    public class AppServiceConventionsRegistrarBaseTest
    {
        [Test]
        public void RegisterConventions_hierarchy_with_properties_with_same_name()
        {
            var serviceContracts = new List<TypeInfo>
                                       {
                                           typeof(ICloneable).GetTypeInfo(),
                                           typeof(IConvertible).GetTypeInfo(),
                                           typeof(IServiceProvider).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? new AppServiceContractAttribute { AllowMultiple = ti == typeof(IServiceProvider).GetTypeInfo() } : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<TypeInfo>(serviceContracts)
                            {
                                typeof(TestBaseImporter).GetTypeInfo(),
                                typeof(TestDerivedImporter).GetTypeInfo(),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsTrue(builderEntry.Key(typeof(TestBaseImporter)));
            Assert.IsTrue(builderEntry.Key(typeof(TestDerivedImporter)));

            var conventionBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)builderEntry.Value;
            Assert.AreEqual(1, conventionBuilder.ImportedProperties.Count);
            var filter = conventionBuilder.ImportedProperties[0].Item1;
            var baseProperty = typeof(TestBaseImporter).GetTypeInfo().GetDeclaredProperty(nameof(TestBaseImporter.ImportedService));
            var derivedProperty = typeof(TestDerivedImporter).GetTypeInfo().GetDeclaredProperty(nameof(TestDerivedImporter.ImportedService));
            Assert.IsTrue(filter(baseProperty));
            Assert.IsTrue(filter(derivedProperty));
        }

        [Test]
        public void RegisterConventions_hierarchy_with_missing_default_constructor()
        {
            var serviceContracts = new List<TypeInfo>
                                       {
                                           typeof(IGeneric2<,>).GetTypeInfo(),
                                           typeof(INonGeneric).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<TypeInfo>(serviceContracts)
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
            Assert.IsNotNull(constructorInfo);
        }

        [Test]
        public void RegisterConventions_hierarchy_with_multiple_constructors_in_abstract_base()
        {
            var serviceContracts = new List<TypeInfo>
                                       {
                                           typeof(IClassifierFactory).GetTypeInfo(),
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<TypeInfo>(serviceContracts)
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
            Assert.IsNotNull(constructorInfo);
            constructorInfo = conventionBuilder.ConstructorSelector(typeof(IntClassifierFactory).GetTypeInfo().DeclaredConstructors);
            Assert.IsNotNull(constructorInfo);
        }

        [Test]
        public void RegisterConventions_exported_classes()
        {
            var serviceContracts = new List<TypeInfo>
                                       {
                                           typeof(ExportedClass).GetTypeInfo(),
                                           typeof(DerivedExportedClass).GetTypeInfo()
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<TypeInfo>(serviceContracts)
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
            var serviceContracts = new List<TypeInfo>
                                       {
                                           typeof(IConverter).GetTypeInfo(),
                                       };
            var registrar = new TestRegistrar(ti => serviceContracts.Contains(ti) ? ti.GetCustomAttribute<AppServiceContractAttribute>() : null);

            var conventions = new CompositionContainerBuilderBaseTest.TestConventionsBuilder();

            var parts = new List<TypeInfo>(serviceContracts)
                            {
                                typeof(ConverterBase<,>).GetTypeInfo(),
                            };

            registrar.RegisterConventions(conventions, parts, new TestRegistrationContext());

            Assert.AreEqual(1, conventions.MatchingConventionsBuilders.Count);
            var builderEntry = conventions.MatchingConventionsBuilders.First();
            Assert.IsFalse(builderEntry.Key(typeof(ConverterBase<,>)));
        }

        [ExcludeFromComposition]
        public class TestRegistrar : AppServiceConventionsRegistrarBase
        {
            private readonly Func<TypeInfo, AppServiceContractAttribute> attrProvider;

            public TestRegistrar(Func<TypeInfo, AppServiceContractAttribute> attrProvider)
            {
                this.attrProvider = attrProvider;
            }

            protected override AppServiceContractAttribute TryGetAppServiceContractAttribute(TypeInfo typeInfo)
            {
                return this.attrProvider(typeInfo);
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

        [SharedAppServiceContract(AllowMultiple = true)]
        public interface IConverter
        {
        }

        public class ConverterBase<TSource, TTarget> : IConverter
        {
        }

        [SharedAppServiceContract(AllowMultiple = true)]
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