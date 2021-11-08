﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAppServiceInfosProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Model;
using Kephas.Services;

[assembly: AppServices(providerType: typeof(ModelAppServiceInfosProvider))]

namespace Kephas.Model
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An <see cref="IAppServiceInfosProvider"/> for the model.
    /// </summary>
    public class ModelAppServiceInfosProvider : IAppServiceInfosProvider
    {
        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(IContext? context = null)
        {
            yield return new ContractDeclaration(
                typeof(IModelSpace),
                new AppServiceInfo(
                    typeof(IModelSpace),
                    ctx => ctx.Resolve<IModelSpaceProvider>().GetModelSpace(),
                    AppServiceLifetime.Singleton));
        }
    }
}