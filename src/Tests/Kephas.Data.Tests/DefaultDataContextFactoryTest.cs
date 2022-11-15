// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataContextFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data context provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Collections.Generic;

    using Kephas.Data;
    using Kephas.Data.Store;
    using Kephas.Injection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataContextFactoryTest
    {
        [Test]
        public void CreateDataContext_success()
        {
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStore("test-store").Returns(new DataStore("test-store", "kind1"));
            var dataContext1 = Substitute.For<IDataContext>();

            var provider = new DefaultDataContextFactory(
                new List<IExportFactory<IDataContext, DataContextMetadata>>
                    {
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext1, new DataContextMetadata(new[] { "kind1" }))
                    },
                dataStoreProvider);

            var dataContext = provider.CreateDataContext("test-store");
            Assert.AreSame(dataContext1, dataContext);
        }

        [Test]
        public void CreateDataContext_proper_initialized()
        {
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            var dcConfig = Substitute.For<IDataContextSettings>();
            var dataStore = new DataStore("test-store", "kind1", dataContextSettings: dcConfig);
            dataStoreProvider.GetDataStore("test-store").Returns(dataStore);
            IDataInitializationContext initContext = null;
            var dataContext1 = Substitute.For<IDataContext>();
            dataContext1.When(ctx => ctx.Initialize(Arg.Any<IContext>())).Do(ci => initContext = ci.Arg<IContext>() as IDataInitializationContext);

            var provider = new DefaultDataContextFactory(
                new List<IExportFactory<IDataContext, DataContextMetadata>>
                    {
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext1, new DataContextMetadata(new[] { "kind1" }))
                    },
                dataStoreProvider);

            var initData = Substitute.For<IContext>();
            var dataContext = provider.CreateDataContext("test-store", initData);
            Assert.AreSame(dataStore, initContext.DataStore);
            Assert.AreSame(initData, initContext.InitializationContext);
        }

        [Test]
        public void CreateDataContext_ambiguous_match_for_two_same_kinds()
        {
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStore("test-store").Returns(new DataStore("test-store", "kind1"));
            var dataContext1 = Substitute.For<IDataContext>();
            var dataContext2 = Substitute.For<IDataContext>();

            var provider = new DefaultDataContextFactory(
                new List<IExportFactory<IDataContext, DataContextMetadata>>
                    {
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext1, new DataContextMetadata(new[] { "kind1" })),
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext2, new DataContextMetadata(new[] { "kind1" }))
                    },
                dataStoreProvider);

            Assert.Throws<AmbiguousMatchDataException>(() => provider.CreateDataContext("test-store"));
        }
    }
}