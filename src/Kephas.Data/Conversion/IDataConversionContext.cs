// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConversionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for data conversion contexts.
    /// </summary>
    public interface IDataConversionContext : IContext
    {
        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        IDataSpace DataSpace { get; }

        /// <summary>
        /// Gets the data conversion service.
        /// </summary>
        /// <value>
        /// The data conversion service.
        /// </value>
        IDataConversionService DataConversionService { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception when an error occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> to throw an error on exceptions, <c>false</c> if not.
        /// </value>
        bool ThrowOnError { get; set; }

        /// <summary>
        /// Gets or sets the type of the root source.
        /// </summary>
        /// <value>
        /// The type of the root source.
        /// </value>
        Type RootSourceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the root target.
        /// </summary>
        /// <value>
        /// The type of the root target.
        /// </value>
        Type RootTargetType { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataConversionContext"/>.
    /// </summary>
    public static class DataConversionContextExtensions
    {
        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="conversionContext">The conversionContext to act on.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataContext GetDataContext(this IDataConversionContext conversionContext, Type entityType)
        {
            Requires.NotNull(conversionContext, nameof(conversionContext));

            return conversionContext.DataSpace[entityType, conversionContext];
        }

        /// <summary>
        /// Gets the data context for the provided entity.
        /// </summary>
        /// <param name="conversionContext">The conversionContext to act on.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataContext GetDataContext(this IDataConversionContext conversionContext, object entity)
        {
            Requires.NotNull(conversionContext, nameof(conversionContext));

            return conversionContext.DataSpace[entity.GetType(), conversionContext];
        }

        /// <summary>
        /// Sets a value indicating whether to throw on failure.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the data conversion context.</typeparam>
        /// <param name="context">The data conversion context.</param>
        /// <param name="value">True to throw on failure, false otherwise.</param>
        /// <returns>
        /// This <see cref="IDataConversionContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext ThrowOnError<TContext>(this TContext context, bool value)
            where TContext : class, IDataConversionContext
        {
            Requires.NotNull(context, nameof(context));

            context.ThrowOnError = value;
            return context;
        }

        /// <summary>
        /// Sets the root source type.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the data conversion context.</typeparam>
        /// <param name="context">The data conversion context.</param>
        /// <param name="rootSourceType">The root source type.</param>
        /// <returns>
        /// This <see cref="IDataConversionContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext RootSourceType<TContext>(this TContext context, Type rootSourceType)
            where TContext : class, IDataConversionContext
        {
            Requires.NotNull(context, nameof(context));

            context.RootSourceType = rootSourceType;
            return context;
        }

        /// <summary>
        /// Sets the root target type.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the data conversion context.</typeparam>
        /// <param name="context">The data conversion context.</param>
        /// <param name="rootTargetType">The root target type.</param>
        /// <returns>
        /// This <see cref="IDataConversionContext"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext RootTargetType<TContext>(this TContext context, Type rootTargetType)
            where TContext : class, IDataConversionContext
        {
            Requires.NotNull(context, nameof(context));

            context.RootTargetType = rootTargetType;
            return context;
        }
    }
}