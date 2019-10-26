// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for serialization contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Contract for serialization contexts.
    /// </summary>
    public interface ISerializationContext : IContext
    {
        /// <summary>
        /// Gets the serialization service.
        /// </summary>
        /// <value>
        /// The serialization service.
        /// </value>
        ISerializationService SerializationService { get; }

        /// <summary>
        /// Gets or sets the media type.
        /// </summary>
        /// <value>
        /// The media type.
        /// </value>
        Type MediaType { get; set; }

        /// <summary>
        /// Gets or sets the type of the root object.
        /// </summary>
        /// <value>
        /// The type of the root object.
        /// </value>
        Type RootObjectType { get; set; }

        /// <summary>
        /// Gets or sets the root object factory.
        /// </summary>
        /// <value>
        /// The root object factory.
        /// </value>
        Func<object> RootObjectFactory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the serialized output should be indented.
        /// </summary>
        /// <value>
        /// True if the output should be indented, false if not.
        /// </value>
        bool Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type information should be included.
        /// </summary>
        /// <value>
        /// True to include type information, false otherwise.
        /// </value>
        bool IncludeTypeInfo { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="ISerializationContext"/>.
    /// </summary>
    public static class SerializationContextExtensions {

        /// <summary>
        /// Sets a value indicating whether to indent the serialized value.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the serialization context.</typeparam>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">True to indent, false otherwise.</param>
        /// <returns>
        /// This <see cref="ISerializationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Indent<TContext>(this TContext context, bool value)
            where TContext : class, ISerializationContext
        {
            Requires.NotNull(context, nameof(context));

            context.Indent = value;
            return context;
        }

        /// <summary>
        /// Sets a value indicating whether to include the type information in the serialized value.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the serialization context.</typeparam>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">True to include type information, false otherwise.</param>
        /// <returns>
        /// This <see cref="ISerializationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext IncludeTypeInfo<TContext>(this TContext context, bool value)
            where TContext : class, ISerializationContext
        {
            Requires.NotNull(context, nameof(context));

            context.IncludeTypeInfo = value;
            return context;
        }

        /// <summary>
        /// Sets the root object type.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the serialization context.</typeparam>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">The root object type.</param>
        /// <returns>
        /// This <see cref="ISerializationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext RootObjectType<TContext>(this TContext context, Type value)
            where TContext : class, ISerializationContext
        {
            Requires.NotNull(context, nameof(context));

            context.RootObjectType = value;
            return context;
        }

        /// <summary>
        /// Sets the root object factory.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the serialization context.</typeparam>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">The root object factory.</param>
        /// <returns>
        /// This <see cref="ISerializationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext RootObjectFactory<TContext>(this TContext context, Func<object> value)
            where TContext : class, ISerializationContext
        {
            Requires.NotNull(context, nameof(context));

            context.RootObjectFactory = value;
            return context;
        }

        /// <summary>
        /// Sets the media type.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the serialization context.</typeparam>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">The media type.</param>
        /// <returns>
        /// This <see cref="ISerializationContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext MediaType<TContext>(this TContext context, Type value)
            where TContext : class, ISerializationContext
        {
            Requires.NotNull(context, nameof(context));

            context.MediaType = value;
            return context;
        }
    }
}