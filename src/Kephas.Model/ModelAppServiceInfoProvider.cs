// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Model;
using Kephas.Services;

[assembly: AppServices(providerType: typeof(ModelAppServiceInfoProvider))]

namespace Kephas.Model
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An <see cref="IAppServiceInfoProvider"/> for the model.
    /// </summary>
    public class ModelAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts()
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