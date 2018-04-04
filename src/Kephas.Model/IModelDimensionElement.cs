// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimensionElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for model dimension elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Contract for model dimension elements.
    /// </summary>
    [MemberNameDiscriminator(":")]
    public interface IModelDimensionElement : IModelElement
    {
        /// <summary>
        /// Gets the name of the dimension.
        /// </summary>
        /// <value>
        /// The name of the dimension.
        /// </value>
        string DimensionName { get; }

        /// <summary>
        /// Gets the dimension declaring this element.
        /// </summary>
        /// <value>
        /// The declaring dimension.
        /// </value>
        IModelDimension DeclaringDimension { get; }
    }
}