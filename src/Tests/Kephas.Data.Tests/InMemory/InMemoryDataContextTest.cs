// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDataContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the InMemoryDataContextTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.InMemory
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Composition;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory;
    using Kephas.Data.Store;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(Substitute.For<ICompositionContext>(), string.Empty)));

            dataContext.Attach("mama").ChangeState = ChangeState.Added;
            dataContext.Attach("papa").ChangeState = ChangeState.Added;
            dataContext.Attach(1).ChangeState = ChangeState.Added;

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
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "")));

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

            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "InitialData=dummy-will-be-mocked")));

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
                    new InMemoryDataContextConfiguration(Substitute.For<ICompositionContext>(), string.Empty)
                    {
                        InitialData =
                                new[]
                                    {
                                        new EntityEntry("mama"),
                                        new EntityEntry("papa")
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
            initializationContext.WithInitialData(new[]
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
            initializationContext.WithInitialData(new[]
                                                     {
                                                         new EntityEntry("papa")
                                                     });
            dataContext.Initialize(
                this.GetDataInitializationContext(
                    dataContext,
                    new InMemoryDataContextConfiguration(Substitute.For<ICompositionContext>(), string.Empty)
                    {
                        InitialData =
                                new[]
                                    {
                                        new EntityEntry("mama"),
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
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "UseSharedCache=true")));

            var dataContext2 = this.CreateInMemoryDataContext();
            dataContext2.Initialize(this.GetDataInitializationContext(dataContext2, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "UseSharedCache=true")));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.Attach(sharedItem).ChangeState = ChangeState.Added;
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreSame(sharedItem, sharedItemActual);
        }

        [Test]
        public void Initialize_non_shared_cache()
        {
            var dataContext = this.CreateInMemoryDataContext();
            dataContext.Initialize(this.GetDataInitializationContext(dataContext, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "UseSharedCache=false")));

            var dataContext2 = this.CreateInMemoryDataContext();
            dataContext2.Initialize(this.GetDataInitializationContext(dataContext2, new DataContextConfiguration(Substitute.For<ICompositionContext>(), "UseSharedCache=false")));

            var sharedItem = Substitute.For<IIdentifiable>();
            dataContext.Attach(sharedItem).ChangeState = ChangeState.Added;
            var sharedItemActual = dataContext2.Query<IIdentifiable>().FirstOrDefault();

            Assert.AreNotSame(sharedItem, sharedItemActual);
            Assert.IsNull(sharedItemActual);
        }

        private InMemoryDataContext CreateInMemoryDataContext(
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            ISerializationService serializationService = null)
        {
            return new InMemoryDataContext(
                compositionContext ?? Substitute.For<ICompositionContext>(),
                dataCommandProvider ?? Substitute.For<IDataCommandProvider>(),
                dataBehaviorProvider ?? Substitute.For<IDataBehaviorProvider>(),
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
            var activator = this.CreateActivatorForInterfaces();
            var dataStore = new DataStore("test", "test-kind", dataContextConfiguration: config, entityActivator: activator);

            return dataStore;
        }

        private IActivator CreateActivatorForInterfaces()
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
    }
}