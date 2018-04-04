// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    using NSubstitute;

    public class TestDataContext : DataContextBase
    {
        public TestDataContext(
            IAmbientServices ambientServices = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataContextCache localCache = null)
            : base(GetTestAmbientServices(ambientServices), GetTestDataCommandProvider(dataCommandProvider), localCache: localCache)
        {
        }

        public override IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
        {
            return this.LocalCache.Values.Select(ei => ei.Entity).OfType<T>().AsQueryable();
        }

        private static IAmbientServices GetTestAmbientServices(IAmbientServices ambientServices)
            => ambientServices ?? Substitute.For<IAmbientServices>();

        private static IDataCommandProvider GetTestDataCommandProvider(IDataCommandProvider dataCommandProvider)
            => dataCommandProvider ?? Substitute.For<IDataCommandProvider>();
    }
}