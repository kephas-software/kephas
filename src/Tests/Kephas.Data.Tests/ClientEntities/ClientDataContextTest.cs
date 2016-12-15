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
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>());
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
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCommand = Substitute.For<IFindCommand<ClientDataContext, string>>();
            dataCommandProvider.CreateCommand(typeof(ClientDataContext), typeof(IFindCommand<ClientDataContext, string>)).Returns(findCommand);
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), dataCommandProvider);

            var actualCommand = clientDataContext.CreateCommand<IFindCommand<ClientDataContext, string>>();
            Assert.AreSame(findCommand, actualCommand);
        }

        [Test]
        public void TryGetCapability_IIdentifiable()
        {
            var clientDataContext = new ClientDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>());

            var entity = Substitute.For<IIdentifiable>();
            var idCapability = clientDataContext.TryGetCapability<IIdentifiable>(entity, null);
            Assert.AreSame(idCapability, entity);

            idCapability = clientDataContext.TryGetCapability<IIdentifiable>("a string", null);
            Assert.IsNull(idCapability);
        }
    }
}