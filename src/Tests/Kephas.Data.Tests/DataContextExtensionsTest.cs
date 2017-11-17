// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_not_attached()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";

            var detachedEntityInfo = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.IsNull(detachedEntityInfo);
        }

        [Test]
        public void DetachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = DataContextExtensions.DetachEntity(dataContext, entity);
            Assert.AreSame(entityInfo, detachedEntityInfo);
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
    }
}