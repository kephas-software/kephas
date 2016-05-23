// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributedAppServiceConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Tries to get the <see cref="AppServiceContractAttribute"/> for the provided type.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An <see cref="AppServiceContractAttribute"/> or <c>null</c>, if the provided type is not a service contract.
        /// </returns>
        protected override AppServiceContractAttribute TryGetAppServiceContractAttribute(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute<AppServiceContractAttribute>();
        }
    }
}