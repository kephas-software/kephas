// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The model dimension is used to model multiple models.
//   One possible use is the association of each model with a persistence store, like a database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Model;

    /// <summary>
    /// The model dimension is used to model multiple models. 
    /// One possible use is the association of each model with a persistence store, like a database. 
    /// </summary>
    /// <remarks>
    /// By default, Kephas provides an "Primitives" model which can be used to provide primitives to all the other models.
    /// </remarks>
    [ModelDimension(1, DefaultDimensionElement = typeof(IPrimitivesModelDimensionElement))]
    public interface IModelDimension
    {
    }
}