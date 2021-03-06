﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An <see cref="IAppServiceInfoProvider"/> for the model.
    /// </summary>
    public class ModelAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            yield return (
                typeof(IModelSpace),
                new AppServiceInfo(
                    typeof(IModelSpace),
                    ctx => ctx.GetExport<IModelSpaceProvider>().GetModelSpace(),
                    AppServiceLifetime.Singleton));
        }
    }
}