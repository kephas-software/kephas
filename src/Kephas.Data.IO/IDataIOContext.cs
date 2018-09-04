// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataIOContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// Interface for data i/o context.
    /// </summary>
    public interface IDataIOContext : IContext
    {
        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        Type RootObjectType { get; set; }

        /// <summary>
        /// Gets or sets the serialization context configuration.
        /// </summary>
        /// <value>
        /// The serialization context configuration.
        /// </value>
        Action<ISerializationContext> SerializationContextConfig { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataIOContext"/>.
    /// </summary>
    public static class DataIOContextExtensions
    {
        /// <summary>
        /// Sets the serialization context configuration.
        /// </summary>
        /// <param name="dataExportContext">The data I/O context to act on.</param>
        /// <param name="serializationContextConfig">The serialization context configuration.</param>
        /// <returns>
        /// The data I/O context.
        /// </returns>
        public static IDataIOContext WithSerializationContextConfig(
            this IDataIOContext dataExportContext,
            Action<ISerializationContext> serializationContextConfig)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.SerializationContextConfig = serializationContextConfig;

            return dataExportContext;
        }

        /// <summary>
        /// Sets the root object type.
        /// </summary>
        /// <param name="dataExportContext">The data I/O context to act on.</param>
        /// <param name="rootObjectType">The root object type.</param>
        /// <returns>
        /// The data I/O context.
        /// </returns>
        public static IDataIOContext WithRootObjectType(
            this IDataIOContext dataExportContext,
            Type rootObjectType)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.RootObjectType = rootObjectType;

            return dataExportContext;
        }
    }
}