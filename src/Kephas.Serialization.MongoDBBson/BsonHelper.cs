// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bson helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Bson
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using MongoDB.Bson.Serialization;

    /// <summary>
    /// A BSON helper.
    /// </summary>
    public static class BsonHelper
    {
        private const string BsonDeserializeOptionsKey = "__BsonDeserializeOptions";
        private const string BsonSerializeOptionsKey = "__BsonSerializeOptions";

        /// <summary>
        /// Configures the deserialization with the provided options.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="options">The custom options.</param>
        /// <returns>The <paramref name="context"/>.</returns>
        public static ISerializationContext DeserializeConfigurator(
            this ISerializationContext context,
            Action<BsonDeserializationContext.Builder> options)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context[BsonDeserializeOptionsKey] = options;

            return context;
        }

        /// <summary>
        /// Configures the deserialization with the provided options.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <returns>The configurator.</returns>
        public static Action<BsonDeserializationContext.Builder>? DeserializeConfigurator(
            this ISerializationContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            return context[BsonDeserializeOptionsKey] as Action<BsonDeserializationContext.Builder>;
        }

        /// <summary>
        /// Configures the serialization with the provided options.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="options">The custom options.</param>
        /// <returns>The <paramref name="context"/>.</returns>
        public static ISerializationContext SerializeConfigurator(
            this ISerializationContext context,
            Action<BsonSerializationContext.Builder> options)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context[BsonSerializeOptionsKey] = options;

            return context;
        }

        /// <summary>
        /// Configures the serialization with the provided options.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <returns>The configurator.</returns>
        public static Action<BsonSerializationContext.Builder>? SerializeConfigurator(
            this ISerializationContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            return context[BsonSerializeOptionsKey] as Action<BsonSerializationContext.Builder>;
        }
    }
}
