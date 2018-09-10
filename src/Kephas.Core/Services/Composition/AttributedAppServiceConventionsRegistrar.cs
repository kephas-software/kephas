// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributedAppServiceConventionsRegistrar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions registrar for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System.Reflection;

    /// <summary>
    /// Conventions registrar for application services.
    /// </summary>
    public class AttributedAppServiceConventionsRegistrar : AppServiceConventionsRegistrarBase
    {
        /// <summary>
        /// Tries to get the <see cref="IAppServiceInfo"/> for the provided type.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An <see cref="IAppServiceInfo"/> or <c>null</c>, if the provided type is not a service contract.
        /// </returns>
        protected override IAppServiceInfo TryGetAppServiceInfo(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute<AppServiceContractAttribute>();
        }
    }
}