// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context query provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Linq
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextQueryProviderTest
    {
        [Test]
        public void CreateQuery_non_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var dataContextQuery = provider.CreateQuery(query.Expression);
            Assert.IsInstanceOf<DataContextQuery<int>>(dataContextQuery);
        }

        [Test]
        public void CreateQuery_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var dataContextQuery = provider.CreateQuery<int>(query.Expression);
            Assert.IsInstanceOf<DataContextQuery<int>>(dataContextQuery);
        }

        [Test]
        public void Execute_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = provider.Execute<IEnumerable<int>>(query.Expression).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(i => i > 1));
        }

        [Test]
        public void Execute_non_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = ((IEnumerable<int>)provider.Execute(query.Expression)).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(i => i > 1));
        }

        [Test]
        public async Task ExecuteAsync_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = (await provider.ExecuteAsync<IEnumerable<int>>(query.Expression)).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(i => i > 1));
        }

        [Test]
        public async Task ExecuteAsync_non_generic()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = ((IEnumerable<int>) await provider.ExecuteAsync(query.Expression)).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(i => i > 1));
        }

        [Test]
        public void Execute_attaches_identifiable_entities_collection()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var item1 = Substitute.For<IIdentifiable>();
            item1.Id.Returns(1);
            var item2 = Substitute.For<IIdentifiable>();
            item2.Id.Returns(2);
            var item3 = Substitute.For<IIdentifiable>();
            item3.Id.Returns(3);
            var query = new List<IIdentifiable>(new[] { item1, item2, item3 }).AsQueryable().Where(i => (int)i.Id > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = ((IEnumerable<IIdentifiable>)provider.Execute(query.Expression)).ToList();
            Assert.AreEqual(2, result.Count);

            dataContext.AttachEntity(Arg.Any<object>()).Received(2);
        }

        [Test]
        public void Execute_does_not_attach_non_identifiable_entities_collection()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var item2 = Substitute.For<IIdentifiable>();
            item2.Id.Returns(2);
            var item3 = Substitute.For<IIdentifiable>();
            item3.Id.Returns(3);
            var query = new List<object>(new object[] { 1, item2, item3 }).AsQueryable();
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = ((IEnumerable<object>)provider.Execute(query.Expression)).ToList();
            Assert.AreEqual(3, result.Count);

            dataContext.AttachEntity(Arg.Any<object>()).Received(2);
        }

        [Test]
        public void Execute_attaches_identifiable_entities_single_entity()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var item1 = Substitute.For<IIdentifiable>();
            item1.Id.Returns(1);
            var item2 = Substitute.For<IIdentifiable>();
            item2.Id.Returns(2);
            var item3 = Substitute.For<IIdentifiable>();
            item3.Id.Returns(3);
            var query = new List<IIdentifiable>(new[] { item1, item2, item3 }).AsQueryable();
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = provider.CreateQuery<IIdentifiable>(query.Expression).FirstOrDefault();
            Assert.AreSame(item1, result);

            dataContext.AttachEntity(Arg.Any<object>()).Received(1);
        }

        [Test]
        public void Execute_does_not_attach_non_identifiable_entities_single_entity()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var item2 = Substitute.For<IIdentifiable>();
            item2.Id.Returns(2);
            var item3 = Substitute.For<IIdentifiable>();
            item3.Id.Returns(3);
            var query = new List<object>(new object[] { 1, item2, item3 }).AsQueryable();
            var provider = new DataContextQueryProvider(context, query.Provider);
            var result = provider.CreateQuery<object>(query.Expression).FirstOrDefault();
            Assert.AreEqual(1, result);

            dataContext.AttachEntity(Arg.Any<object>()).Received(0);
        }

        [Test]
        public void CreateQuery_is_orderable()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 3, 1, 2 }).AsQueryable();
            var provider = new DataContextQueryProvider(context, query.Provider);
            var orderableQuery = provider.CreateQuery<int>(query.Expression).OrderBy(e => e);
            Assert.IsInstanceOf<IOrderedQueryable<int>>(orderableQuery);
        }
    }
}