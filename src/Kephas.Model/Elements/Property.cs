// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Definition class for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Definition class for properties.
    /// </summary>
    public class Property : ModelElementBase<IProperty, IPropertyInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property" /> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <param name="modelSpace">The model space.</param>
        public Property(IPropertyInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
        {
        }
    }
}