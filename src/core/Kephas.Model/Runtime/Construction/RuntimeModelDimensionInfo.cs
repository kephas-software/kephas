// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for model dimensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for model dimensions.
    /// </summary>
    public class RuntimeModelDimensionInfo : RuntimeModelElementInfo<TypeInfo>, IModelDimensionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelDimensionInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        public RuntimeModelDimensionInfo(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
            var modelDimensionAttribute = runtimeElement.GetCustomAttribute<ModelDimensionAttribute>();
            if (modelDimensionAttribute != null)
            {
                this.Index = modelDimensionAttribute.Index;
                this.IsAggregatable = modelDimensionAttribute.IsAggregatable;
            }
        }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregatable { get; private set; }
    }
}