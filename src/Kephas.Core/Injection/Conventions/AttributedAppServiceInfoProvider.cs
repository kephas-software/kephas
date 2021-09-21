// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributedAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the attributed application service information provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection.Hosting;
    using Kephas.Services;

    /// <summary>
    /// An attributed application service information provider.
    /// </summary>
    public class AttributedAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        ///     The contract declaration types.
        /// </returns>
        IEnumerable<Type>? IAppServiceInfoProvider.GetContractDeclarationTypes(dynamic? context) => ((IInjectionRegistrationContext?)context)?.Parts;
    }
}