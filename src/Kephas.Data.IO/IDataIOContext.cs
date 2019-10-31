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
        /// Gets or sets the serialization options configuration.
        /// </summary>
        /// <value>
        /// The serialization options configuration.
        /// </value>
        Action<ISerializationContext> SerializationConfig { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataIOContext"/>.
    /// </summary>
    public static class DataIOContextExtensions
    {
        /// <summary>
        /// Sets the serialization options configuration.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataIOContext">The data I/O context.</param>
        /// <param name="serializationContextConfig">The serialization context configuration.</param>
        /// <returns>
        /// This <paramref name="dataIOContext"/>.
        /// </returns>
        public static TContext SetSerializationConfig<TContext>(
            this TContext dataIOContext,
            Action<ISerializationContext> serializationContextConfig)
            where TContext : class, IDataIOContext
        {
            Requires.NotNull(dataIOContext, nameof(dataIOContext));

            dataIOContext.SerializationConfig = serializationContextConfig;

            return dataIOContext;
        }

        /// <summary>
        /// Sets the root object type.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataIOContext">The data I/O context.</param>
        /// <param name="rootObjectType">The root object type.</param>
        /// <returns>
        /// This <paramref name="dataIOContext"/>.
        /// </returns>
        public static TContext RootObjectType<TContext>(
            this TContext dataIOContext,
            Type rootObjectType)
            where TContext : class, IDataIOContext
        {
            Requires.NotNull(dataIOContext, nameof(dataIOContext));

            dataIOContext.RootObjectType = rootObjectType;

            return dataIOContext;
        }
    }
}