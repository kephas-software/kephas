// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataPrimitiveTypesModelInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data primitive types model information provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Builders;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// A model information provider for data related types.
    /// </summary>
    public class DataPrimitiveTypesModelInfoProvider : RuntimeModelInfoProviderBase<DataPrimitiveTypesModelInfoProvider>
    {
        /// <summary>
        /// List of data primitive types.
        /// </summary>
        private static readonly Type[] DataPrimitiveTypes =
            {
                typeof(IRef<>),
                typeof(IServiceRef<>),
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(IDictionary<,>),
            };

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
        /// Tries to get an <see cref="IElementInfo"/> of the model space based on the provided native element information.
        /// </summary>
        /// <param name="nativeElementInfo">The native element information.</param>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <returns>
        /// The constructed generic type or <c>null</c> if the provider cannot handle the provided type information.
        /// </returns>
        /// <remarks>
        /// A return value of <c>null</c> indicates only that the provided <paramref name="nativeElementInfo"/> cannot be handled.
        /// For any other errors an exception should be thrown.
        /// </remarks>
        public override IElementInfo TryGetModelElementInfo(IElementInfo nativeElementInfo, IModelConstructionContext constructionContext)
        {
            if (!(nativeElementInfo is IRuntimeTypeInfo runtimeElementInfo))
            {
                return null;
            }

            if (DataPrimitiveTypes.Contains(runtimeElementInfo.Type) ||
                (runtimeElementInfo.IsConstructedGenericType() && DataPrimitiveTypes.Contains(((IRuntimeTypeInfo)runtimeElementInfo.GenericTypeDefinition).Type)))
            {
                return new ValueTypeBuilder(constructionContext, runtimeElementInfo.Type).AsPrimitive().Element;
            }

            return null;
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
            var elementInfos = DataPrimitiveTypes
                .Select(t => new ValueTypeBuilder(constructionContext, t).AsPrimitive().Element)
                .Cast<IElementInfo>()
                .ToList();

            return Task.FromResult((IEnumerable<IElementInfo>)elementInfos);
        }
    }
}