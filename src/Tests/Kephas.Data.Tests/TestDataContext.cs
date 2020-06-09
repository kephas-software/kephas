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
    using Kephas.Composition;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Store;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    using NSubstitute;

    public class TestDataContext : DataContextBase
    {
        public TestDataContext(
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null)
            : base(GetTestCompositionContext(compositionContext), GetTestDataCommandProvider(dataCommandProvider), dataBehaviorProvider, localCache: localCache)
        {
        }

        public static TestDataContext CreateDataContext(
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null)
        {
            return new TestDataContext(compositionContext, dataCommandProvider, dataBehaviorProvider, localCache);
        }

        public static TestDataContext InitializeDataContext(
            IContext initializationContext = null,
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null)
        {

            var dataContext = new TestDataContext(compositionContext, dataCommandProvider, dataBehaviorProvider, localCache);
            dataContext.Initialize(initializationContext ?? dataContext.CreateDataInitializationContext());
            return dataContext;
        }

        public static IActivator CreateActivatorForInterfaces()
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
                                return inst.GetType().AsRuntimeTypeInfo();
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

        private static ICompositionContext GetTestCompositionContext(ICompositionContext compositionContext)
            => compositionContext ?? CreateCompositionContext();

        private static ICompositionContext CreateCompositionContext()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            return compositionContext;
        }

        private static IDataCommandProvider GetTestDataCommandProvider(IDataCommandProvider dataCommandProvider)
            => dataCommandProvider ?? Substitute.For<IDataCommandProvider>();
    }
}