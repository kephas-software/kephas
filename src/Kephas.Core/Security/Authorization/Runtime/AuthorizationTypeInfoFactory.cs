// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Runtime;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// The <see cref="IRuntimeTypeInfoFactory"/> for the authorization subsystem.
    /// </summary>
    public class AuthorizationTypeInfoFactory : IRuntimeTypeInfoFactory
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationTypeInfoFactory"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        public AuthorizationTypeInfoFactory(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Tries to create the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public IRuntimeTypeInfo? TryCreateRuntimeTypeInfo(Type type)
        {
            if (type.Name.EndsWith("Permission") && type.GetCustomAttributes().OfType<IPermissionInfoAttribute>().FirstOrDefault() != null)
            {
                return new RuntimePermissionInfo(this.typeRegistry, type);
            }

            return null;
        }
    }
}