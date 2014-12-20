// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelDimensionInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Runtime provider for model dimension information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using System.Reflection;

    using Kephas.Model.AttributedModel;
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// Runtime provider for model dimension information.
    /// </summary>
    public class RuntimeModelDimensionInfoFactory : RuntimeModelElementInfoFactoryBase<IModelDimensionInfo, TypeInfo>
    {
        /// <summary>
        /// Tries to get the model dimension information.
        /// </summary>
        /// <param name="runtimeElementInfo">The runtime element information.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c> if the runtime element information is not supported.
        /// </returns>
        protected override IModelDimensionInfo TryGetElementInfoCore(TypeInfo runtimeElementInfo)
        {
            if (!runtimeElementInfo.IsInterface)
            {
                return null;
            }

            var dimensionAttribute = runtimeElementInfo.GetCustomAttribute<ModelDimensionAttribute>();
            if (dimensionAttribute == null)
            {
                return null;
            }

            return new RuntimeModelDimensionInfo(runtimeElementInfo);
        }
    }
}