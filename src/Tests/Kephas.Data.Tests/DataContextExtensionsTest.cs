// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextExtensionsTest
    {
        [Test]
        public void DetachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityEntry = dataContext.AttachEntity(entity);

            var detachedEntityEntry = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.AreSame(entityEntry, detachedEntityEntry);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_not_attached()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";

            var detachedEntityEntry = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.IsNull(detachedEntityEntry);
        }

        [Test]
        public void DetachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityEntry = dataContext.AttachEntity(entity);

            var detachedEntityEntry = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.AreSame(entityEntry, detachedEntityEntry);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void CreateCommand()
        {
            var dataContext = Substitute.For<IDataContext>();
            var command = Substitute.For<IExecuteCommand>();
            dataContext.CreateCommand(typeof(IExecuteCommand)).Returns(command);
            var actualCommand = DataContextExtensions.CreateCommand<IExecuteCommand>(dataContext);
            Assert.AreSame(command, actualCommand);
        }

        [Test]
        public async Task ExecuteAsync_commandText()
        {
            var dataContext = Substitute.For<IDataContext>();
            var command = Substitute.For<IExecuteCommand>();
            command.ExecuteAsync(Arg.Any<IExecuteContext>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        var res = Substitute.For<IExecuteResult>();
                        res.Result.Returns(ci.Arg<IExecuteContext>().CommandText + " executed");
                        return Task.FromResult(res);
                    });
            dataContext.CreateCommand(typeof(IExecuteCommand)).Returns(command);
            var result = await DataContextExtensions.ExecuteAsync(dataContext, "do this");
            Assert.AreEqual("do this executed", result);
        }

        [Test]
        public async Task ExecuteAsync_commandContext()
        {
            var dataContext = Substitute.For<IDataContext>();
            var command = Substitute.For<IExecuteCommand>();
            command.ExecuteAsync(Arg.Any<IExecuteContext>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        var res = Substitute.For<IExecuteResult>();
                        res.Result.Returns(ci.Arg<IExecuteContext>().CommandText + " executed");
                        return Task.FromResult(res);
                    });
            dataContext.CreateCommand(typeof(IExecuteCommand)).Returns(command);
            var commandContext = new ExecuteContext(dataContext) { CommandText = "do this" };
            var result = await DataContextExtensions.ExecuteAsync(dataContext, commandContext);
            Assert.AreEqual("do this executed", result);
        }

        [Test]
        public async Task ExecuteAsync_commandContext_mismatched_data_context()
        {
            var dataContext = Substitute.For<IDataContext>();
            var command = Substitute.For<IExecuteCommand>();
            command.ExecuteAsync(Arg.Any<IExecuteContext>(), Arg.Any<CancellationToken>()).Returns(
                ci =>
                    {
                        var res = Substitute.For<IExecuteResult>();
                        res.Result.Returns(ci.Arg<IExecuteContext>().CommandText + " executed");
                        return Task.FromResult(res);
                    });
            dataContext.CreateCommand(typeof(IExecuteCommand)).Returns(command);
            var commandContext = new ExecuteContext(Substitute.For<IDataContext>());

            Assert.ThrowsAsync<DataException>(() => DataContextExtensions.ExecuteAsync(dataContext, commandContext));
        }

        [Test]
        public async Task CreateEntityAsync_delegate_to_data_context()
        {
            var dataContext = Substitute.For<IDataContext>();
            var stringCmd = Substitute.For<ICreateEntityCommand>();
            dataContext.CreateCommand(typeof(ICreateEntityCommand)).Returns(stringCmd);
            stringCmd.ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new CreateEntityResult("created", Substitute.For<IEntityEntry>()));

            var newEntity = await dataContext.CreateEntityAsync<string>();

            Assert.AreEqual("created", newEntity);

            dataContext.Received(1).CreateCommand(typeof(ICreateEntityCommand));
            stringCmd.Received(1).ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task PersistChangesAsync_delegate_to_command()
        {
            var dataContext = Substitute.For<IDataContext>();
            var stringCmd = Substitute.For<IPersistChangesCommand>();
            dataContext.CreateCommand(typeof(IPersistChangesCommand)).Returns(stringCmd);

            await dataContext.PersistChangesAsync();

            dataContext.Received(1).CreateCommand(typeof(IPersistChangesCommand));
            stringCmd.Received(1).ExecuteAsync(Arg.Any<IPersistChangesContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void DiscardChanges_delegate_to_command()
        {
            var dataContext = Substitute.For<IDataContext>();
            var stringCmd = Substitute.For<IDiscardChangesCommand>();
            dataContext.CreateCommand(typeof(IDiscardChangesCommand)).Returns(stringCmd);

            dataContext.DiscardChanges();

            dataContext.Received(1).CreateCommand(typeof(IDiscardChangesCommand));
            stringCmd.Received(1).Execute(Arg.Any<IDiscardChangesContext>());
        }
    }
}