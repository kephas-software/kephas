// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceTypeConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// An application service type constructor.
    /// </summary>
    public class AppServiceTypeConstructor : ClassifierConstructorBase<AppServiceType, IAppServiceType>
    {
        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return runtimeElement.Annotations.OfType<IAppServiceInfo>().Any();
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override AppServiceType TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var appServiceAttr = runtimeElement.Annotations.OfType<IAppServiceInfo>().Single();
            var appServiceRuntimeElement = appServiceAttr.ContractType?.AsRuntimeTypeInfo() ?? runtimeElement;

            return new AppServiceType(constructionContext, appServiceAttr, this.TryComputeName(constructionContext, appServiceRuntimeElement));
        }
    }
}