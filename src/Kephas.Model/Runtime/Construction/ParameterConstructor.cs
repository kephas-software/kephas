// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the parameter constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A parameter constructor.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class ParameterConstructor : ModelElementConstructorBase<Parameter, IParameter, IRuntimeParameterInfo>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override Parameter? TryCreateModelElementCore(
            IModelConstructionContext constructionContext,
            IRuntimeParameterInfo runtimeElement)
        {
            var parameter = new Parameter(constructionContext, this.TryComputeNameCore(runtimeElement, constructionContext))
                             {
                                 Position = runtimeElement.Position,
                                 IsOptional = runtimeElement.IsOptional,
                                 IsIn = runtimeElement.IsIn,
                                 IsOut = runtimeElement.IsOut,
                             };
            return parameter;
        }
    }
}