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
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Model.Runtime.Factory;
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
            var elementInfos = new List<INamedElementInfo>
                        {
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Boolean)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Byte)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.SByte)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Int16)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.UInt16)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Int32)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.UInt32)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Int64)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.UInt64)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Decimal)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Double)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Single)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.DateTime)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.DateTimeOffset)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.TimeSpan)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Uri)).AsPrimitive().InCoreProjection().Element,
                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.Guid)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(System.String)).AsPrimitive().InCoreProjection().Element,

                           new ValueTypeBuilder(this.RuntimeModelElementFactory, typeof(Id)).AsPrimitive().InCoreProjection().Element,
                        };

            return Task.FromResult((IEnumerable<IElementInfo>)elementInfos);
        }
    }
}