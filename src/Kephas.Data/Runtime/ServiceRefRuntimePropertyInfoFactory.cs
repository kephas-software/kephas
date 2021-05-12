// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRefRuntimePropertyInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Runtime
{
    using System.Reflection;

    using Kephas.Data.Reflection;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;

    /// <summary>
    /// Implementation of <see cref="IRuntimePropertyInfoFactory"/> for <see cref="IServiceRefPropertyInfo"/>.
    /// </summary>
    public class ServiceRefRuntimePropertyInfoFactory : RuntimePropertyInfoFactoryBase
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
        public override IRuntimePropertyInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, PropertyInfo reflectInfo, int position = -1, ILogger? logger = null)
        {
            if (typeof(IServiceRef).IsAssignableFrom(reflectInfo.PropertyType))
            {
                return new ServiceRefRuntimePropertyInfo(registry, reflectInfo, position, logger);
            }

            return null;
        }
    }
}