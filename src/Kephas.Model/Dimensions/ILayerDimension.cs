﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayerDimension.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Dimension identifying abstraction layers within the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Dimension identifying layers. The layers contain element specializations: the higher the layer, the higher the specialization.
    /// For example, Foundation is the bottom-most layer providing core functionality;
    /// a general purpose CRM system may be built upon it providing common CRM functionality;
    /// further on a customer specific layer may be added to customize the CRM core with customer specific needs.
    /// Following this principle, an application may be structured in such a way that it enables a maximum reuse of the code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The layer is aggregatable because each of them may provide
    /// a further specialization of an application element which, after aggregation, 
    /// provide an integral functional view of that element.
    /// </para>
    /// <para>
    /// Caution: the number of layers should be kept small, because they may introduce a complexity which can get out of the 
    /// control quickly. A number of maximum four (4), including System and Foundation, is the recommended one.
    /// </para>
    /// </remarks>
    [AggregatableModelDimension(0)]
    public interface ILayerDimension
    {
    }
}