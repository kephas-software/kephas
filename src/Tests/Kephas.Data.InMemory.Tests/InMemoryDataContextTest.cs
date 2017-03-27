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
    using Kephas.Serialization;
    using Kephas.Services.Transitioning;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InMemoryDataContextTest
    {
        [Test]
        public void Query_not_initialized_exception()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());

            Assert.Throws<ServiceTransitioningException>(
                () =>
                    {
                        var query = dataContext.Query<string>();
                    });
        }

        [Test]
        public void Query_of_string()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());
            dataContext.Initialize(new DataContextConfiguration(string.Empty));

            dataContext.GetOrAddCacheableItem(null, new EntityInfo("mama", ChangeState.Added));
            dataContext.GetOrAddCacheableItem(null, new EntityInfo("papa", ChangeState.Added));
            dataContext.GetOrAddCacheableItem(null, new EntityInfo(1, ChangeState.Added));

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
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), dataCommandProvider, Substitute.For<ISerializationService>());

            var actualCommand = dataContext.CreateCommand<IFindCommand>();
            Assert.AreSame(findCommand, actualCommand);
        }

        [Test]
        public void TryGetCapability_IIdentifiable()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());

            var entity = Substitute.For<IIdentifiable>();
            var idCapability = dataContext.TryGetCapability<IIdentifiable>(entity, null);
            Assert.AreSame(idCapability, entity);

            idCapability = dataContext.TryGetCapability<IIdentifiable>("a string", null);
            Assert.IsNull(idCapability);
        }

        [Test]
        public void Initialize_initial_data_from_connection_string()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(
                Arg.Any<TextReader>(),
                Arg.Any<ISerializationContext>(),
                Arg.Any<CancellationToken>()).Returns(Task.FromResult((object)new[] { "mama", "papa" }));

            dataContext.SerializationService.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);

            dataContext.Initialize(new DataContextConfiguration("InitialData=dummy-will-be-mocked"));

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void Initialize_initial_data_from_configuration_context()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());

            var serializer = Substitute.For<ISerializer>();
            serializer.DeserializeAsync(
                Arg.Any<TextReader>(),
                Arg.Any<ISerializationContext>(),
                Arg.Any<CancellationToken>()).Returns(Task.FromResult((object)new[] { "mama", "papa" }));

            dataContext.SerializationService.GetSerializer(Arg.Any<ISerializationContext>()).Returns(serializer);

            dataContext.Initialize(new InMemoryDataContextConfiguration(string.Empty) { InitialData = new[] { new EntityInfo("mama"), new EntityInfo("papa") } });

            var query = dataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void Initialize_shared_cache()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());
            dataContext.Initialize(new DataContextConfiguration("UseSharedCache=true"));

            var dataContext2 = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());
            dataContext2.Initialize(new DataContextConfiguration("UseSharedCache=true"));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.GetOrAddCacheableItem(new DataOperationContext(dataContext), new EntityInfo(sharedItem, ChangeState.Added));
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreSame(sharedItem, sharedItemActual);
        }

        [Test]
        public void Initialize_non_shared_cache()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());
            dataContext.Initialize(new DataContextConfiguration("UseSharedCache=false"));

            var dataContext2 = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), Substitute.For<ISerializationService>());
            dataContext2.Initialize(new DataContextConfiguration("UseSharedCache=false"));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.GetOrAddCacheableItem(new DataOperationContext(dataContext), new EntityInfo(sharedItem, ChangeState.Added));
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreNotSame(sharedItem, sharedItemActual);
            Assert.IsNull(sharedItemActual);
        }
    }
}