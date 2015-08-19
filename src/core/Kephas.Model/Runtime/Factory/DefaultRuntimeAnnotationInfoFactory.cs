// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRuntimeAnnotationInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime factory for annotation information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System;

    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// A default runtime factory for annotation information.
    /// </summary>
    public class DefaultRuntimeAnnotationInfoFactory : RuntimeAnnotationInfoFactoryBase<DefaultRuntimeAnnotationInfo, Attribute>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="runtimeElementInfoFactoryDispatcher">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        protected override DefaultRuntimeAnnotationInfo TryGetElementInfoCore(
            IRuntimeElementInfoFactoryDispatcher runtimeElementInfoFactoryDispatcher,
            Attribute runtimeElement)
        {
            return new DefaultRuntimeAnnotationInfo(runtimeElement);
        }
    }
}