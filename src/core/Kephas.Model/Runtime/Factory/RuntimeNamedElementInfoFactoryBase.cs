// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeNamedElementInfoFactoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of an element information provider based on the .NET runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// Base implementation of a named element information provider based on the .NET runtime.
    /// </summary>
    /// <typeparam name="TElementInfo">The type of the element information.</typeparam>
    /// <typeparam name="TRuntimeInfo">The type of the runtime information.</typeparam>
    public abstract class RuntimeNamedElementInfoFactoryBase<TElementInfo, TRuntimeInfo> : IRuntimeElementInfoFactory<TElementInfo, TRuntimeInfo>
        where TElementInfo : RuntimeNamedElementInfo<TRuntimeInfo>
        where TRuntimeInfo : class
    {
        /// <summary>
        /// Tries to create an element information structure based on the provided runtime element
        /// information.
        /// </summary>
        /// <param name="runtimeModelInfoProvider">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        public virtual INamedElementInfo TryGetElementInfo(IRuntimeModelInfoProvider runtimeModelInfoProvider, object runtimeElement)
        {
            var typedRuntimeInfo = runtimeElement as TRuntimeInfo;
            if (typedRuntimeInfo == null)
            {
                return null;
            }

            var elementInfo = this.TryGetElementInfoCore(runtimeModelInfoProvider, typedRuntimeInfo);
            elementInfo?.ConstructInfo(runtimeModelInfoProvider);
            return elementInfo;
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="runtimeModelInfoProvider">The runtime model information provider.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        protected abstract TElementInfo TryGetElementInfoCore(IRuntimeModelInfoProvider runtimeModelInfoProvider, TRuntimeInfo runtimeElement);
    }
}