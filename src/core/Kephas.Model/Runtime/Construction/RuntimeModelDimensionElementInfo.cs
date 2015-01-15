// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for model dimension elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for model dimension elements.
    /// </summary>
    public class RuntimeModelDimensionElementInfo : RuntimeModelElementInfo<TypeInfo>, IModelDimensionElementInfo
    {
        /// <summary>
        /// The dimension element name discriminator.
        /// </summary>
        public const string DimensionElementNameDiscriminator = "DimensionElement";

        /// <summary>
        /// The dimension name.
        /// </summary>
        private string dimensionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeModelDimensionElementInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        public RuntimeModelDimensionElementInfo(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
            var dimensionElementAttribute = runtimeElement.GetCustomAttribute<ModelDimensionElementAttribute>();
            if (dimensionElementAttribute != null)
            {
                // TODO dimension element attributes
            }

            this.IsMemberOf = me => me is IModelDimension && me.Name == this.DimensionName;
        }

        /// <summary>
        /// Gets the name of the parent dimension.
        /// </summary>
        /// <value>
        /// The name of the parent dimension.
        /// </value>
        public string DimensionName
        {
            get { return this.dimensionName ?? (this.dimensionName = this.ComputeDimensionName(this.RuntimeElement.Namespace)); }
        }

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This dicriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator
        {
            get { return DimensionElementNameDiscriminator; }
        }

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string ComputeName(TypeInfo runtimeElement)
        {
            var baseName = base.ComputeName(runtimeElement);
            if (baseName.EndsWith(this.DimensionName))
            {
                return baseName.Substring(0, baseName.Length - this.DimensionName.Length);
            }

            return baseName;
        }

        /// <summary>
        /// Computes the name of the parent dimension based on the runtime element namespace.
        /// </summary>
        /// <param name="ns">The namenspace.</param>
        /// <returns>The dimension name.</returns>
        protected virtual string ComputeDimensionName(string ns)
        {
            var lastNsPartIndex = ns.LastIndexOf('.');
            if (lastNsPartIndex > 0)
            {
                return ns.Substring(lastNsPartIndex + 1);
            }

            return ns;
        }
    }
}