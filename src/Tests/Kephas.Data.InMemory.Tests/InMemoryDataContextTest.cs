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
    using System.Linq;

    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InMemoryDataContextTest
    {
        [Test]
        public void Query_of_string()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>());
            dataContext.GetOrAddCacheableItem(null, "mama", true);
            dataContext.GetOrAddCacheableItem(null, "papa", true);
            dataContext.GetOrAddCacheableItem(null, 1, true);

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
            var findCommand = Substitute.For<IFindCommand<InMemoryDataContext, string>>();
            dataCommandProvider.CreateCommand(typeof(InMemoryDataContext), typeof(IFindCommand<InMemoryDataContext, string>)).Returns(findCommand);
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), dataCommandProvider);

            var actualCommand = dataContext.CreateCommand<IFindCommand<InMemoryDataContext, string>>();
            Assert.AreSame(findCommand, actualCommand);
        }

        [Test]
        public void TryGetCapability_IIdentifiable()
        {
            var dataContext = new InMemoryDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>());

            var entity = Substitute.For<IIdentifiable>();
            var idCapability = dataContext.TryGetCapability<IIdentifiable>(entity, null);
            Assert.AreSame(idCapability, entity);

            idCapability = dataContext.TryGetCapability<IIdentifiable>("a string", null);
            Assert.IsNull(idCapability);
        }
    }
}