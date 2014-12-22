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
    using System.Reflection;

    /// <summary>
    /// Runtime based constructor information for classifiers.
    /// </summary>
    public abstract class RuntimeClassifierInfo : RuntimeModelElementInfo<TypeInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeClassifierInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime member information.</param>
        protected RuntimeClassifierInfo(TypeInfo runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}