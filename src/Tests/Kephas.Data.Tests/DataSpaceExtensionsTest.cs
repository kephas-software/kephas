// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NSubstitute;
    using NSubstitute.Core;

    using NUnit.Framework;

    [TestFixture]
    public class DataSpaceExtensionsTest
    {
        [Test]
        public async Task CreateAsync_delegate_to_data_context()
        {
            var dataSpace = this.CreateTestDataSpace();
            var stringCmd = Substitute.For<ICreateEntityCommand>();
            dataSpace[typeof(string)].CreateCommand(typeof(ICreateEntityCommand)).Returns(stringCmd);
            stringCmd.ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new CreateEntityResult("created", Substitute.For<IEntityEntry>()));
            var sbCmd = Substitute.For<ICreateEntityCommand>();
            dataSpace[typeof(StringBuilder)].CreateCommand(typeof(ICreateEntityCommand)).Returns(sbCmd);
            sbCmd.ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new CreateEntityResult(new StringBuilder("should not get here"), Substitute.For<IEntityEntry>()));

            var newEntity = await dataSpace.CreateAsync<string>();

            Assert.AreEqual("created", newEntity);

            dataSpace[typeof(string)].Received(1).CreateCommand(typeof(ICreateEntityCommand));
            dataSpace[typeof(StringBuilder)].Received(0).CreateCommand(typeof(ICreateEntityCommand));

            stringCmd.Received(1).ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>());
            sbCmd.Received(0).ExecuteAsync(Arg.Any<ICreateEntityContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Delete_delegate_to_data_context()
        {
            var dataSpace = this.CreateTestDataSpace();
            var stringCmd = Substitute.For<IDeleteEntityCommand>();
            dataSpace[typeof(string)].CreateCommand(typeof(IDeleteEntityCommand)).Returns(stringCmd);
            stringCmd.ExecuteAsync(Arg.Any<IDeleteEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new DataCommandResult("deleted"));
            var sbCmd = Substitute.For<IDeleteEntityCommand>();
            dataSpace[typeof(StringBuilder)].CreateCommand(typeof(IDeleteEntityCommand)).Returns(sbCmd);
            sbCmd.ExecuteAsync(Arg.Any<IDeleteEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new DataCommandResult(new StringBuilder("should not get here").ToString()));

            dataSpace.Delete("gigi");

            dataSpace[typeof(string)].Received(1).CreateCommand(typeof(IDeleteEntityCommand));
            dataSpace[typeof(StringBuilder)].Received(0).CreateCommand(typeof(IDeleteEntityCommand));

            stringCmd.Received(1).Execute(Arg.Any<IDeleteEntityContext>());
            sbCmd.Received(0).Execute(Arg.Any<IDeleteEntityContext>());
        }

        [Test]
        public void Delete_multiple_delegate_to_data_context()
        {
            var dataSpace = this.CreateTestDataSpace();
            var stringCmd = Substitute.For<IDeleteEntityCommand>();
            dataSpace[typeof(string)].CreateCommand(typeof(IDeleteEntityCommand)).Returns(stringCmd);
            stringCmd.ExecuteAsync(Arg.Any<IDeleteEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new DataCommandResult("deleted"));
            var sbCmd = Substitute.For<IDeleteEntityCommand>();
            dataSpace[typeof(StringBuilder)].CreateCommand(typeof(IDeleteEntityCommand)).Returns(sbCmd);
            sbCmd.ExecuteAsync(Arg.Any<IDeleteEntityContext>(), Arg.Any<CancellationToken>())
                .Returns(new DataCommandResult(new StringBuilder("should not get here").ToString()));

            dataSpace.Delete((IEnumerable<string>)new[] { "gigi" });

            dataSpace[typeof(string)].Received(1).CreateCommand(typeof(IDeleteEntityCommand));
            dataSpace[typeof(StringBuilder)].Received(0).CreateCommand(typeof(IDeleteEntityCommand));

            stringCmd.Received(1).Execute(Arg.Any<IDeleteEntityContext>());
            sbCmd.Received(0).Execute(Arg.Any<IDeleteEntityContext>());
        }

        [Test]
        public async Task PersistChangesAsync_delegate_to_data_contexts()
        {
            var dataSpace = this.CreateTestDataSpace();
            var stringCmd = Substitute.For<IPersistChangesCommand>();
            dataSpace[typeof(string)].CreateCommand(typeof(IPersistChangesCommand)).Returns(stringCmd);
            var sbCmd = Substitute.For<IPersistChangesCommand>();
            dataSpace[typeof(StringBuilder)].CreateCommand(typeof(IPersistChangesCommand)).Returns(sbCmd);

            await dataSpace.PersistChangesAsync();

            dataSpace[typeof(string)].Received(1).CreateCommand(typeof(IPersistChangesCommand));
            dataSpace[typeof(StringBuilder)].Received(1).CreateCommand(typeof(IPersistChangesCommand));

            stringCmd.Received(1).ExecuteAsync(Arg.Any<IPersistChangesContext>(), Arg.Any<CancellationToken>());
            sbCmd.Received(1).ExecuteAsync(Arg.Any<IPersistChangesContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void DiscardChanges_delegate_to_data_contexts()
        {
            var dataSpace = this.CreateTestDataSpace();
            var stringCmd = Substitute.For<IDiscardChangesCommand>();
            dataSpace[typeof(string)].CreateCommand(typeof(IDiscardChangesCommand)).Returns(stringCmd);
            var sbCmd = Substitute.For<IDiscardChangesCommand>();
            dataSpace[typeof(StringBuilder)].CreateCommand(typeof(IDiscardChangesCommand)).Returns(sbCmd);

            dataSpace.DiscardChanges();

            dataSpace[typeof(string)].Received(1).CreateCommand(typeof(IDiscardChangesCommand));
            dataSpace[typeof(StringBuilder)].Received(1).CreateCommand(typeof(IDiscardChangesCommand));

            stringCmd.Received(1).Execute(Arg.Any<IDiscardChangesContext>());
            sbCmd.Received(1).Execute(Arg.Any<IDiscardChangesContext>());
        }

        private IDataSpace CreateTestDataSpace()
        {
            var stringDataContext = Substitute.For<IDataContext>();
            var sbDataContext = Substitute.For<IDataContext>();

            var dataSpace = Substitute.For<IDataSpace>();
            dataSpace[typeof(string)].Returns(stringDataContext);
            dataSpace[typeof(StringBuilder)].Returns(sbDataContext);

            Assert.AreSame(stringDataContext, dataSpace[typeof(string)]);
            Assert.AreSame(sbDataContext, dataSpace[typeof(StringBuilder)]);

            IEnumerator<IDataContext> dcEnumerator(CallInfo c)
            {
                yield return stringDataContext;
                yield return sbDataContext;
            }

            dataSpace.GetEnumerator().Returns(dcEnumerator);

            return dataSpace;
        }
    }
}