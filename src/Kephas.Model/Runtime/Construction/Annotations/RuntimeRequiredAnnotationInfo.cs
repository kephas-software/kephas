// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeRequiredAnnotationInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information about the runtime required annotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Annotations
{
    using Kephas.Model.AttributedModel.Behaviors;

    /// <summary>
    /// Information about the runtime required annotation.
    /// </summary>
    public class RuntimeRequiredAnnotationInfo : RuntimeAnnotationInfoBase<RequiredAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeRequiredAnnotationInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public RuntimeRequiredAnnotationInfo(RequiredAttribute runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}