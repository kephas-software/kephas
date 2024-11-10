// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAreaDimension.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dimension identifying an application scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Area;

    /// <summary>
    /// Dimension identifying an application area. Application areas may be domain models, client models, 
    /// application services, messaging, and so on.
    /// </summary>
    /// <remarks>
    /// The Area dimension is not aggregatable because areas are distinct across the application.
    /// </remarks>
    [ModelDimension(3, DefaultDimensionElement = typeof(IGlobalAreaDimensionElement))]
    public interface IAreaDimension
    {
    }
}