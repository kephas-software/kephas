// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppLayerDimension.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Dimension identifying abstraction layers within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Dimension identifying abstraction layers within the application. The application layers are specializations at application level
    /// comprising for each layer specializations of their elements. For example, Kephas is the bottom-most layer providing 
    /// core functionality; a general purpose CRM system may be built upon it providing common CRM functionality; further on 
    /// a customer specific layer may be added to customize the CRM core with customer specific needs. Following this principle,
    /// an application may be structured in such a way that it enables a maximum reuse of the code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The application layer is aggregatable because each layer may provide
    /// a further specialization of an application element which, after aggregation, 
    /// provide an integral functional view of that element.
    /// </para>
    /// <para>
    /// Caution: the number of layers should be kept small, because the layers may introduce a complexity which can get out of the 
    /// control quickly. A number of maximum three (3) layers, including Kephas, is the recommended one.
    /// </para>
    /// </remarks>
    [AggregatableModelDimension(0)]
    public interface IAppLayerDimension
    {
    }
}