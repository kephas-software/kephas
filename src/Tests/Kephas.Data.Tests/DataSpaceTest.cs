// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Linq;
    using System.Security.Principal;
    using System.Text;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Store;
    using Kephas.Injection;
    using Kephas.Reflection;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataSpaceTest : DataTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.BuildServiceProvider();
            var dataSpace = container.Resolve<IDataSpace>();

            Assert.IsNotNull(dataSpace);
        }

        [Test]
        public void GetEnumerator()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            IDataSpace dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            var dc = dataSpace[typeof(string)];

            var dataContexts = dataSpace.ToList();
            Assert.AreEqual(1, dataContexts.Count);
            Assert.AreSame(dataContext, dataContexts[0]);
        }

        [Test]
        public void Indexer_type()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            IDataSpace dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            var dc = dataSpace[typeof(string)];
            Assert.AreSame(dataContext, dc);
        }

        [Test]
        public void Indexer_typeInfo()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            IDataSpace dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            var ti = (ITypeInfo)typeof(string).AsRuntimeTypeInfo();
            var dc = dataSpace[ti];
            Assert.AreSame(dataContext, dc);
        }

        [Test]
        public void Indexer_same_data_context_when_same_type()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            IDataSpace dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            var dc1 = dataSpace[typeof(string)];
            var dc2 = dataSpace[typeof(string)];
            Assert.AreSame(dc1, dc2);
            Assert.AreEqual(1, dataSpace.Count);
        }

        [Test]
        public void Dispose()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();
            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            IDataSpace dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            var dc = dataSpace[typeof(string)];

            dataSpace.Dispose();
            Assert.AreEqual(0, dataSpace.Count);
        }

        [Test]
        public void Initialize_Identity_set()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();

            var identity = Substitute.For<IIdentity>();
            var context = new Context(injector) { Identity = identity };
            var dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider);
            dataSpace.Initialize(context);

            Assert.AreSame(identity, dataSpace.Identity);
        }

        [Test]
        public void Initialize_Identity_not_overwritten()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();

            var identity = Substitute.For<IIdentity>();
            var context = new Context(Substitute.For<IServiceProvider>()) { Identity = identity };
            var dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider) { Identity = Substitute.For<IIdentity>() };
            dataSpace.Initialize(context);

            Assert.AreNotSame(identity, dataSpace.Identity);
        }

        [Test]
        public void Initialize_initial_data()
        {
            var injector = Substitute.For<IServiceProvider>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreProvider = Substitute.For<IDataStoreProvider>();

            dataStoreProvider.GetDataStoreName(typeof(string)).Returns("string");
            dataStoreProvider.GetDataStoreName(typeof(StringBuilder)).Returns("builder");
            var strDataContext = Substitute.For<IDataContext>();
            var bldDataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("string", Arg.Any<IContext>())
                .Returns(ci =>
                    {
                        var initialData = ci.Arg<IContext>().InitialData();
                        Assert.AreEqual(1, initialData.Count());
                        Assert.AreEqual("gigi", initialData.Single().ToString());
                        return strDataContext;
                    });
            dataContextFactory.CreateDataContext("builder", Arg.Any<IContext>())
                .Returns(ci =>
                    {
                        var initialData = ci.Arg<IContext>().InitialData();
                        Assert.AreEqual(1, initialData.Count());
                        Assert.AreEqual("belogea", initialData.Single().ToString());
                        return bldDataContext;
                    });

            var identity = Substitute.For<IIdentity>();
            var context = new Context(Substitute.For<IServiceProvider>()) { Identity = identity };
            context.InitialData(
                new[]
                    {
                        new EntityEntry("gigi") { ChangeState = ChangeState.Added },
                        new EntityEntry(new StringBuilder("belogea")) { ChangeState = ChangeState.Changed }
                    });
            var dataSpace = new DataSpace(injector, dataContextFactory, dataStoreProvider) { Identity = Substitute.For<IIdentity>() };
            dataSpace.Initialize(context);

            Assert.AreNotSame(identity, dataSpace.Identity);
        }
    }
}