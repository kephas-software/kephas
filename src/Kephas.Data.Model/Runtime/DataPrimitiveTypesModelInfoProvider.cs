// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPrimitiveTypesModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data primitive types model information provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Reflection;

    /// <summary>
    /// A model information provider for data related types.
    /// </summary>
    public class DataPrimitiveTypesModelInfoProvider : RuntimeModelInfoProviderBase<DataPrimitiveTypesModelInfoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPrimitiveTypesModelInfoProvider"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        public DataPrimitiveTypesModelInfoProvider(IRuntimeModelElementFactory runtimeModelElementFactory)
            : base(runtimeModelElementFactory)
        {
            Requires.NotNull(runtimeModelElementFactory, nameof(runtimeModelElementFactory));
        }

        /// <summary>
        /// Gets the element infos used for building the model space (core implementation).
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        protected override Task<IEnumerable<IElementInfo>> GetElementInfosCoreAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken)
        {
            var elementInfos = new List<IElementInfo>
                                   {
                                       new ValueTypeBuilder(constructionContext, typeof(IRef<>)).AsPrimitive().Element,
                                       new ValueTypeBuilder(constructionContext, typeof(ICollection<>)).AsPrimitive().Element,
                                   };

            return Task.FromResult((IEnumerable<IElementInfo>)elementInfos);
        }
    }
}