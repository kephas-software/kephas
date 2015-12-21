// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScopeDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying an application scope.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Scope;

    /// <summary>
    /// Dimension identifying an application scope. Application scopes may be domain models, client models, 
    /// application services and so on.
    /// </summary>
    /// <remarks>
    /// The scope dimension is not aggregatable because the scope provides an orthogonal view over the application.
    /// </remarks>
    [ModelDimension(3, DefaultDimensionElement = typeof(IGlobalScopeDimensionElement))]
    public interface IScopeDimension
    {
    }
}