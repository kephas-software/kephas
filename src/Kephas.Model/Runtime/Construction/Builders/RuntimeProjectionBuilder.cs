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
    using System.Collections.ObjectModel;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// A runtime projection builder.
    /// </summary>
    public class RuntimeProjectionBuilder
    {
        /// <summary>
        /// The projection.
        /// </summary>
        private readonly IList<IRuntimeTypeInfo> projection = new List<IRuntimeTypeInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeProjectionBuilder"/> class.
        /// </summary>
        public RuntimeProjectionBuilder()
        {
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