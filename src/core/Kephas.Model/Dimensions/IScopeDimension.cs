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
    /// <summary>
    /// Dimension identifying an application scope. Application scopes may be domain models, client models, 
    /// application services and so on.
    /// </summary>
    /// <remarks>
    /// The scope dimension is not aggregatable because the scope provides an orthogonal view over the application.
    /// </remarks>
    [ModelDimension(2)]
    public interface IScopeDimension
    {
    }
}