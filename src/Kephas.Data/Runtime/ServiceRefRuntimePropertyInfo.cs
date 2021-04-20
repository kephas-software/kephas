// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRefRuntimePropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Runtime
{
    using System.Linq;
    using System.Reflection;

    using Kephas.Data.Reflection;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Implementation of <see cref="IRuntimePropertyInfo" /> for runtime properties based on <see cref="IServiceRef"/>.
    /// </summary>
    public class ServiceRefRuntimePropertyInfo : RuntimePropertyInfo, IServiceRefPropertyInfo
    {
        private ITypeInfo? refType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRefRuntimePropertyInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="position">Optional. The position.</param>
        /// <param name="logger">Optional. The logger.</param>
        protected internal ServiceRefRuntimePropertyInfo(IRuntimeTypeRegistry typeRegistry, PropertyInfo propertyInfo, int position = -1, ILogger? logger = null)
            : base(typeRegistry, propertyInfo, position, logger)
        {
        }

        /// <summary>
        /// Gets the service reference type.
        /// </summary>
        public ITypeInfo ServiceRefType
        {
            get
            {
                if (this.refType != null)
                {
                    return this.refType;
                }

                var refGenericType = this.PropertyInfo.PropertyType.GetInterfaces()
                    .FirstOrDefault(i => TypeExtensions.IsConstructedGenericOf(i, typeof(IServiceRef<>)));
                return this.refType = this.TypeRegistry.GetTypeInfo(refGenericType?.GenericTypeArguments[0] ?? typeof(object));
            }
        }
    }
}