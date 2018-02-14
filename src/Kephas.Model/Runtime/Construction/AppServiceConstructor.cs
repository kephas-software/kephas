﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application service constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// An application service constructor.
    /// </summary>
    public class AppServiceConstructor : ClassifierConstructorBase<AppService, IAppService>
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
            return runtimeElement.Annotations.OfType<AppServiceContractAttribute>().Any();
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
        protected override AppService TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var appServiceAttr = runtimeElement.Annotations.OfType<AppServiceContractAttribute>().Single();
            var appServiceRuntimeElement = appServiceAttr.ContractType?.AsRuntimeTypeInfo() ?? runtimeElement;

            var element = new AppService(constructionContext, this.TryComputeName(constructionContext, appServiceRuntimeElement));
            if (runtimeElement != appServiceRuntimeElement)
            {
                // add the contract type only if not the same with the provided runtime element.
                ((IConstructableElement)element).AddPart(appServiceRuntimeElement);
            }

            return element;
        }
    }
}