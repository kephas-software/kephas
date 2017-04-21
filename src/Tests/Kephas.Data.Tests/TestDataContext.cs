// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the test data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Linq;

    using Kephas.Data.Caching;
    using Kephas.Data.Commands.Factory;

    public class TestDataContext : DataContextBase
    {
        public TestDataContext(IAmbientServices ambientServices, IDataCommandProvider dataCommandProvider, IDataContextCache localCache = null)
            : base(ambientServices, dataCommandProvider, localCache)
        {
        }

        public override IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
        {
            return this.LocalCache.Values.OfType<T>().AsQueryable();
        }
    }
}