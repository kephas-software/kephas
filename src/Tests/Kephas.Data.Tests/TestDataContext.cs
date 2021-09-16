// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the test data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Linq;
    using System.Security.Principal;

    using Kephas.Activation;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Store;
    using Kephas.Injection;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;

    public class TestDataContext : DataContextBase
    {
        public TestDataContext(
            IInjector injector = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null,
            IRuntimeTypeRegistry typeRegistry = null)
            : base(GetTestInjector(injector, typeRegistry ?? new RuntimeTypeRegistry()), GetTestDataCommandProvider(dataCommandProvider), dataBehaviorProvider, localCache: localCache)
        {
        }

        public static TestDataContext CreateDataContext(
            IInjector injector = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null,
            IRuntimeTypeRegistry typeRegistry = null)
        {
            return new TestDataContext(injector, dataCommandProvider, dataBehaviorProvider, localCache, typeRegistry);
        }

        public static TestDataContext InitializeDataContext(
            IContext initializationContext = null,
            IInjector injector = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null,
            IRuntimeTypeRegistry typeRegistry = null)
        {
            var dataContext = new TestDataContext(injector, dataCommandProvider, dataBehaviorProvider, localCache, typeRegistry);
            dataContext.Initialize(initializationContext ?? dataContext.CreateDataInitializationContext());
            return dataContext;
        }

        public static IActivator CreateActivatorForInterfaces(IRuntimeTypeRegistry typeRegistry)
        {
            var activator = Substitute.For<IActivator>();
            activator
                .GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(
                    ci =>
                        {
                            ITypeInfo implementationType = null;
                            var typeInfo = (IRuntimeTypeInfo)ci.Arg<ITypeInfo>();
                            if (typeInfo.Type.IsInterface || typeInfo.Type.IsAbstract)
                            {
                                var inst = Substitute.For(new[] { typeInfo.Type }, new object[0]);
                                return typeRegistry.GetTypeInfo(inst.GetType());
                            }

                            return typeInfo;
                        });
            return activator;
        }

        public static IActivator CreateIdempotentActivator()
        {
            var activator = Substitute.For<IActivator>();
            activator
                .GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci => ci.Arg<ITypeInfo>());
            return activator;
        }

        public IDataInitializationContext CreateDataInitializationContext(
            IDataStore dataStore = null,
            IIdentity identity = null,
            IActivator activator = null)
        {
            var initializationContext = new DataInitializationContext(this, dataStore ?? CreateDataStore(activator))
                                                 {
                                                     Identity = identity
                                                 };
            return initializationContext;
        }

        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            return this.LocalCache.Values.Select(ei => ei.Entity).OfType<T>().AsQueryable();
        }

        private static IDataStore CreateDataStore(IActivator activator)
        {
            var dataStore = Substitute.For<IDataStore>();
            dataStore.EntityActivator.Returns(activator);
            return dataStore;
        }

        private static IInjector GetTestInjector(IInjector injector, IRuntimeTypeRegistry typeRegistry)
            => injector ?? CreateInjector(typeRegistry);

        private static IInjector CreateInjector(IRuntimeTypeRegistry typeRegistry)
        {
            var injector = Substitute.For<IInjector>();
            injector.Resolve<IRuntimeTypeRegistry>(Arg.Any<string>()).Returns(typeRegistry);
            return injector;
        }

        private static IDataCommandProvider GetTestDataCommandProvider(IDataCommandProvider dataCommandProvider)
            => dataCommandProvider ?? Substitute.For<IDataCommandProvider>();
    }
}