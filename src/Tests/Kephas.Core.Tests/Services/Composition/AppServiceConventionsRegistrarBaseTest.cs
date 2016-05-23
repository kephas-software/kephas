// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConventionsRegistrarBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

            registrar.RegisterConventions(conventions, parts);

            Assert.AreEqual(1, conventions.DerivedConventionsBuilders.Count);
            Assert.IsTrue(conventions.DerivedConventionsBuilders.ContainsKey(typeof(IServiceProvider)));

            var conventionBuilder = (CompositionContainerBuilderBaseTest.TestPartConventionsBuilder)conventions.DerivedConventionsBuilders.First().Value;
            Assert.AreEqual(1, conventionBuilder.ImportedProperties.Count);
            var filter = conventionBuilder.ImportedProperties[0].Item1;
            var baseProperty = typeof(TestBaseImporter).GetTypeInfo().GetDeclaredProperty(nameof(TestBaseImporter.ImportedService));
            var derivedProperty = typeof(TestDerivedImporter).GetTypeInfo().GetDeclaredProperty(nameof(TestDerivedImporter.ImportedService));
            Assert.IsTrue(filter(baseProperty));
            Assert.IsTrue(filter(derivedProperty));
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
    }
}