// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeProjectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime projection builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// A runtime projection builder.
    /// </summary>
    public class RuntimeProjectionBuilder
    {
        private readonly IModelConstructionContext constructionContext;
        private readonly IList<IRuntimeTypeInfo> projection = new List<IRuntimeTypeInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeProjectionBuilder"/> class.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        public RuntimeProjectionBuilder(IModelConstructionContext constructionContext)
        {
            this.constructionContext = constructionContext;
            this.Projection = new ReadOnlyCollection<IRuntimeTypeInfo>(this.projection);
        }

        /// <summary>
        /// Gets the runtime projection.
        /// </summary>
        internal IReadOnlyList<IRuntimeTypeInfo> Projection { get; }

        /// <summary>
        /// Adds the dimension element identified by the type.
        /// </summary>
        /// <typeparam name="TModelDimensionElement">Type of the model dimension element.</typeparam>
        /// <returns>
        /// This <see cref="RuntimeProjectionBuilder"/>.
        /// </returns>
        public RuntimeProjectionBuilder Dim<TModelDimensionElement>()
        {
            this.projection.Add(typeof(TModelDimensionElement).AsRuntimeTypeInfo());
            return this;
        }
    }
}