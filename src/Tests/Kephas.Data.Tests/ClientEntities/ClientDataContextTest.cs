using Kephas.Data.Commands;
using Kephas.Data.Commands.Composition;
using Kephas.Data.Commands.Factory;
using Kephas.Testing.Core.Composition;

namespace Kephas.Data.Tests.ClientEntities
{
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.ClientEntities;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ClientDataContextTest
    {
        [Test]
        public void Query_of_string()
        {
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), Substitute.For<ICompositionContext>());
            clientDataContext.GetOrAddCacheableItem(null, "mama", true);
            clientDataContext.GetOrAddCacheableItem(null, "papa", true);
            clientDataContext.GetOrAddCacheableItem(null, 1, true);

            var query = clientDataContext.Query<string>();
            var list = query.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("mama", list[0]);
            Assert.AreEqual("papa", list[1]);
        }

        [Test]
        public void CreateCommand_Find()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var findCommand = Substitute.For<IFindCommand<ClientDataContext, string>>();
            var findCommandFactory = Substitute.For<IDataCommandFactory<IFindCommand<ClientDataContext, string>>>();
            findCommandFactory.GetCommandFactory(typeof(ClientDataContext)).Returns(() => findCommand);
            compositionContext.GetExport<IDataCommandFactory<IFindCommand<ClientDataContext, string>>>().Returns(findCommandFactory);
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), compositionContext);

            var actualCommand = clientDataContext.CreateCommand<IFindCommand<ClientDataContext, string>>();
            Assert.AreSame(findCommand, actualCommand);
        }

        [Test]
        public void TryGetCapability_IIdentifiable()
        {
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), Substitute.For<ICompositionContext>());

            var entity = Substitute.For<IIdentifiable>();
            var idCapability = clientDataContext.TryGetCapability<IIdentifiable>(entity, null);
            Assert.AreSame(idCapability, entity);

            idCapability = clientDataContext.TryGetCapability<IIdentifiable>("a string", null);
            Assert.IsNull(idCapability);
        }
    }
}