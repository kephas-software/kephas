// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeProjectionBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the runtime projection builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Builders
{
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// A runtime projection builder.
    /// </summary>
    public class RuntimeProjectionBuilder
    {
        /// <summary>
        /// The projection.
        /// </summary>
        private IList<IDynamicTypeInfo> projection = new List<IDynamicTypeInfo>();

        /// <summary>
        /// Gets the runtime projection.
        /// </summary>
        internal IList<IDynamicTypeInfo> Projection => this.projection;

        /// <summary>
        /// Adds the dimension element identified by the type.
        /// </summary>
        /// <typeparam name="TModelDimensionElement">Type of the model dimension element.</typeparam>
        /// <returns>
        /// This <see cref="RuntimeProjectionBuilder"/>.
        /// </returns>
        public RuntimeProjectionBuilder Dim<TModelDimensionElement>()
        {
            this.projection.Add(typeof(TModelDimensionElement).AsDynamicTypeInfo());
            return this;
        }
    }
}