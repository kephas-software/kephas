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

    using Kephas.Composition;
    using Kephas.Data.Caching;
    using Kephas.Data.Commands.Factory;

    using NSubstitute;

    public class TestDataContext : DataContextBase
    {
        public TestDataContext(
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataContextCache localCache = null)
            : base(GetTestCompositionContext(compositionContext), GetTestDataCommandProvider(dataCommandProvider), localCache: localCache)
        {
        }

        public override IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
        {
            return this.LocalCache.Values.Select(ei => ei.Entity).OfType<T>().AsQueryable();
        }

        private static ICompositionContext GetTestCompositionContext(ICompositionContext compositionContext)
            => compositionContext ?? Substitute.For<ICompositionContext>();

        private static IDataCommandProvider GetTestDataCommandProvider(IDataCommandProvider dataCommandProvider)
            => dataCommandProvider ?? Substitute.For<IDataCommandProvider>();
    }
}