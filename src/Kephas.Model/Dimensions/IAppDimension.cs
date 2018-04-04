// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The model dimension is used to model multiple models.
//   One possible use is the association of each model with a persistence store, like a database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.App;

    /// <summary>
    /// The app dimension is used to model multiple applications. 
    /// One possible use is the association of each model with a persistence store, like a database. 
    /// </summary>
    /// <remarks>
    /// By default, Kephas provides the "Kernel" application which can be used to provide primitives to all the other models.
    /// </remarks>
    [ModelDimension(1, DefaultDimensionElement = typeof(IKernelAppDimensionElement))]
    public interface IAppDimension
    {
    }
}