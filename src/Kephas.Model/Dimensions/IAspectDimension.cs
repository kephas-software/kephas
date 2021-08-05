// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAspectDimension.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dimension identifying orthogonal aspects within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Aspect;

    /// <summary>
    /// Dimension identifying orthogonal aspects within the application. 
    /// By default, Kephas provide a default dimension element named 'Default',
    /// identifying the default aspect of a model element.
    /// </summary>
    /// <remarks>
    /// The aspect dimension is aggregatable because it may provide logical aspects
    /// of the same element, which, after aggregation, are weaved into one integral element.
    /// </remarks>
    [AggregatableModelDimension(5, DefaultDimensionElement = typeof(IDefaultAspectDimensionElement))]
    public interface IAspectDimension
    {
    }
}