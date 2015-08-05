// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassifierInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Information for constructing classifiers.
    /// </summary>
    public interface IClassifierInfo : IModelElementInfo
    {
        /// <summary>
        /// Gets the projection as an enumeration of objects.
        /// Each item identifies a dimension element and may be one of:
        /// <list type="bullet">
        /// <item>
        /// <term>a <see cref="KeyValuePair{TKey,TValue}">KeyValuePair&lt;string, string&gt;</see></term>
        /// <description>A couple containing the name of the dimension (as key) and the name of the dimension element (as value).</description>
        /// </item>
        /// <item>
        /// <term>a <see cref="Type"/> or <see cref="TypeInfo"/></term>
        /// <description>The runtime type information of the dimension of its element.</description>
        /// </item>
        /// <item>
        /// <term>The <see cref="IModelDimensionElement"/> object.</term>
        /// <description>The dimension element itself.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        IEnumerable<object> Projection { get; }
    }
}