// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeRequiredAnnotationInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime factory for the required annotation information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory.Annotations
{
    using Kephas.Model.AttributedModel.Behaviors;
    using Kephas.Model.Runtime.Construction.Annotations;
    using Kephas.Services;

    /// <summary>
    /// A runtime factory for the required annotation information.
    /// </summary>
    [ProcessingPriority(Priority.AboveNormal)]
    public class RuntimeRequiredAnnotationInfoFactory : RuntimeAnnotationInfoFactoryBase<RuntimeRequiredAnnotationInfo, RequiredAttribute>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="runtimeElementInfoFactoryDispatcher">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        protected override RuntimeRequiredAnnotationInfo TryGetElementInfoCore(
            IRuntimeElementInfoFactoryDispatcher runtimeElementInfoFactoryDispatcher,
            RequiredAttribute runtimeElement)
        {
            return new RuntimeRequiredAnnotationInfo(runtimeElement);
        }
    }
}