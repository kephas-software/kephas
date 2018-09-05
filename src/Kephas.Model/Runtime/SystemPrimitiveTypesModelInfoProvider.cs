// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemPrimitiveTypesModelInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A model information provider for simple types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Reflection;

    /// <summary>
    /// A model information provider for simple types.
    /// </summary>
    public class SystemPrimitiveTypesModelInfoProvider : RuntimeModelInfoProviderBase<SystemPrimitiveTypesModelInfoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPrimitiveTypesModelInfoProvider"/> class.
        /// </summary>
        /// <param name="runtimeModelElementFactory">  The runtime model information factory. </param>
        public SystemPrimitiveTypesModelInfoProvider(IRuntimeModelElementFactory runtimeModelElementFactory)
            : base(runtimeModelElementFactory)
        {
            Requires.NotNull(runtimeModelElementFactory, nameof(runtimeModelElementFactory));
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
        protected override Task<IEnumerable<IElementInfo>> GetElementInfosCoreAsync(IModelConstructionContext constructionContext, CancellationToken cancellationToken)
        {
            var elementInfos = new List<IElementInfo>
                        {
                           new ValueTypeBuilder(constructionContext, typeof(System.Boolean)).AsPrimitive().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Byte)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.SByte)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int16)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt16)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int32)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt32)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Int64)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.UInt64)).AsPrimitive().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Decimal)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Double)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Single)).AsPrimitive().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.DateTime)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.DateTimeOffset)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.TimeSpan)).AsPrimitive().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.Uri)).AsPrimitive().Element,
                           new ValueTypeBuilder(constructionContext, typeof(System.Guid)).AsPrimitive().Element,

                           new ValueTypeBuilder(constructionContext, typeof(System.String)).AsPrimitive().Element,
                           // TODO Byte Array
                        };

            return Task.FromResult((IEnumerable<IElementInfo>)elementInfos);
        }
    }
}