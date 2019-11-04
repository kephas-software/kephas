// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// A Type extension method that gets a logger.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="context">Optional. The context used to get the log manager.</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILogger GetLogger(this Type type, IContext context = null)
        {
            Requires.NotNull(type, nameof(type));

            var logManager = context?.AmbientServices?.LogManager ?? Loggable.LogManager;
            return logManager.GetLogger(type);
        }

        /// <summary>
        /// An Object extension method that gets a logger.
        /// </summary>
        /// <remarks>
        /// Use this method only if the object cannot get a logger through dependency injection.
        /// </remarks>
        /// <param name="obj">The object to act on.</param>
        /// <param name="context">The context used to get the log manager (optional).</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILogger GetLogger(this object obj, IContext context = null)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IAmbientServices ambientServices)
            {
                return ambientServices.LogManager.GetLogger(obj.GetType());
            }

            if (obj is ICompositionContext compositionContext)
            {
                return compositionContext.GetLogger(obj.GetType());
            }

            if (obj is IContext contextObj)
            {
                ambientServices = contextObj.AmbientServices;
                if (ambientServices != null)
                {
                    return ambientServices.LogManager.GetLogger(obj.GetType());
                }

                compositionContext = contextObj.CompositionContext;
                if (compositionContext != null)
                {
                    return compositionContext.GetLogger(obj.GetType());
                }
            }

            var logManager = context?.AmbientServices?.LogManager ?? Loggable.LogManager;
            return logManager.GetLogger(obj.GetType());
        }
    }
}