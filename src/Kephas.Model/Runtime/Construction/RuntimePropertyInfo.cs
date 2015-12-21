// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime information class for constructing properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Reflection;

    /// <summary>
    /// Runtime information class for constructing properties.
    /// </summary>
    public class RuntimePropertyInfo : RuntimeModelElementInfo<PropertyInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kephas.Model.Runtime.Construction.RuntimePropertyInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public RuntimePropertyInfo(PropertyInfo runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}