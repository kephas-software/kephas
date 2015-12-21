// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModuleDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying modules within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Dimensions.Module;

    /// <summary>
    /// Dimension identifying modules within the application. Modules are functional units of an application which are autonomous
    /// but at the same time possibly integrated with one another.
    /// </summary>
    /// <remarks>
    /// The module dimension is not aggregatable because it splits the application into
    /// autonomous units, which can interact with one another but which cannot be aggregated.
    /// </remarks>
    [ModelDimension(2, DefaultDimensionElement = typeof(ICoreModuleDimensionElement))]
    public interface IModuleDimension
    {
    }
}