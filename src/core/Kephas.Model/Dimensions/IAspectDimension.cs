// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAspectDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// By default, Kephas provide a default dimension element named 'Main',
    /// identifying the main concern of a model element.
    /// </summary>
    /// <remarks>
    /// The aspect dimension is aggregatable because it may provide logical aspects
    /// of the same element, which, after aggregation, are weaved into one integral element.
    /// </remarks>
    [AggregatableModelDimension(5, DefaultDimensionElement = typeof(IMainAspectDimensionElement))]
    public interface IAspectDimension
    {
    }
}