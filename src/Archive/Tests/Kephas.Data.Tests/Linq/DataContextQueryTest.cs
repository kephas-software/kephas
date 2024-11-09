// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context query test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Linq;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextQueryTest
    {
        [Test]
        public void GetEnumerator()
        {
            var dataContext = Substitute.For<IDataContext>();
            var context = new QueryOperationContext(dataContext);

            var query = new List<int>(new[] { 1, 2, 3 }).AsQueryable().Where(i => i > 1);
            var provider = new DataContextQueryProvider(context, query.Provider);
            var dcQuery = new DataContextQuery<int>(provider, query);
            var enumerator = dcQuery.GetEnumerator();
            Assert.NotNull(enumerator);
        }
    }
}