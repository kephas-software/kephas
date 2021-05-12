// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePermissionInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Security.Authorization.AttributedModel;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// A permission info based on a runtime type.
    /// </summary>
    public class RuntimePermissionInfo : RuntimeTypeInfo, IPermissionInfo
    {
        private IEnumerable<IPermissionInfo>? grantedPermissions;
        private IEnumerable<IPermissionInfo>? requiredPermissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimePermissionInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="type">The type.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal RuntimePermissionInfo(IRuntimeTypeRegistry typeRegistry, Type type, ILogger? logger = null)
            : base(typeRegistry, type, logger)
        {
            var permTypeAttr = this.GetAttributes<Attribute>().OfType<IScoped>().FirstOrDefault();
            this.Scoping = permTypeAttr?.Scoping ?? Scoping.Global;
            this.TokenName = this.ComputeTokenName(type);
        }

        /// <summary>
        /// Gets the token name.
        /// </summary>
        public string TokenName { get; }

        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        public Scoping Scoping { get; }

        /// <summary>
        /// Gets the granted permissions.
        /// </summary>
        /// <remarks>
        /// When this permission is granted, the permissions granted by this are also granted.
        /// Using this mechanism one can define a hierarchy of permissions.
        /// </remarks>
        /// <value>
        /// The granted permissions.
        /// </value>
        public IEnumerable<IPermissionInfo> GrantedPermissions =>
            this.grantedPermissions ??= this.GetAttributes<GrantsPermissionAttribute>()
                .SelectMany(attr => attr.PermissionTypes)
                .Union(new List<Type>(this.Type.GetInterfaces()) { this.Type.BaseType }
                    .Where(t => t != null))
                .Select(t => this.TypeRegistry.GetTypeInfo(t))
                .OfType<IPermissionInfo>()
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Gets the required permissions to access this permission.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        public IEnumerable<IPermissionInfo> RequiredPermissions =>
            this.requiredPermissions ??= this.GetAttributes<RequiresPermissionAttribute>()
                .SelectMany(attr => attr.PermissionTypes)
                .Select(t => this.TypeRegistry.GetTypeInfo(t))
                .OfType<IPermissionInfo>()
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"[{this.TokenName}] {base.ToString()}";
        }

        private string ComputeTokenName(Type type)
        {
            var attr = type.GetCustomAttributes<Attribute>()
                .OfType<IToken>()
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(attr?.TokenName))
            {
                return attr!.TokenName;
            }

            var tokenName = type.Name;
            if (type.IsInterface && tokenName.StartsWith("i", StringComparison.OrdinalIgnoreCase))
            {
                tokenName = tokenName.Substring(1);
            }

            const string ending = "permission";
            if (tokenName.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
            {
                tokenName = tokenName.Substring(0, tokenName.Length - ending.Length);
            }

            return tokenName;
        }
    }
}