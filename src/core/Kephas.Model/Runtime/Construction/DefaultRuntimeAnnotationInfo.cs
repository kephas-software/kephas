// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeAnnotationInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Default runtime information for constructing annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System;

    /// <summary>
    /// Default runtime information for constructing annotations.
    /// </summary>
    public class DefaultRuntimeAnnotationInfo : RuntimeAnnotationInfoBase<Attribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeAnnotationInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public DefaultRuntimeAnnotationInfo(Attribute runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}