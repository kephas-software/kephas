// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContextTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the InMemoryDataContextTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory;
    using Kephas.Data.Store;
    using Kephas.Security;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Services.Transitioning;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InMemoryDataContextTest
    {
        [Test]
        public void Query_not_initialized_exception()
        {
            var dataContext = this.CreateInMemoryDataContext();

            Assert.Throws<ServiceTransitioningException>(
                () =>
                    {
                        var query = dataContext.Query<string>();
                    });
        }

        [Test]
        public void Query_of_string()
        {
            var dataContext = this.CreateInMemoryDataContext();
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(string.Empty)));

            dataContext.AttachEntity("mama").ChangeState = ChangeState.Added;
            dataContext.AttachEntity("papa").ChangeState = ChangeState.Added;
            dataContext.AttachEntity(1).ChangeState = ChangeState.Added;

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void CreateCommand_Find()
        {
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCommand = Substitute.For<IFindCommand>();
            dataCommandProvider.CreateCommand(typeof(InMemoryDataContext), typeof(IFindCommand)).Returns(findCommand);
            var dataContext = this.CreateInMemoryDataContext(dataCommandProvider: dataCommandProvider);

            var actualCommand = dataContext.CreateCommand(typeof(IFindCommand));
            Assert.AreSame(findCommand, actualCommand);
        }

        [Test]
        public void Initialize_initial_data_from_connection_string()
        {
            var dataContext = this.CreateInMemoryDataContext();

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(
                Arg.Any<TextReader>(),
                Arg.Any<ISerializationContext>(),
                Arg.Any<CancellationToken>()).Returns(Task.FromResult((object)new[] { "mama", "papa" }));

            dataContext.SerializationService.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);

            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration("InitialData=dummy-will-be-mocked")));

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void Initialize_initial_data_from_configuration_context()
        {
            var dataContext = this.CreateInMemoryDataContext();

            dataContext.Initialize(
                this.GetDataInitializationContext(
                    dataContext,
                    new InMemoryDataContextConfiguration(string.Empty)
                        {
                            InitialData =
                                new[]
                                    {
                                        new EntityInfo("mama"),
                                        new EntityInfo("papa")
                                    }
                        }));

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void Initialize_initial_data_from_initialization_context()
        {
            var dataContext = this.CreateInMemoryDataContext();

            var initializationContext = new DataOperationContext(dataContext);
            initializationContext.SetInitialData(new[]
                                                     {
                                                         "mama",
                                                         "papa"
                                                     });
            dataContext.Initialize(
                this.GetDataInitializationContext(
                    dataContext,
                    null,
                    initializationContext));

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }


        [Test]
        public void Initialize_initial_data_from_config_and_initialization_context()
        {
            var dataContext = this.CreateInMemoryDataContext();

            var initializationContext = new DataOperationContext(dataContext);
            initializationContext.SetInitialData(new[]
                                                     {
                                                         new EntityInfo("papa")
                                                     });
            dataContext.Initialize(
                this.GetDataInitializationContext(
                    dataContext,
                    new InMemoryDataContextConfiguration(string.Empty)
                        {
                            InitialData =
                                new[]
                                    {
                                        new EntityInfo("mama"),
                                    }
                        },
                    initializationContext));

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void Initialize_shared_cache()
        {
            var dataContext = this.CreateInMemoryDataContext();
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration("UseSharedCache=true")));

            var dataContext2 = this.CreateInMemoryDataContext();
            dataContext2.Initialize(this.GetDataInitializationContext(dataContext2, new DataContextConfiguration("UseSharedCache=true")));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.AttachEntity(sharedItem).ChangeState = ChangeState.Added;
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreSame(sharedItem, sharedItemActual);
        }

        [Test]
        public void Initialize_non_shared_cache()
        {
            var dataContext = this.CreateInMemoryDataContext();
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration("UseSharedCache=false")));

            var dataContext2 = this.CreateInMemoryDataContext();
            dataContext2.Initialize(this.GetDataInitializationContext(dataContext2, new DataContextConfiguration("UseSharedCache=false")));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.AttachEntity(sharedItem).ChangeState = ChangeState.Added;
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreNotSame(sharedItem, sharedItemActual);
            Assert.IsNull(sharedItemActual);
        }

        private InMemoryDataContext CreateInMemoryDataContext(
            IAmbientServices ambientServices = null,
            IDataCommandProvider dataCommandProvider = null,
            IIdentityProvider identityProvider = null,
            ISerializationService serializationService = null)
        {
            return new InMemoryDataContext(
                ambientServices ?? Substitute.For<IAmbientServices>(),
                dataCommandProvider ?? Substitute.For<IDataCommandProvider>(),
                identityProvider ?? Substitute.For<IIdentityProvider>(),
                serializationService ?? Substitute.For<ISerializationService>());
        }

        private IDataInitializationContext GetDataInitializationContext(
            IDataContext dataContext,
            IDataContextConfiguration config,
            IContext initializationContext = null)
        {
            return new DataInitializationContext(dataContext, this.GetDataStore(config), initializationContext);
        }

        private IDataStore GetDataStore(IDataContextConfiguration config)
        {
            return new DataStore("test", "test-kind", dataContextConfiguration: config);
        }
    }
}