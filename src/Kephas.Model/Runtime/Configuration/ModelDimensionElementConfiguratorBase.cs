// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDimensionElementConfiguratorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model dimension element runtime configurator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using System;

    /// <summary>
    /// A model dimension element runtime configurator.
    /// </summary>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    public abstract class ModelDimensionElementConfiguratorBase<TRuntimeElement> : RuntimeModelElementConfiguratorBase<IModelDimensionElement, TRuntimeElement, ModelDimensionElementConfiguratorBase<TRuntimeElement>>
    {
        /// <summary>
        /// Marks the model dimension element depending on the provided other dimension elements identified by their runtime types.
        /// </summary>
        /// <param name="elements">The other dimension elements identified by their runtime types.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        public ModelDimensionElementConfiguratorBase<TRuntimeElement> DependsOn(params Type[] elements)
        {
            // TODO...

            return this;
        }
    }
}