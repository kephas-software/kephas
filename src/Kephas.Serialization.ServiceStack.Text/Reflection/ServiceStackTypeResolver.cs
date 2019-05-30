// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceStackTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service stack type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Reflection
{
    using System;

    using global::ServiceStack.Text;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A type resolver using the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.AboveNormal)]
    public class ServiceStackTypeResolver : DefaultTypeResolver, ITypeResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStackTypeResolver" /> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public ServiceStackTypeResolver(IAssemblyLoader assemblyLoader)
            : base(assemblyLoader)
        {
        }

        /// <summary>
        /// Resolves a type based on the provided type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        public new Type ResolveType(string typeName, bool throwOnNotFound = true)
        {
            try
            {
                var type = AssemblyUtils.FindType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            catch
            {
                if (throwOnNotFound)
                {
                    throw;
                }

                return null;
            }

            return base.ResolveType(typeName, throwOnNotFound);
        }
    }
}