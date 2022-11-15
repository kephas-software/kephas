// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistryJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// JSON converter for <see cref="DynamicTypeRegistry"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="DynamicTypeRegistry"/> should be processed before expandos, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.High)]
    public class DynamicTypeRegistryJsonConverter : ExpandoJsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeRegistryJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="injectableFactory">The injectable factory.</param>
        public DynamicTypeRegistryJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver, IInjectableFactory injectableFactory)
            : base(typeRegistry, typeResolver, injectableFactory, typeof(DynamicTypeRegistry), typeof(DynamicTypeRegistry))
        {
        }

        /// <summary>
        /// Creates the expando value which should collect the JSON values.
        /// </summary>
        /// <param name="expandoTypeInfo">The type information of the target expando value.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>The newly created expando collector.</returns>
        protected override IDynamic CreateExpandoCollector(IRuntimeTypeInfo expandoTypeInfo, object? existingValue)
        {
            return existingValue == null ? new DynamicTypeRegistry(this.TypeRegistry, this.TypeResolver) : (IDynamic)existingValue;
        }
    }
}