// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;
    using Kephas.Security.Permissions.AttributedModel;
    using Kephas.Security.Permissions.Runtime;

    /// <summary>
    /// The <see cref="IRuntimeTypeInfoFactory"/> for the security subsystem.
    /// </summary>
    public class SecurityTypeInfoFactory : RuntimeTypeInfoFactoryBase
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, int position = -1, ILogger? logger = null)
        {
            if (reflectInfo.Name.EndsWith("Permission") && reflectInfo.GetCustomAttributes().OfType<IPermissionInfoAnnotation>().FirstOrDefault() != null)
            {
                return new RuntimePermissionInfo(registry, reflectInfo);
            }

            return null;
        }
    }
}