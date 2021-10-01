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
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.ExceptionHandling;
    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Merges the loggers into one aggregate.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="loggers">A variable-length parameters list containing loggers.</param>
        /// <returns>
        /// An aggregated logger.
        /// </returns>
        public static ILogger? Merge(this ILogger? logger, params ILogger?[]? loggers)
        {
            var validLoggers = loggers?.Where(l => l != null).ToList();
            if (validLoggers == null || validLoggers.Count == 0)
            {
                return logger;
            }

            if (logger == null)
            {
                return validLoggers.Count == 1 ? validLoggers[0] : new AggregateLogger(validLoggers!);
            }

            validLoggers.Add(logger);
            return new AggregateLogger(validLoggers!);
        }

        /// <summary>
        /// A Type extension method that gets a logger.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <param name="context">Optional. The context used to get the log manager.</param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILogger GetLogger(this Type type, IContext? context = null)
        {
            Requires.NotNull(type, nameof(type));

            var logManager = context?.AmbientServices?.LogManager ?? LoggingHelper.DefaultLogManager;
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
        public static ILogger GetLogger(this object obj, IContext? context = null)
        {
            Requires.NotNull(obj, nameof(obj));

            var objType = obj as Type ?? obj.GetType();
            if (obj is IAmbientServices ambientServices)
            {
                return ambientServices.LogManager.GetLogger(objType);
            }

            if (obj is IInjector injector)
            {
                return injector.GetLogger(objType);
            }

            if (obj is IContext contextObj)
            {
                ambientServices = contextObj.AmbientServices;
                if (ambientServices != null)
                {
                    return ambientServices.LogManager.GetLogger(objType);
                }

                injector = contextObj.Injector;
                if (injector != null)
                {
                    return injector.GetLogger(objType);
                }
            }

            var logManager = context?.AmbientServices?.LogManager ?? LoggingHelper.DefaultLogManager;
            return logManager.GetLogger(objType);
        }
    }
}