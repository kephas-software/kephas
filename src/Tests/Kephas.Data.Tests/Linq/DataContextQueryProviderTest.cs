// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryProviderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}