// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataContextProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data context provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Composition;
    using Kephas.Data.Store;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultDataContextProviderTest
    {
        [Test]
        public void GetDataContext_success()
        {
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStore("test-store").Returns(new DataStore("test-store", "kind1"));
            var dataContext1 = Substitute.For<IDataContext>();

            var provider = new DefaultDataContextProvider(
                new List<IExportFactory<IDataContext, DataContextMetadata>>
                    {
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext1, new DataContextMetadata(new[] { "kind1" }))
                    },
                dataStoreProvider);

            var dataContext = provider.GetDataContext("test-store");
            Assert.AreSame(dataContext1, dataContext);
        }

        [Test]
        public void GetDataContext_ambiguous_match_for_two_same_kinds()
        {
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStore("test-store").Returns(new DataStore("test-store", "kind1"));
            var dataContext1 = Substitute.For<IDataContext>();
            var dataContext2 = Substitute.For<IDataContext>();

            var provider = new DefaultDataContextProvider(
                new List<IExportFactory<IDataContext, DataContextMetadata>>
                    {
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext1, new DataContextMetadata(new[] { "kind1" })),
                        new ExportFactory<IDataContext, DataContextMetadata>(() => dataContext2, new DataContextMetadata(new[] { "kind1" }))
                    },
                dataStoreProvider);

            Assert.Throws<AmbiguousMatchDataException>(() => provider.GetDataContext("test-store"));
        }
    }
}