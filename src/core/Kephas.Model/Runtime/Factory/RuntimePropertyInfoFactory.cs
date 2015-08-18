// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Factory class for runtime property information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// Factory class for runtime property information.
    /// </summary>
    public class RuntimePropertyInfoFactory : RuntimeModelElementInfoFactoryBase<RuntimePropertyInfo, PropertyInfo>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="runtimeModelInfoProvider">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        protected override RuntimePropertyInfo TryGetElementInfoCore(
            IRuntimeModelInfoProvider runtimeModelInfoProvider,
            PropertyInfo runtimeElement)
        {
            return new RuntimePropertyInfo(runtimeElement);
        }
    }
}