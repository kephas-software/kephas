// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeClassifierInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime based constructor information for classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Runtime based constructor information for classifiers.
    /// </summary>
    public abstract class RuntimeClassifierInfo : RuntimeModelElementInfo<TypeInfo>, IClassifierInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeClassifierInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        protected RuntimeClassifierInfo(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
        }

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
        public IEnumerable<object> Projection { get; internal set; }
    }
}