// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreSimpleTypesModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A model information provider for simple types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Reflection;

    /// <summary>
    /// A model information provider for simple types.
    /// </summary>
    public class CoreSimpleTypesModelInfoProvider : RuntimeModelInfoProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreSimpleTypesModelInfoProvider"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        public CoreSimpleTypesModelInfoProvider(IRuntimeModelElementFactory runtimeModelElementFactory)
            : base(runtimeModelElementFactory)
        {
            Contract.Requires(runtimeModelElementFactory != null);
        }

        /// <summary>
        /// Gets the element infos used for building the model space.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task promising an enumeration of element information.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
        public override Task<IEnumerable<IElementInfo>> GetElementInfosAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var elementInfos = new List<IElementInfo>
                        {
                           new ValueTypeBuilder(constructionContext, typeof(System.Boolean)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Byte)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.SByte)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int16)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt16)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int32)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt32)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int64)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt64)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Decimal)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Double)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Single)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.DateTime)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.DateTimeOffset)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.TimeSpan)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Uri)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Guid)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.String)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(constructionContext, typeof(Id)).AsPrimitive().InCoreProjection().Element,
                        };

            return Task.FromResult((IEnumerable<IElementInfo>)elementInfos);
        }
    }
}