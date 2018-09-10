﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfoProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition.Hosting;
    using Kephas.Services;

    /// <summary>
    /// Interface for application service information provider.
    /// </summary>
    public interface IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        IEnumerable<(TypeInfo contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext);
    }
}